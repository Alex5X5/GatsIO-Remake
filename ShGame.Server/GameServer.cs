namespace ShGame.Net.Server;

using ShGame.Game;
using ShGame.Game.GameObjects;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class GameServer:Socket {

	#region fields

	private bool stop = false;
	private readonly Logger logger;

	private long serverTimer = 0;

	private short PlayerIdCounter=0;

	//some constants
	//public const int TARGET_TPS = 50;
	//public const int MAP_WIDTH = 2100, MAP_HEIGHT = 1400;
	//public const int OBSTACKLE_ROWS = 5, OBSTACKLE_LINES = 8;
	//public const int OBSTACLE_ROW_DISANCE = MAP_WIDTH / OBSTACKLE_ROWS;
	//public const int OBSTACLE_LINE_DISTANCE = MAP_HEIGHT / OBSTACKLE_LINES;
	//public const int MAX_PLAYER_COUNT = 20;
	//public const int OBSTACLE_COUNT = OBSTACKLE_ROWS*OBSTACKLE_LINES, BULLET_COUNT = 35;

	private readonly GameInstance Game;

	private readonly ServerConnection?[] clients = new ServerConnection[Constants.PLAYER_COUNT];

    #endregion fields

    #region constructors
    public GameServer() : this(5000) { }

	public GameServer(int port) : this(NetUtil.GetLocalIP(), (uint)System.Math.Abs(port)) { }

	public GameServer(IPAddress address, uint port) : base(address.AddressFamily == AddressFamily.InterNetwork ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp){
		logger = new Logger(new LoggingLevel("GameServer"));
		logger.Log(
			"address port constructor",
			new MessageParameter("address", address),
			new MessageParameter("addressFamily", AddressFamily.ToString()),
			new MessageParameter("port", port)
		);
		//create an IPEndpoint with the given address and the given port and bind the server to the IPEndpoint
		IPEndPoint point = new(address, (int)port);
		logger.Log("binding, endPoint = "+point.ToString()+" endpint address = " + point.AddressFamily.ToString());
		Bind(point);
		logger.Log("bound endPoint="+point.ToString());
		//start the main threads
		Game = new(null);
        logger.Log("starting game loops");
        Game.StartAllLoops();
		Game.SpreadObstacles();
		AcceptLoop();
	}

	private void OnAccept(Socket s) {
		logger.Log("OnAccept("+s.ToString()+")");
		//search for a slot for the new connection
		bool found = false;
		for (int i = 0; i<clients.Length; i++) {
			if (clients[i]==null) {
				clients[i]=new ServerConnection(s, this, i);
				found = true;
				break;
			}
		}
		if (!found) {
			byte[] buffer = Protocoll.PreparePacket(Headers.PAYER_LIMIT);
			s.Send(buffer);
			s.Close();
			s.Dispose();
		}
	}

	#endregion constructors

	#region request events

	internal unsafe byte[] OnMapRequest() {
		logger.Log("processing map request");
		//prepare a new Packet
		byte[] result = Protocoll.PreparePacket(Headers.MAP);
		int counter = 0;
		//serialize all of the obstacles into the packet
		fixed(byte* ptr = &result[0])
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++)
			//logger.Log("serializing requested obstacle",new MessageParameter(" packet offset", (Protocoll.PAYLOAD_OFFSET+i*Obstacle.OBSTACLE_BYTE_LENGTH)),new MessageParameter(" Obstacle"+ptr->ToString()));
			Obstacle.SerializeObstacle(ptr, Game.Obstacles[i], i*Obstacle.OBSTACLE_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET);
		return result;
	}

	internal byte[]? OnPingRequest(byte[] packet) {
		if(Protocoll.UnloadPing(packet)) {
			logger.Log("answering ping");
			return Protocoll.LoadPing(false);
		} else
			logger.Log("not answering ping");
		return null;
	}

	internal unsafe byte[]? OnExchangePlayerRequest(byte[] packet) {
		logger.Log("on exchange player request");
		//check if the packet is actually a player request
		if (Protocoll.AnalyzePacket(packet)==Headers.PLAYER) {
			//create a temporary player and read it's properties from the packet
			Player temp = new(null, 0, 0);
			fixed (byte* ptr = &packet[0])
				Player.DeserializePlayer(ptr, temp, Protocoll.PAYLOAD_OFFSET);
			logger.Log("processing player request", new MessageParameter("player",temp));
			//if (!IsPlayerRegistered(temp)) {
			//	RegisterNewPlayer(temp);
			//}
			//prepare a new packet
			byte[] result = Protocoll.PreparePacket(Headers.PLAYER);

			//add all of the players to the packet
			fixed (byte* ptr = &result[0])
				for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
					if (Game.Players[i]==null)
						continue;
					if (Game.Players[i].PlayerUUID == temp.PlayerUUID) {
						//Game.Players[i].Pos = temp.Pos;
						Game.Players[i].Dir = temp.Dir;
					}
					//logger.Log("serializing player",new MessageParameter("player", players[i].ToString()));
					Player.SerializePlayer(ptr, Game.Players[i], i*Player.PLAYER_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET);
				}
			return result;
		} else {
			logger.Log("wrong request");
		}
		return null;
	}

	internal unsafe byte[]? OnRegisterPlayerRequest(byte[] packet) {
		logger.Log("processing player register request");
		if (Protocoll.AnalyzePacket(packet)==Headers.REGISTER_PLAYER) {
			PlayerIdCounter++;
			Player temp = new(null, 100, PlayerIdCounter);
			for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
				if (i==Constants.PLAYER_COUNT-1 && Game.Players[i].Health!=-1)
					return Protocoll.PreparePacket(Headers.PAYER_LIMIT);
				if (Game.Players[i].Health==-1) {
					Game.Players[i]=temp;
					break;
				}
			}
			byte[] result = Protocoll.PreparePacket(Headers.REGISTER_PLAYER);
			fixed (byte* ptr = &result[0])
				Player.SerializePlayer(ptr, temp, Protocoll.PAYLOAD_OFFSET);
			return result;
		} else {
			logger.Log("wrong request");
		}
		return null;
	}

	internal unsafe byte[]? OnBulletRequest(byte[] packet) {
		//check if the packet actually is a player request
		if (Protocoll.AnalyzePacket(packet)==Headers.BULLET) {
			logger.Log("processing bullet request");
			byte[] result = Protocoll.PreparePacket(Headers.BULLET);
			//add all of the bullets to the packet
			fixed (byte* ptr = &result[0])
			for (int i = 0; i<Constants.BULLET_COUNT-1; i++) {
				if (Game.Bullets[i]==null)
					continue;
				Bullet.SerializeBullet(ptr, Game.Bullets[i], i*Bullet.BULLET_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET);
			}
			return result;
		} else {
			logger.Log("wrong request");
		}
		return null;
	}


	#endregion request events

	private bool IsPlayerRegistered(Player player) {
		bool found = false;
		for (int i = 0; i<Constants.PLAYER_COUNT-1; i++) {
			if (Game.Players[i]==null)
				continue;
			if (Game.Players[i].PlayerUUID == player.PlayerUUID) {
				found = true;
				break;
			}
		}
		return found;
	}

	private bool RegisterNewPlayer(Player player) {
		//since the player isn't known, try to register it
		logger.Log("registering new player", new MessageParameter("UUID", player.PlayerUUID));
		//loop through the player array and search for an unused player
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			//the slot is considered empty if the player's health is -1
			if (Game.Players[i].Health==-1) {
				Game.Players[i].Health=100;
				Game.Players[i].PlayerUUID = player.PlayerUUID;
				Game.Players[i].Dir=player.Dir.Nor();
				logger.Log("sucessfully registred new player", new MessageParameter("UUID", player.PlayerUUID));
				return true;
			}
		}
		logger.Log("failed to register player", new MessageParameter("UUID", player.PlayerUUID));
		return false;
	}

	private void DisposeObjects() {
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			if (clients[i]!=null) {
				if (clients[i].disposalCooldown<1000)
					clients[i].disposalCooldown--;
				if (clients[i].disposalCooldown==800)
					clients[i].Stop();
				if (clients[i].disposalCooldown<=0)
					clients[i] = null;
			}
		}
	}

	public void Stop() {
		logger.Log("stopping");
		//the AcceptLoop Thread only stops if stop is set to true
		stop = true;
		Game.Stop();
		//stopp all connections
		foreach (ServerConnection? c in clients)
			c?.Stop();
		Thread.Sleep(1000);
		//the socket must be closed and disposed or the garbage collector won't free the memory
		Close();
		Dispose();
	}

	private void AcceptLoop() {
		logger.Log("accept loop");
		//loop until the server is about to stop
		while (!stop) {
			Listen(1);
			while (!stop) {
				try {
					//create a Task that starts to try to accept a socket and in case of success stops to listen
					//the result of the listening is a  socket that is connected to a client
					Socket clientConnection = Accept();
					logger.Log("accepted!");
					Task s = Task.Run(() => OnAccept(clientConnection));
				} catch (Exception e) {
					if (!stop) {
						//if the Exception wasn't caught because the server is stopping and it's socket is therefore closing,
						//a different error must have happened so print it's message
						Console.WriteLine(e.ToString());
					} else {
						//since the server is stopping, ignore the error and break the main loop
						break;
					}
				}
			}
		}
	}
}

