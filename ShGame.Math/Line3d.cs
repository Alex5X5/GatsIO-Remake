namespace ShGame.Math;

public class Line3d {

	public required Vector3d origin;
	public required Vector3d direction;

	public Vector3d A => origin;
	public Vector3d B => origin + direction;

	public unsafe double Dist(Vector3d point) {
		return point.Sub(origin).Crs(direction).Len()/direction.Len();
	}

	public bool Contains(Vector3d point) {
		//originally this.origin.x + r* this.direction.x = point.x
		double r = (point.x-origin.x)/direction.x;
		return origin.y+r*direction.y==point.y;
	}

	public Vector3d Intercept(Line3d other) {

		Vector3d this2 = origin.Cpy().Add(direction);
		Vector3d other2 = other.origin.Cpy().Add(other.direction);

		double u = (
			(other.origin.x-origin.x)*(this2.y-origin.y)-
			(other.origin.y-origin.y)*(this2.x-origin.x)
		)/(
			(other2.y-other.origin.y)*(this2.x-origin.x)-
			(other2.x-other.origin.x)*(this2.y-origin.y)
		);
		return new Vector3d(
			other2.x+u*(other2.x-other.origin.x),
			other.origin.y+u*(other2.y-other.origin.y),
			other.origin.z+u*(other2.y-other.origin.z)
		);
	}

	public static Line3d FromDirection(Vector3d origin, Vector3d direction) {
		Line3d l = new() {
			origin=origin,
			direction=direction
		};
		return l;
	}

	public static Line3d FromPoints(Vector3d point1, Vector3d point2) {
		Line3d l = new() {
			origin=point1,
			direction=point2.Sub(point1).Nor()
		};
		return l;
	}


	public override string ToString() {
		return "Game.client.logic.vector.Line3d[origin:"+origin.ToString()+",direction:"+direction.ToString()+"]";
	}
}
