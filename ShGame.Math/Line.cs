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
		double r = (point.x- Origin.x)/Direction.x;
		return Origin.y+r*Direction.y==point.y;
	}

	public Vector3d Intercept(Line other) {

		Vector3d this2 = Origin.Add(Direction);
		Vector3d other2 = other.Origin.Add(other.Direction);

		double u = (
			(other.Origin.x-Origin.x)*(this2.y-Origin.y)-
			(other.Origin.y-Origin.y)*(this2.x-Origin.x)
		)/(
			(other2.y-other.Origin.y)*(this2.x-Origin.x)-
			(other2.x-other.Origin.x)*(this2.y-Origin.y)
		);
		return new Vector3d(
			other2.x+u*(other2.x-other.Origin.x),
			other.Origin.y+u*(other2.y-other.Origin.y),
			other.Origin.z+u*(other2.y-other.Origin.z)
		);
	}

	public override string ToString() {
		return "Game.client.logic.vector.Line3d[Origin:"+Origin.ToString()+",Direction:"+Direction.ToString()+"]";
	}
}
