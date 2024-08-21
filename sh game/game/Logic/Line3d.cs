using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sh_game.game.Logic {
	public class Line3d {

		public Vector3d origin;
		public Vector3d direction;

		public Line3d() {
			//		System.out.println("[Line3D]");
			//		this.origin = a.cpy();
			////		this.direction = 
			//		Vector3d c = b.cpy().Sub(a);
			//		if(c.Len()>0)this.direction = c.Nor();
			//		else System.out.println("vectors cannot be the same");
		}

		public double Dist(Vector3d point) {
			return point.Sub(this.origin).Crs(this.direction).Len()/this.direction.Len();
		}

		public bool Contains(Vector3d point) {
			//originally this.origin.x + r* this.direction.x = point.x
			double r = (point.x-this.origin.x)/this.direction.x;
			return this.origin.y+r*this.direction.y==point.y;
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

		public Vector3d Intercept(Line3d other) {
			double th1X = this.origin.x;
			double th1Y = this.origin.y;

			Vector3d th2 = this.origin.cpy().Add(this.direction);
			double th2X = th2.x;
			double th2Y = th2.y;

			double oth1X = other.origin.x;
			double oth1Y = other.origin.y;
			double oth1Z = other.origin.z;

			Vector3d oth2 = other.origin.cpy().Add(other.direction);
			double oth2X = oth2.x;
			double oth2Y = oth2.y;
			double oth2Z = oth2.z;

			double u = ((oth1X-th1X)*(th2Y-th1Y)-(oth1Y-th1Y)*(th2X-th1X))/
					((oth2Y-oth1Y)*(th2X-th1X)-(oth2X-oth1X)*(th2Y-th1Y));
			//		System.out.println(u);
			//		System.out.println(((oth1X-th1X)*(th2Y-th1Y)-(oth1Y-th1Y)*(th2X-th1X)));
			//		System.out.println(((oth2Y-oth1Y)*(th2X-th1X)-(oth2X-oth1X)*(th2Y-th1Y)));
			return new Vector3d(oth1X+u*(oth2X-oth1X), oth1Y+u*(oth2Y-oth1Y), oth1Z+u*(oth2Z-oth1Z));
		}

		public static Line3d FromDirection(Vector3d origin, Vector3d direction) {
			Line3d l = new Line3d {
				origin=origin,
				direction=direction
			};
			return l;
		}

		public static Line3d FromPoints(Vector3d point1, Vector3d point2) {
			Line3d l = new Line3d {
				origin=point1.cpy(),
				direction=point2.cpy().Sub(point1).Nor()
			};
			return l;
		}

		
	public override String ToString() {
			return "game.client.logic.vector.Line3d[origin:"+origin.ToString()+",direction:"+direction.ToString()+"]";
		}

		//	public static void main(String[] args) {
		//		Line3d l = Line3d.fromDirection(new Vector3d(0,0,0), new Vector3d(1,5,2).Nor());
		//		Line3d l2 = Line3d.FromPoints(new Vector3d(0,0,0), new Vector3d(2,5,7).Nor());
		//		System.out.println(l.intercept(l2));
		//	}
	}
}
