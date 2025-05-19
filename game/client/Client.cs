namespace ShGame.game.Client;

using ShGame.game.Net;
using System.Threading;
using System.Net;
using ShGame.game.Client.Rendering;
using Silk.NET.Windowing;
using Silk.NET.Input;
using System.Numerics;
//using Silk.NET.GLFW;
//using Silk.NET.Windowing;

//#pragma warning disable CS8500 //insert spaces instead of tabs

public class Client {

	public static readonly int SCREEN_PIXEL_WIDTH = Silk.NET.Windowing.Monitor.GetMainMonitor(null).Bounds.Size.Y;
	public static readonly int SCREEN_PIXEL_HEIGHT = Silk.NET.Windowing.Monitor.GetMainMonitor(null).Bounds.Size.X;

	private readonly RendererGl renderer;
	private IWindow? window;
	private IInputContext? inputContext;

	private bool stop = false;
	internal bool keyUp = false;
	internal bool keyDown = false;
	internal bool keyLeft = false;
	internal bool keyRight = false;
	internal bool mouseLeftDown = false;
	internal bool mouseRightDown = false;

	public Vector2 mousePos = new(0,0);

	private readonly Logger logger;
	private readonly LoggingLevel mlvl = new("Client");

	private NetHandler? NetHandler;

	internal Player player;
	public Player[] foreignPlayers;
	public Obstacle[] obstacles;
	public Bullet[] bullets;


	private Thread renderThread = new(() => { });
	private Thread connectionThread = new(() => { });
	private Thread moveThread = new(() => { });
	private Thread bulletThread = new(() => { });
	private Thread abilityThread = new(() => { });

	//public Vector2 GetCursorPosition() => inputContext.Mice[0].Position;

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
		NetHandler=new NetHandler();
		byte[] temp = new byte[8];
		new Random().NextBytes(temp);
		player=new Player(new Vector3d(100, 100, 0), 100, BitConverter.ToInt64(temp, 0));
		foreignPlayers=new Player[GameServer.MAX_PLAYER_COUNT];
		obstacles=new Obstacle[GameServer.OBSTACLE_COUNT];
		bullets=new Bullet[GameServer.BULLET_COUNT];
		for (int i = 0; i<GameServer.MAX_PLAYER_COUNT; i++)
			foreignPlayers[i] = new Player(new Vector3d(0, 0, 0), -1, 1);
		for (int i = 0; i<GameServer.OBSTACLE_COUNT; i++)
			obstacles[i] = new Obstacle(this, new Vector3d(100, 100, 0),1);
		for (int i = 0; i<GameServer.BULLET_COUNT; i++)
			bullets[i] = new Bullet(null, null, 10, 20);
		StartThreads(address, port);
	}

	public Vector3d WindowRelativePosition(Vector2 pos) =>
		new Vector3d(
			pos.X = window.Size.X-pos.X*((window!=null ? window.Size.X : 0)/(GameServer.MAP_WIDTH)),
			pos.Y = window.Size.Y-pos.Y*((window!=null ? window.Size.Y : 0)/(GameServer.MAP_HEIGHT)),
			0
		);


	private void SetVisible() {
		logger.Log("setting vivible");

		var options = WindowOptions.Default;
		options.Size = new Silk.NET.Maths.Vector2D<int>(RendererGl.WIDTH, RendererGl.HEIGHT);
		options.Title = "ShGame";

		window = Window.Create(options);
		window.Load += () =>
				renderer.OnLoad(window, this);
		window.Load += () => {
			inputContext = window.CreateInput();
			for (int i = 0; i < inputContext.Keyboards.Count; i++)
				inputContext.Keyboards[i].KeyDown += KeyDown_;
			for (int i = 0; i < inputContext.Keyboards.Count; i++)
				inputContext.Keyboards[i].KeyUp += KeyUp_;
			Console.WriteLine(inputContext);
			foreach (IMouse mouse in inputContext.Mice) {
				mouse.MouseDown += OnMouseDown;
				mouse.MouseUp += OnMouseUp;
				mouse.MouseMove += OnMouseMove;
			}
		};

		window.Closing+=Stop;

		window.Render += (double deltaTime) =>
				renderer.OnRender(deltaTime, window, this);
		window.Closing += () => 
				renderer.OnClosing(window, this);

		window.Run();

		return;
	}

	private unsafe void StartThreads(IPAddress address, int port) {
		logger.Log("start threads!");
		connectionThread=new Thread(
			() => {
					NetHandler = new(address, port);
					if (NetHandlerConnected())
						NetHandler.GetMap(this, ref obstacles);
					Console.WriteLine(player);
					while (!stop && NetHandlerConnected()) {
						//logger.Log("asking for players");
						NetHandler.ExchangePlayers(player, ref foreignPlayers);
						Thread.Sleep(50);
					}
					NetHandler?.Dispose();
			}
		);
		connectionThread.Start();

		renderThread=new Thread(SetVisible);

		renderThread.Start();
		logger.Log("started render thread");

		moveThread=new Thread(
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
		moveThread.Start();
		logger.Log("started player move thread");

		bulletThread=new Thread(
			() => {
				while (!stop) {
					foreach (Player p in foreignPlayers) {
						if (p.shooting == 0x1 && p.weaponCooldownTicksDone==0) {
							AllocBullet(p);
							p.weaponCooldownTicksDone = p.weaponCooldownTicks;
						}
						if (p.weaponCooldownTicksDone>0)
							p.weaponCooldownTicksDone--;
					}
					if (player.shooting == 0x1 && player.weaponCooldownTicksDone==0) {
						AllocBullet(player);
						player.weaponCooldownTicksDone = player.weaponCooldownTicks;
					}
					if (player.weaponCooldownTicksDone>0)
						player.weaponCooldownTicksDone--;
					foreach (Bullet b in bullets) {
						b.Move();
					}
					Thread.Sleep(1000/GameServer.TARGET_TPS);
				}
			}
		);
		bulletThread.Start();
	}

	private void AllocBullet(Player p) {
		logger.Log("alloc bullet");
		for(int i=0; i<bullets.Length; i++) {
			logger.Log(bullets[i].Speed.ToString());
			if (bullets[i].Speed==0x0) {
				//Console.WriteLine("v:"+(new Vector3d(mousePos.X, mousePos.Y, 0).Sub(p.Pos)));
				//Console.WriteLine("v2:"+player.Pos.Cpy().Add(new Vector3d(Player.SIZE/2, Player.SIZE/2, 0)));
				
				bullets[i].Pos.Set(p.Pos.Cpy().Add(new Vector3d(Player.SIZE/2, Player.SIZE/2, 0)));
				Vector3d temp1 = new(mousePos.X, mousePos.Y, 0);
				temp1.Sub(p.Pos);
				temp1.Nor();
				bullets[i].Dir.Set(temp1);
				bullets[i].Speed = p.default_shoot_speed;
				Console.WriteLine(bullets[i]);
				break;
			}
		}
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

	private void OnMouseDown(IMouse cursor, MouseButton button) {
		Console.WriteLine("Mouse Down! "+mousePos);
		if (button==MouseButton.Left) {
			mouseLeftDown=true;
			player.shooting = 0x1;
		}
		if (button==MouseButton.Right)
			mouseRightDown=true;
	}

	private void OnMouseUp(IMouse cursor, MouseButton button) {
		Console.WriteLine("Mouse Up! "+mousePos);
		if (button==MouseButton.Left)
			mouseLeftDown=false;
			player.shooting = 0x0;
		if (button==MouseButton.Right) {
			mouseRightDown=false;
		}
	}

	private void OnMouseMove(IMouse cursor, System.Numerics.Vector2 pos) {
		mousePos = pos;
		//Console.WriteLine("I Moved! "+mousePos);
	}

	private bool NetHandlerConnected() {
		if (NetHandler != null)
			if (NetHandler.Connected)
				return true;
		return false;
	}

	private unsafe void Stop() {
		stop=true;
		
		inputContext?.Dispose();
	}
}