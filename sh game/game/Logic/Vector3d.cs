using sh_game.game.net;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sh_game.game.Logic {

	[Serializable]
	public class Vector3d {

		public double x;
		public double y;
		public double z;

		public static readonly Vector3d X=new Vector3d(1, 0, 0);
		public static readonly Vector3d Y=new Vector3d(0, 1, 0);
		public static readonly Vector3d Z=new Vector3d(0, 0, 1);
		public static readonly Vector3d Zero=new Vector3d(0, 0, 0);

		public Vector3d() {
		}

		public Vector3d(double x, double y, double z) {
			this.Set(x, y, z);
		}

		public Vector3d(Vector3d vector) {
			this.Set(vector);
		}

		public Vector3d(double[] values) {
			this.Set(values[0], values[1], values[2]);
		}

		public Vector3d Set(double x, double y, double z) {
			this.x=x;
			this.y=y;
			this.z=z;
			return this;
		}

		public Vector3d Set(Vector3d vector) {
			return this.Set(vector.x, vector.y, vector.z);
		}

		public Vector3d Cpy() {
			return new Vector3d(this);
		}

		public Vector3d Add(Vector3d vector) {
			return this.Add(vector.x, vector.y, vector.z);
		}

		public Vector3d Add(double x, double y, double z) {
			return this.Set(this.x+x, this.y+y, this.z+z);
		}

		public Vector3d Add(double values) {
			return this.Set(this.x+values, this.y+values, this.z+values);
		}

		public Vector3d Sub(Vector3d a_vec) {
			return this.Sub(a_vec.x, a_vec.y, a_vec.z);
		}

		public Vector3d Sub(double x, double y, double z) {
			return this.Set(this.x-x, this.y-y, this.z-z);
		}

		public Vector3d Sub(double value) {
			return this.Set(this.x-value, this.y-value, this.z-value);
		}

		public Vector3d Scl(double scalar) {
			return this.Set(this.x*scalar, this.y*scalar, this.z*scalar);
		}

		public Vector3d Scl(Vector3d other) {
			return this.Set(x*other.x, y*other.y, z*other.z);
		}

		public Vector3d Scl(double vx, double vy, double vz) {
			return this.Set(this.x*vx, this.y*vy, this.z*vz);
		}

		public Vector3d MulAdd(Vector3d vec, double scalar) {
			this.x+=vec.x*scalar;
			this.y+=vec.y*scalar;
			this.z+=vec.z*scalar;
			return this;
		}

		public Vector3d MulAdd(Vector3d vec, Vector3d mulVec) {
			this.x+=vec.x*mulVec.x;
			this.y+=vec.y*mulVec.y;
			this.z+=vec.z*mulVec.z;
			return this;
		}

		public static double Len(double x, double y, double z) {
			return Math.Sqrt(x*x+y*y+z*z);
		}

		public double Len() {
			return Math.Sqrt(x*x+y*y+z*z);
		}

		public static double Len2( double x,  double y,  double z) {
			return x*x+y*y+z*z;
		}

		public double Len2() {
			return x*x+y*y+z*z;
		}

		public bool Idt( Vector3d vector) {
			return x==vector.x&&y==vector.y&&z==vector.z;
		}

		public static double Dst(double x1,  double y1,  double z1,  double x2,  double y2,  double z2) {
			 double a=x2-x1;
			 double b=y2-y1;
			 double c=z2-z1;
			return (double)Math.Sqrt(a*a+b*b+c*c);
		}

		public double Dst( Vector3d vector) {
			 double a=vector.x-x;
			 double b=vector.y-y;
			 double c=vector.z-z;
			return (double)Math.Sqrt(a*a+b*b+c*c);
		}

		public double Dst(double x, double y, double z) {
			 double a=x-this.x;
			 double b=y-this.y;
			 double c=z-this.z;
			return (double)Math.Sqrt(a*a+b*b+c*c);
		}

		public static double Dst2( double x1,  double y1,  double z1,  double x2,  double y2,  double z2) {
			 double a=x2-x1;
			 double b=y2-y1;
			 double c=z2-z1;
			return a*a+b*b+c*c;
		}

		public double Dst2(Vector3d point) {
			 double a=point.x-x;
			 double b=point.y-y;
			 double c=point.z-z;
			return a*a+b*b+c*c;
		}

		public double Dst2(double x, double y, double z) {
			 double a=x-this.x;
			 double b=y-this.y;
			 double c=z-this.z;
			return a*a+b*b+c*c;
		}

		public Vector3d Nor() {
			 double len2=this.Len2();
			if(len2==0f||len2==1f)
				return this;
			return this.Scl(1f/(double)Math.Sqrt(len2));
		}

		public static double Dot(double x1, double y1, double z1, double x2, double y2, double z2) {
			return x1*x2+y1*y2+z1*z2;
		}

		public double Dot( Vector3d vector) {
			return x*vector.x+y*vector.y+z*vector.z;
		}

		public double Dot(double x, double y, double z) {
			return this.x*x+this.y*y+this.z*z;
		}

		public Vector3d Crs( Vector3d vector) {
			return this.Set(y*vector.z-z*vector.y, z*vector.x-x*vector.z, x*vector.y-y*vector.x);
		}

		public Vector3d Crs(double x, double y, double z) {
			return this.Set(this.y*z-this.z*y, this.z*x-this.x*z, this.x*y-this.y*x);
		}

		public override string ToString() {
			return "("+x+","+y+","+z+")";
		}

		public Vector3d Limit(double limit) {
			return Limit2(limit*limit);
		}

		public Vector3d Limit2(double limit2) {
			double len2=Len2();
			if(len2>limit2) {
				Scl((double)Math.Sqrt(limit2/len2));
			}
			return this;
		}

		public Vector3d SetLength(double len) {
			return SetLength2(len*len);
		}

		public Vector3d SetLength2(double len2) {
			double oldLen2=Len2();
			return (oldLen2==0||oldLen2==len2) ? this : Scl((double)Math.Sqrt(len2/oldLen2));
		}

		public Vector3d SetZero() {
			this.x=0;
			this.y=0;
			this.z=0;
			return this;
		}
	}
}
