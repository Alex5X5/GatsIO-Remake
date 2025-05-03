namespace ShGame.game.Client;

using ShGame.game.Net;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Net;
using ShGame.game.Client.Rendering;
using Silk.NET.Windowing;
using Silk.NET.Input;

#pragma warning disable CS8500 //insert spaces instead of tabs

public class Client2 {

	private RendererGl renderer;
	private IWindow window;
	private IInputContext inputContext;

	private bool stop = false;
	internal bool keyUp = false;
	internal bool keyDown = false;
	internal bool keyLeft = false;
	internal bool keyRight = false;

	private readonly Logger logger;
	private readonly LoggingLevel mlvl = new("Client");

	private NetHandler? netHandler;

	internal Player2 player;
	unsafe public Player2[] foreignPlayers;
	unsafe internal Obstacle2[] obstacles;

	private Thread renderThread = new(() => { });
	private Thread connectionThread = new(() => { });
	private Thread playerMoveThread = new(() => { });


	public Client2() : this(5000) { }


	public Client2(int port) : this(GameServer.GetLocalIP(), port) { }


	public Client2(IPAddress address, int port) {
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
		player=new Player2(new Vector3d(100, 100, 0), 100, BitConverter.ToInt64(temp, 0));
		foreignPlayers=new Player2[GameServer.MAX_PLAYER_COUNT];
        obstacles=new Obstacle2[GameServer.OBSTACLE_COUNT];
        for (int i = 0; i<GameServer.MAX_PLAYER_COUNT; i++)
            foreignPlayers[i] = new Player2(new Vector3d(0, 0, 0), -1, 1);
        for (int i = 0; i<GameServer.OBSTACLE_COUNT; i++)
            obstacles[i] = new Obstacle2(new Vector3d(300, 500, 0),1);
		StartThreads(address, port);
	}


	private void SetVisible() {
		logger.Log("setting vivible");
		
		var options = WindowOptions.Default;
		options.Size = new Silk.NET.Maths.Vector2D<int>(RendererGl.WIDTH, RendererGl.HEIGHT);
		options.Title = "ShGame";

		window = Silk.NET.Windowing.Window.Create(options);
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

		window.Run();

		return;
	}

	private unsafe void StartThreads(IPAddress address, int port) {
		logger.Log("start threads!");
		connectionThread=new Thread(
			() => {
				netHandler = new(address, port);
				if (NetHandlerConnected())
					netHandler.GetMap(ref obstacles);
				Console.WriteLine(player);
				while (!stop && NetHandlerConnected()) {
					logger.Log("asking for players");
					netHandler.ExchangePlayers(player, ref foreignPlayers);
					Thread.Sleep(100);
				}
				netHandler?.Dispose();
			}
		);
		connectionThread.Start();

		renderThread=new Thread(
				() => {
					SetVisible();
				}
		);
		renderThread.Start();
		logger.Log("started render thread");

		playerMoveThread=new Thread(
				() => {
					//while (!stop)
						//Thread.Sleep(10);
					while (!stop) {
						foreach (Player2 p in foreignPlayers) {
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
		//window.Dispose();
	}


	private class Panel : System.Windows.Forms.Panel {
		public Panel() : base() {
		}
		protected override void OnPaintBackground(PaintEventArgs e) {

		}

		protected override void OnPaint(PaintEventArgs e) {
			//if (!stop)
			//e.Graphics.DrawImage(renderer.Render(ref players, ref player, ref obstacles), 0, 0);
		}
	}
}