namespace ShGame.Net.Server;

using ShGame.Game;
using ShGame.Net.Shared;
using ShGame.Net.Shared.Services;
using ShGame.Types;
using ShGame.Util;

using System.Threading.Tasks;

public class GameServer : IDisposable {

	private readonly Logger logger = new(new LoggingLevel("GameServer"));

	private readonly GameService gameService;

	private readonly ServerSocket socket;

	private readonly ClientConnection?[] clients = new ClientConnection[Constants.PLAYER_COUNT];

	private short PlayerIdCounter = 1;

	public GameServer(Configuration config) {
		socket = new(config, OnAccept);
		gameService = new(new Player());
		gameService.SpreadObstacles();
        gameService.StartAllLoops();
	}

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

	internal unsafe Packet? OnPingRequest(Packet packet) {
		if (packet.Payload[0] == 1) {
			logger.Log("answering ping");
			return new Packet(PacketType.Ping);
		} else {
			logger.Log("not answering ping");
			return null;
		}
	}

	internal Packet? OnMapRequest() {
		logger.Log("processing map request");
		Packet response = new(PacketType.Map);
		unsafe {
			byte* ptr = response.Payload;
			for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
				int offset = i*Obstacle.SizeInBytesForNetwork;
				SerializerService.SerializeObstacle(gameService.Map.Obstacles[i], ptr, offset);
				ptr += Obstacle.SizeInBytesForNetwork;
			}
		}
		return response;
	}

	internal unsafe Packet? OnUpdatePlayerRequest(Packet request) {
		Player player = SerializerService.DeserializePlayer(request.Payload);
		logger.Log("processing update player request", new MessageParameter("player", player));
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			if (gameService.Players[i].PlayerUUID == player.PlayerUUID) {
				gameService.Players[i].Dir = player.Dir;
			}
		}
		return null;
	}

	internal unsafe Packet? OnGetPlayersRequest(Packet request) {
		logger.Log("processing get players request");
		Packet response = new(PacketType.GetPlayers);
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			SerializerService.SerializePlayer(response.Payload, gameService.Players[i], i*Player.SizeInBytesForNetwork);
		}
		return response;
	}

	internal unsafe Packet? OnRegisterPlayerRequest(Packet packet) {
		logger.Log("processing player register request");
		PlayerIdCounter++;
		Player temp = new(new(100,100,0), 100, PlayerIdCounter);
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			if (gameService.Players[i].Health==-1) {
				gameService.Players[i]=temp;
				break;
			} else if (i==Constants.PLAYER_COUNT-1) {
				return new Packet(PacketType.PlayerLimit);
			}
		}
		Packet response = new Packet(PacketType.GetPlayers);
		SerializerService.SerializePlayer(response.Payload, temp, 0);
		return response;
	}

	internal unsafe Packet? OnBulletRequest() {
		logger.Log("processing bullet request");
		Packet response = new Packet(PacketType.Bullets);
		byte* ptr = response.Payload;
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
			int offset = i*Obstacle.SizeInBytes;
			SerializerService.SerializeBullet(gameService.Bullets[i], ptr, offset);
			ptr += Bullet.BULLET_BYTE_LENGTH;
		}
		return response;
	}


	#endregion request events

	public void Dispose() {
		gameService.Stop();
		socket.Dispose();
		foreach (ClientConnection? c in clients)
			c?.Dispose();
	}
}

