using System.Net.Sockets;
using System.Threading;

namespace ShGame.game.Net;

internal class ServerConnection:Socket {

	private bool stop = false;
	private readonly Logger logger;
	internal int disposalCooldown = 100;
	internal readonly int id;

	internal ServerConnection(SocketInformation info, GameServer gs, int id_) : base(info) {
		logger = new Logger(new LoggingLevel("ServerConnection"));
		logger.Log("Constructor");
		id = id_;
		new Thread(
				() => Run(gs)
		).Start();
	}

	private bool RecievePacket(ref byte[] buffer) {
		if (!Connected)
			//if the socket isn't connected, return that the atempt to recieve a packet was unsuccessfull
			return false;
		//ins case the buffer doesn't have the standard packet length resize it
		Array.Resize(ref buffer, Protocoll.PACKET_BYTE_LENGTH);
		buffer.Initialize();
		//the number of bytes recieved so far together
		int recieved = 0;
		while (recieved < Protocoll.PACKET_BYTE_LENGTH && !stop) {
			//the number of bytes recieved during this listening cycle
			int bytes;
			try {
				//recive bytes from the socket and write them to the buffer
				//the offset is the difference from the recieved bytes so far and the expected amount of bytes
				bytes = Receive(buffer, recieved, Protocoll.PACKET_BYTE_LENGTH - recieved, SocketFlags.None);
			} catch (Exception){
				//if an exception occurs return that the attemt to recieve a packet was incomplete
				return false;
			}
			//if no additional bytes could be recieved stop the listening
			if (bytes == 0)
				break;
			//add the amount of recieved bytes in this cycle to all the recieved bytes so far
			recieved += bytes;
        }
        //if the amount of recieved bytes matches the amount of expected bytes, return that the attemt to recieve a packet was successfull
        return recieved >= Protocoll.PACKET_BYTE_LENGTH;
	}

	private void SendPacket(byte[]? send) {
		if(send==null)
			return;
		_=Send(send);
	}

	private void Run(GameServer gs) {
		logger.Log("run");
		byte[] buffer = new byte[Protocoll.PACKET_BYTE_LENGTH];
		while(!stop) {
			try {
				//recieve a packet from the client
				if (!RecievePacket(ref buffer))
					continue;
				//depending on the packet type, send a request to the server and send back the result
				switch (Protocoll.AnalyzePacket(buffer)) {
					case Headers.PING:
						SendPacket(gs.OnPingRequest(buffer));
						break;

					case Headers.PLAYER:
						SendPacket(gs.OnPlayerRequest(buffer));
						break;

					case Headers.MAP:
						SendPacket(gs.OnMapRequest());
						break;

					default:
						Console.WriteLine("[ServerConnection]:type of recieved Protocoll is unknown (protocoll.type="+Protocoll.AnalyzePacket(buffer)+")");
						break;
				}
			} catch(SocketException e) {
				logger.Error(e.Message);
				disposalCooldown--;
				break;
			}
		}
	}

	public override string? ToString() => base.ToString();

	internal void Stop() {
		logger.Log("stopping");
		stop=true;
		Close();
		Dispose();
	}
}
