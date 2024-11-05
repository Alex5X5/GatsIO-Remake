namespace ShGame.game.Client;

using ShGame.game.Net;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Net;

#pragma warning disable CS8500 //insert spaces instead of tabs

public class Client : Form {
	internal bool keyUp = false;
	internal bool keyDown = false;
	internal bool keyLeft = false;
	internal bool keyRight = false;

	private bool stop = false;

	private readonly Logger logger;

	Client.Panel? panel;
	private readonly Renderer renderer;
	private readonly LoggingLevel mlvl = new("Client");
	private NetHandler? netHandler;

	internal Player player;

	unsafe private Player[] players;
	unsafe internal Obstacle[] obstacles = new Obstacle[20];
	private Thread renderThread = new(() => { });
	private Thread connectionThread = new(() => { });
	private Thread playerMoveThread = new(() => { });

	public Client() : this(5000) { }

	public Client(uint port) : this(GameServer.GetLocalIP(), port) { }

	public Client(IPAddress address, uint port) : base() {
		logger=new Logger(mlvl);
		logger.Log(
			"address port constructor",
			new MessageParameter("address", address.ToString()),
			new MessageParameter("port", port)
		);
		SetVisible();
		//Thread.Sleep(500);
		//handler=new NetHandler();
		byte[] temp = new byte[8];
		new Random().NextBytes(temp);
		player=new Player(new Vector3d(100, 100, 0), 100, BitConverter.ToInt64(temp, 0));
		players=new Player[GameServer.MAX_PLAYER_COUNT];
		for (int i = 0; i<GameServer.MAX_PLAYER_COUNT; i++)
			players[i] = new Player(new Vector3d(0, 0, 0), -1, 1);
		obstacles.Initialize();
		renderer=new Renderer();
		StartThreads(address, port);
	}

	private void SetVisible() {
		logger.Log("setting vivible");

		SuspendLayout();

		AutoScaleMode=AutoScaleMode.None;
		ClientSize=new Size(Renderer.WIDTH, Renderer.HEIGHT);
		Name="Client";
		FormClosing+=Stop;
		KeyDown+=new KeyEventHandler(KeyDown_);
		KeyUp+=new KeyEventHandler(KeyUp_);
		Text="Client";
		panel = new() {
			ClientSize=new Size(Renderer.WIDTH, Renderer.HEIGHT),
			Name="Panel"
		};

		ResumeLayout(false);
		PerformLayout();
		logger.Log("performed layout");
	}

	private unsafe void StartThreads(IPAddress address, uint port) {
		connectionThread=new Thread(
			() => {
				netHandler = new(address, port);

				if (NetHandlerConnected())
					netHandler.GetMap(ref obstacles);
				Console.WriteLine(player);
				while (!stop && NetHandlerConnected()) {
					netHandler.ExchangePlayers(player, ref players);
					Thread.Sleep(500);
				}
				netHandler?.Dispose();
			}
		);
		connectionThread.Start();

		logger.Log("start threads!");
		renderThread=new Thread(
				() => {
					while (!CanRaiseEvents&&!stop)
						Thread.Sleep(10);
					while (!stop) {
						Thread.Sleep(30);
						Invalidate();
					}
				}
		);
		renderThread.Start();
		logger.Log("started render thread");

		playerMoveThread=new Thread(
				() => {
					while (!CanRaiseEvents&&!stop)
						Thread.Sleep(10);
					while (!stop) {
						foreach (Player p in players) {
							if (p!=null)
								if (p.Health!=-1)
									p.Move();
						}
						if (player!=null)
							if (player.Health!=-1)
								player.Move();
						Thread.Sleep(10);
					}
				}
		);
		playerMoveThread.Start();
		logger.Log("started player move thread");
	}

	private void KeyUp_(object? sender, KeyEventArgs e) {
		switch (e.KeyCode) {
			case Keys.W:
				keyUp=false;
				break;
			case Keys.S:
				keyDown=false;
				break;
			case Keys.A:
				keyLeft=false;
				break;
			case Keys.D:
				keyRight=false;
				break;
		}
		player.OnKeyEvent(c: this);
	}

	private void KeyDown_(object? sender, KeyEventArgs e) {
		switch (e.KeyCode) {
			case Keys.W:
				keyUp=true;
				break;
			case Keys.S:
				keyDown=true;
				break;
			case Keys.A:
				keyLeft=true;
				break;
			case Keys.D:
				keyRight=true;
				break;
			case Keys.Escape:
				Stop(this, null);
				break;
		}
		player.OnKeyEvent(c: this);
	}

	protected override void OnPaint(PaintEventArgs e) {
		unsafe {
			fixed (Obstacle[]* ob = &obstacles)
				if (!stop)
					e.Graphics.DrawImage(renderer.Render(ref players, ref player, ob), 0, 0);
		}
	}

	protected override void OnPaintBackground(PaintEventArgs e) {
		//Don't allow the background to paint
	}

private bool NetHandlerConnected() {
		if (netHandler != null)
			if (netHandler.Connected)
				return true;
		return false;
	}

	private unsafe void Stop(object? sender, FormClosingEventArgs? e) {
		stop=true;
		if (sender==this) {
			logger.Log("stopping");
			renderer.Dispose();
			Dispose();
			Thread.Sleep(500);
		}
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