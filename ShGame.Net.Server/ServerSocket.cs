namespace ShGame.Net.Shared;

using ShGame.Util;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class ServerSocket : IDisposable {

	private readonly Logger logger = new(new LoggingLevel("Socket"));

	private readonly IPAddress address = new([0, 0, 0, 0]);
	private readonly int port = 100;

	private bool stop = false;

	private readonly System.Net.Sockets.Socket socket;

	private Action<System.Net.Sockets.Socket> acceptHandler;

	public bool Bound => socket.IsBound;

	public ServerSocket(Configuration config, Action<System.Net.Sockets.Socket> acceptHandler) {
		address = IPAddress.Parse(config.Address);
		port = config.Port;
		socket = new(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		this.acceptHandler = acceptHandler;

		IPEndPoint point = new(address, port);
		
		try {
			logger.Log("trying to bind, point="+point.ToString()+", family="+point.Address.AddressFamily);
			socket.Bind(point);
		} catch (SocketException e) {
			logger.Warn("failed to bind (reason="+e.ToString()+")");
		}
		if (Bound)
			logger.Log("bound");
		else
			logger.Warn("not bound");
	}

	public async Task StartAcceptLoopAsync() {
		logger.Log("accept loop");
		while (!stop) {
			socket.Listen(1);
			while (!stop) {
				try {
					System.Net.Sockets.Socket incoming = await socket.AcceptAsync();
					await Task.Run(
						() => {
							logger.Log("accepted!");
							acceptHandler(incoming);
						}
					);
				} catch (Exception e) {
					if (!stop) {
						logger.Error(e.ToString());
					} else {
						break;
					}
				}
			}
		}
	}

	public void Dispose() {
		GC.SuppressFinalize(this);
	}

	public void Stop() {
		socket.Disconnect(false);
		socket.Dispose();
		Dispose();
	}
}
