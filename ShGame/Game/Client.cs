namespace ShGame.Game;

using ShGame.Game.Net;
using System.Threading;
using System.Net;
using ShGame.Game.Client.Rendering;
using Silk.NET.Windowing;
using Silk.NET.Input;
using System.Numerics;
using ShGame.Game.Logic.Math;
using System.Linq;

//using Silk.NET.GLFW;
//using Silk.NET.Windowing;

//#pragma warning disable CS8500 //insert spaces instead of tabs

public class Client {

	public static readonly int SCREEN_PIXEL_WIDTH = Silk.NET.Windowing.Monitor.GetMainMonitor(null).Bounds.Size.Y;
	public static readonly int SCREEN_PIXEL_HEIGHT = Silk.NET.Windowing.Monitor.GetMainMonitor(null).Bounds.Size.X;

	private readonly RendererGl renderer;
	private NetHandler? NetHandler;
	public GameInstance Game;
	
	private IWindow? window;
	private IInputContext? inputContext;
	public Vector2 mousePos = new(0, 0);

	private bool stop = false;
	internal bool keyUp = false;
	internal bool keyDown = false;
	internal bool keyLeft = false;
	internal bool keyRight = false;
	internal bool mouseLeftDown = false;
	internal bool mouseRightDown = false;

	private readonly Logger logger;

	internal Player ControlledPlayer;


	private Thread renderThread = new(() => { });
	private Thread connectionThread = new(() => { });
	private Thread moveThread = new(() => { });
	private Thread bulletThread = new(() => { });
	private Thread abilityThread = new(() => { });

	//public Vector2 GetCursorPosition() => inputContext.Mice[0].Position;

	public Client() : this(5000) { }


	public Client(int port) : this(GameServer.GetLocalIP(), port) { }


	public Client(IPAddress address, int port) {
		logger=new Logger(new LoggingLevel("Client"));
		logger.Log(
			"address port constructor",
			new MessageParameter("address", address.ToString()),
			new MessageParameter("port", port)
		);
		renderer=new();
		//foreignPlayers=new Player[GameServer.MAX_PLAYER_COUNT];
		//obstacles=new Obstacle[GameServer.OBSTACLE_COUNT];
		//bullets=new Bullet[GameServer.BULLET_COUNT];
		//for (int i = 0; i<GameServer.MAX_PLAYER_COUNT; i++)
		//	foreignPlayers[i] = new Player(new Vector3d(0, 0, 0), -1, 1);
		//for (int i = 0; i<GameServer.OBSTACLE_COUNT; i++)
		//	obstacles[i] = new Obstacle(this, new Vector3d(100, 100, 0), 1);
		//for (int i = 0; i<GameServer.BULLET_COUNT; i++)
		//	bullets[i] = new Bullet(null, null, 10, 20);
		
		Game = new(this);
		//ControlledPlayer=new(new(100,100,0),100,1);
		//ControlledPlayer = Game.Players[0];
		//ControlledPlayer = new Player(new Vector3d(100, 100, 0), 100, BitConverter.ToInt64(temp, 0));
		Game.StartAllLoops();
		StartThreads(address, port);
		SetVisible();
	}

	public Vector3d WindowRelativePosition(Vector2 pos) =>
		new Vector3d(
			pos.X = window.Size.X-pos.X*((window!=null ? window.Size.X : 0)/GameServer.MAP_WIDTH),
			pos.Y = window.Size.Y-pos.Y*((window!=null ? window.Size.Y : 0)/GameServer.MAP_HEIGHT),
			0
		);


	private void SetVisible() {
		logger.Log("setting vivible");

		var options = WindowOptions.Default;
		options.Size = new Silk.NET.Maths.Vector2D<int>(GameServer.MAP_WIDTH, GameServer.MAP_HEIGHT);
		options.Title = "ShGame";

		window = Window.Create(options);
		window.Load += 
			() => renderer.OnLoad(window, this);
		window.Load += 
			() => {
				inputContext = window.CreateInput();
				for (int i = 0; i < inputContext.Keyboards.Count; i++)
					inputContext.Keyboards[i].KeyDown += KeyDown_;
				for (int i = 0; i < inputContext.Keyboards.Count; i++)
					inputContext.Keyboards[i].KeyUp += KeyUp_;
				foreach (IMouse mouse in inputContext.Mice) {
					mouse.MouseDown += OnMouseDown;
					mouse.MouseUp += OnMouseUp;
					mouse.MouseMove += OnMouseMove;
			}
		};

		window.Closing+=Stop;

		window.Render += (deltaTime) =>
			renderer.OnRender(deltaTime, window, this);
		window.Closing += OnClosing;

		window.Run();

		return;
	}

	public unsafe void OnClosing() {
		stop = true;
		Game.Stop();
		NetHandler.Stop();
		//for (int i = 0; i<obstacles.Length; i++)
		//	obstacles[i].Dispose();
		//for (int i = 0; i<foreignPlayers.Length; i++)
		//	foreignPlayers[i].Dispose();
		//for (int i = 0; i<bullets.Length; i++)
		//	bullets[i].Dispose();
		//ControlledPlayer.Dispose();
	}

	private unsafe void StartThreads(IPAddress address, int port) {
		logger.Log("start threads!");
		connectionThread=new Thread(
			() => {
				//create a Nethandler with the information of this client
				NetHandler = new(address, port);
				if (NetHandlerConnected()) {
					//get the positions of the obstacles from the server
					NetHandler.GetMap(this, ref Game.Obstacles);
					NetHandler.RegisterToServer(ref ControlledPlayer, ref Game.Players);
				}
				//Console.WriteLine(ControlledPlayer);
				while (!stop && NetHandlerConnected()) {
					logger.Log("asking for players");
					NetHandler.ExchangePlayers(ControlledPlayer, Game.Players, false);
					Thread.Sleep(50);
				}
				NetHandler?.Dispose();
			}
		);
		connectionThread.Start();
		logger.Log("started connection thread:"+connectionThread.IsAlive);

		//logger.Log("started render thread");

		//moveThread=new Thread(
		//		() => {
		//			while (!stop) {
		//				foreach (Player p in foreignPlayers) {
		//					if (p!=null)
		//						if (p.Health!=-1)
		//							p.Move();
		//				}
		//				if (ControlledPlayer!=null)
		//					if (ControlledPlayer.Health!=-1) {
		//						//logger.Log("moving player ", new MessageParameter("player", player.ToString()));
		//						ControlledPlayer.Move();
		//					}
		//				Thread.Sleep(1000/GameInstance.TARGET_TPS);
		//			}
		//		}
		//);
		//moveThread.Start();
		//logger.Log("started player move thread");

		//bulletThread=new Thread(
		//	() => {
		//		while (!stop) {
		//			foreach (Player p in foreignPlayers) {
		//				if (p.IsShooting == 0x1 && p.weaponCooldownTicksDone==0) {
		//					AllocBullet(p);
		//					p.weaponCooldownTicksDone = p.WeaponCooldownTicks;
		//				}
		//				if (p.weaponCooldownTicksDone>0)
		//					p.weaponCooldownTicksDone--;
		//			}
		//			if (ControlledPlayer.IsShooting == 0x1 && ControlledPlayer.weaponCooldownTicksDone==0) {
		//				AllocBullet(ControlledPlayer);
		//				ControlledPlayer.weaponCooldownTicksDone = ControlledPlayer.WeaponCooldownTicks;
		//			}
		//			if (ControlledPlayer.weaponCooldownTicksDone>0)
		//				ControlledPlayer.weaponCooldownTicksDone--;
		//			foreach (Bullet b in bullets) {
		//				b.Move();
		//				b.CheckObstacleCollision(obstacles);
		//			}
		//			Thread.Sleep(1000/GameInstance.TARGET_TPS);
		//		}
		//	}
		//);
		//bulletThread.Start();
	}

	//private void AllocBullet(Player p) {
	//	logger.Log("alloc bullet");
	//	for (int i = 0; i<bullets.Length; i++) {
	//		logger.Log(bullets[i].Speed.ToString());
	//		if (bullets[i].Speed==0x0) {
	//			//Console.WriteLine("v:"+(new Vector3d(mousePos.X, mousePos.Y, 0).Sub(p.Pos)));
	//			//Console.WriteLine("v2:"+player.Pos.Cpy().Add(new Vector3d(Player.SIZE/2, Player.SIZE/2, 0)));

	//			bullets[i].Pos.Set(p.Pos.Cpy().Add(new Vector3d(Player.SIZE/2, Player.SIZE/2, 0)));
	//			Vector3d temp1 = new(mousePos.X, mousePos.Y, 0);
	//			temp1.Sub(p.Pos);
	//			temp1.Nor();
	//			bullets[i].Dir.Set(temp1);
	//			bullets[i].Speed = p.InitialBulletSpeed;
	//			Console.WriteLine(bullets[i]);
	//			break;
	//		}
	//	}
	//}

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
		if (ControlledPlayer!=null)
			ControlledPlayer.OnKeyEvent(this);
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
		if(ControlledPlayer!=null)
			ControlledPlayer.OnKeyEvent(this);
		//Console.WriteLine("key up, p:"+player.ToString());
	}

	private void OnMouseDown(IMouse cursor, MouseButton button) {
		Console.WriteLine("Mouse Down! "+mousePos);
		if (button==MouseButton.Left) {
			mouseLeftDown=true;
			if(ControlledPlayer!=null)
				ControlledPlayer.IsShooting = 0x1;
		}
		if (button==MouseButton.Right)
			mouseRightDown=true;
	}

	private void OnMouseUp(IMouse cursor, MouseButton button) {
		Console.WriteLine("Mouse Up! "+mousePos);
		if (button==MouseButton.Left) {
			mouseLeftDown=false;
			if(ControlledPlayer!=null)
				ControlledPlayer.IsShooting = 0x0;
		}
		if (button==MouseButton.Right) {
			mouseRightDown=false;
		}
	}

	private void OnMouseMove(IMouse cursor, Vector2 pos) {
		mousePos.X = pos.X*(GameServer.MAP_WIDTH/window.Size.X);
		mousePos.Y = GameServer.MAP_HEIGHT-pos.Y*(GameServer.MAP_HEIGHT/window.Size.Y);
		//mousePos = pos-new Vector2(window.Position.X,window.Size.Y-window.Position.Y);
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