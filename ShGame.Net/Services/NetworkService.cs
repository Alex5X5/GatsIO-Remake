namespace ShGame.Net.Client.Services;

using ShGame.Net.Shared;
using ShGame.Net.Shared.Services;
using ShGame.Types;
using ShGame.Util;

using System.Threading.Tasks;

public class NetworkService : IDisposable {

	private readonly Logger logger = new(new LoggingLevel("NetHandler"));

	private bool stop = false;

	private readonly Socket socket;

	public bool Connected => socket.Connected;

	public NetworkService(Configuration config) {
		socket = new(config);
	}

	public void Dispose() {
		socket.Dispose();
	}

	public async Task<Map> GetMapAsync() {
		logger.Log("getting Map");
		await socket.SendPacketAsync(new Packet(PacketType.Map));
		Packet response = await socket.RecievePacketAsync();
		Map map = new();
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
			int offset = i*Obstacle.SizeInBytesForNetwork;
			unsafe {
				map.Obstacles[i] = SerializerService.DeserializeObstacle(response.Payload, offset);
			}
		}
		response.Dispose();
		return map;
	}

	public async Task UpdatePlayerAsync(Player player) {
		Packet request = new Packet(PacketType.UpdatePlayer);
		unsafe {
			SerializerService.SerializePlayer(request.Payload, player);
		}
		await socket.SendPacketAsync(request);
	}

	public async Task<Player[]> GetPlayersAsync() {
		Packet request = new Packet(PacketType.GetPlayers);
		await socket.SendPacketAsync(request);
		Packet response = await socket.RecievePacketAsync();
		Player[] players = new Player[Constants.PLAYER_COUNT];
		for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
			int offset = i*Player.SizeInBytesForNetwork;
			unsafe {
				players[i] = SerializerService.DeserializePlayer(response.Payload, offset);
			}
		}
		response.Dispose();
		return players;
	}

	public async Task<Player> RegisterToServerAsync() {
		await socket.SendPacketAsync(new Packet(PacketType.Register));
		Packet response = await socket.RecievePacketAsync();
		Player player;
		unsafe {
			player = SerializerService.DeserializePlayer(response.Payload);
		}
		response.Dispose();
		return player;
		
	}
}
