using sh_game.game.net;
using sh_game.game.net.protocoll;
using sh_game.game.server;

using SimpleLogging.logging;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

//using sh_game.game.server.Server;

namespace sh_game.game.client{
	public class Client:Form {

		internal bool keyUp = false;
		internal bool keyDown = false;
		internal bool keyLeft = false;
		internal bool keyRight = false;

		private bool stop = false;

		private readonly Logger logger;

		private readonly Renderer renderer;
		private readonly LoggingLevel mlvl = new LoggingLevel("Client");
		private NetHandler handler;

		internal Player player;
		//internal readonly SemaphoreSlim playersLock;
		private Player[] players;
		internal Obstacle[] obstacles = new Obstacle[20];

		private Thread renderThread;
		private Thread connectionThread;
		private Thread playerMoveThread;

		public Client() : base() {
			logger=new Logger(mlvl);
			logger.Log("Costructor");

			SetVisible();
			//Thread.Sleep(500);
			//handler=new NetHandler();
			byte[] temp = new byte[8];
			new Random().NextBytes(temp);
			player=new Player(new Logic.Vector3d(100, 100, 0), 100, BitConverter.ToInt64(temp, 0));
			players=new Player[GameServer.MAX_PLAYER_COUNT];

			for(int i = 0; i<GameServer.MAX_PLAYER_COUNT; i++)
				players[i]=new Player(new Logic.Vector3d(0, 0, 0), -1, 1);
			for(int i = 0; i<obstacles.Length; i++)
				obstacles[i]=new Obstacle(null, 0);
			renderer=new Renderer();
			StartThreads();
		}

		private void SetVisible() {
			logger.Log("setting vivible");

			SuspendLayout();

			AutoScaleMode=AutoScaleMode.None;
			ClientSize=new Size(Renderer.WIDTH, Renderer.HEIGHT);
			Name="Client";
			Text="Client";
			
			FormClosing+=Stop;
			KeyDown+=new KeyEventHandler(KeyDown_);
			KeyUp+=new KeyEventHandler(KeyUp_);

			ResumeLayout(false);
			PerformLayout();
			logger.Log("performed layout");
		}

		private void StartThreads() {
			connectionThread=new Thread(
				() => {
					handler = new NetHandler();
					handler?.GetMap(ref obstacles);
					Console.WriteLine(player);
					handler?.ExchangePlayers(player, ref players);
					if(handler!=null) {
						
					}
				}
			);
			connectionThread.Start();

			logger.Log("start threads!");
			renderThread = new Thread(
					() => {
						while(!CanRaiseEvents&&!stop)
							Thread.Sleep(10);
						while(!stop) {
							Thread.Sleep(30);
							Invalidate();
						}
					}
			);
			renderThread.Start();
			logger.Log("started render thread");

			playerMoveThread=new Thread(
					() => {
						while(!CanRaiseEvents&&!stop)
							Thread.Sleep(10);
						while(!stop) {
							foreach(Player p in players) {
								if(p!=null)
									if(p.Health!=-1)
										p.Move();
							}
							if(player!=null)
								if(player.Health!=-1)
									player.Move();
							Thread.Sleep(10);
						}
					}
			);
			playerMoveThread.Start();
			logger.Log("started player move thread");
		}

		private void KeyUp_(Object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
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
			//logger.Log("key released", new MessageParameter("player", player.toString()));
		}

		private void KeyDown_(Object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
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
			//logger.Log("key released", new MessageParameter("player", player.toString()));
		}

		protected override void OnPaint(PaintEventArgs e) {
			e.Graphics.DrawImage(renderer.Render(ref players, ref player, ref obstacles), 0, 0);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) {
			//Don't allow the background to paint
		}

		private void Stop(object sender, FormClosingEventArgs e) {
			stop = true;
			if(sender==this) {
				logger.Log("stopping");
				renderer.Dispose();
				Thread.Sleep(500);
				Environment.Exit(0);
				//System.Stop();
			}
		}
	}
}
