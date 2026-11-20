namespace ShGame.Net.Server;

using ShGame.Game;
using ShGame.Net.Shared;
using ShGame.Net.Shared.Services;
using ShGame.Types;
using ShGame.Util;

using System.Threading.Tasks;
using System.Net;
using System.Runtime.CompilerServices;

public class GameServer : IDisposable {

	private readonly Logger logger = new(new LoggingLevel("GameServer"));

	private readonly GameService gameService;

	private readonly ServerSocket socket;

	private readonly ClientConnection?[] clients = new ClientConnection[Constants.PLAYER_COUNT];

	private short PlayerIdCounter = 1;

	public GameServer(Configuration config) {
		var address = IPAddress.Parse(config.Address);
		var port = config.Port;
		socket = new(address, port, OnAccept);
		gameService = new();
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

	internal static bool ShouldAnswer(Packet packet) {
		var type = packet.Type;
		if (type == PacketType.Bullets)
			return true;
		if (type == PacketType.GetPlayers)
			return true;
		if (type == PacketType.UpdatePlayer)
			return false;
		if (type == PacketType.Map)
			return true;
		if (type == PacketType.Register)
			return true;
		if (type == PacketType.Ping)
			unsafe {
				if (Unsafe.Read<byte>(packet.Payload) == 0x1)
					return true;
				else
					return false;
			}
		return false;
	}

	internal unsafe Packet OnPingRequest(Packet packet) {
		if (packet.Payload[0] == 1) {
			logger.Log("answering ping");
			return new Packet(PacketType.Ping);
		} else {
			logger.Log("not answering ping");
			return Packet.Empty;
		}
	}

	internal Packet OnMapRequest() {
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

	internal unsafe Packet OnUpdatePlayerRequest(Packet request) {
		//logger.Log("processing update player request", new MessageParameter("player", player));
		Player updated = new();
		var player = gameService.GetPlayerById(SerializerService.DeserializePlayerId(request.Payload));
		SerializerService.DeserializePlayer(request.Payload, updated);
		player.MoveDir = updated.MoveDir;
		player.LookDir = updated.LookDir;
		player.IsShooting = updated.IsShooting;
		return Packet.Empty;
	}

	internal unsafe Packet OnRegisterPlayerRequest(Packet packet) {
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

	internal unsafe Packet OnGetPlayersRequest(Packet request) {
		//logger.Log("processing get players request");
		Packet response = new(PacketType.GetPlayers);
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			SerializerService.SerializePlayer(response.Payload, gameService.Players[i], i*Player.SizeInBytesForNetwork);
		}
		return response;
	}

	internal unsafe Packet OnGetBulletsRequest() {
		//logger.Log("processing get bullets request");
		Packet response = new Packet(PacketType.Bullets);
		for (int i = 0; i<Constants.BULLET_COUNT; i++) {
			SerializerService.SerializeBullet(response.Payload, gameService.Bullets[i], i*Bullet.SizeInBytesForNetwork);
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

