namespace ShGame.game.Client.Rendering;

using ShGame.game.Logic;

using System.Drawing;
using System.Drawing.Drawing2D;

#pragma warning disable CS8500 //a pointer is created to a variable of a managed type

internal class Renderer : IDisposable
{

	#region fields

	private static readonly bool RESTRICTED_VIEW = true;

	public const int WIDTH = 1400, HEIGHT = 900;

	private readonly Brush GROUND_COLOR = new SolidBrush(Color.FromArgb(85, 85, 90));
	private readonly Brush SHADOW_COLOR = new SolidBrush(Color.FromArgb(0, 0, 0));
	private readonly Brush OBSTACLE_COLOR = new SolidBrush(Color.Gray);
	private readonly Brush PLAYER_RED_COLOR = new SolidBrush(Color.Red);

	public static readonly Line3d BORDER_TOP = Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(WIDTH, 0, 0));
	public static readonly Line3d BORDER_BOTTOM = Line3d.FromPoints(new Vector3d(0, HEIGHT, 0), new Vector3d(WIDTH, HEIGHT, 0));
	public static readonly Line3d BORDER_LEFT = Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(0, HEIGHT, 0));
	public static readonly Line3d BORDER_RIGHT = Line3d.FromPoints(new Vector3d(WIDTH, 0, 0), new Vector3d(WIDTH, HEIGHT, 0));

	private readonly Logger logger = new(new LoggingLevel("Renderer"));

	private readonly Bitmap image;
	private readonly Graphics graphics;

	private Vector3d mouseVector = new(0, 0, 0);
	private Vector3d logicalMouseVector = new(0, 0, 0);

	#endregion fields

	public Renderer()
	{
		image = new Bitmap(WIDTH, HEIGHT);
		graphics = Graphics.FromImage(image);
	}

	public unsafe Image Render(ref Player[] players, ref Player player, Obstacle2[]* obstacles)
	{
		//create a graphics object from the main image
		using (Graphics g = Graphics.FromImage(image))
		{
			g.SmoothingMode=SmoothingMode.AntiAlias;
			//fill the whole imagge with the ground color
			g.FillRectangle(GROUND_COLOR, new RectangleF(0, 0, WIDTH, HEIGHT));
			//check that the player array isn't null and loop through it
			if (players!=null)
				foreach (Player p in players)
				{
					//if the player isn't null draw it
					if (p!=null&&p.Health>=0)
					{
						//logger.Log("drawing player ",new MessageParameter("player",p.ToString()));
						DrawPlayer(p, g);
					}
				}
			//checked that the main player isn't null and draw it
			if (player != null)
				DrawPlayer(player, g);
			//check whether the main player and the obstackles are not null
			if (player!=null&&obstacles!=null)
				if (RESTRICTED_VIEW){
					//if the obstackles, the main player are not null and
					//RESTRICTED_VIEW is enabled draw the obstacle shadows
					fixed (Vector3d* ptr = &player.Pos)
						RenderObstacleShadows(ptr, obstacles);
					GetVievRestrictions();
				}
			//draw the obstacles after the shadows so all the obstacles overlay the shadows
			if (obstacles!=null)
				RenderObstacles(*obstacles, g);
		}
		return image;
	}

	private void RenderObstacles(Obstacle2[] l, Graphics g)
	{
		foreach (Obstacle2 o in l)
		{
			switch (o?.type)
			{
				case 1:
					DrawObstacle1(o.Pos, g: g);
					break;
				case 2:
					DrawObstacle2(o.Pos, g: g);
					break;
				case 3:
					DrawObstacle3(o.Pos, g: g);
					break;
				default:
					break;
			}
		}
	}

	#region direction calculations

	private Vector3d[] GetVievRestrictions() {
		Vector3d r = logicalMouseVector.Cpy();
		//		logger.log("",new MessageParameter("r",r.toString()));
		double angle = Math.Tan(r.y / r.x);
		angle -= 10*Math.PI/180;
		Vector3d v1 = new(Math.Cos(angle), Math.Sin(angle), 0);
		angle += 20*Math.PI/180;
		Vector3d v2 = new(Math.Cos(angle), Math.Sin(angle), 0);
		unsafe {
			v1.Scl(100);
			v2.Scl(100);
		}
		return [v1, v2];
	}

	private Dir GetRoundedVievDirection() {
		if (logicalMouseVector.y > 1.0 / Math.Sqrt(2))
		{
			return Dir.T;
		}
		else if (logicalMouseVector.y < -1.0 / Math.Sqrt(2))
		{
			return Dir.B;
		}
		else if (logicalMouseVector.x > 1.0 / Math.Sqrt(2))
		{
			return Dir.R;
		}
		else
		{
			return Dir.L;
		}
	}


	private unsafe Dir RelativeDir(Vector3d pos, Vector3d relativeTo) {
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

	#endregion direction calculations

	#region shadow rendering

	private void RenderBackHalf(Vector3d pos) {
		switch (GetRoundedVievDirection())
		{
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
		}
	}

	private unsafe void RenderObstacleShadows(Vector3d* v, Obstacle2[]* l) {
		ArgumentNullException.ThrowIfNull(&v);
		//a list of points on the screen
		PointF[] points = new PointF[3];
		Vector3d ShadowPoint1 = new(0, 0, 0);
		Vector3d ShadowPoint2 = new(0, 0, 0);
		Vector3d ShadowPoint3 = new(0, 0, 0);
		Vector3d ShadowPoint4 = new(0, 0, 0);
		Vector3d[] sp;
		//return;
		foreach (Obstacle2 o in *l)
		{
			if (o==null)
				continue;
			//o.GetShadowPoints(v, &ShadowPoint1, &ShadowPoint2);
			//sp=o.GetShadowPoints(v);
			if (!(v->x >= o.Pos.x && v->x <= o.Pos.x + o.WIDTH && v->y >= o.Pos.y && v->y <= o.Pos.y + o.HEIGHT))
			{
				switch (RelativeDir(*v, o.Pos.Cpy().Add(new Vector3d(o.WIDTH / 2.0, o.HEIGHT / 2.0, 0))))
				{
					case Dir.T:
						fixed (Line3d* line = &BORDER_TOP)
						{
							ShadowPoint3 = ShadowHit(v, &ShadowPoint1, line);
							ShadowPoint4 = ShadowHit(v, &ShadowPoint2, line);
						}
						break;
					case Dir.B:
						fixed (Line3d* line = &BORDER_BOTTOM)
						{
							ShadowPoint3=ShadowHit(v, &ShadowPoint1, line);
							ShadowPoint4=ShadowHit(v, &ShadowPoint2, line);
						}
						break;
					case Dir.R:
						fixed (Line3d* line = &BORDER_RIGHT)
						{
							ShadowPoint3=ShadowHit(v, &ShadowPoint1, line);
							ShadowPoint4=ShadowHit(v, &ShadowPoint2, line);
						}
						break;
					case Dir.L:
						fixed (Line3d* line = &BORDER_LEFT)
						{
							ShadowPoint3=ShadowHit(v, &ShadowPoint1, line);
							ShadowPoint4=ShadowHit(v, &ShadowPoint2, line);
						}
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

	private unsafe Vector3d ShadowHit(Vector3d* playerPosition, Vector3d* shadowPoint, Line3d* border)
	{
		//get the coordinates of the origin point of the border
		double oth1X = border->origin.x;
		double oth1Y = border->origin.y;
		double oth1Z = border->origin.z;
		//calculate the coordinates of a vector that starts at the origin point of the border
		//and points towards its second definition point
		Vector3d oth2 = border->origin.Cpy().Add(border->direction);
		double oth2X = oth2.x;
		double oth2Y = oth2.y;
		double oth2Z = oth2.z;
		//calculate a magical factor
		double u =
			(
				(oth1X - playerPosition->x) * (shadowPoint->y - playerPosition->y) -
				(oth1Y - playerPosition->y) * (shadowPoint->x - playerPosition->x)
			) / (
				(oth2Y - oth1Y) * (shadowPoint->x - playerPosition->x) -
				(oth2X - oth1X) * (shadowPoint->y - playerPosition->y)
			);
		//magically merge the factor with the border
		return border->
			origin.
				Cpy().
					Add(
						oth2.
							Sub(border->origin).
								Scl(u)
					);

		return new Vector3d(
			oth1X + u * (oth2X - oth1X),
			oth1Y + u * (oth2Y - oth1Y),
			oth1Z + u * (oth2Z - oth1Z));
	}

	private void DrawShadowTriangle(
		ref PointF[] points,
		ref double v1,
		ref double v2,
		ref double v3,
		ref double v4,
		ref double v5,
		ref double v6
	)
	{
		points[0].X=(float)v1;
		points[0].Y=(float)v2;
		points[1].X=(float)v3;
		points[1].Y=(float)v4;
		points[2].X=(float)v5;
		points[2].Y=(float)v6;

		graphics.FillPolygon(
			SHADOW_COLOR,
			points
		);
	}

	#endregion shadow rendering

	#region draw methods

	private void DrawPlayer(Player p, Graphics g)
	{
		//graphics2D.SetStroke(new BasicStroke(1 / gScl));
		//graphics2D.drawLine(0, 0, (int)p.pos.x / gScl, (int)p.pos.y / gScl);
		//Console.WriteLine("drawing at"+p.Pos.ToString());
		if (p.Visible) ;
		g.FillEllipse(PLAYER_RED_COLOR, new Rectangle((int)p.Pos.x-Player.Radius, (int)p.Pos.y-Player.Radius, Player.Radius*2, Player.Radius*2));
		//		logger.log(String.valueOf(p.pos.x/gScl)+" "+String.valueOf(p.pos.y/gScl));
	}

	private void DrawObstacle1(Vector3d pos, Graphics g)
	{
		g.FillRectangle(OBSTACLE_COLOR, new RectangleF((int)pos.x, (int)pos.y, 35, 70));
	}

	private void DrawObstacle2(Vector3d pos, Graphics g)
	{
		g.FillRectangle(OBSTACLE_COLOR, new RectangleF((int)pos.x, (int)pos.y, 70, 35));
	}

	private void DrawObstacle3(Vector3d pos, Graphics g)
	{
		g.FillRectangle(OBSTACLE_COLOR, new RectangleF((int)pos.x, (int)pos.y, 70, 70));
	}

	public unsafe void UpdateMouse(Vector3d mouse, Vector3d pos){
		logicalMouseVector = mouseVector.Cpy().Sub(pos).Nor();
	}

	#endregion draw methods

	public void Dispose()
	{
		logger.Log("stoppping");
		//renderLock.Dispose();
		image.Dispose();
		graphics.Dispose();
		PLAYER_RED_COLOR.Dispose();
		SHADOW_COLOR.Dispose();
		OBSTACLE_COLOR.Dispose();
		GROUND_COLOR.Dispose();
	}

	public enum Dir : byte
	{
		T, B, L, R,
	}
}
