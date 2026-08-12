namespace ShGame.Math;

public struct Line {

	public Vector3d A;
	public Vector3d B;

	public Vector3d Origin => A;
	public Vector3d Direction => B.Sub(A).Nor();

	public Line(Vector3d point1, Vector3d point2) {
		A = point1;
		B = point2;
	}

	public Line(double x1, double y1, double x2, double y2) {
		A = new Vector3d(x1, y1, 0);
		B = new Vector3d(x2, y2, 0);
	}

	public unsafe double Dist(Vector3d point) {
		return point.Sub(Origin).Crs(Direction).Len()/Direction.Len();
	}

	public bool Contains(Vector3d point) {
		//Originally this.Origin.x + r* this.Direction.x = point.x
		double r = (point.X- Origin.X)/Direction.X;
		return Origin.Y+r*Direction.Y==point.Y;
	}

	public Vector3d Intercept(Line other) {

		Vector3d this2 = Origin.Add(Direction);
		Vector3d other2 = other.Origin.Add(other.Direction);

		double u = (
			(other.Origin.X-Origin.X)*(this2.Y-Origin.Y)-
			(other.Origin.Y-Origin.Y)*(this2.X-Origin.X)
		)/(
			(other2.Y-other.Origin.Y)*(this2.X-Origin.X)-
			(other2.X-other.Origin.X)*(this2.Y-Origin.Y)
		);
		return new Vector3d(
			other2.X+u*(other2.X-other.Origin.X),
			other.Origin.Y+u*(other2.Y-other.Origin.Y),
			other.Origin.Z+u*(other2.Y-other.Origin.Z)
		);
	}

	public override string ToString() {
		return "Game.client.logic.vector.Line3d[Origin:"+Origin.ToString()+",Direction:"+Direction.ToString()+"]";
	}
}
