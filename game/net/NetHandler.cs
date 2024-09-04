namespace ShGame.game.Net;

using System.Net;
using System.Net.Sockets;


public class NetHandler:Socket {

	private readonly IPAddress IP = null;
	private readonly int PORT = 100;

	//private readonly NetworkStream input;
	//private readonly NetworkStream output;

	//private readonly BinaryFormatter formatter = new BinaryFormatter();

	private readonly Logger logger = new Logger(new LoggingLevel("NetHandler"));

	internal NetHandler() : this(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], 100) {
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

	private byte[] RecievePacket() {
		if(!Connected)
			throw new ConnectException("no Connection");
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
			throw new ArgumentException("cannot send null");
		try {
			_=Send(send);
		} catch(SocketException e) {
			logger.Error(e.ToString());
		}
	}

	public void GetMap(ref Obstacle[] obstacles) {
		logger.Log("getting map");
		SendPacket(Protocoll.PreparePacket(Protocoll.MAP_HEADER));
		byte[] temp = RecievePacket();
		int counter = 0;
		for(int i = 0; i<20; i++) {
			if(temp!=null)
				Obstacle.DeserializeObstacleCountable(ref temp, ref obstacles[i], ref counter);
		}
		foreach(Obstacle obstacle in obstacles)
			if(obstacle!=null)
				Console.WriteLine(obstacle.ToString());
	}

	public void ExchangePlayers(Player p, ref Player[] players) {
		logger.Log("exchanging players");
		byte[] send = Protocoll.PreparePacket(Protocoll.PLAYER_HEADER);
		Player.SerializePlayer(ref send, ref p, Protocoll.PAYLOAD_OFFSET);
		Console.WriteLine("NetHandler:"+p);
		byte[] temp = RecievePacket();
		int counter = 0;
		for(int i = 0; i<GameServer.MAX_PLAYER_COUNT-1; i++) {
			Console.WriteLine("NetHandler:"+players[i]);
            if (temp != null)
				Player.DeserializePlayerCountable(ref temp, ref players[i], ref counter);
		}
	}

	public override string ToString() {
		return "sh_game.game.net.NetHandler:[ip="+IP.ToString()+";port="+Convert.ToString(PORT)+"]";
	}
}
