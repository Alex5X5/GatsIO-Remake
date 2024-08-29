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
		private readonly BinaryFormatter formatter;
		private bool stop = false;
		private readonly Logger logger;

		internal ServerConnection(SocketInformation info, GameServer gs) : base(info) {
			logger = new Logger(new LoggingLevel("ServerConnection"));
			logger.Log("Constructor");
			formatter = new BinaryFormatter();
			//output = new NetworkStream(this, FileAccess.Write);
			//output.Flush();
			//input = new NetworkStream(this, FileAccess.Read);
			new Thread(
					() => Run(gs)
			).Start();
		}

		private Protocoll RecievePacket() {
			logger.Log("trying to recieve packet");
			byte[] buffer = new byte[1024];
			int recieved = 0;
			while(recieved<1) {
				int bytes = Receive(buffer, recieved, 1024-recieved, SocketFlags.None);
				logger.Log("recieved bytes", new MessageParameter(buffer.ToString()));
				if(bytes==0)
					break;
				recieved+=bytes;
			}
			using(MemoryStream ms = new MemoryStream(buffer)) {
				Protocoll p = (Protocoll)formatter.Deserialize(ms);
				logger.Log("recieved packet", new MessageParameter("packet", p.ToString()));
				return p;
			}
		}

		private void SendPacket(Protocoll send) {
			if(send==null)
				return;
			using(MemoryStream ms = new MemoryStream()) {
				formatter.Serialize(ms, send);
				_=Send(ms.ToArray());
			}
		}

		private void Run(GameServer gs) {
			logger.Log("run");
			while(!stop) {
				Protocoll protocoll = RecievePacket();
				switch(protocoll.type) {
					case ProtocollType.Ping:
						SendPacket(gs.OnPingRequest(protocoll));
						break;
					case ProtocollType.Player:
						SendPacket(gs.OnPlayerRequest(protocoll));
						break;
					case ProtocollType.Map:
						SendPacket(gs.OnMapRequest());
						break;
					default:
						Console.WriteLine("[ServerConnection]:type of recieved Protocoll is unknown (protocoll.type="+protocoll.type+")");
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
