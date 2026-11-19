namespace ShGame.Rendering;

using System;

using ShGame.Math;
using ShGame.Types;
using ShGame.Util;

public class Shadow {

	public Quad Shape;

	private Obstacle attatch;

	public Shadow(Obstacle obstacle) {
		attatch = obstacle;
	}

	private int RelativeX(Vector3d v) {
		if (v.X<attatch.Pos.X)
			return 1;
		else if (v.X>attatch.Pos.X+attatch.WIDTH)
			return 3;
		else
			return 2;
	}

	private int RelativeY(Vector3d v) {
		if (v.Y<attatch.Pos.Y)
			return 1;
		else if (v.Y>attatch.Pos.Y+attatch.HEIGHT)
			return 3;
		else
			return 2;
	}

	public void Update(Player controlled) {
		GetShadowOrigins(controlled, out Vector3d ShadowPoint1, out Vector3d ShadowPoint2, out Dir dir);
		Vector3d pointOfView = new(controlled.Pos);
		Line facingBoder = dir switch {
			Dir.T => Constants.BORDER_BOTTOM,
			Dir.B => Constants.BORDER_TOP,
			Dir.L => Constants.BORDER_RIGHT,
			Dir.R => Constants.BORDER_LEFT,
			_ => throw new InvalidOperationException($"Can not resolve border to look at for direction {dir}.")
		};
		Vector3d ShadowPoint3 = new Line(pointOfView, ShadowPoint1).Intercept(facingBoder);
		Vector3d ShadowPoint4 = new Line(pointOfView, ShadowPoint2).Intercept(facingBoder);
		Shape = new(ShadowPoint1, ShadowPoint2, ShadowPoint3, ShadowPoint4);
	}

	public void GetShadowOrigins(Player controlled, out Vector3d point1, out Vector3d point2, out Dir dir) {
		dir=Dir.B;
		var rect = new Rect(attatch.Pos, attatch.WIDTH, attatch.HEIGHT);
		if (RelativeX(controlled.Pos)==1&&RelativeY(controlled.Pos)==1) {
			point1 = rect.BottomLeft; //top left
			point2 = rect.TopRight;
			dir = Dir.T;
		} else if (RelativeX(controlled.Pos)==2&&RelativeY(controlled.Pos)==1) {
			point1 = rect.TopLeft; //top middle
			point2 = rect.TopRight;
			dir = Dir.T;
		} else if (RelativeX(controlled.Pos)==3&&RelativeY(controlled.Pos)==1) {
			point1 = rect.TopLeft; //top right
			point2 = rect.BottomRight;
			dir = Dir.T;
		} else if (RelativeX(controlled.Pos)==1&&RelativeY(controlled.Pos)==2) {
			point1 = rect.TopLeft; //middle left
			point2 = rect.BottomLeft;
			dir = Dir.L;
		} else if (RelativeX(controlled.Pos)==3&&RelativeY(controlled.Pos)==2) {
			point1 = rect.TopRight; //middle right
			point2 = rect.BottomRight;
			dir = Dir.R;
		} else if (RelativeX(controlled.Pos)==1&&RelativeY(controlled.Pos)==3) {
			point1 = rect.TopLeft; // bottom left
			point2 = rect.BottomRight;
			dir = Dir.B;
		} else if (RelativeX(controlled.Pos)==2&&RelativeY(controlled.Pos)==3) {
			point1 = rect.BottomLeft; // bottom middle
			point2 = rect.BottomRight;
			dir = Dir.B;
		} else {
			point1 = rect.TopRight; // bottom right
			point2 = rect.BottomLeft;
			dir = Dir.B;
		}
	}
}