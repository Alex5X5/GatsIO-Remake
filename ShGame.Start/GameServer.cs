using System.Net;

namespace ShGame.Start;
internal class GameServer {
	private IPAddress address;
	private uint port;

	public GameServer(IPAddress address, uint port) {
		this.address=address;
		this.port=port;
	}
}