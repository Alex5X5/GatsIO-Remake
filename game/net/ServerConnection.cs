using sh_game.game.net;
using sh_game.game.net.protocoll;

using SimpleLogging.logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sh_game.game.server {
	
	internal class ServerConnection:Socket {

		//private readonly NetworkStream input;
		//private readonly NetworkStream output;
		//private readonly BinaryFormatter formatter;
		private bool stop = false;
		private readonly Logger logger;

		internal ServerConnection(SocketInformation info, GameServer gs) : base(info) {
			logger = new Logger(new LoggingLevel("ServerConnection"));
			logger.Log("Constructor");
			//formatter = new BinaryFormatter();
			//output = new NetworkStream(this, FileAccess.Write);
			//output.Flush();
			//input = new NetworkStream(this, FileAccess.Read);

			new Thread(
					() => Run(gs)
			).Start();
		}

		//private Protocoll RecievePacket() {
		//	logger.Log("trying to recieve packet");
		//	byte[] buffer = new byte[1024];
		//	int recieved = 0;
		//	while(recieved<1) {
		//		int bytes = Receive(buffer, recieved, 1024-recieved, SocketFlags.None);
		//		logger.Log("recieved bytes", new MessageParameter(buffer.ToString()));
		//		if(bytes==0)
		//			break;
		//		recieved+=bytes;
		//	}
		//	using(MemoryStream ms = new MemoryStream(buffer)) {
		//		Protocoll p = (Protocoll)formatter.Deserialize(ms);
		//		logger.Log("recieved packet", new MessageParameter("packet", p.ToString()));
		//		return p;
		//	}
		//}

		private byte[] RecievePacket() {
			if(!Connected)
				return null;
			byte[] buffer = new byte[Protocoll.PACKET_BYTE_LENGTH];
			int recieved = 0;
			while(recieved<Protocoll.PACKET_BYTE_LENGTH) {
				int bytes = Receive(buffer, recieved, Protocoll.PACKET_BYTE_LENGTH-recieved, SocketFlags.None);
				if(bytes==0)
					break;
				recieved+=bytes;
			}
			return buffer;
		}

		private void SendPacket(byte[] send) {
			if(send==null)
				return;
			try {
				_=Send(send);
			} catch(SocketException e) {
				logger.Error(e.ToString());
			}
		}

		private void Run(GameServer gs) {
			logger.Log("run");
			while(!stop) {
				byte[] buffer = RecievePacket();
				switch(Protocoll.AnalyzePacket(buffer)) {

					case Protocoll.PING_HEADER:
						SendPacket(gs.OnPingRequest(buffer));
						break;

					case Protocoll.PLAYER_HEADER:
						SendPacket(gs.OnPlayerRequest(buffer));
						break;
					
					case Protocoll.MAP_HEADER:
						SendPacket(gs.OnMapRequest());
						break;
					
					default:
						Console.WriteLine("[ServerConnection]:type of recieved Protocoll is unknown (protocoll.type="+Protocoll.AnalyzePacket(buffer)+")");
						break;
				}
			}
		}

		public override string ToString() {
			return base.ToString();
		}

		internal void Stop() {
			stop = true;
		}
	}
}
