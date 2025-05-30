﻿namespace ShGame.Game.Net;

using ShGame.Game.Client;
using ShGame.Game.Logic.Math;

using System.Buffers;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


internal class GameServer:Socket {

	#region fields

	private bool stop = false;
	private readonly Logger logger;

	private long serverTimer = 0;

	//some constants
	public const int TARGET_TPS = 50;
	public const int MAP_WIDTH = 2100, MAP_HEIGHT = 1400;
	public const int OBSTACKLE_ROWS = 5, OBSTACKLE_LINES = 8;
	public const int OBSTACLE_ROW_DISANCE = MAP_WIDTH / OBSTACKLE_ROWS;
	public const int OBSTACLE_LINE_DISTANCE = MAP_HEIGHT / OBSTACKLE_LINES;
	public const int MAX_PLAYER_COUNT = 20;
	public const int OBSTACLE_COUNT = OBSTACKLE_ROWS*OBSTACKLE_LINES, BULLET_COUNT = 200;

	internal Player[] players = new Player[MAX_PLAYER_COUNT];
	private readonly ServerConnection?[] clients = new ServerConnection[MAX_PLAYER_COUNT];
	private unsafe readonly Obstacle[] obstacles = new Obstacle[OBSTACLE_COUNT];
    private Bullet[] bullets;

    #endregion fields

    #region constructors
    public GameServer() : this(5000) { }

	public GameServer(int port) : this(GetLocalIP(), Math.Abs(port)) { }

	public GameServer(IPAddress address, int port) : base(address.AddressFamily == AddressFamily.InterNetwork ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp){
		logger = new Logger(new LoggingLevel("GameServer"));
		logger.Log(
			"address port constructor",
			new MessageParameter("address", address),
			new MessageParameter("addressFamily", AddressFamily.ToString()),
			new MessageParameter("port", port)
		);
		SpreadObstacles();
		//fill the players with invalid players so the serializers don't face nullpointers
		for (int i = 0; i < MAX_PLAYER_COUNT; i++)
			players[i] = new Player();
		logger.Log(players.ToString()??"");
		//create an IPEndpoint with the given address and the given port and bind the server to the IPEndpoint
		IPEndPoint point = new(address, (int)port);
		logger.Log("binding, endPoint = "+point.ToString()+" endpint address = " + point.AddressFamily.ToString());
		Bind(point);
		logger.Log("bound endPoint="+point.ToString());
		//start the main threads
		StartNewLoop(PlayerLoop);
		//StartNewLoop(BulletLoop);
		AcceptLoop();
	}

	private void OnAccept(Socket s) {
		logger.Log("OnAccept("+s.ToString()+")");
		//search for a slot for the new connection
		bool found = false;
		for (int i = 0; i<clients.Length; i++) {
			if (clients[i]==null) {
				//close the newly created socket an create a ServerConnection from the socket's information so the ServerConnection is bound to the incoming connection
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
		//prepare a new Packet
		byte[] result = Protocoll.PreparePacket(Headers.MAP);
		int counter = 0;
		//serialize all of the obstacles into the packet
		for (int i = 0; i<OBSTACLE_COUNT; i++)
			//logger.Log("serializing requested obstacle",new MessageParameter(" packet offset", (Protocoll.PAYLOAD_OFFSET+i*Obstacle.OBSTACLE_BYTE_LENGTH)),new MessageParameter(" Obstacle"+ptr->ToString()));
			fixed(byte* ptr = &result[i*Obstacle.OBSTACLE_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET])
				Obstacle.SerializeObstacle(ptr, obstacles[i], 0);
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

	internal unsafe byte[]? OnPlayerRequest(byte[] packet) {
		//check if the packet actually is a player request
		if (Protocoll.AnalyzePacket(packet)==Headers.PLAYER) {
			//create a temporary player and read it's properties from the packet
			Player temp = new(null, 0, 0);
			fixed(byte* ptr = &packet[Protocoll.PAYLOAD_OFFSET])
			Player.DeserializePlayer(ptr, temp, 0);
			//logger.Log("processing player request",new MessageParameter("player",temp));
			if (!IsPlayerRegistered(temp)) {
				RegisterNewPlayer(temp);
			}
			//prepare a new packet
			byte[] result = Protocoll.PreparePacket(Headers.PLAYER);

			//add all of the players to the packet
			for (int i = 0; i<MAX_PLAYER_COUNT-1; i++) {
				if (players[i]==null)
					continue;
				if (players[i].PlayerUUID == temp.PlayerUUID) {
					players[i].Pos = temp.Pos;
					players[i].Dir = temp.Dir;
				}
				//logger.Log("serializing player",new MessageParameter("player", players[i].ToString()));
				//create a pointer to a player in the array of the players
				fixed (byte* ptr = &result[i*Player.PLAYER_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET])
					Player.SerializePlayer(ptr, players[i], 0);
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
		for (int i = 0; i<MAX_PLAYER_COUNT-1; i++) {
			if (players[i]==null)
				continue;
			if (players[i].PlayerUUID == player.PlayerUUID) {
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
		for (int i = 0; i<MAX_PLAYER_COUNT; i++) {
			//the slot is considered empty if the player's health is -1
			if (players[i].Health==-1) {
				players[i].Health=100;
				players[i].PlayerUUID = player.PlayerUUID;
				unsafe {
					players[i].Dir=player.Dir.Nor();
				}
				//logger.Log("sucessfully registred new player", new MessageParameter("UUID", player.PlayerUUID));
				return true;
			}
		}
		return false;
	}

	private void DisposeObjects() {
		for (int i = 0; i<MAX_PLAYER_COUNT; i++) {
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

	private void SpreadObstacles() {
		logger.Log("generating Obstacles");
		int c = 0;
		//spreading obstacles over OBSTACKLE_ROWS rows
		for (int row = 0; row<OBSTACKLE_ROWS; row++)
			//spreading obstacles over OBSTACKLE_LINES lines so there are OBSTACKLE_ROWS*OBSTACKLE_LINES obstacles all together
			for (int line = 0; line<OBSTACKLE_LINES; line++) {
				PlaceObstacles(1 + row, 1 + line, c);
				//c is the position of the obstacle in the arary
				c++;
			}
	}

	private void PlaceObstacles(int row, int line, int offset) {
		//since there are OBSTACLE_ROWS rows the distance between the rows has to be MAP_WIDTH/OBSTACLE_ROWS
		row = MAP_WIDTH / OBSTACKLE_ROWS * row;
		//substract half of the distance between the rows so the obstakles get placed in the middle of each row
		row -= (int)(0.5 * MAP_WIDTH / OBSTACKLE_ROWS);
		//since there are OBSTACKLE_LINES lines the distance between the lines has to be MAP_HEIGHT/OBSTACKLE_LINES
		line = MAP_HEIGHT / OBSTACKLE_LINES * line;
		//substract half of the distance between the lines so the obstakles get placed in the middle of each line
		line -= (int)(0.5 * MAP_HEIGHT / (OBSTACKLE_LINES));
		Random r = new();
		obstacles[offset] = new Obstacle(
			null,
			new Vector3d(
				//the obstacles may also be offset by half the distance to the next row/line
				//first add half of the distance between the rows to x
				//then substract a random number between 0 and OBSTACLE_ROW_DISANCE from it
				row + OBSTACLE_ROW_DISANCE / 2 - r.Next(0, OBSTACLE_ROW_DISANCE),
				//first add half of the distance between the lines to y
				//then substract a random number between 0 and OBSTACLE_LINE_DISTANCE from it
				line + OBSTACLE_LINE_DISTANCE /2 + r.Next(0, OBSTACLE_LINE_DISTANCE),
				0
			),
			//the upper bound of the type must be 4 becuase 3 ist the maxumum possible tytpe but the upper bound is not included
			(byte)r.Next(1, 4)

		);
		logger.Log("generated new Obstacle ", new MessageParameter("obstacle", obstacles[offset]));
	}
	public void Stop() {
		logger.Log("stopping");
		//the AcceptLoop Thread only stops if stop is set to true
		stop = true;
		//stopp all connections
		foreach (ServerConnection? c in clients)
			c?.Stop();
		Thread.Sleep(1000);
		//the socket must be closed and disposed or the garbage collector won't free the memory
		Close();
		Dispose();
	}

	private void ClockLoop() {
		//a loop with as little load as possible, so the timer of the server increases steadily
		serverTimer++;
		Thread.Sleep(1/TARGET_TPS);
	}

	private void AcceptLoop() {
		//logger.Log("accept loop");
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

	private void StartNewLoop(Action loop) {
		new Thread(
			() => {
				logger.Log("start loop");
				long loopTime = serverTimer;
				while (!stop) {
					while (loopTime+TARGET_TPS<serverTimer && !stop)
						Thread.Sleep(1);
					loop();
				}
			}
		).Start();
	}

	private void BulletLoop() {
		//logger.Log("player loop");
		foreach (Bullet b in bullets) {
			b.Move();
		}
	}

	private void PlayerLoop() {
        //logger.Log("player loop")
        foreach (Player p in players) {
            p.Move();
        }
    }

    public static IPAddress GetLocalIPv4() =>
		NetworkInterface.GetAllNetworkInterfaces()[0].
			GetIPProperties().UnicastAddresses[^1].
				Address;

	public static IPAddress GetLocalIPv6() =>
		NetworkInterface.GetAllNetworkInterfaces()[0].
			GetIPProperties().UnicastAddresses[^2].
				Address;

	public static IPAddress GetLocalIP()=>
		NetworkInterface.GetAllNetworkInterfaces()[0].
			GetIPProperties().UnicastAddresses[^1].
				Address;
}

