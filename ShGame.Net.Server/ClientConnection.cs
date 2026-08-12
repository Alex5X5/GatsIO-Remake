namespace ShGame.Net.Server;

using ShGame.Net.Shared;

using SimpleLogging.logging;

using System;
using System.Net.Sockets;
using System.Threading.Tasks;

internal class ClientConnection : IDisposable {
	
	private readonly Shared.Socket socket;
	private readonly GameServer server;

	private bool stop = false;
	private readonly Logger logger = new(new LoggingLevel("ClientConnection"));
	internal int disposalCooldown = 100;

	internal ClientConnection(System.Net.Sockets.Socket socket, GameServer server) {
		this.socket = new Shared.Socket(socket);
		this.server = server;
	}

	internal async Task RunAsync() {
		logger.Log("run");
		while (!stop) {
			try {
				Packet request = await socket.RecievePacketAsync();
				Packet? response = null;
				switch (request.Type) {
					case PacketType.Bullets:
						response = server.OnBulletRequest();
						break;
					case PacketType.UpdatePlayer:
						response = server.OnUpdatePlayerRequest(request);
						break;
					case PacketType.GetPlayers:
						response = server.OnGetPlayersRequest(request);
						break;
					case PacketType.Ping:
						response = server.OnPingRequest(request);
						break;
					case PacketType.Map:
						response = server.OnMapRequest();
						break;
					case PacketType.Register:
						response = server.OnRegisterPlayerRequest(request);
						break;
					default:
						logger.Warn("Unknown protocol type (protocol.type=" + request.Type + ")");
						break;
				}
				request.Dispose();
				if(response != null)
					await socket.SendPacketAsync(response);
			} catch (SocketException e) {
				logger.Error(e.Message);
				Dispose();
				break;
			}
		}
	}

	public override string? ToString()
		=> socket.ToString();

	public void Dispose() {
		logger.Log("stopping");
		stop = true;
		socket.Dispose();
	}
}