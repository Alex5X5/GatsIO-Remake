namespace ShGame.Math;

using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vector3d {

	public double X, Y, Z;

	public const int SizeInBytes = 24;

	public static readonly Vector3d XAxis = new(1, 0, 0);
	public static readonly Vector3d YAxis = new(0, 1, 0);
	public static readonly Vector3d ZAxis = new(0, 0, 1);
	public static readonly Vector3d Zero = new(0, 0, 0);

	public static implicit operator Vector256<double>(Vector3d v) =>
		Vector256.Create(v.X, v.Y, v.Z, 0.0);

	public static Vector3d operator +(Vector3d vector1, Vector3d vector2) =>
		vector1.Add(vector2);

	public static Vector3d operator -(Vector3d vector1, Vector3d vector2) =>
		vector1.Sub(vector2);

	public static Vector3d operator *(Vector3d vector1, double scalar) =>
		vector1.Scl(scalar);


	public Vector3d() : this(0, 0, 0) { }

	public Vector3d(Vector256<double> vec) : this(vec.GetElement(0), vec.GetElement(1), vec.GetElement(2)) { }

	public Vector3d(Vector3d vector) : this(vector.X, vector.Y, vector.Z) { }

	public Vector3d(double[] values) : this(values[0], values[1], values[2]) { }

	public Vector3d(double x, double y, double z) {
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public Vector3d Add(double _x, double _y, double _z) {
		if (Vector256.IsHardwareAccelerated) {
			Vector256<double> vecA = this;
			Vector256<double> vecB = Vector256.Create(_x, _y, _z, 0.0);
			return new Vector3d(Avx.Add(vecA, vecB));
		}
		return new Vector3d(X+_x, Y+_y, Z+_z);
	}

	public Vector3d Add(Vector3d vector) =>
		Add(vector.X, vector.Y, vector.Z);


	public Vector3d Add(double value) =>
		Add(value, value, value);

	public Vector3d Crs(Vector3d vector) {
		return new Vector3d(Y * vector.Z - Z * vector.Y, Z * vector.X - X * vector.Z, X * vector.Y - Y * vector.X);
	}

	public Vector3d Crs(double x, double y, double z) {
		return new Vector3d(this.Y*z-this.Z*y, this.Z*x-this.X*z, this.X*y-this.Y*x);
	}

	public readonly Vector3d Cpy() =>
		new(this);

	public static double Dot(double x1, double y1, double z1, double x2, double y2, double z2) {
		return x1*x2+y1*y2+z1*z2;
	}

	public readonly double Dot(Vector3d vector) {
		return X*vector.X+Y*vector.Y+Z*vector.Z;
	}

	public readonly double Dot(double x, double y, double z) {
		return this.X*x+this.Y*y+this.Z*z;
	}

	public static double Dst(double x1, double y1, double z1, double x2, double y2, double z2) {
		double a = x2-x1;
		double b = y2-y1;
		double c = z2-z1;
		return (double)System.Math.Sqrt(a*a+b*b+c*c);
	}

	public readonly double Dst(Vector3d vector) {
		double a = vector.X-X;
		double b = vector.Y-Y;
		double c = vector.Z-Z;
		return System.Math.Sqrt(a*a+b*b+c*c);
	}

	public readonly double Dst(double _x, double _y, double _z) {
		if (Vector256.IsHardwareAccelerated) {
			Vector256<double> vecA = Vector256.Create(_x, _y, _z, 0.0);
			vecA = Vector256.Multiply(vecA, vecA);
			return System.Math.Sqrt(Vector256.Sum(vecA));
		} else {
			return Len(_x-X, _y-Y, _z-Z);
		}
	}

	public static double Dst2(double x1, double y1, double z1, double x2, double y2, double z2) {
		double a = x2-x1;
		double b = y2-y1;
		double c = z2-z1;
		return a*a+b*b+c*c;
	}

	public readonly double Dst2(Vector3d point) {
		double a = point.X-X;
		double b = point.Y-Y;
		double c = point.Z-Z;
		return a*a+b*b+c*c;
	}

	public readonly double Dst2(double x, double y, double z) {
		double a = x-this.X;
		double b = y-this.Y;
		double c = z-this.Z;
		return a*a+b*b+c*c;
	}

	public readonly bool Idt(Vector3d vector) {
		return X==vector.X&&Y==vector.Y&&Z==vector.Z;
	}

	public static double Len(double x, double y, double z) {
		if (Vector256.IsHardwareAccelerated) {
			Vector256<double> vec = Vector256.Create(x, y, z, 0.0);
			return Len(vec);
		}
		return System.Math.Sqrt(x*x+y*y+z*z);
	}

	private static double Len(Vector256<double> vec) {
		vec = Vector256.Multiply(vec, vec);
		return System.Math.Sqrt(Vector256.Sum(vec));
	}

	public readonly double Len() =>
		Len(X, Y, Z);

	public readonly double Len2() {
		return X*X+Y*Y+Z*Z;
	}

	public static double Len2(double x, double y, double z) {
		return x*x+y*y+z*z;
	}

	public Vector3d Limit(double limit) {
		return Limit2(limit*limit);
	}

	public unsafe Vector3d Limit2(double limit2) {
		double len2 = Len2();
		if (len2 > limit2) {
			return Scl(System.Math.Sqrt(limit2 / len2));
		} else {
			return this;
		}
	}

	public Vector3d MulAdd(Vector3d vec, double scalar) =>
		new Vector3d(X + vec.X * scalar, Y + vec.Y * scalar, Z + vec.Z * scalar);

	public Vector3d MulAdd(Vector3d vec, Vector3d mulVec) =>
		new Vector3d(X + vec.X * mulVec.X, Y + vec.Y * mulVec.Y, Z + vec.Z * mulVec.Z);

	public Vector3d Nor() {
		double len2 = Len2();
		if (len2==0f||len2==1f)
			return this;
		return Scl(1f/(double)System.Math.Sqrt(len2));
	}

	public Vector3d Scl(double _x, double _y, double _z) {
		if (Vector256.IsHardwareAccelerated)
			return new Vector3d(Avx.Multiply(this, Vector256.Create(_x, _y, _z, 0.0)));
		return new Vector3d(X*_x, Y*_y, Z*_z);
	}

	public Vector3d Scl(double value) =>
		Scl(value, value, value);

	public Vector3d Scl(Vector3d vec) =>
		Scl(vec.X, vec.Y, vec.Z);

	public Vector3d SetLength(double len) {
		return SetLength2(len*len);
	}

	public Vector3d SetLength2(double len2) {
		double oldLen2 = Len2();
		return oldLen2==0||oldLen2==len2 ? this : Scl(System.Math.Sqrt(len2/oldLen2));
	}

	public Vector3d Sub(double _x, double _y, double _z) {
		if (Vector256.IsHardwareAccelerated)
			return new Vector3d(Vector256.Subtract(this, Vector256.Create(_x, _y, _z, 0.0)));
		return new Vector3d(X - _x, Y - _y, Z - _z);
	}

	public Vector3d Sub(double value) =>
		new Vector3d(X-value, Y-value, Z-value);

	public Vector3d Sub(Vector3d vec) =>
		Sub(vec.X, vec.Y, vec.Z);

	public override string ToString() => "("+X+"|"+Y+"|"+Z+")";
}