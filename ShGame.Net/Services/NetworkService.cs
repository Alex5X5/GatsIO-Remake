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
		socket.Stop();
	}

	public async Task<Map> GetMapAsync() {
		logger.Log("getting Map");
		await socket.SendPacketAsync(new Packet(PacketType.Map));
		Packet packet = await socket.RecievePacketAsync();
		Map map = new();
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
			int offset = i*Obstacle.SizeInBytesForNetwork;
			unsafe {
				map.Obstacles[i] = SerializerService.DeserializeObstacle(packet.Payload, offset);
			}
		}
		return map;
	}

	public async Task PublishPlayerAsync(Player player) {
		
	}

	public async Task<Player[]> GetPlayersAsync() {
		return [];
	}

	public async Task<Player> RegisterToServerAsync() {
		await socket.SendPacketAsync(new Packet(PacketType.Register));
		Packet packet = await socket.RecievePacketAsync();
		Player player;
		unsafe {
			player = SerializerService.DeserializePlayer(packet.Payload);
		}
		packet.Dispose();
		return player;
		
	}
}
