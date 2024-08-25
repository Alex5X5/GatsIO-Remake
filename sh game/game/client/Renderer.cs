using sh_game.game.Logic;
using SimpleLogging.logging;

using System;
using System.Drawing;

namespace sh_game.game.client {

	internal class Renderer {

		public static readonly int gScl = 1;
		private static readonly bool RESTRICTED_VIEW = true;
	
		public static readonly int UNSCALED_WIDTH = 1125;
		public static readonly double SCALED_WIDTH = UNSCALED_WIDTH / gScl;
		public static readonly int UNSCALED_HEIGHT = 900;
		public static readonly double SCALED_HEIGHT = UNSCALED_HEIGHT / gScl;

		private readonly Brush SHADOW_COLOR = new SolidBrush(Color.FromArgb(85,85,90));
		private readonly Brush OBSTACLE_COLOR = new SolidBrush(Color.Gray);
		private readonly Brush PLAYER_RED_COLOR = new SolidBrush(Color.Red);

		public static readonly Line3d BORDER_TOP = Line3d.FromPoints(new Vector3d(0,0,0),new Vector3d(UNSCALED_WIDTH,0,0));
		public static readonly Line3d BORDER_BOTTOM = Line3d.FromPoints(new Vector3d(0, UNSCALED_HEIGHT,0), new Vector3d(UNSCALED_WIDTH, UNSCALED_HEIGHT, 0));
		public static readonly Line3d BORDER_LEFT = Line3d.FromPoints(new Vector3d(0,0,0), new Vector3d(0, UNSCALED_HEIGHT,0));
		public static readonly Line3d BORDER_RIGHT = Line3d.FromPoints(new Vector3d(UNSCALED_WIDTH,0,0), new Vector3d(UNSCALED_WIDTH, UNSCALED_HEIGHT,0));

		private readonly Logger logger = new Logger(new LoggingLevel("Renderer"));

		private readonly Bitmap image;
		private readonly Graphics graphics;
		private readonly SolidBrush brush;
		//private readonly Pen pen;
		//Brush brush;

		private Vector3d mouseVector = new Vector3d(0, 0, 0);
		private Vector3d logicalMouseVector = new Vector3d(0, 0, 0);


		public Renderer() {
			image = new Bitmap(UNSCALED_WIDTH, UNSCALED_HEIGHT);
			brush = new SolidBrush(Color.White);
			//pen = new Pen(Color.White);
			graphics = Graphics.FromImage(image);
			//		logger.log("",
			//				new MessageParameter("bt",BORDER_TOP.toString()),
			//				new MessageParameter("bb",BORDER_BOTTOM.toString()),
			//				new MessageParameter("bl",BORDER_LEFT.toString()),
			//				new MessageParameter("br",BORDER_RIGHT.toString())
			//		);
		}

		public Image Render(Client c) {
			//		logger.log("render normal");
			//		Vector3d pp = c.player.pos.cpy();
			//graphics2D.SetColor(new Color(90, 90, 110));
			//graphics2D.fillRect(-1, -1, (int)SCALED_WIDTH + 2, (int)SCALED_HEIGHT + 2
			if(c.players!=null)
				foreach(Player p in c.players) {
					if(p!=null) {
						drawPlayer(p);
					}
				}
			if(c.player!=null)
				drawPlayer(c.player);
			if(c.player!=null && c.obstacles!=null)
				if(RESTRICTED_VIEW) {
					RenderObstacleShadows(c.player.Pos, c.obstacles);
					getVievRestrictions();
				}
			if(c.obstacles!=null)
				renderObstacles(c.obstacles);
				//g.Dispose();
			return (Image)image;
		}

		private Vector3d[] getVievRestrictions() {
			Vector3d r = logicalMouseVector.cpy();
			//		logger.log("",new MessageParameter("r",r.toString()));
			double angle = Math.Tan(r.y / r.x);
			angle -= (10)*Math.PI/180;
			Vector3d v1 = new Vector3d(Math.Cos(angle), Math.Sin(angle), 0);
			angle += (20)*Math.PI/180;
			Vector3d v2 = new Vector3d(Math.Cos(angle), Math.Sin(angle), 0);
			v1.Scl(100);
			v2.Scl(100);
			return new Vector3d[] { v1, v2 };
		}

		private Dir GetRoundedVievDirection() {
			if (logicalMouseVector.y > 1.0 / Math.Sqrt(2)) {
				return Dir.T;
			}
			else if (logicalMouseVector.y < -1.0 / Math.Sqrt(2)) {
				return Dir.B;
			}
			else if (logicalMouseVector.x > 1.0 / Math.Sqrt(2)) {
				return Dir.R;
			}
			else {
				return Dir.L;
			}
		}

		private Dir relativeDir(Vector3d pos, Vector3d relativeTo)
		{
			Vector3d dir = relativeTo.cpy().Sub(pos).Nor();
			if (dir.y > 1.0 / Math.Sqrt(2))
			{
				return Dir.B;
			}
			else if (dir.y <= -1.0 / Math.Sqrt(2))
			{
				return Dir.T;
			}
			else if (dir.x >= 1.0 / Math.Sqrt(2))
			{
				return Dir.R;
			}
			else
			{
				return Dir.L;
			}
		}

		//	private Dir relativeDir(Vector3d pos, Obstacle relativeTo) {
		//		return (Dir)null;
		//	}
		private void renderBackHalf(Vector3d pos, Brush b) {
			switch (GetRoundedVievDirection()) {
				case Dir.T:
					graphics.FillRectangle(SHADOW_COLOR, new Rectangle(-1, -1, (int)SCALED_WIDTH + 2, (int)(pos.y / gScl + 1)));
					break;
				case Dir.B:
					graphics.FillRectangle(SHADOW_COLOR, new Rectangle(-1, (int)pos.y / gScl, (int)SCALED_WIDTH + 2, (int)(SCALED_HEIGHT - pos.y / gScl) + 1));
					break;
				case Dir.R:
					graphics.FillRectangle(SHADOW_COLOR, new Rectangle(-1, -1, (int)pos.x / gScl + 1, (int)SCALED_HEIGHT + 2));
					break;
				case Dir.L:
					graphics.FillRectangle(SHADOW_COLOR, new Rectangle((int)pos.x / gScl, -1, (int)(SCALED_WIDTH - pos.x / gScl) + 1, (int)(SCALED_HEIGHT) + 2));
					break;
				//default:
				//	logger.warn("unexpected direction", new MessageParameter("direction", getRoundedVievDirection().toString()));
			}
		}

		private void RenderObstacleShadows(Vector3d v, Obstacle[] l) {
			foreach (Obstacle o in l)
			{
				Vector3d[] sp = o.GetShadowPoints(v);
				Vector3d p1 = new Vector3d(0, 0, 0);
				Vector3d p2 = new Vector3d(0, 0, 0);
				if (!(v.x >= o.pos.x && v.x <= o.pos.x + o.WIDTH && v.y >= o.pos.y && v.y <= o.pos.y + o.HEIGHT)) {
					switch (relativeDir(v, o.pos.cpy().Add(new Vector3d(o.WIDTH / 2.0, o.HEIGHT / 2.0, 0).Nor()))) {
						case Dir.T:
							p1 = shadowHit(v, sp[0], BORDER_TOP);
							p2 = shadowHit(v, sp[1], BORDER_TOP);
							break;
						case Dir.B:
							p1 = shadowHit(v, sp[0], BORDER_BOTTOM);
							p2 = shadowHit(v, sp[1], BORDER_BOTTOM);
							break;
						case Dir.R:
							p1 = shadowHit(v, sp[0], BORDER_RIGHT);
							p2 = shadowHit(v, sp[1], BORDER_RIGHT);
							break;
						case Dir.L:
							p1 = shadowHit(v, sp[0], BORDER_LEFT);
							p2 = shadowHit(v, sp[1], BORDER_LEFT);
							break;
						//default:
						//	logger.warn("unexpected direction", new MessageParameter("direction", getRoundedVievDirection().toString()));
					}
					//.fillPolygon(new int[] { (int)p1.x / gScl, (int)sp[0].x / gScl, (int)sp[1].x / gScl }, new int[] { (int)p1.y / gScl, (int)sp[0].y / gScl, (int)sp[1].y / gScl }, 3);
					graphics.FillPolygon(
						SHADOW_COLOR,
						new PointF[] {
							new PointF((int)p1.x, (int)p1.y),
							new PointF((int)p2.x, (int)p2.y),
							new PointF((int)sp[1].x, (int)sp[1].y)
						}
					);
					graphics.FillPolygon(
						SHADOW_COLOR,
						new PointF[] {
							new PointF((int)p1.x, (int)p1.y),
							new PointF((int)sp[0].x, (int)sp[0].y),
							new PointF((int)sp[1].x, (int)sp[1].y)
						}
					);
					graphics.FillPolygon(
						SHADOW_COLOR,
						new PointF[] {
							new PointF((int)p1.x, (int)p1.y),
							new PointF((int)p2.x, (int)p2.y),
							new PointF((int)sp[1].x, (int)sp[1].y)
						}
					);
					//graphics2D.fillPolygon(new int[] { (int)p1.x / gScl, (int)p2.x / gScl, (int)sp[1].x / gScl }, new int[] { (int)p1.y / gScl, (int)p2.y / gScl, (int)sp[1].y / gScl }, 3);
				}
			}
		}

		//	private bool pointOnField(Vector3d v) {
		//		return v.x>=0&&v.x<=UNSCALED_WIDTH&&v.y>=0&&v.y<=UNSCALED_HEIGHT;
		//	}

		private Vector3d shadowHit(Vector3d pp, Vector3d sp, Line3d l) {
			double oth1X = l.origin.x;
			double oth1Y = l.origin.y;
			double oth1Z = l.origin.z;

			Vector3d oth2 = l.origin.cpy().Add(l.direction);
			double oth2X = oth2.x;
			double oth2Y = oth2.y;
			double oth2Z = oth2.z;

			double u = ((oth1X - pp.x) * (sp.y - pp.y) - (oth1Y - pp.y) * (sp.x - pp.x)) / ((oth2Y - oth1Y) * (sp.x - pp.x) - (oth2X - oth1X) * (sp.y - pp.y));
			return new Vector3d(oth1X + u * (oth2X - oth1X), oth1Y + u * (oth2Y - oth1Y), oth1Z + u * (oth2Z - oth1Z));
		}

		private void renderObstacles(Obstacle[] l) {
			foreach(Obstacle o in l) {
				switch(o.type) {
					case 1:
						drawObstacle1(o.pos);
						break;
					case 2:
						drawObstacle2(o.pos);
						break;
					case 3:
						drawObstacle3(o.pos);
						break;
				}
			}
		}

		private void drawPlayer(Player p) {
			//graphics2D.SetStroke(new BasicStroke(1 / gScl));
			//graphics2D.drawLine(0, 0, (int)p.pos.x / gScl, (int)p.pos.y / gScl);
			graphics.FillEllipse(PLAYER_RED_COLOR, new Rectangle((int)p.Pos.x, (int)p.Pos.y, Player.radius*2, Player.radius*2));
			//		logger.log(String.valueOf(p.pos.x/gScl)+" "+String.valueOf(p.pos.y/gScl));
		}

		private void drawObstacle1(Vector3d pos) {
			graphics.FillRectangle(OBSTACLE_COLOR, new RectangleF((int)pos.x / gScl, (int)pos.y / gScl, 35 / gScl, 70 / gScl));
		}

		private void drawObstacle2(Vector3d pos) {
			graphics.FillRectangle(OBSTACLE_COLOR, new RectangleF((int)pos.x / gScl, (int)pos.y / gScl, 70 / gScl, 35 / gScl));
		}

		private void drawObstacle3(Vector3d pos) {
			graphics.FillRectangle(OBSTACLE_COLOR, new RectangleF((int)pos.x/gScl, (int)pos.y/gScl, 70/gScl, 70/gScl));
		}

		public void updateMouse(Vector3d mouse, Vector3d pos) {
			mouseVector.Set(mouse);
			logicalMouseVector = mouseVector.cpy().Sub(pos).Nor();
		}

		public void Stop() {
			image.Dispose();
			graphics.Dispose();
			PLAYER_RED_COLOR.Dispose();
			SHADOW_COLOR.Dispose();
			OBSTACLE_COLOR.Dispose();
		}

		public enum Dir {
			T, B, L, R,
		}

		//public String toString()
		//{
		//	switch (this)
		//	{
		//		case T:
		//			return "game.graphics.client.Renderer.Dir.Top";
		//		case B:
		//			return "game.graphics.client.Renderer.Dir.Top";
		//		case L:
		//			return "game.graphics.client.Renderer.Dir.Left";
		//		case R:
		//			return "game.graphics.client.Renderer.Dir.Right";
		//		default:
		//			return "";

		//	}
		//}
	}
}
