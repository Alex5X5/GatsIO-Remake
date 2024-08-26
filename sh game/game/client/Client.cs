using sh_game.game.net.protocoll;
using sh_game.game.server;

using SimpleLogging.logging;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

//using sh_game.game.server.Server;

namespace sh_game.game.client{
	public class Client:Form {

		[NonSerialized]
		internal bool keyUp = false;
		[NonSerialized]
		internal bool keyDown = false;
		[NonSerialized]
		internal bool keyLeft = false;
		[NonSerialized]
		internal bool keyRight = false;

		internal static readonly int WIDTH = 1000, HEIGHT = 1000;
		private bool stop = false;

		private readonly Logger logger;

		private Panel panel;
		private readonly Renderer renderer;
		private readonly LoggingLevel mlvl = new LoggingLevel("Client");
		private readonly NetHandler handler;

		internal readonly Player player;
		//internal readonly SemaphoreSlim playersLock;
		internal Player[] players;

		internal Obstacle[] obstacles;

		private Thread renderThread;
		private Thread connectionThread;

		public Client() : base() {
			logger=new Logger(mlvl);
			logger.Log("Costructor");

			SetVisible();
			//Thread.Sleep(500);
			handler = new NetHandler();
			player = new Player(new Logic.Vector3d(100,100,0));
			//if(handler != null ) {
			//	ParsableObstacle[] obstacles_ = handler.GetMap().obstacles;
			//	List<Obstacle> list = new List<Obstacle>();
			//	foreach(ParsableObstacle o in obstacles_)
			//		list.Add(new Obstacle(o));
			//	obstacles=list.ToArray();
			//}

			//Console.WriteLine("[Client]:handler="+handler.ToString());
			renderer=new Renderer();
			StartThreads();
		}

		private void SetVisible() {
			logger.Log("setting vivible");
			//logger.Log("")
			panel=new Panel();
			//panel.BackColor = System.Drawing.Color.FromArgb(100,100,100);
			panel.Paint+=new PaintEventHandler(Paint_);

			SuspendLayout();

			panel.Location=new System.Drawing.Point(0, 0);
			panel.Name="panel";
			panel.Size=new System.Drawing.Size(WIDTH, HEIGHT);
			panel.BorderStyle=BorderStyle.FixedSingle;
			//this.panel.TabIndex=0;

			//AutoScaleDimensions=new System.Drawing.SizeF(9F, 20F);
			//AutoScaleMode=AutoScaleMode.Font;
			AutoScaleMode=AutoScaleMode.None;
			ClientSize=new System.Drawing.Size(WIDTH, HEIGHT);
			Controls.Add(this.panel);
			Name="Client";
			Text="Client";
			FormClosing+=Stop;
			ResumeLayout(false);
			PerformLayout();
			logger.Log("performed layout");
		}

		private void StartThreads() {
			connectionThread = new Thread(
				() => {
					if(handler!=null) {
						ParsableObstacle[] obstacles_ = handler.GetMap().obstacles;
						List<Obstacle> list = new List<Obstacle>();
						foreach(ParsableObstacle o in obstacles_)
							list.Add(new Obstacle(o));
						obstacles=list.ToArray();
					}
				}
			);

			connectionThread.Start();
			logger.Log("start threads!");
			renderThread = new Thread(
					() => {
						while(!this.CanRaiseEvents&&!stop) {
							Thread.Sleep(100);
						}
						while(!stop)
							renderer.Render(this);
					}
			);
			renderThread.Start();
			//renderThread.Start();
		}

		public void KeyUp(Object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.W) {
				keyUp=false;
				player.OnKeyEvent(this);
			};
			if(e.KeyCode==Keys.S) {
				keyDown=false;
				player.OnKeyEvent(this);
			};
			if(e.KeyCode==Keys.A) {
				keyLeft=false;
				player.OnKeyEvent(this);
			};
			if(e.KeyCode==Keys.D) {
				keyRight=false;
				player.OnKeyEvent(this);
			};
			//						logger.log("key released", new MessageParameter("player",player.toString()));
		}

		private delegate void RenderDelegate(Image im);

		private void DoRender(Image im) {
			logger.Log("doRender");
			using(Graphics g = panel.CreateGraphics()) {
				g.DrawImage(im, new Point(0, 0));
			}
		}

		internal void RenderImage(Image im) {
			if(!this.IsDisposed && this.CanRaiseEvents)
				panel.Invoke(new RenderDelegate(DoRender), new object[] {im});
		}

		private void Paint_(Object sender, PaintEventArgs a) {
			logger.Log("repainting");
			//if(sender==panel)
				//renderer.Render(this);
		}

		private void Stop(object sender, FormClosingEventArgs e) {
			stop = true;
			if(sender==this) {
				logger.Log("stopping");
				renderer.Stop();
				//System.Stop();
			}
		}
	}
}

