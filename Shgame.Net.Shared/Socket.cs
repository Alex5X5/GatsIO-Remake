namespace ShGame.Net.Shared;

using ShGame.Net.Shared.Exceptions;
using ShGame.Util;

using SimpleLogging.logging;

using System.Net;
using System.Net.Sockets;

public class Socket : IDisposable {

	private readonly Logger logger = new(new LoggingLevel("Socket"));

	private readonly IPAddress address = new([0, 0, 0, 0]);
	private readonly int port = 100;

	private bool stop = false;

	private readonly System.Net.Sockets.Socket socket;

	public bool Connected => socket.Connected;

	public Socket(Configuration config) {
		address = IPAddress.Parse(config.Address);
		port = config.Port;
		socket = new(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		IPEndPoint point = new(address, port);
		try {
			logger.Log("trying to connect, point="+point.ToString()+", family="+point.Address.AddressFamily);
			socket.Connect(point);
		} catch (SocketException e) {
			logger.Warn("failed to connect (reason="+e.ToString()+")");
		}
		LogConnected();
	}

	public Socket(System.Net.Sockets.Socket socket) {
		this.socket = socket;
		LogConnected();
	}

	private void LogConnected() {
		if (Connected)
			logger.Log("connected!");
		else
			logger.Warn("no connection");
	}

	public async Task<Packet> RecievePacketAsync() {
		ThrowIfNotConnected();
		byte[] buffer = new byte[Protocoll.PACKET_BYTE_LENGTH];
		int recieved = 0;
		while (recieved < Protocoll.PACKET_BYTE_LENGTH && !stop) {
			int recievedBytesCount;
			try {
				ArraySegment<byte> segment = new(buffer, recieved, Protocoll.PACKET_BYTE_LENGTH - recieved);
				recievedBytesCount = await socket.ReceiveAsync(segment, SocketFlags.None);
			} catch (Exception) {
				break;
			}
			if (recievedBytesCount == 0)
				break;
			recieved += recievedBytesCount;
		}
		unsafe {
			fixed (byte* ptr = &buffer[0])
				return new Packet(ptr);
		}
	}

	public async Task SendPacketAsync(Packet packet) {
		ThrowIfNotConnected();
		byte[] buffer = packet.GetBufferArray();
		packet.Dispose();
		int sent = 0;
		while (sent < Protocoll.PACKET_BYTE_LENGTH && !stop) {
			int sentBytesCount;
			try {
				ArraySegment<byte> segment = new(buffer, sent, Protocoll.PACKET_BYTE_LENGTH - sent);
				sentBytesCount = await socket.SendAsync(segment, SocketFlags.None);
			} catch (Exception) {
				break;
			}
			if (sentBytesCount == 0)
				break;
			sent += sentBytesCount;
		}
	}

	public void Dispose() {
		Stop();
		GC.SuppressFinalize(this);
	}

	public void Stop() {
		socket.Disconnect(false);
		socket.Dispose();
		Dispose();
	}

	private void ThrowIfNotConnected() {
		if (!Connected)
			throw new ConnectException("not connected");
	}
}
