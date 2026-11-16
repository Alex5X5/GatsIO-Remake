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
				Packet response = request.Type switch {
					PacketType.Bullets => server.OnGetBulletsRequest(),
					PacketType.UpdatePlayer => server.OnUpdatePlayerRequest(request),
					PacketType.GetPlayers => server.OnGetPlayersRequest(request),
					PacketType.Ping => server.OnPingRequest(request),
					PacketType.Map => server.OnMapRequest(),
					PacketType.Register => server.OnRegisterPlayerRequest(request),
					_ => throw new InvalidOperationException($"invalid protocol type {request.Type}")
				};
				request.Dispose();
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