using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace ShGame.Game.Logic.Math;

[StructLayout(LayoutKind.Sequential, Pack = 1)]// Ensures no extra padding is added
public unsafe struct Vector3d {

	public double x;
	public double y;
	public double z;

	public const int Size = 12;

	public static readonly Vector3d X = new(1, 0, 0);
	public static readonly Vector3d Y = new(0, 1, 0);
	public static readonly Vector3d Z = new(0, 0, 1);
    public static readonly Vector3d Zero = new(0, 0, 0);

    public static implicit operator Vector3f(Vector3d? v) =>
        new(
            (float)(v!=null ? v.Value.x : 0),
            (float)(v!=null ? v.Value.y : 0),
            (float)(v!=null ? v.Value.z : 0)
        );

	public static implicit operator Vector256<double>(Vector3d v) =>
		Vector256.Create(v.x, v.y, v.z, 0.0);

	public Vector3d():this(0,0,0) {
	}

	public Vector3d(double x, double y, double z) {
		Set(x, y, z);
	}

	public Vector3d(Vector3d vector) {
		Set(vector);
	}

	public Vector3d(double[] values) {
		Set(values[0], values[1], values[2]);
	}

	public Vector3d Add(double _x, double _y, double _z) {
		if (Vector256.IsHardwareAccelerated) {
			Vector256<double> vecA = this;
			Vector256<double> vecB = Vector256.Create(_x, _y, _z, 0.0);
			return Set(Avx.Add(vecA, vecB));
		}
		return Set(x+_x, y+_y, z+_z);
	}

	public Vector3d Add(Vector3d vector) =>
		Add(vector.x, vector.y, vector.z);


	public Vector3d Add(double value) =>
		Add(value, value, value);

	public Vector3d Crs(Vector3d vector) {
		return Set(y*vector.z-z*vector.y, z*vector.x-x*vector.z, x*vector.y-y*vector.x);
	}

	public Vector3d Crs(double x, double y, double z) {
		return Set(this.y*z-this.z*y, this.z*x-this.x*z, this.x*y-this.y*x);
	}

	public readonly Vector3d Cpy() =>
		new(this);

	public static double Dot(double x1, double y1, double z1, double x2, double y2, double z2) {
		return x1*x2+y1*y2+z1*z2;
	}

	public readonly double Dot(Vector3d vector) {
		return x*vector.x+y*vector.y+z*vector.z;
	}

	public readonly double Dot(double x, double y, double z) {
		return this.x*x+this.y*y+this.z*z;
	}

	public static double Dst(double x1, double y1, double z1, double x2, double y2, double z2) {
		double a = x2-x1;
		double b = y2-y1;
		double c = z2-z1;
		return (double)System.Math.Sqrt(a*a+b*b+c*c);
	}

	public readonly double Dst(Vector3d vector) {
		double a = vector.x-x;
		double b = vector.y-y;
		double c = vector.z-z;
		return System.Math.Sqrt(a*a+b*b+c*c);
	}

	public readonly double Dst(double _x, double _y, double _z) {
		if (Vector256.IsHardwareAccelerated) {
			Vector256<double> vecA = Vector256.Create(_x, _y, _z, 0.0);
			vecA=Vector256.Multiply(vecA, vecA);
			return System.Math.Sqrt(Vector256.Sum(vecA));
		}
		return Len(_x-x, _y-y, _z-z);
	}

	public static double Dst2(double x1, double y1, double z1, double x2, double y2, double z2) {
		double a = x2-x1;
		double b = y2-y1;
		double c = z2-z1;
		return a*a+b*b+c*c;
	}

	public readonly double Dst2(Vector3d point) {
		double a = point.x-x;
		double b = point.y-y;
		double c = point.z-z;
		return a*a+b*b+c*c;
	}

	public readonly double Dst2(double x, double y, double z) {
		double a = x-this.x;
		double b = y-this.y;
		double c = z-this.z;
		return a*a+b*b+c*c;
	}

	public readonly bool Idt(Vector3d vector) {
		return x==vector.x&&y==vector.y&&z==vector.z;
	}

	public static double Len(double x, double y, double z) {
		return System.Math.Sqrt(x*x+y*y+z*z);
	}

	public readonly double Len() {
		if (Vector256.IsHardwareAccelerated) {
			Vector256<double> vecA = this;
			vecA=Vector256.Multiply(vecA, vecA);
			return System.Math.Sqrt(Vector256.Sum(vecA));
		}
		return System.Math.Sqrt(x*x+y*y+z*z);
	}

	public readonly double Len2() {
		return x*x+y*y+z*z;
	}

	public static double Len2(double x, double y, double z) {
		return x*x+y*y+z*z;
	}

	public Vector3d Limit(double limit) {
		return Limit2(limit*limit);
	}

	public unsafe Vector3d Limit2(double limit2) {
		double len2 = Len2();
		if (len2>limit2) {
			Scl((double)System.Math.Sqrt(limit2/len2));
		}
		return this;
	}

	public Vector3d MulAdd(Vector3d vec, double scalar) {
		x+=vec.x*scalar;
		y+=vec.y*scalar;
		z+=vec.z*scalar;
		return this;
	}

	public Vector3d MulAdd(Vector3d vec, Vector3d mulVec) {
		x+=vec.x*mulVec.x;
		y+=vec.y*mulVec.y;
		z+=vec.z*mulVec.z;
		return this;
	}

	public Vector3d Nor() {
		double len2 = Len2();
		if (len2==0f||len2==1f)
			return this;
		return Scl(1f/(double)System.Math.Sqrt(len2));
	}

	public Vector3d Scl(double _x, double _y, double _z) {
		if (Vector256.IsHardwareAccelerated)
			return Set(Avx.Multiply(this, Vector256.Create(_x, _y, _z, 0.0)));
		return Set(x*_x, y*_y, z*_z);
	}

	public Vector3d Scl(double value) =>
		Scl(value, value, value);

	public Vector3d Scl(Vector3d vec) =>
		Scl(vec.x, vec.y, vec.z);

	public unsafe Vector3d Set(double x_, double y_, double z_) {
		x=x_;
		y=y_;
		z=z_;
		return this;
	}
	public unsafe Vector3d Set(Vector256<double> vec) {
		x=vec.GetElement(0);
		y=vec.GetElement(1);
		z=vec.GetElement(2);
		return this;
	}

	public Vector3d Set(Vector3d vector) =>
		Set(vector.x, vector.y, vector.z);

	public Vector3d SetLength(double len) {
		return SetLength2(len*len);
	}

	public Vector3d SetLength2(double len2) {
		double oldLen2 = Len2();
		return oldLen2==0||oldLen2==len2 ? this : Scl(System.Math.Sqrt(len2/oldLen2));
	}

	public Vector3d SetZero() {
		x=0;
		y=0;
		z=0;
		return this;
	}

	public Vector3d Sub(double _x, double _y, double _z) {
		if (Vector256.IsHardwareAccelerated)
			return Set(Vector256.Subtract(this, Vector256.Create(_x, _y, _z, 0.0)));
		return Set(x-_x, y-_y, z-_z);
	}

	public Vector3d Sub(double value) =>
		Set(x-value, y-value, z-value);

	public Vector3d Sub(Vector3d vec) =>
		Sub(vec.x, vec.y, vec.z);

	public override string ToString() => "("+x+"|"+y+"|"+z+")";
}