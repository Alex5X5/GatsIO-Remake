namespace ShGame.game.Net;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS8500 //a pointer is created to a variable of an unmanaged type

internal class GameServer:Socket {

	#region fields

	private bool stop = false;
	private readonly Logger logger;
	private readonly ServerConsole console;


	//some constants
	public const int MAP_WIDTH = Renderer.WIDTH, MAP_HEIGHT = Renderer.HEIGHT;
	public const int OBSTACKLE_ROWS = 5, OBSTACKLE_LINES = 8;
	public const int OBSTACLE_ROW_DISANCE = MAP_WIDTH / OBSTACKLE_ROWS;
	public const int OBSTACLE_LINE_DISTANCE = MAP_HEIGHT / OBSTACKLE_LINES;
	public const int MAX_PLAYER_COUNT = 10, OBSTACLE_COUNT = OBSTACKLE_ROWS*OBSTACKLE_LINES;

	internal readonly ServerConnection[] clients = new ServerConnection[MAX_PLAYER_COUNT];
	internal Player[] players = new Player[MAX_PLAYER_COUNT];
	private unsafe readonly Obstacle[] obstacles = new Obstacle[OBSTACLE_COUNT];

	public GameServer() : this(100) { }
	public GameServer(int port) : this(GetLocalhost(), (uint)Math.Abs(port)) { }

	#endregion fields

	#region constructors


	public GameServer(IPAddress address, uint port) : base(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp){
		logger = new Logger(new LoggingLevel("GameServer"));
		//create a new console that shows the messages of the server
		console = new(this);
		new Thread(
				() => console.ShowDialog()
		).Start();
		Thread.Sleep(100);

		//console.WriteLine("[Gameserver]:address port constructor (address="+address.ToString()+", port=+"+port+")");

		logger.Log(
			"address port constructor",
			new MessageParameter("address",address.ToString()),
			new MessageParameter("port",port)
		);
		SpreadObstacles();
		//fill the players with invalid players so the serializers don't face nullpointers
		players.Initialize();
		//create an IPEndpoint with the given address and the given port and bind the server to the IPEndpoint
		IPEndPoint point = new(address, (int)port);
		logger.Log("binding, endPoint = "+point.ToString());
		Bind(point);
		logger.Log("bound endPoint="+point.ToString());
		logger.Log(Convert.ToString(IsBound));
		//start the main thread
		new Thread(
				start: Run
		).Start();
	}

	private void OnAccept(Socket s) {
		Console.WriteLine("[Server]:OnAccept("+s.ToString()+")");
		//search for a slot for the new connection
		for(int i = 0; i<clients.Length; i++) {
			if(clients[i]==null) {
				//close the newly created socket an create a ServerConnection from the socket's information so the ServerConnection is bound to the incoming connection
				clients[i]=new ServerConnection(s.DuplicateAndClose(Environment.ProcessId), this);
				break;
			}
		}
	}

	#endregion constructors

	#region request events

	internal unsafe byte[] OnMapRequest() {
		//prepare a new Packet
		byte[] result = [];
		result  = Protocoll.PreparePacket(ProtocollType.Map);
		int counter = 0;
		//serialize all of the obstacles into the packet
		for(int i = 0; i<20; i++)
			fixed(Obstacle* ptr = &obstacles[i])
				Obstacle.SerializeObstacleCountable(ref result, ptr, ref counter);
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
		if(Protocoll.AnalyzePacket(packet)==ProtocollType.Player) {
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
			byte[] result = Protocoll.PreparePacket(ProtocollType.Player);
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
					//create a Task that starts to try to accept a socket and in case of success stops to listen
					// the result of the listening is a  socket that is connectedd to a client
					Socket clientConnection = 
						await Task.Factory.FromAsync(BeginAccept, EndAccept, null);
					_=Task.Run(() => OnAccept(clientConnection));
				} catch (Exception e) {
					//if the wasn't caught because the socket server is sopping and it is closing,
					//an rerror must have happened so print it's message
					if (!stop) {
						Console.WriteLine(e.ToString());
						//since the sorver is stopping, ignore the error and break the main loop
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
		//stopp all connections
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
		//spreading obstacles over OBSTACKLE_ROWS rows
		for (int x = 0; x<OBSTACKLE_ROWS; x++)
			//spreading obstacles over OBSTACKLE_LINES lines so there are OBSTACKLE_ROWS*OBSTACKLE_LINES obstacles all together
			for (int y = 0; y<OBSTACKLE_LINES; y++) {
				PlaceObstacles(1 + x, 1 + y, c);
				//c is the position of the obstacle in the arary
				c++;
			}
	}

	private void PlaceObstacles(int x, int y, int c) {
		//since there are OBSTACLE_ROWS rows the distance between the rows has to be MAP_WIDTH/OBSTACLE_ROWS
		x = MAP_WIDTH / OBSTACKLE_ROWS * x;
		//substract half of the distance between the rows so the obstakles get placed in the middle of each row
		x -= MAP_WIDTH / (OBSTACKLE_ROWS*2);
		//since there are OBSTACKLE_LINES lines the distance between the lines has to be MAP_HEIGHT/OBSTACKLE_LINES
		y = MAP_HEIGHT / OBSTACKLE_LINES * y;
		//substract half of the distance between the lines so the obstakles get placed in the middle of each line
		y -= MAP_HEIGHT / (OBSTACKLE_LINES*2);
		Random r = new();
		obstacles[c] = new Obstacle(
			new Vector3d(
				//the obstacles may also be offset by half the distance to the next row/line
				//first add half of the distance between the rows to x
				//then substract a random number between 0 and OBSTACLE_ROW_DISANCE to it
				x + OBSTACLE_ROW_DISANCE / 2 - r.Next(0, OBSTACLE_ROW_DISANCE),
				//first add half of the distance between the lines to y
				//then substract a random number between 0 and the full distance between the lines to it
				y + OBSTACLE_LINE_DISTANCE /2 + r.Next(0, OBSTACLE_LINE_DISTANCE),
				0
			),
			//the upper bound of the type must be 4 becuase 3 ist the maxumum possible tytpe but the upper bound is not included
			r.Next(1, 4)

		);
	}

	public static IPAddress GetLocalIPv4() => 
		GetLocalhost().MapToIPv4();
	public static IPAddress GetLocalIPv6() =>
		GetLocalhost().MapToIPv6();
	public static IPAddress GetLocalhost(){
		NetworkInterface ni = 
			NetworkInterface.GetAllNetworkInterfaces()[1];
		return ni.GetIPProperties().UnicastAddresses[^2].Address;
	}
}

