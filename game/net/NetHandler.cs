namespace ShGame.game.Net;

using System.Net;
using System.Net.Sockets;
using System.Threading;

#pragma warning disable CS8500 //a pointer is created to a variable of an unmanaged type


public class NetHandler:Socket {

	private readonly IPAddress IP = new([0, 0, 0, 0]);
	private readonly uint PORT = 100;

	private bool stop = false;

	//private readonly NetworkStream input;
	//private readonly NetworkStream output;

	//private readonly BinaryFormatter formatter = new BinaryFormatter();

	private readonly Logger logger = new(new LoggingLevel("NetHandler"));

	internal NetHandler() : this(5000) {
		logger.Log("enpty constructor");
	}

	internal NetHandler(uint port) : this(GameServer.GetLocalIP(), port) {
		logger.Log("port constructor");
	}

    public NetHandler(IPAddress address, uint port) : base(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp) {
		logger.Log("port addresss constructor");
		logger.Log(address.AddressFamily.ToString());
		IP = address;
		PORT = port;
		//logger.Log(ToString());
		IPEndPoint point = new(address, (int)port);
		logger.Log(point.ToString());
		try {
			logger.Log("trying to connect, point="+point.ToString()+", family="+point.Address.AddressFamily);
			//Connect_(address, port);
			Connect(point);
		} catch(SocketException e) {
			logger.Warn("failed to connect (reason="+e.ToString()+")");
		} 
		if(Connected)
			logger.Log("connected!");
		else
			logger.Warn("no connection");
			//output=new NetworkStream(this, FileAccess.Write);
			//output.Flush();
			//input=new NetworkStream(this, FileAccess.Read);
	}

	private bool Connect_(IPAddress address, int port) {
		IPEndPoint point = new(address, port);
		logger.Log("connecting "+point);
		IAsyncResult result = BeginConnect(point, null, null);
		bool success = result.AsyncWaitHandle.WaitOne(10000, true);
		while (!success)
			Thread.Sleep(100);
		logger.Log(Convert.ToString(Connected));
		if (!Connected) {
			EndConnect(result);
			return success;
		} else {
			Close();
			throw new SocketException(0, "Connect Timeout");
		}
		return success;
	}

	private byte[] RecievePacket(){
		if(!Connected)
			throw new ConnectException("not connected");
		byte[] buffer = new byte[Protocoll.PACKET_BYTE_LENGTH];
		int recieved = 0;
		while (recieved < Protocoll.PACKET_BYTE_LENGTH && !stop) {
			int bytes;
			try {
				bytes = Receive(buffer, recieved, Protocoll.PACKET_BYTE_LENGTH - recieved, SocketFlags.None);
			} catch (Exception) {
				break;
			}
			if (bytes == 0)
				break;
			recieved += bytes;
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

	public unsafe void GetMap(ref Obstacle[] obstacles) {
		logger.Log("getting map");
		SendPacket(Protocoll.PreparePacket(Headers.MAP));
		byte[] packet = RecievePacket();
		int counter = 0;
        if (packet!=null)
            for (int i = 0; i<GameServer.OBSTACLE_COUNT; i++)
				fixed (Obstacle* ptr = &obstacles[i])
					Obstacle.DeserializeObstacle(&packet, ptr, i*Obstacle.OBSTACLE_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET);
		foreach(Obstacle obstacle in obstacles)
			Console.WriteLine(obstacle.ToString());
	}

	public unsafe void ExchangePlayers(Player p, ref Player[] players) {
		logger.Log("exchanging players", [new MessageParameter("player",p.ToString())]);
		byte[] send = Protocoll.PreparePacket(Headers.PLAYER);
		Player.SerializePlayer(&send, &p, Protocoll.PAYLOAD_OFFSET);
		Send(send);
		byte[] packet = RecievePacket();
        if (packet != null)
            for (int i = 0; i<GameServer.MAX_PLAYER_COUNT-1; i++) {
				logger.Log("deserializing player",new MessageParameter("player",players[i].ToString()));
				Player temp = new();
				Player.DeserializePlayer(&packet, &temp, i*Player.PLAYER_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET);
				if(temp != null && temp.PlayerUUID != p.PlayerUUID)
					players[i] = temp;
				logger.Log("deserialized player", new MessageParameter("player", players[i].ToString()));
			}
	}

	internal void Stop() {
		logger.Log("stopping");
		stop = true;
		Close();
		Dispose();
	}

	public override string ToString() {
		return "sh_game.game.net.NetHandler:[ip="+IP.ToString()+", port="+Convert.ToString(PORT)+"]";
	}
}
