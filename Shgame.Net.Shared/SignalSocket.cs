namespace ShGame.Net.Shared;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

public class SignalSocket : IDisposable {

	private Socket socket;

	private SignalHandler? recievedHandler;

	private bool stop = false;

	private readonly SignalRoutingDirection direction;

	private CancellationTokenSource InterrupSouce;
	
	public SignalSocket(IPAddress address, int port, SignalRoutingDirection direction, SignalHandler? recievedHandler = null) {
		socket = new(address, port);
		this.direction = direction;
		
		InterrupSouce = new();

		if (CanRecieveEvent) {
			this.recievedHandler = recievedHandler;
			_ = RecieveLoop();
		}
	}

	private bool CanSendEvent =>
		direction == SignalRoutingDirection.ToOther | direction == SignalRoutingDirection.BothWays;
	private bool CanRecieveEvent =>
		direction == SignalRoutingDirection.FromOther | direction == SignalRoutingDirection.BothWays;

	public void SendEvent(Packet packet) {
		if (CanSendEvent)
			_ = SendEventAsync(packet);
	}

	private async Task SendEventAsync(Packet packet) {
		try {
			await socket.SendPacketAsync(packet);
		} finally {
			packet.Dispose();
		}
	}

	private async Task RecieveLoop() {
		while(!stop) {
			Packet packet = new();
			try{
				packet = await socket.RecievePacketAsync();
				recievedHandler?.Invoke(packet);
			} finally {
				packet.Dispose();
			}

		}
	}

	public void Dispose() {
		InterrupSouce.Dispose();
		socket.Dispose();
		GC.SuppressFinalize(this);
	}

	public void Stop() {

	}

	public delegate void SignalHandler(Packet packet);
}
