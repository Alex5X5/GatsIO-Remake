using System.Net.Sockets;
using System.Threading;

namespace ShGame.game.Net;

internal class ServerConnection:Socket {

	private bool stop = false;
	private readonly Logger logger;

	internal ServerConnection(SocketInformation info, GameServer gs) : base(info) {
		logger=new Logger(new LoggingLevel("ServerConnection"));
		logger.Log("Constructor");

		new Thread(
				() => Run(gs)
		).Start();
	}

	private bool RecievePacket(ref byte[] buffer) {
		if (!Connected)
			//if the socket isn't connected, return that the attemt to recieve a packet was incomplete
			return false;
		//ins case the buffer doesn't have the standard packet length refize
		Array.Resize(ref buffer, Protocoll.PACKET_BYTE_LENGTH);
		buffer.Initialize();
        //the number of bytes recieved so far togeather
        int recieved = 0;
		while (recieved < Protocoll.PACKET_BYTE_LENGTH && !stop) {
			//the number of bytes recieved in this listening cycle
			int bytes;
			try {
				//recive bytes from the socket and write them to the buffer
				//the offset id the difference from the recieved bytes so far and the expected amount of bytes
				bytes = Receive(buffer, recieved, Protocoll.PACKET_BYTE_LENGTH - recieved, SocketFlags.None);
			} catch (Exception){
                //if an exception occurs return that the attemt to recieve a packet was incomplete
                return false;
			}
			//if no additional bytes could be recieved sto the listening
			if (bytes == 0)
				break;
			//add the amount of recieved bytes in this cycle to all the recieved bytes so far
			recieved += bytes;
		}
		if(recieved == Protocoll.PACKET_BYTE_LENGTH)
            //if the amoutnt of recieved bytes matches the amount of expected bytes, return that the attemt to recieve a packet was successfull
            return true;
		else
			return false;
	}

	private void SendPacket(byte[]? send) {
		if(send==null)
			return;
		try {
			_=Send(send);
		} catch (SocketException e) {
			logger.Error(e.ToString());
		} catch (ObjectDisposedException) {
			
		}
	}

	private void Run(GameServer gs) {
		logger.Log("run");
		byte[] buffer = new byte[Protocoll.PACKET_BYTE_LENGTH];
		while(!stop) {
			//recieve a packet from the client
			if(RecievePacket(ref buffer));
				//depending on the packet type, send a request to the server and send back the result
				switch(Protocoll.AnalyzePacket(buffer)) {
					case ProtocollType.Ping:
						SendPacket(gs.OnPingRequest(buffer));
						break;

					case ProtocollType.Player:
						SendPacket(gs.OnPlayerRequest(buffer));
						break;

					case ProtocollType.Map:
						SendPacket(gs.OnMapRequest());
						break;

					default:
						Console.WriteLine("[ServerConnection]:type of recieved Protocoll is unknown (protocoll.type="+Protocoll.AnalyzePacket(buffer)+")");
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
