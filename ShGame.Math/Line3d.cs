namespace ShGame.Math;

public class Line3d {

	public required Vector3d origin;
	public required Vector3d direction;

	//public Line3d() {
		//		System.out.println("[Line3D]");
		//		this.origin = a.cpy();
		////		this.direction = 
		//		Vector3d c = b.cpy().Sub(a);
		//		if(c.Len()>0)this.direction = c.Nor();
		//		else System.out.println("vectors cannot be the same");
	//}

	public unsafe double Dist(Vector3d point) {
		return point.Sub(origin).Crs(direction).Len()/direction.Len();
	}

	public bool Contains(Vector3d point) {
		//originally this.origin.x + r* this.direction.x = point.x
		double r = (point.x-origin.x)/direction.x;
		return origin.y+r*direction.y==point.y;
	}

	//	public Vector3d intercept(Line3d other) throws VectorMathException{
	//		Vector3d interception = null;
	//		//dot==1 => parallel => no intercept
	//		if(this.direction.dot(other.direction)!=1) {
	//			double thBX = this.origin.x;
	//			double thBY = this.origin.y;
	//			double thDX = this.direction.x;
	//			double thDY = this.direction.y;
	//			double othBX = other.origin.x;
	//			double othBY = other.origin.y;
	//			double othDX = other.direction.x;
	//			double othDY = other.direction.y;
	//			//lovely linear equation systems
	//			double a = -(thBX*othDY-thBY*othDX-othBX*othDY+othBY*othBX)/(thDX*othDY-thDY*othDX);
	//			interception = this.origin.cpy().Add(this.direction.cpy().scl(a));
	//		} else {
	//			new VectorMathException("no interception possible");
	//		}
	//		return interception;
	//	}

	public unsafe Vector3d Intercept(ref Line3d other) {
		//double th1X = origin.x;
		//double th1Y = origin.y;

		//Vector3d th2 = origin.Cpy().Add(direction);
		//double th2X = th2.x;
		//double th2Y = th2.y;

		//double oth1X = other.origin.x;
		//double oth1Y = other.origin.y;
		//double oth1Z = other.origin.z;
		//TempMeshData<PrimitiveVector3I> data;

		Vector3d this2 = origin.Cpy().Add(direction);
		Vector3d other2 = other.origin.Cpy().Add(other.direction);
		//double oth2X = other2.x;
		//double oth2Y = other2.y;
		//double oth2Z = other2.z;

		double u = (
					(other.origin.x-origin.x)*(this2.y-origin.y)-
					(other.origin.y-origin.y)*(this2.x-origin.x)
				)/
				(
					(other2.y-other.origin.y)*(this2.x-origin.x)-
					(other2.x-other.origin.x)*(this2.y-origin.y)
		);
		//		System.out.println(u);
		//		System.out.println(((oth1X-th1X)*(th2Y-th1Y)-(oth1Y-th1Y)*(th2X-th1X)));
		//		System.out.println(((oth2Y-oth1Y)*(th2X-th1X)-(oth2X-oth1X)*(th2Y-th1Y)));
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

	public static unsafe Line3d FromPoints(Vector3d point1, Vector3d point2) {
		Line3d l = new() {
			origin=point1.Cpy(),
			direction=point2.Cpy().Sub(point1).Nor()
		};
		return l;
	}


	public override string ToString() {
		return "Game.client.logic.vector.Line3d[origin:"+origin.ToString()+",direction:"+direction.ToString()+"]";
	}
}
