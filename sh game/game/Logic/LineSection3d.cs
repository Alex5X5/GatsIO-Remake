using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sh_game.game.Logic {
	[Serializable]
	public class LineSection3d {

		//private static readonly long serialVersionUID = 6732041890222210211L;

		public Vector3d point1;
		public Vector3d point2;
		//	public double length;

		public LineSection3d(Line3d l, double ln) {
			this.point1=l.origin.Cpy();
			this.point2=point1.Cpy().Add(l.direction.Cpy().Scl(ln));
		}

		public double GetLength() {
			return point1.Dst(point2);
		}

		public LineSection3d(Vector3d p1, Vector3d p2) {
			//		System.out.println("[LineSection3d]: (constructor): p1="+p1.ToString()+" p2="+p2.ToString());
			this.point1=p1.Cpy();
			this.point2=p2.Cpy();
		}

		public double Dst(Vector3d point) {
			return point.Cpy().Sub(this.point1).Crs(point2.Cpy().Sub(point1)).Len()/point2.Cpy().Sub(point1).Len();
		}

		public bool Contains(Vector3d p) {
			Line3d l = Line3d.FromPoints(point1, point2);
			//		System.out.println("[LineSection3d]: (checking Contains): this:"+ToString()+", p:"+p.ToString());
			return (l.Contains(p)&&Contains1(p));
		}

		private bool Contains1(Vector3d p) {
			return (point1.Dst(p)<=GetLength()&&point2.Dst(p)<=GetLength());
		}

		public Vector3d Intercept(Line3d l2) {
			Line3d l = Line3d.FromPoints(point1, point2);
			Vector3d v = l.Intercept(l2);
			//		System.out.println("[LineSection3d]: (getting Intercept): this:"+v+" result:"+Contains(v));
			if(Contains1(v))
				return v;
			return v;
			//		else return null;
		}

		public Vector3d Intercept(LineSection3d l2) {
			Line3d l = Line3d.FromPoints(point1, point2);
			Vector3d v = l.Intercept(Line3d.FromPoints(point1, point2));
			//		System.out.println("[LineSection3d]: (getting Intercept): this:"+v+" result:"+Contains(v));
			if(Contains1(v))
				return v;
			else
				return null;
		}

		public override String ToString() {
			return "LineSection3d["+point1.ToString()+";"+point2.ToString()+";"+Convert.ToString(GetLength())+"]";
		}
	}

}
