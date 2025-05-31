namespace ShGame.Game.Client.Rendering;

using ShGame.Game.Logic.Math;
using ShGame.Game.Util;

using System;

public enum Dir : byte {
	T, B, L, R,
}


public class Shadow : TextureDrawable {

	static Logger logger = new(new LoggingLevel("shadow"));
	private ISupportsShadow attatch;

	public unsafe override void UpdateVertices() {
		GetShadow(out Vector3d shadowTarget1, out Vector3d shadowTarget2);
		attatch.GetShadowOrigins(out Vector3d shadowOrigin1, out Vector3d shadowOrigin2, out Dir dir);
		float* ptr = VertexDataPtr;
		*ptr=(float)shadowOrigin1.x;
		ptr++;
		*ptr=(float)shadowOrigin1.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)shadowOrigin2.x;
		ptr++;
		*ptr=(float)shadowOrigin2.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)shadowTarget2.x;
		ptr++;
		*ptr=(float)shadowTarget2.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)shadowOrigin1.x;
		ptr++;
		*ptr=(float)shadowOrigin1.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)shadowTarget1.x;
		ptr++;
		*ptr=(float)shadowTarget1.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)shadowTarget2.x;
		ptr++;
		*ptr=(float)shadowTarget2.y;
		ptr++;
		*ptr=0;

		//string s = "";
		//foreach (float f in vertices)
		//	s+=f+" ";
		//logger.Log(s);
	}

	public Shadow(ISupportsShadow attatch_) : base(Paths.AssetsPath("shadow.png"), 18) {
		attatch = attatch_;
	}

	private static unsafe void CalculatePoints(Obstacle* obstacle) {
		obstacle->WIDTH = obstacle->type switch {
			1 => 35,
			2 => 70,
			3 => 70,
			_ => 0,
		};
		obstacle->HEIGHT = obstacle->type switch {
			1 => 70,
			2 => 35,
			3 => 70,
			_ => 0,
		};
		obstacle->boundL.point1.Set(obstacle->Pos.x, obstacle->Pos.y, 0);
		obstacle->boundL.point2.Set(obstacle->Pos.x, obstacle->Pos.y+obstacle->HEIGHT, 0);
		obstacle->boundR.point1.Set(obstacle->Pos.x+obstacle->WIDTH, obstacle->Pos.y, 0);
		obstacle->boundR.point2.Set(obstacle->Pos.x+obstacle->WIDTH, obstacle->Pos.y+obstacle->HEIGHT, 0);
		obstacle->boundT.point1.Set(obstacle->boundT.point1);
		obstacle->boundT.point2.Set(obstacle->boundR.point1);
		obstacle->boundB.point1.Set(obstacle->boundL.point2);
		obstacle->boundB.point2.Set(obstacle->boundR.point2);
	}

	private unsafe void GetShadow(out Vector3d shadowTarget1, out Vector3d shadowTarget2) {
		attatch.GetShadowOrigins(out Vector3d shadowOrigin1, out Vector3d shadowOrigin2, out Dir dir);
		Vector3d pointOfView = attatch.GetPointOfView();
		Vector3d ShadowPoint3 = new(0, 0, 0);
		Vector3d ShadowPoint4 = new(0, 0, 0);
		switch (dir) {
			case Dir.T:
				fixed (Line3d* line = &RendererGl.BORDER_BOTTOM) {
					shadowTarget1 = ShadowHit(&pointOfView, &shadowOrigin1, line);
					shadowTarget2 = ShadowHit(&pointOfView, &shadowOrigin2, line);
				}
				break;
			case Dir.B:
				fixed (Line3d* line = &RendererGl.BORDER_TOP) {
					shadowTarget1=ShadowHit(&pointOfView, &shadowOrigin1, line);
					shadowTarget2=ShadowHit(&pointOfView, &shadowOrigin2, line);
				}
				break;
			case Dir.R:
				fixed (Line3d* line = &RendererGl.BORDER_LEFT) {
					shadowTarget1=ShadowHit(&pointOfView, &shadowOrigin1, line);
					shadowTarget2=ShadowHit(&pointOfView, &shadowOrigin2, line);
				}
				break;
			case Dir.L:
				fixed (Line3d* line = &RendererGl.BORDER_RIGHT) {
					shadowTarget1=ShadowHit(&pointOfView, &shadowOrigin1, line);
					shadowTarget2=ShadowHit(&pointOfView, &shadowOrigin2, line);
				}
				break;
			default:
				shadowTarget1 = new(0, 0, 0);
				shadowTarget2 = new(0, 0, 0);
				//logger.warn("unexpected direction", new MessageParameter("direction", getRoundedVievDirection().toString()));
				break;
		}
	}

	private unsafe Vector3d ShadowHit(Vector3d* playerPosition, Vector3d* shadowPoint, Line3d* border) {
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
				(oth1X - (playerPosition->x + Player.SIZE / 2)) * (shadowPoint->y - (playerPosition->y + Player.SIZE / 2)) -
				(oth1Y - (playerPosition->y + Player.SIZE / 2)) * (shadowPoint->x - (playerPosition->x + Player.SIZE / 2))
			) / (
				(oth2Y - oth1Y) * (shadowPoint->x - (playerPosition->x + Player.SIZE / 2)) -
				(oth2X - oth1X) * (shadowPoint->y - (playerPosition->y + Player.SIZE / 2))
			);
		//magically merge the factor with the border
		return border->
			origin
				.Cpy()
					.Add(
						oth2
							.Sub(border->origin)
								.Scl(u)
					);

		return new Vector3d(
			oth1X + u * (oth2X - oth1X),
			oth1Y + u * (oth2Y - oth1Y),
			oth1Z + u * (oth2Z - oth1Z));
	}
}
public interface ISupportsShadow {

	public Vector3d GetRelativeVector();

	public Vector3d GetPointOfView();

	public void GetShadowOrigins(out Vector3d point1, out Vector3d point2, out Dir dir);
}