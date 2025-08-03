namespace ShGame.Game.Net;

using ShGame.Game;

using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class NetHandler : Socket {

    private readonly Logger logger = new(new LoggingLevel("NetHandler"));
    
    private readonly IPAddress IP = new([0, 0, 0, 0]);
    private readonly int PORT = 100;

    private bool stop = false;

    internal NetHandler() : this(5000) {
        logger.Log("enpty constructor");
    }

    internal NetHandler(int port) : this(GameServer.GetLocalIP(), port) {
        logger.Log("port constructor");
    }

    public NetHandler(IPAddress address, int port) : base(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp) {
        logger.Log("port addresss constructor");
        logger.Log(address.AddressFamily.ToString());
        IP = IPAddress.Parse("192.168.2.112");
        PORT = port;
        IPEndPoint point = new(address, port);
        logger.Log(point.ToString());
        try {
            logger.Log("trying to connect, point="+point.ToString()+", family="+point.Address.AddressFamily);
            Connect(point);
        } catch (SocketException e) {
            logger.Warn("failed to connect (reason="+e.ToString()+")");
        }
        if (Connected)
            logger.Log("connected!");
        else
            logger.Warn("no connection");
    }

    private bool Connect_(IPAddress address, int port) {
        IPEndPoint point = new(address, port);
        logger.Log("connecting "+point);
        IAsyncResult result = BeginConnect(point, null, null);
        bool success = result.AsyncWaitHandle.WaitOne(10, true);
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

    private byte[] RecievePacket() {
        if (!Connected)
            throw new ConnectException("not connected");
        byte[] buffer = new byte[Protocoll.PACKET_BYTE_LENGTH];
        int recieved = 0;
        while (recieved < Protocoll.PACKET_BYTE_LENGTH && !stop) {
            int recievedBytesCount;
            try {
                recievedBytesCount = Receive(buffer, recieved, Protocoll.PACKET_BYTE_LENGTH - recieved, SocketFlags.None);
            } catch (Exception) {
                break;
            }
            if (recievedBytesCount == 0)
                break;
            recieved += recievedBytesCount;
        }
        return buffer;
    }

    private void SendPacket(byte[] send) {
        if (send==null)
            throw new ArgumentException("cannot send null");
        try {
            _=Send(send);
        } catch (SocketException e) {
            logger.Error(e.ToString());
        }
    }

    public unsafe void GetMap(Client client, ref Obstacle[] obstacles) {
        logger.Log("getting map");
        SendPacket(Protocoll.PreparePacket(Headers.MAP));
        byte[] packet = RecievePacket();
        int counter = 0;
        if (packet!=null)
            for (int i = 0; i<GameServer.OBSTACLE_COUNT; i++)
                fixed(byte* ptr = &packet[i*Obstacle.OBSTACLE_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET])
                    Obstacle.DeserializeObstacle(client, ptr, ref obstacles[i], 0);
        foreach (Obstacle obstacle in obstacles)
            Console.WriteLine(obstacle.ToString());
    }

    public unsafe void RegisterToServer(ref Player controlledPlayer, ref Player[] allPlayers) {
		SendPacket(Protocoll.PreparePacket(Headers.REGISTER_PLAYER));
		//ask the server to add a new player to its list and create a new Player with the recieved id
		byte[] packet = RecievePacket();
        short id = 0;
        fixed (byte* ptr = &packet[0])
            id = Player.DeserializePlayerId(ptr, Protocoll.PAYLOAD_OFFSET);
        controlledPlayer = allPlayers.First(p => p.Health==-1);
		//exchange all players with the client's player beeing the newly created one

		ExchangePlayers(controlledPlayer, allPlayers, true);
		//controlledPlayer = allPlayers.FirstOrDefault(p => p.PlayerUUID==id, controlledPlayer);

	}

	public unsafe void ExchangePlayers(Player controlledPlayer, Player[] players, bool includeControlledPlayer) {
        //logger.Log("exchanging players", [new MessageParameter("player",p.ToString())]);
        byte[] send = Protocoll.PreparePacket(Headers.PLAYER);
        fixed(byte* ptr = &send[0])
        Player.SerializePlayer(ptr, controlledPlayer, Protocoll.PAYLOAD_OFFSET);
        try {
            Send(send);
            byte[] packet = RecievePacket();
            if (packet != null)
                for (int i = 0; i<GameServer.MAX_PLAYER_COUNT; i++) {
                    //logger.Log("deserializing player", new MessageParameter("player", players[i].ToString()));
                    fixed (byte* ptr = &packet[0])
                        if (Player.DeserializePlayerId(ptr, i*Player.PLAYER_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET)!=controlledPlayer.PlayerUUID|includeControlledPlayer)
                            Player.DeserializePlayer(ptr, players[i], i*Player.PLAYER_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET);
                    //logger.Log("deserialized player", new MessageParameter("player", players[i].ToString()));
                }
        } catch (Exception e) {
            logger.Log(e.ToString());
        }
    }

    public unsafe void GetBullets(Bullet[] bullets) {
		//logger.Log("getting bullets", [new MessageParameter("player",p.ToString())]);
		byte[] send = Protocoll.PreparePacket(Headers.BULLET);
		try {
			Send(send);
			byte[] packet = RecievePacket();
			if (packet != null)
				for (int i = 0; i<GameServer.MAX_PLAYER_COUNT; i++) {
					//logger.Log("deserializing player", new MessageParameter("player", bullets[i].ToString()));
					fixed (byte* ptr = &packet[0])
						Bullet.DeserializeBullet(ptr, bullets[i], i*Player.PLAYER_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET);
					//logger.Log("deserialized player", new MessageParameter("player", bullets[i].ToString()));
				}
		} catch (Exception e) {
			logger.Log(e.ToString());
		}

	}

	internal void Stop() {
        logger.Log("stopping");
        stop = true;
        Close();
        Dispose();
    }

    public override string ToString() {
        return "sh_game.Game.net.NetHandler:[ip="+IP.ToString()+", port="+Convert.ToString(PORT)+"]";
    }
}
