namespace ShGame.game.Net;

using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Loader;
using System.Threading;

public class NetHandler:Socket {

	private readonly IPAddress IP = new([0, 0, 0, 0]);
	private readonly int PORT = 100;

	//private readonly NetworkStream input;
	//private readonly NetworkStream output;

	//private readonly BinaryFormatter formatter = new BinaryFormatter();

	private readonly Logger logger = new(new LoggingLevel("NetHandler"));

	internal NetHandler() : this(100) {
		logger.Log("enpty constructor");
	}

	internal NetHandler(int port) : this(GameServer.GetLocalIPv4(), port) {
		logger.Log("port constructor");
	}

	internal NetHandler(IPAddress address, int port) : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
		logger.Log("port addresss constructor");
		IP=address;
		PORT=port;
		//logger.Log(ToString());
		IPEndPoint point = new(address, port);
		logger.Log(point.ToString());
		try {
			logger.Log("trying to connect");
			//Connect_(address, port);
			Connect(point);
		} catch(SocketException e) {
			logger.Log("failed to bind (reason="+e.ToString()+")");
		} 
		if(Connected)
			logger.Log("connected!");
		else
			logger.Log("no connection");
			//output=new NetworkStream(this, FileAccess.Write);
			//output.Flush();
			//input=new NetworkStream(this, FileAccess.Read);
	}

	private bool Connect_(IPAddress address, int port) {
		IPEndPoint point = new(address, port);
		logger.Log("connecting "+point);
		IAsyncResult result = BeginConnect(point, null, null);
		bool success = result.AsyncWaitHandle.WaitOne(5000, true);
		while (!success)
			Thread.Sleep(100);
		logger.Log(Convert.ToString(Connected));
		if (Connected) {
			EndConnect(result);
			return success;
		} else {
			Close();
			throw new SocketException(0, "Connect Timeout");
		}
		return success;
	}

	private byte[] RecievePacket() {
		if(!Connected)
			throw new ConnectException("not connected");
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
			Console.WriteLine("NetHandler:"+players[i]);
		}
	}

	public override string ToString() {
		return "sh_game.game.net.NetHandler:[ip="+IP.ToString()+", port="+Convert.ToString(PORT)+"]";
	}
}
