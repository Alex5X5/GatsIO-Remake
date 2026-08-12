namespace ShGame.Net.Server;

using ShGame.Game;
using ShGame.Net.Shared;
using ShGame.Net.Shared.Services;
using ShGame.Types;
using ShGame.Util;

using System.Threading.Tasks;

public class GameServer {

	private readonly Logger logger = new(new LoggingLevel("GameServer"));

	private readonly GameInstance Game;

	private readonly ServerSocket socket;

	private readonly ClientConnection?[] clients = new ClientConnection[Constants.PLAYER_COUNT];

	private short PlayerIdCounter = 1;
    #region constructors

	public GameServer(Configuration config) {
		socket = new(config, OnAccept);
		Game = new(null);
        Game.StartAllLoops();
		Game.SpreadObstacles();
	}

	#endregion constructors

	private void OnAccept(System.Net.Sockets.Socket socket) {
		logger.Log("OnAccept("+socket.ToString()+")");
		for (int i = 0; i<clients.Length; i++) {
			if (clients[i]==null) {
				clients[i]=new ClientConnection(socket, this);
				if(clients[i] != null)
					_ = RunClientConnectionAsync(clients[i]!);
				break;
			}
		}
	}

	private async Task RunClientConnectionAsync(ClientConnection connection) {
		try {
			await connection.RunAsync();
		} catch (Exception ex) {
			logger.Error($"Client loop Crashed because of:{ex}");
		}
	}

	public async Task StartAcceptLoopAsync() {
		await socket.StartAcceptLoopAsync();
	}

	#region request events

	internal Packet OnMapRequest() {
		logger.Log("processing map request");
		Packet response = new(PacketType.Map);
		unsafe {
			byte* ptr = response.Payload;
			for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
				int offset = i*Obstacle.SizeInBytesForNetwork;
				SerializerService.SerializeObstacle(Game.Obstacles[i], ptr, offset);
				ptr += Obstacle.SizeInBytesForNetwork;	
			}
		}
		return response;
	}

	internal unsafe Packet? OnPingRequest(Packet packet) {
		if (packet.Payload[0] == 1) {
			logger.Log("answering ping");
			return new Packet(PacketType.Ping);
		} else {
			logger.Log("not answering ping");
			return null;
		}
	}

	internal unsafe Packet? OnExchangePlayerRequest(Packet packet) {
		Player temp = SerializerService.DeserializePlayer(packet.Payload, Protocoll.PAYLOAD_OFFSET);
		logger.Log("processing player request", new MessageParameter("player",temp));
		Packet response = new Packet(PacketType.Player);
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			if (Game.Players[i]==null)
				continue;
			if (Game.Players[i].PlayerUUID == temp.PlayerUUID) {
				Game.Players[i].Dir = temp.Dir;
			}
			byte* ptr = packet.Payload;
			SerializerService.SerializePlayer(ptr, Game.Players[i], i*Player.SizeInBytes);
		}
		return response;
	}

	internal unsafe Packet? OnRegisterPlayerRequest(Packet packet) {
		logger.Log("processing player register request");
		PlayerIdCounter++;
		Player temp = new(new(100,100,0), 100, PlayerIdCounter);
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			if (i==Constants.PLAYER_COUNT-1 && Game.Players[i].Health!=-1)
				return new Packet(PacketType.PlayerLimit);
			if (Game.Players[i].Health==-1) {
				Game.Players[i]=temp;
				break;
			}
		}
		Packet response = new Packet(PacketType.Player);
		SerializerService.SerializePlayer(response.Payload, temp, Protocoll.PAYLOAD_OFFSET);
		return response;
	}

	internal unsafe Packet? OnBulletRequest() {
		logger.Log("processing bullet request");
		Packet response = new Packet(PacketType.Bullets);
		byte* ptr = response.Payload;
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
			int offset = i*Obstacle.SizeInBytes;
			SerializerService.SerializeBullet(Game.Bullets[i], ptr, offset);
			ptr += Bullet.BULLET_BYTE_LENGTH;
		}
		return response;
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

	//public void Stop() {
	//	logger.Log("stopping");
	//	//the AcceptLoop Thread only stops if stop is set to true
	//	stop = true;
	//	Game.Stop();
	//	//stopp all connections
	//	foreach (ClientConnection? c in clients)
	//		c?.Stop();
	//	Thread.Sleep(1000);
	//	//the socket must be closed and disposed or the garbage collector won't free the memory
	//	Close();
	//	Dispose();
	//}
}

