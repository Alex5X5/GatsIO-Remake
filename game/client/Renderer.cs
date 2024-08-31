using ShGame.game.Logic;

using SimpleLogging.logging;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace sh_game.game.client {

	internal class Renderer:IDisposable {

		//public static readonly int gScl = 1;
		private static readonly bool RESTRICTED_VIEW = true;

		//public static readonly int UNSCALED_WIDTH = 1125;
		//public static readonly double SCALED_WIDTH = UNSCALED_WIDTH / gScl;
		//public static readonly int UNSCALED_HEIGHT = 900;
		//public static readonly double SCALED_HEIGHT = UNSCALED_HEIGHT / gScl;

		public const int WIDTH = 1000, HEIGHT = 1000;

		private readonly Brush SHADOW_COLOR = new SolidBrush(Color.FromArgb(85,85,90));
		private readonly Brush OBSTACLE_COLOR = new SolidBrush(Color.Gray);
		private readonly Brush PLAYER_RED_COLOR = new SolidBrush(Color.Red);

		public static readonly Line3d BORDER_TOP = Line3d.FromPoints(new Vector3d(0,0,0),new Vector3d(WIDTH,0,0));
		public static readonly Line3d BORDER_BOTTOM = Line3d.FromPoints(new Vector3d(0, HEIGHT, 0), new Vector3d(WIDTH, HEIGHT, 0));
		public static readonly Line3d BORDER_LEFT = Line3d.FromPoints(new Vector3d(0,0,0), new Vector3d(0, HEIGHT,0));
		public static readonly Line3d BORDER_RIGHT = Line3d.FromPoints(new Vector3d(WIDTH, 0,0), new Vector3d(WIDTH, HEIGHT,0));

		private readonly Logger logger = new Logger(new LoggingLevel("Renderer"));

		//private readonly Semaphore renderLock;

		private readonly Bitmap image;
		private readonly Graphics graphics;

		private Vector3d mouseVector = new Vector3d(0, 0, 0);
		private Vector3d logicalMouseVector = new Vector3d(0, 0, 0);

		public Renderer() {


			//Image temp;
			image = new Bitmap(WIDTH, HEIGHT);
			//renderLock = new Semaphore(1,1);
			graphics = Graphics.FromImage(image);
			//logger.log("",
			//		new MessageParameter("bt", BORDER_TOP.toString()),
			//		new MessageParameter("bb", BORDER_BOTTOM.toString()),
			//		new MessageParameter("bl", BORDER_LEFT.toString()),
			//		new MessageParameter("br", BORDER_RIGHT.toString())
			//);
		}

		public Image Render(ref Player[] players, ref Player player, ref Obstacle[] obstacles) {
			using(Graphics g = Graphics.FromImage(image)) {
				g.SmoothingMode=SmoothingMode.AntiAlias;
				g.FillRectangle(SHADOW_COLOR, new RectangleF(0, 0, WIDTH, HEIGHT));

				if(players!=null)
					foreach(Player p in players) {
						if(p!=null)
							DrawPlayer(p, g);
					}
				if(player!=null)
					DrawPlayer(player, g);

				if(obstacles!=null)
					RenderObstacles(obstacles, g);

				if(player!=null&&obstacles!=null)
					if(RESTRICTED_VIEW) {
						//RenderBackHalf(c.player.Pos);
						RenderObstacleShadows(player.Pos, obstacles);
						GetVievRestrictions();
					}
				g.Dispose();
			}
			return image;
		}

		private Vector3d[] GetVievRestrictions() {
			Vector3d r = logicalMouseVector.Cpy();
			//		logger.log("",new MessageParameter("r",r.toString()));
			double angle = Math.Tan(r.y / r.x);
			angle -= (10)*Math.PI/180;
			Vector3d v1 = new(Math.Cos(angle), Math.Sin(angle), 0);
			angle += (20)*Math.PI/180;
			Vector3d v2 = new(Math.Cos(angle), Math.Sin(angle), 0);
			v1.Scl(100);
			v2.Scl(100);
			return [v1, v2];
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

		private Dir RelativeDir(Vector3d pos, Vector3d relativeTo)
		{
			Vector3d dir = relativeTo.Cpy().Sub(pos).Nor();
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
		private void RenderBackHalf(Vector3d pos) {
			switch (GetRoundedVievDirection()) {
				case Dir.T:
					graphics.FillRectangle(SHADOW_COLOR, new Rectangle(-1, -1, WIDTH + 2, (int)(pos.y + 1)));
					break;
				case Dir.B:
					graphics.FillRectangle(SHADOW_COLOR, new Rectangle(-1, (int)pos.y, WIDTH+2, (int)(HEIGHT-pos.y)+1));
					break;
				case Dir.R:
					graphics.FillRectangle(SHADOW_COLOR, new Rectangle(-1, -1, (int)pos.x+1, HEIGHT+2));
					break;
				case Dir.L:
					graphics.FillRectangle(SHADOW_COLOR, new Rectangle((int)pos.x, -1, (int)(WIDTH-pos.x)+1, HEIGHT+2));
					break;
				//default:
				//	logger.warn("unexpected direction", new MessageParameter("direction", getRoundedVievDirection().toString()));
			}
		}

		private void RenderObstacleShadows(Vector3d v, Obstacle[] l) {
			//a list of points on the screen
			PointF[] points = new PointF[3];
			Vector3d ShadowPoint1 = new Vector3d(0, 0, 0);
			Vector3d ShadowPoint2 = new Vector3d(0, 0, 0);
			Vector3d ShadowPoint3 = new Vector3d(0, 0, 0);
			Vector3d ShadowPoint4 = new Vector3d(0, 0, 0);
			Vector3d[] sp;
			//return;
			foreach (Obstacle o in l)
			{
				o.GetShadowPoints(ref v, ref ShadowPoint1, ref ShadowPoint2);
				//sp=o.GetShadowPoints(v);
				if (!(v.x >= o.Pos.x && v.x <= o.Pos.x + o.WIDTH && v.y >= o.Pos.y && v.y <= o.Pos.y + o.HEIGHT)) {
					switch (RelativeDir(v, o.Pos.Cpy().Add(new Vector3d(o.WIDTH / 2.0, o.HEIGHT / 2.0, 0).Nor()))) {
						case Dir.T:
							ShadowPoint3 = ShadowHit(v, ShadowPoint1, BORDER_TOP);
							ShadowPoint4 = ShadowHit(v, ShadowPoint2, BORDER_TOP);
							break;
						case Dir.B:
							ShadowPoint3=ShadowHit(v, ShadowPoint1, BORDER_BOTTOM);
							ShadowPoint4=ShadowHit(v, ShadowPoint2, BORDER_BOTTOM);
							break;
						case Dir.R:
							ShadowPoint3=ShadowHit(v, ShadowPoint1, BORDER_RIGHT);
							ShadowPoint4=ShadowHit(v, ShadowPoint2, BORDER_RIGHT);
							break;
						case Dir.L:
							ShadowPoint3=ShadowHit(v, ShadowPoint1, BORDER_LEFT);
							ShadowPoint4=ShadowHit(v, ShadowPoint2, BORDER_LEFT);
							break;
						//default:
						//	logger.warn("unexpected direction", new MessageParameter("direction", getRoundedVievDirection().toString()));
					}
					//.fillPolygon(new int[] { (int)p1.x / gScl, (int)sp[0].x / gScl, (int)sp[1].x / gScl }, new int[] { (int)p1.y / gScl, (int)sp[0].y / gScl, (int)sp[1].y / gScl }, 3);

					//DrawShadowTriangle(ref points, ref p1.x, ref p1.y, ref p2.x, ref p2.y, ref sp[1].x, ref sp[1].y);
					//DrawShadowTriangle(ref points, ref p1.x, ref p1.y, ref sp[0].x, ref sp[0].y, ref sp[1].x, ref sp[1].y);
					DrawShadowTriangle(ref points, ref ShadowPoint1.x, ref ShadowPoint1.y, ref ShadowPoint2.x, ref ShadowPoint2.y, ref ShadowPoint4.x, ref ShadowPoint4.y);
					DrawShadowTriangle(ref points, ref ShadowPoint1.x, ref ShadowPoint1.y, ref ShadowPoint3.x, ref ShadowPoint3.y, ref ShadowPoint4.x, ref ShadowPoint4.y);



					//graphics.FillPolygon(
					//	SHADOW_COLOR,
					//	new PointF[] {
					//		new PointF((int)p1.x, (int)p1.y),
					//		new PointF((int)p2.x, (int)p2.y),
					//		new PointF((int)sp[1].x, (int)sp[1].y)
					//	}
					//);
					//graphics.FillPolygon(
					//	SHADOW_COLOR,
					//	new PointF[] {
					//		new PointF((int)p1.x, (int)p1.y),
					//		new PointF((int)sp[0].x, (int)sp[0].y),
					//		new PointF((int)sp[1].x, (int)sp[1].y)
					//	}
					//);
					//graphics.FillPolygon(
					//	SHADOW_COLOR,
					//	new PointF[] {
					//		new PointF((int)p1.x, (int)p1.y),
					//		new PointF((int)p2.x, (int)p2.y),
					//		new PointF((int)sp[1].x, (int)sp[1].y)
					//	}
					//);
					//graphics2D.fillPolygon(new int[] { (int)p1.x / gScl, (int)p2.x / gScl, (int)sp[1].x / gScl }, new int[] { (int)p1.y / gScl, (int)p2.y / gScl, (int)sp[1].y / gScl }, 3);
				}
			}
		}

		//	private bool pointOnField(Vector3d v) {
		//		return v.x>=0&&v.x<=UNSCALED_WIDTH&&v.y>=0&&v.y<=UNSCALED_HEIGHT;
		//	}

		private Vector3d ShadowHit(Vector3d pp, Vector3d sp, Line3d l) {
			double oth1X = l.origin.x;
			double oth1Y = l.origin.y;
			double oth1Z = l.origin.z;

			Vector3d oth2 = l.origin.Cpy().Add(l.direction);
			double oth2X = oth2.x;
			double oth2Y = oth2.y;
			double oth2Z = oth2.z;

			double u = ((oth1X - pp.x) * (sp.y - pp.y) - (oth1Y - pp.y) * (sp.x - pp.x)) / ((oth2Y - oth1Y) * (sp.x - pp.x) - (oth2X - oth1X) * (sp.y - pp.y));
			return new Vector3d(oth1X + u * (oth2X - oth1X), oth1Y + u * (oth2Y - oth1Y), oth1Z + u * (oth2Z - oth1Z));
		}

		private void DrawShadowTriangle(
			ref PointF[] points,
			ref double v1,
			ref double v2,
			ref double v3,
			ref double v4,
			ref double v5,
			ref double v6
			//ref int v7, ref int v8, ref int v9, ref int v10, ref int v11, ref int v12
		) {
			points[0].X=(float)v1;
			points[0].Y=(float)v2;
			points[1].X=(float)v3;
			points[1].Y=(float)v4;
			points[2].X=(float)v5;
			points[2].Y=(float)v6;
			//points[0].X = v1; points[1].X = v2; points[2].X = v3; points[3].X = v4; points[4].X = v5; points[5].X = v6;
			//points[6].X = v7; points[7].X = v8; points[8].X = v9; points[9].X = v10; points[10].X = v11; points[11].X = v12;
			graphics.FillPolygon(
				SHADOW_COLOR,
				//new PointF[] {
				//	new PointF((int)p1.x, (int)p1.y),
				//	new PointF((int)p2.x, (int)p2.y),
				//	new PointF((int)sp[1].x, (int)sp[1].y)
				//}
				points
			);
		}

		private void RenderObstacles(Obstacle[] l, Graphics g) {
			foreach(Obstacle o in l) {
				switch(o.type) {
					case 1:
						DrawObstacle1(o.Pos, g: g);
						break;
					case 2:
						DrawObstacle2(o.Pos, g: g);
						break;
					case 3:
						DrawObstacle3(o.Pos, g: g);
						break;
				}
			}
		}

		private void DrawPlayer(Player p, Graphics g) {
			//graphics2D.SetStroke(new BasicStroke(1 / gScl));
			//graphics2D.drawLine(0, 0, (int)p.pos.x / gScl, (int)p.pos.y / gScl);
			//Console.WriteLine("drawing at"+p.Pos.ToString());
			if(p.visible)
				g.FillEllipse(PLAYER_RED_COLOR, new Rectangle((int)p.Pos.x, (int)p.Pos.y, Player.radius*2, Player.radius*2));
			//		logger.log(String.valueOf(p.pos.x/gScl)+" "+String.valueOf(p.pos.y/gScl));
		}

		private void DrawObstacle1(Vector3d pos, Graphics g) {
			g.FillRectangle(OBSTACLE_COLOR, new RectangleF((int)pos.x, (int)pos.y, 35, 70));
		}

		private void DrawObstacle2(Vector3d pos, Graphics g) {
			g.FillRectangle(OBSTACLE_COLOR, new RectangleF((int)pos.x, (int)pos.y, 70, 35));
		}

		private void DrawObstacle3(Vector3d pos, Graphics g) {
			g.FillRectangle(OBSTACLE_COLOR, new RectangleF((int)pos.x, (int)pos.y, 70, 70));
		}

		public void UpdateMouse(Vector3d mouse, Vector3d pos) {
			mouseVector.Set(mouse);
			logicalMouseVector = mouseVector.Cpy().Sub(pos).Nor();
		}

		public void Dispose() {
			logger.Log("stoppping");
			//renderLock.Dispose();
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
