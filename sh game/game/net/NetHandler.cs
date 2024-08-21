using sh_game.game.client;
using sh_game.game.net.protocoll;

using SimpleLogging.logging;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace sh_game.game.server {
	public class NetHandler:Socket {

		private readonly IPAddress IP = null;
		private readonly int PORT = 100;

		//private readonly NetworkStream input;
		//private readonly NetworkStream output;

		private readonly BinaryFormatter formatter = new BinaryFormatter();

		private readonly Logger logger = new Logger(new LoggingLevel("NetHandler"));

		internal NetHandler():this(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], 100) {
			logger.Log("Constructor 1");
		}

		internal NetHandler(string ip, int port) : this(IPAddress.Parse(ip), port) {
			logger.Log("Constructor2");
		}

		internal NetHandler(IPAddress ip, int port) : base(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp) {
			logger.Log("Constructor 3");
			IP=ip;
			PORT=port;
			try {
				logger.Log("trying to connect");
				Connect(new IPEndPoint(IP, PORT));
			} catch(SocketException e) {
				logger.Log("failed to bind (reason="+e.ToString()+")");
			}
			if(Connected) {
				//output=new NetworkStream(this, FileAccess.Write);
				//output.Flush();
				//input=new NetworkStream(this, FileAccess.Read);
			}
		}

		private Protocoll RecievePacket() {
			if(!Connected)
				return null;
			byte[] buffer = new byte[2048];
			int recieved = 0;
			while(recieved<2084) {
				int bytes = Receive(buffer, recieved, 2048-recieved, SocketFlags.None);
				if(bytes==0)
					break;
				recieved+=bytes;
			}
			using(MemoryStream ms = new MemoryStream(buffer)) {
				return (Protocoll)formatter.Deserialize(ms);
			}
		}

		private void SendPacket(Protocoll send) {
			if(send==null)
				return;
			using(MemoryStream ms = new MemoryStream()) {
				formatter.Serialize(ms, send);
				try {
					_=Send(ms.ToArray());
				} catch(SocketException e) {
					logger.Error(e.ToString());
				}
			}
		}

		public MapProtocoll GetMap() {
			logger.Log("getting map");
			SendPacket(new MapProtocoll(true,(Obstacle[])null));
			return (MapProtocoll)RecievePacket();
		}

		public PlayerProtocoll ExchangePlayers(Player p) {
			logger.Log("exchanging players");
			SendPacket(new PlayerProtocoll(true, p));
			return (PlayerProtocoll)RecievePacket();
		}

		public override string ToString() {
			return "sh_game.game.net.NetHandler:[ip="+IP.ToString()+";port="+Convert.ToString(PORT)+"]";
		}
	}
}
