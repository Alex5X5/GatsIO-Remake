namespace ShGame.game.Client;

using ShGame.game.Net;
using System.Threading;
using System.Net;
using ShGame.game.Client.Rendering;
using Silk.NET.Windowing;
using Silk.NET.Input;
using System.Numerics;

//#pragma warning disable CS8500 //insert spaces instead of tabs

public class Client {

	private readonly RendererGl renderer;
	private IWindow? window;
	private IInputContext? inputContext;

	private bool stop = false;
	internal bool keyUp = false;
	internal bool keyDown = false;
	internal bool keyLeft = false;
	internal bool keyRight = false;

	private readonly Logger logger;
	private readonly LoggingLevel mlvl = new("Client");

	private NetHandler? netHandler;

	internal Player player;
	unsafe public Player[] foreignPlayers;
	unsafe internal Obstacle[] obstacles;

	private Thread renderThread = new(() => { });
	private Thread connectionThread = new(() => { });
	private Thread playerMoveThread = new(() => { });

	public Vector2 GetCursorPosition() => inputContext.Mice[0].Position;


	public Client() : this(5000) { }


	public Client(int port) : this(GameServer.GetLocalIP(), port) { }


	public Client(IPAddress address, int port) {
		logger=new Logger(mlvl);
		logger.Log(
			"address port constructor",
			new MessageParameter("address", address.ToString()),
			new MessageParameter("port", port)
		);
		renderer=new();
		Thread.Sleep(500);
		netHandler=new NetHandler();
		byte[] temp = new byte[8];
		new Random().NextBytes(temp);
		player=new Player(new Vector3d(100, 100, 0), 100, BitConverter.ToInt64(temp, 0));
		foreignPlayers=new Player[GameServer.MAX_PLAYER_COUNT];
		obstacles=new Obstacle[GameServer.OBSTACLE_COUNT];
		for (int i = 0; i<GameServer.MAX_PLAYER_COUNT; i++)
			foreignPlayers[i] = new Player(new Vector3d(0, 0, 0), -1, 1);
		for (int i = 0; i<GameServer.OBSTACLE_COUNT; i++)
			obstacles[i] = new Obstacle(this, new Vector3d(300, 500, 0),1);
		StartThreads(address, port);
	}


	private void SetVisible() {
		logger.Log("setting vivible");
		
		var options = WindowOptions.Default;
		options.Size = new Silk.NET.Maths.Vector2D<int>(RendererGl.WIDTH, RendererGl.HEIGHT);
		options.Title = "ShGame";

		window = Window.Create(options);
		window.Load += ()=> renderer.OnLoad(window, this);
		window.Load += ()=> {
			inputContext = window.CreateInput();
			for (int i = 0; i < inputContext.Keyboards.Count; i++)
				inputContext.Keyboards[i].KeyDown += KeyDown_;
			for (int i = 0; i < inputContext.Keyboards.Count; i++)
				inputContext.Keyboards[i].KeyUp += KeyUp_;
		};
		window.Closing+=Stop;

		window.Render += (double deltaTime) => renderer.OnRender(deltaTime, window, this);
		window.Closing += () => renderer.OnClosing(window, this);

		window.Run();

		return;
	}

	private unsafe void StartThreads(IPAddress address, int port) {
		logger.Log("start threads!");
		connectionThread=new Thread(
			() => {
				netHandler = new(address, port);
				if (NetHandlerConnected())
					netHandler.GetMap(this, ref obstacles);
				Console.WriteLine(player);
				while (!stop && NetHandlerConnected()) {
					//logger.Log("asking for players");
					netHandler.ExchangePlayers(player, ref foreignPlayers);
					Thread.Sleep(100);
				}
				netHandler?.Dispose();
			}
		);
		connectionThread.Start();

		renderThread=new Thread(SetVisible);

		renderThread.Start();
		logger.Log("started render thread");

		playerMoveThread=new Thread(
				() => {
					while (!stop) {
						foreach (Player p in foreignPlayers) {
							if (p!=null)
								if (p.Health!=-1)
									p.Move();
						}
						if (player!=null)
							if (player.Health!=-1) {
								//logger.Log("moving player ", new MessageParameter("player", player.ToString()));
								player.Move();
							}
						Thread.Sleep(10);
					}
				}
		);
		playerMoveThread.Start();
		logger.Log("started player move thread");
	}

	private void KeyUp_(IKeyboard keyboard, Key key, int keyCode) {
		//logger.Log("key "+key+" up");
		switch (key) {
			case Key.W:
				keyUp=false;
				break;
			case Key.S:
				keyDown=false;
				break;
			case Key.A:
				keyLeft=false;
				break;
			case Key.D:
				keyRight=false;
				break;
		}
		player.OnKeyEvent(this);
		//Console.WriteLine("key up, p:"+player.ToString());
	}

	private void KeyDown_(IKeyboard keyboard, Key key, int keyCode) {
		//logger.Log("key "+key+" down");
		switch (key) {
			case Key.W:
				keyUp=true;
				break;
			case Key.S:
				keyDown=true;
				break;
			case Key.A:
				keyLeft=true;
				break;
			case Key.D:
				keyRight=true;
				break;
			case Key.Escape:
				Stop();
				break;
		}
		player.OnKeyEvent(this);
		//Console.WriteLine("key up, p:"+player.ToString());
	}

	private bool NetHandlerConnected() {
		if (netHandler != null)
			if (netHandler.Connected)
				return true;
		return false;
	}

	private unsafe void Stop() {
		stop=true;
		
		inputContext?.Dispose();
	}
}