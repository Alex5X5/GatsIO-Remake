using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ShGame.game.Net;

internal class GameServer:Socket {

    #region fields

    private bool stop = false;
	private readonly Logger logger;
	private readonly ServerConsole console;

	public const int MAP_WIDTH = 1000, MAP_HEIGHT = 1000, MAX_PLAYER_COUNT = 10, OBSTACLE_COUNT = 20;

	internal readonly ServerConnection[] clients = new ServerConnection[MAX_PLAYER_COUNT];
	internal Player[] players = new Player[MAX_PLAYER_COUNT];
	private readonly Obstacle[] obstacles = new Obstacle[OBSTACLE_COUNT];

	public GameServer() : this(100) { }
	public GameServer(int port) : this(GetLocalhost(), (uint)Math.Abs(port)) { }

	#endregion fields

    #region constructors


    public GameServer(IPAddress address, uint port) : base(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp){
		logger = new Logger(new LoggingLevel("GameServer"));
		console = new(this);
		new Thread(
				() => console.ShowDialog()
		).Start();
		//console.Writeline("test");
		logger.Log(
			"address port constructor",
			new MessageParameter("address",address.ToString()),
			new MessageParameter("port",port)
		);
		SpreadObstacles();
		//fill the players with 'invalid' players so the serializers don't face nullpointers
		players.Initialize();
		foreach (Obstacle obstacle in obstacles)
			Console.WriteLine(obstacle.ToString());
		//create an IPEndpoint wwith the givven address and the given port and bind the server to it
		IPEndPoint point = new(address, (int)port);
		logger.Log("binding, endPoint = "+point.ToString());
		Bind(point);
		logger.Log("bound endPoint="+point.ToString());
		logger.Log(Convert.ToString(IsBound));
		new Thread(
				start: Run
		).Start();
	}

	private void OnAccept(Socket s) {
		Console.WriteLine("[Server]:OnAccept("+s.ToString()+")");
		//search for a slot for the new connection
		for(int i = 0; i<clients.Length; i++) {
			if(clients[i]==null) {
				//close the newly created socket an create a Serverconnection from the socket's information so the ServerConnection is bound to the incoming connection
				clients[i]=new ServerConnection(s.DuplicateAndClose(Environment.ProcessId), this);
				break;
			}
		}
	}

    #endregion constructors

	#region request events

    internal byte[] OnMapRequest() {
		byte[] result = Protocoll.PreparePacket(Protocoll.MAP_HEADER);
		int counter = 0;
		//serialize all of the obstacles into the packet
		for(int i = 0; i<20; i++)
			Obstacle.SerializeObstacleCountable(ref result, ref obstacles[i], ref counter);
		return result;
	}

	internal byte[]? OnPingRequest(byte[] packet) {
		if(Protocoll.UnloadPing(packet)) {
			logger.Log("answering ping");
			return Protocoll.LoadPing(false);
		} else
			logger.Log("not answering ping");
		return null;
	}

	internal byte[]? OnPlayerRequest(byte[] packet) {
		//check if the packet actually is a player request
		if(Protocoll.AnalyzePacket(packet)==Protocoll.PLAYER_HEADER) {
			//create a temporary player and read it's properties from the packet
			Player temp = new(null, 0, 0);
			Player.DeserializePlayer(ref packet, ref temp, Protocoll.PAYLOAD_OFFSET);
			bool playerFound=false;
			//loop through the current players and check whether the recieved player's and the stored player's UUID match
			for(int i = 0; i<MAX_PLAYER_COUNT; i++) {
				if(players[i].PlayerUUID == temp.PlayerUUID) {
					//if the player was found in the stored players, set its direction to the direction of the recieved player
					players[i].Dir = temp.Dir.Nor();
					playerFound=true;
					break;
				}
			}
			//since the player isn't known, loop through the players array serarch for an empty slot for the new player
			if (!playerFound) {
				logger.Log("registering new player",new MessageParameter("UUID",temp.PlayerUUID));
                for (int i = 0; i<MAX_PLAYER_COUNT; i++) {
                    //the slot is considered empty if the players health is -1
                    if (players[i].Health==-1) {
                        players[i].Health=100;
                        players[i].Dir=temp.Dir.Nor();
                        break;
                    }
                }
            }
			//prepare a new packet
			byte[] result = Protocoll.PreparePacket(Protocoll.PLAYER_HEADER);
			int count = Protocoll.PAYLOAD_OFFSET;
			//add all of the players to the packet
			for(int i = 0; i<MAX_PLAYER_COUNT-1; i++) {
				Player.SerializePlayerCountable(ref result, ref players[i], ref count);
			}
			return result;
		} else
			logger.Log("wrong request");
		return null;
	}

    #endregion request events

    private async void Run() {
        logger.Log("run");
		//loop until the server is about to stop
        while (!stop) {
            Listen(1);
            while (!stop) {
                try {
                    Socket clientConnection = await Task.Factory.FromAsync(this.BeginAccept, this.EndAccept, null);
                    _=Task.Run(() => OnAccept(clientConnection));
                } catch (Exception e) {
                    if (!stop) {
                        Console.WriteLine(e.ToString());
                    } else {
                        break;
                    }
                }
            }
        }
    }

    public void Stop() {
		logger.Log("stopping");
		//the Run Thread only stops if stop is set to true
		stop = true;
		foreach(ServerConnection c in clients)
			c?.Stop();
		Thread.Sleep(1000);
		//the socket must be closed and disposed or the garbage collector won't free the memory
		Close();
		Dispose();
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

	public static IPAddress GetLocalIPv4() => 
		GetLocalhost().MapToIPv4();
    public static IPAddress GetLocalIPv6() =>
        GetLocalhost().MapToIPv6();
    public static IPAddress GetLocalhost(){
		NetworkInterface ni = NetworkInterface.GetAllNetworkInterfaces()[1];
		return ni.GetIPProperties().UnicastAddresses[^1].Address;
	}
}

