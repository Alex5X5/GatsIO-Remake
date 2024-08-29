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

		internal Obstacle[] obstacles;

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
			players=new Player[30];

			//for(int i = 1; i<players.Length; i++) {
			//	players[i]=new Player(new Logic.Vector3d(0, 0, 0), -1);
			//}
			//if(handler != null ) {
			//	ParsableObstacle[] obstacles_ = handler.GetMap().obstacles;
			//	List<Obstacle> list = new List<Obstacle>();
			//	foreach(ParsableObstacle o in obstacles_)
			//		list.Add(new Obstacle(o));
			//	obstacles=list.ToArray();
			//}

			renderer=new Renderer();
			StartThreads();
		}

		private void SetVisible() {
			logger.Log("setting vivible");

			SuspendLayout();

			AutoScaleMode=AutoScaleMode.None;
			ClientSize=new System.Drawing.Size(Renderer.WIDTH, Renderer.HEIGHT);
			Name="Client";
			Text="Client";
			
			FormClosing+=Stop;
			this.KeyDown+=new System.Windows.Forms.KeyEventHandler(this.KeyDown_);
			this.KeyUp+=new System.Windows.Forms.KeyEventHandler(this.KeyUp_);

			ResumeLayout(false);
			PerformLayout();
			logger.Log("performed layout");
		}

		private void StartThreads() {
			connectionThread=new Thread(
				() => {
					handler = new NetHandler();
					if(handler!=null) {
						//ParsableObstacle[] obstacles_ = handler.GetMap().obstacles;
						//List<Obstacle> list = new List<Obstacle>();
						//foreach(ParsableObstacle o in obstacles_)
						//list.Add(new Obstacle(o));
						//obstacles=list.ToArray();
					}
				}
			);
			connectionThread.Start();
			logger.Log("start threads!");
			renderThread = new Thread(
					() => {
						while(!CanRaiseEvents&&!stop) {
							Thread.Sleep(10);
						}
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
						while(!CanRaiseEvents&&!stop) {
							Thread.Sleep(10);
						}
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
			if(e.KeyCode==Keys.W) {
				keyUp=false;
				player.OnKeyEvent(c: this);
			};
			if(e.KeyCode==Keys.S) {
				keyDown=false;
				player.OnKeyEvent(c: this);
			};
			if(e.KeyCode==Keys.A) {
				keyLeft=false;
				player.OnKeyEvent(c: this);
			};
			if(e.KeyCode==Keys.D) {
				keyRight=false;
				player.OnKeyEvent(c: this);
			};
			//logger.Log("key released", new MessageParameter("player", player.toString()));
		}

		private void KeyDown_(Object sender, KeyEventArgs e) {
			if(e.KeyCode==Keys.W) {
				keyUp=true;
				player.OnKeyEvent(c: this);
			};
			if(e.KeyCode==Keys.S) {
				keyDown=true;
				player.OnKeyEvent(c: this);
			};
			if(e.KeyCode==Keys.A) {
				keyLeft=true;
				player.OnKeyEvent(c: this);
			};
			if(e.KeyCode==Keys.D) {
				keyRight=true;
				player.OnKeyEvent(c: this);
			};
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

