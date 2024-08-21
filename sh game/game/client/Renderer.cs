using SimpleLogging.logging;

using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sh_game.game.client {

	internal class Renderer {

		private static readonly bool RESTRICTED_VIEW = false;

		private readonly Logger logger;

		private readonly Bitmap image;
		private bool lockDraw = false;
		private readonly Graphics imageGraphics;
		//private readonly Semaphore lockDraw = new Semaphore(0,1);

		private int x = 0;
		private bool stop = false;

		internal Renderer() {
			logger = new Logger(new LoggingLevel("Renderer"));
			logger.Log("Constructor");
			image = new Bitmap(Client.WIDTH, Client.HEIGHT);
			imageGraphics = Graphics.FromImage(image);
		}

		internal void Stop() {
			stop = true;
		}

		internal void Render(Client c) {
			//logger.Log("render");
			if(RESTRICTED_VIEW)
				DrawRestricted();
			else
				DrawNormal();
			if(!stop) {
				c.RenderImage((Image)image.Clone());
			}
			
		}

		internal void DrawNormal() {
			while(lockDraw) {}
			lockDraw=true;
			//logger.Log("draw normal");
			//this.Finalize();
			//ObjectDisposedException()
			var brush = new SolidBrush(Color.FromArgb(90, 90, 110));
			//var g = Graphics.FromImage(image);
			imageGraphics.FillRectangle(brush, 0, 0, Client.WIDTH, Client.HEIGHT);
			brush.Color = Color.FromArgb(200, 100, 100);
			imageGraphics.FillEllipse(brush, x, 100, 100, 100);
			x+=3;
			lockDraw = false;
		}

		private void DrawRestricted() {

		}
	}
}
