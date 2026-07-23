namespace ShGame.Net.Services;

using ShGame.Game.Models;
using ShGame.Util.Services;

using System.Net;
using System.Net.Sockets;
using System.Threading;


public class ClientNetworkService : IDisposable {

    private readonly ConfigurationService configurationService;
	private readonly Logger logger = new(new LoggingLevel("NetHandler"));

    private Socket? socket;

    private bool stop = false;

    public bool Connected => socket?.Connected ?? false;

    public ClientNetworkService(ConfigurationService configurationService) {
        this.configurationService = configurationService;
    }

	public void Connect() {
		socket = new(configurationService.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		logger.Log(configurationService.Address.AddressFamily.ToString());
		IPEndPoint point = new(configurationService.Address, configurationService.Port);
		logger.Log(point.ToString());
		try {
			logger.Log("trying to connect, point="+point.ToString()+", family="+point.Address.AddressFamily);
			socket.Connect(point);
		} catch (SocketException e) {
			logger.Warn("failed to connect (reason="+e.ToString()+")");
		}
		if (socket.Connected)
			logger.Log("connected!");
		else
			logger.Warn("no connection");
	}

    private byte[] RecievePacket() {
        if (!socket.Connected)
            throw new ConnectException("not connected");
        byte[] buffer = new byte[Protocoll.PACKET_BYTE_LENGTH];
        int recieved = 0;
        while (recieved < Protocoll.PACKET_BYTE_LENGTH && !stop) {
            int recievedBytesCount;
            try {
                recievedBytesCount = socket.Receive(buffer, recieved, Protocoll.PACKET_BYTE_LENGTH - recieved, SocketFlags.None);
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
            _=socket.Send(send);
        } catch (SocketException e) {
            logger.Error(e.ToString());
        }
    }

    public unsafe void GetMap(Player contorlledPlayer, ref Obstacle[] obstacles) {
        logger.Log("getting map");
        SendPacket(Protocoll.PreparePacket(Headers.MAP));
        byte[] packet = RecievePacket();
        int counter = 0;
        if (packet!=null)
            for (int i = 0; i<Constants.OBSTACLE_COUNT; i++)
                fixed(byte* ptr = &packet[i*Obstacle.OBSTACLE_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET])
                    Obstacle.DeserializeObstacle(ptr, ref obstacles[i], 0);
        foreach (Obstacle obstacle in obstacles)
            Console.WriteLine(obstacle.ToString());
    }

    public unsafe void RegisterToServer(ref Player controlledPlayer, ref Player[] allPlayers) {
		SendPacket(Protocoll.PreparePacket(Headers.REGISTER_PLAYER));
		//ask the server to add a new player to its list and create a new Player with the recieved id
		byte[] packet = RecievePacket();
        //short id = Player.DeserializePlayerId((byte*)&packet, Protocoll.PAYLOAD_OFFSET);
        fixed(byte* ptr = &packet[0])
            Player.DeserializePlayer(ptr, controlledPlayer, Protocoll.PAYLOAD_OFFSET);
        //controlledPlayer = allPlayers.First(p => p.PlayerUUID==id);
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
			socket?.Send(send);
            byte[] packet = RecievePacket();
            if (packet != null)
                for (int i = 0; i<Constants.PLAYER_COUNT; i++) {
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
			socket?.Send(send);
			byte[] packet = RecievePacket();
			if (packet != null)
				for (int i = 0; i<Constants.BULLET_COUNT; i++) {
					//logger.Log("deserializing player", new MessageParameter("player", bullets[i].ToString()));
					fixed (byte* ptr = &packet[0])
						Bullet.DeserializeBullet(ptr, bullets[i], i*Player.PLAYER_BYTE_LENGTH+Protocoll.PAYLOAD_OFFSET);
					//logger.Log("deserialized player", new MessageParameter("player", bullets[i].ToString()));
				}
		} catch (Exception e) {
			logger.Log(e.ToString());
		}

	}

	public void Stop() {
        logger.Log("stopping");
        stop = true;
		socket?.Close();
    }

    public override string ToString() {
        return "sh_game.Game.net.NetHandler:[ip="+configurationService.Address.ToString()+", port="+Convert.ToString(configurationService.Port)+"]";
    }

	public void Dispose() {
        socket?.Dispose();
	}
}
