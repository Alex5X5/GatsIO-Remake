namespace ShGame.game.Net;

using ShGame.game.Client;

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

internal class GameServer:Socket {

	private bool stop = false;
	private readonly Logger logger;
	//private Socket serverSocket;

	public const int MAP_WIDTH = 1000, MAP_HEIGHT = 1000, MAX_PLAYER_COUNT = 10, OBSTACLE_COUNT = 20;

	//private TcpClient
	internal readonly ServerConnection[] clients = new ServerConnection[MAX_PLAYER_COUNT];
	internal Player[] players = new Player[MAX_PLAYER_COUNT];
	private readonly Obstacle[] obstacles = new Obstacle[OBSTACLE_COUNT];

	internal GameServer(int port) : base(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], port).AddressFamily, SocketType.Stream, ProtocolType.Tcp) {
		logger = new Logger(new LoggingLevel("GameServer"));
		logger.Log("constructor");
		SpreadObstacles();
		foreach(Obstacle obstacle in obstacles)
			Console.WriteLine(obstacle.ToString());
		for(int i = 0; i<MAX_PLAYER_COUNT; i++)
			players[i]=new Player(new Logic.Vector3d(0, 0, 0), -1, 1);
		//Console.WriteLine("[Server]:constructor");
		logger.Log("binding", new MessageParameter("server",this.ToString()));
		Bind(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], port));
		new Thread(
				() => Run()
		).Start();
	}

	private void OnAccept(Socket s) {
		Console.WriteLine("[Server]:OnAccept("+s.ToString()+")");
		for(int i = 0; i<clients.Length; i++) {
			if(clients[i]==null) {
				clients[i]=new ServerConnection(s.DuplicateAndClose(Process.GetCurrentProcess().Id), this);
				break;
			}
		}
	}

	private async void Run() {
		logger.Log("run");
		while(!stop) {
			Listen(1);
			while(true) {
				try {
					Socket handler = await Task.Factory.FromAsync(this.BeginAccept, this.EndAccept, null);
					_=Task.Run(() => OnAccept(handler));
				} catch(Exception e) {
					Console.WriteLine(e.ToString());
					break;
				}
			}
		}
	}

	internal byte[] OnMapRequest() {
		byte[] result = Protocoll.PreparePacket(Protocoll.MAP_HEADER);
		int counter = 0;
		for(int i = 0; i<20; i++)
			Obstacle.SerializeObstacleCountable(ref result, ref obstacles[i], ref counter);
		return result;
	}

	internal byte[] OnPingRequest(byte[] packet) {
		if(Protocoll.UnloadPing(packet)) {
			logger.Log("answering ping");
			return Protocoll.LoadPing(false);
		} else
			logger.Log("not answering ping");
		return null;
	}

	internal byte[] OnPlayerRequest(byte[] packet) {
		if(Protocoll.AnalyzePacket(packet)==Protocoll.PLAYER_HEADER) {
			Player player = null;
			Player.DeserializePlayer(ref packet, ref player, Protocoll.PAYLOAD_OFFSET);
			int resultCount=-1;
			for(int i = 0; i<MAX_PLAYER_COUNT-1; i++) {
				if(
					players[i].Health!=-1&&
					players[i].PlayerUUID==player.PlayerUUID
				) {
					players[i].Dir=player.Dir.Nor();
					resultCount=i;
					break;
				}
			}
			if(resultCount!=-1)
				for(int i = 0;i<MAX_PLAYER_COUNT-1;i++) {
					if(players[i].Health==-1) {
						players[i]=player;
						break;
					}
				}
			byte[] result = Protocoll.PreparePacket(Protocoll.PLAYER_HEADER);
			int count = Protocoll.PAYLOAD_OFFSET;
			for(int i = 0; i<MAX_PLAYER_COUNT-1; i++) {
				Player.SerializePlayerCountable(ref result, ref players[i], ref count);
			}
			return result;
		} else
			logger.Log("not request");
		return null;
	}

	public void Stop() {
		stop = true;
		Thread.Sleep(2000);
		foreach(ServerConnection c in clients)
			c.Stop();
	}

	private void SpreadObstacles() {
		logger.Log("generating Obstacles");
		int c = 0;
		//spreading obstacles over 5 rows
		for(int x = 0; x<5; x++)
			//spreading obstacles over 4 lines so there are 20 obstacles all together
			for(int y = 0; y<4; y++) {
                PlaceObstacles(1 + x, 1 + y, c);
				//c is the position of the obstacle in the arary
				c++;
			}
	}

	private void PlaceObstacles(int x, int y, int c) {
        //since there are 5 rows the distance between the rows has to be one fifth of the map width
        x = MAP_WIDTH / 5 * x - MAP_WIDTH / 10;
        //since there are 4 lines the distance between the lines has to be one fourth of the map heigth
        y = MAP_HEIGHT / 4 * y - MAP_HEIGHT / 8;
        Random r = new();
        obstacles[c] = new Obstacle(
			new Vector3d(
                // the obstacles may also be offset by half the distance to the next row/line
                // since there are 5 rows half te distance between them is MapWidth/10
                Math.Floor(MAP_WIDTH / -5.0 + r.Next(0, MAP_WIDTH * 2 / 5) + x),
                Math.Floor(MAP_HEIGHT / -4.0 + r.Next(0, MAP_WIDTH * 2 / 4) + y),
                0
            ),
            //the upper bound must be 4 becuase 3 ist the maxumum possible tytpe but the upper bound is not included
            r.Next(1, 4)

		);
		Console.WriteLine("Pos "+obstacles[c].Pos.ToString());
	}
}

