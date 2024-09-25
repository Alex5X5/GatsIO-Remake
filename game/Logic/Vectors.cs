using System.Runtime.InteropServices;

namespace ShGame.game.Logic.PrimitiveVector3i {

	[StructLayout(LayoutKind.Sequential, Pack = 1)] // Ensures no extra padding is added
	public struct PrimitiveVector3i {
		public int X;
		public int Y;
		public int Z;
	}

	public static class Vector3iOperations {

		public static unsafe PrimitiveVector3i Cpy(this PrimitiveVector3i v) {
			PrimitiveVector3i res = new();
			return *res.Set(&v.X, &v.Y, &v.Z);
		}

		public static unsafe PrimitiveVector3i* Set(this PrimitiveVector3i vec, int* x, int* y, int* z) {
			vec.X = *x;
			vec.Y = *y;
			vec.Z = *z;
			return &vec;
		}

		//public static unsafe void Set(ref PrimitiveVector3i vec, int* x, int* y, int* z) {
		//	vec.X = (int)Math.Floor((double)*x);
		//	vec.Y = (int)Math.Floor((double)*y);
		//	vec.Z = (int)Math.Floor((double)*z);
		//}

		public static void Add(ref PrimitiveVector3i vec, int x, int y, int z) {
			vec.X += x;
			vec.Y += y;
			vec.Z += z;
		}

		public static void Set(ref PrimitiveVector3i vec, ref int[] values) {
			vec.X = values[0];
			vec.Y = values[1];
			vec.Z = values[2];
		}

		public static void Add(ref PrimitiveVector3i vector1, ref PrimitiveVector3i vector2) {
			Add(ref vector1, vector2.X, vector2.Y, vector2.Z);
		}

		public static PrimitiveVector3i Cpy() {
			return new PrimitiveVector3i();
		}

		public static void Sub(ref PrimitiveVector3i vector, int x, int y, int z) {
			vector.X -= x;
			vector.Y -= y;
			vector.Z -= z;
		}

		public static void Sub(ref PrimitiveVector3i vector, ref PrimitiveVector3i vector2) {
			Sub(ref vector, vector2.X, vector2.Y, vector2.Z);
		}

		public static unsafe void Sub(ref PrimitiveVector3i vector, int value) {
			vector.Set((int*)(vector.X-value), (int*)(vector.Y-value), (int*)(vector.Z-value));
		}

		public static unsafe void Scl(ref PrimitiveVector3i vector, int value) {
			vector.Set((int*)(vector.X*value), (int*)(vector.Y*value), (int*)(vector.Z*value));
		}

		public unsafe static void Scl(ref PrimitiveVector3i vector, int value1, int value2, int value3) {
			vector.Set((int*)(vector.X*value1), (int*)(vector.Y*value2), (int*)(vector.Z*value3));
		}

		public static double Len(double x, double y, double z)
			=> Math.Sqrt(x*x+y*y+z*z);

		public static double Len(ref PrimitiveVector3i vector)
			=> Math.Sqrt(vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z);

		public static double Len2(ref PrimitiveVector3i vector)
			=> vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z;

		public static bool Idt(ref PrimitiveVector3i vector1, ref PrimitiveVector3i vector2)
			=> vector1.X==vector2.X&&vector1.Y==vector2.Y&&vector1.Z==vector2.Z;

		public static double Dst(int x1, int y1, int z1, int x2, int y2, int z2) {
			double a = x2-x1;
			double b = y2-y1;
			double c = z2-z1;
			return (double)Math.Sqrt(a*a+b*b+c*c);
		}

		public static double Dst(ref PrimitiveVector3i vector1, ref PrimitiveVector3i vector2)
			=> Dst(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);

		public static double Dst(ref PrimitiveVector3i vector, int x, int y, int z)
			=> Dst(vector.X, vector.Y, vector.Z, x, y, z);

		public static double Dst2(double x1, double y1, double z1, double x2, double y2, double z2) {
			double a = x2-x1;
			double b = y2-y1;
			double c = z2-z1;
			return a*a+b*b+c*c;
		}

		public static double Dst2(ref PrimitiveVector3i vector1, PrimitiveVector3i vector2)
			=> Dst2(ref vector1, vector2.X, vector2.Y, vector2.Z);

		public static double Dst2(ref PrimitiveVector3i vector, int x, int y, int z)
			=> Dst2(vector.X, vector.Y, vector.Z, x, y, z);

		public static string ToString(ref PrimitiveVector3i vector)
			=> "("+vector.X+","+vector.Y+","+vector.Z+")";

		public static void SetZero(ref PrimitiveVector3i vector) {
			vector.X = 0;
			vector.Y = 0;
			vector.Z = 0;
		}
	}
}


namespace ShGame.game.Logic.PrimitiveVector3d {

	[StructLayout(LayoutKind.Sequential, Pack = 1)] [Serializable]// Ensures no extra padding is added
	public struct Vector3d(double x, double y, double z) {
		public double X = x;
		public double Y = y;
		public double Z = z;
	}

	public static class Vector3dOperations {
		public static unsafe Vector3d* Set(this Vector3d vector, double x, double y, double z) {
			vector.X=x;
			vector.Y=y;
			vector.Z=z;
			return &vector;
		}

		public static unsafe Vector3d* Set(this Vector3d vector, Vector3d* vector2) =>
			Set(vector, vector2->X, vector2->Y, vector2->Z);

		public static unsafe Vector3d Cpy(this Vector3d vector) {
			Vector3d result = new();
			return *vector.Set(&result); ;
		}

		public static unsafe Vector3d* Add(this Vector3d vector, double x, double y, double z) =>
			Set(vector, vector.X+x, vector.Y+y, vector.Z+z);

		public static unsafe Vector3d* Add(this Vector3d vector, Vector3d vector2) =>
			Add(vector, vector2.X, vector2.Y, vector2.Z);

		public static unsafe Vector3d* Add(this Vector3d vector, double value) =>
			Set(vector, vector.X + value, vector.Y + value, vector.Z + value);

		public static unsafe Vector3d* Sub(this Vector3d vector, double x, double y, double z) =>
			Set(vector, vector.X - x, vector.Y - y, vector.Z - z);

		public static unsafe Vector3d* Sub(this Vector3d vector, double value) =>
			Set(vector, vector.X - value, vector.Y - value, vector.Z - value);

		public static unsafe Vector3d* Sub(this Vector3d vector, Vector3d vector2) =>
			Sub(vector, vector2.X, vector2.Y, vector2.Z);

		public static unsafe Vector3d* Scl(this Vector3d vector, double scalar) =>
			Set(vector, vector.X*scalar, vector.Y*scalar, vector.Z*scalar);

		public static unsafe Vector3d* Scl(this Vector3d vector, Vector3d other) =>
			Set(vector, vector.X * other.X, vector.Y * other.Y, vector.Z * other.Z);

		public static unsafe Vector3d* Scl(this Vector3d vector, double vx, double vy, double vz) =>
			Set(vector, vector.X * vx, vector.Y * vy, vector.Z * vz);

		public static double Len(double x, double y, double z) =>
			Math.Sqrt(x*x+y*y+z*z);

		public static double Len(this Vector3d vector) =>
			Math.Sqrt(vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z);

		public static double Len2(double x, double y, double z) =>
			x*x+y*y+z*z;

		public static double Len2(this Vector3d vector) =>
			vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z;

		public static unsafe bool Idt(this Vector3d vector, Vector3d* vector2) {
			return vector.X==vector2->X && vector.Y==vector2->Y &&vector.Z==vector2->Z;
		}

		public static double Dst(double x1, double y1, double z1, double x2, double y2, double z2) {
			double a = x2-x1;
			double b = y2-y1;
			double c = z2-z1;
			return (double)Math.Sqrt(a*a+b*b+c*c);
		}

		public static unsafe double Dst(this Vector3d vector, Vector3d* vector2) {
			double a = vector2->X-vector.X;
			double b = vector2->Y-vector.Y;
			double c = vector2->Z-vector.Z;
			return Math.Sqrt(a*a+b*b+c*c);
		}

		public static unsafe double Dst(this Vector3d vector, double x, double y, double z) {
			double a = x-vector.X;
			double b = y-vector.Y;
			double c = z-vector.Z;
			return Math.Sqrt(a*a+b*b+c*c);
		}

		public static unsafe double Dst2(double x1, double y1, double z1, double x2, double y2, double z2) {
			double a = x2-(x1);
			double b = y2-(y1);
			double c = z2-(z1);
			return a*a+b*b+c*c;
		}

		public static unsafe double Dst2(this Vector3d vector, double x, double y, double z) {
			double a = x-vector.X;
			double b = y-vector.Y;
			double c = z-vector.Z;
			return a*a+b*b+c*c;
		}

		public static unsafe double Dst2(this Vector3d vector, Vector3d* vector2) {
			double a = vector.X-vector2->X;
			double b = vector.Y-vector2->Y;
			double c = vector.Z-vector2->Z;
			return a*a+b*b+c*c;
		}

		public static unsafe Vector3d* Nor(this Vector3d vector) {
			double len2 = Len2(vector);
			if (len2==0f||len2==1f)
				return &vector;
			return Scl(vector, 1f/Math.Sqrt(len2));
		}

		public static unsafe double Dot(double x1, double y1, double z1, double x2, double y2, double z2) {
			return x1*x2+y1*y2+z1*z2;
		}

		public static unsafe double Dot(this Vector3d vector, Vector3d* vector2) =>
			vector.X*vector2->X+vector.Y*vector2->Y+vector.Z*vector2->Z;

		public static unsafe double Dot(this Vector3d vector, double x, double y, double z) {
			return vector.X*x+vector.Y*y+vector.Z*z;
		}

		public static unsafe Vector3d* Crs(this Vector3d vector, ref Vector3d* vector2) {
			return Set(
				vector,
				vector.Y*vector2->Z-vector.Z*vector2->Y,
				vector.Z*vector2->X -vector.X*vector2->Z,
				vector.X*vector2->Y-vector.Y*vector2->X
			);
		}

		public static unsafe Vector3d* Crs(this Vector3d vector, double x, double y, double z) {
			return Set(
				vector, vector.Y*z-vector.Z*y,
				vector.Z*x-vector.X*z,
				vector.X*y-vector.Y*x
			);
		}
		public static string ToString(Vector3d v) =>
			$"({v.X},{v.Y},{v.Z})";
		public static unsafe Vector3d* Limit(this Vector3d vector, double limit) {
			return Limit2(vector, limit*limit);
		}
		public static unsafe Vector3d* Limit2(this Vector3d vector, double limit2) {
			double len2 = Len2(vector);
			if (len2>limit2) {
				return Scl(vector, Math.Min(limit2, len2));
			}
			return &vector;
		}
		public static unsafe Vector3d* SetLength(this Vector3d vector, double len) {
			return SetLength2(vector, len*len);
		}
		public static unsafe Vector3d* SetLength2(this Vector3d vector, double len2) {
			double oldLen2 = Len2(vector);
			double scl = (double)Math.Sqrt(len2/oldLen2);
			return oldLen2==0||oldLen2==len2 ? &vector : Scl(vector, Math.Sqrt(len2/oldLen2));
		}

		public static unsafe Vector3d* SetZero(this Vector3d vector) {
			vector.X=0;
			vector.Y=0;
			vector.Z=0;
			return &vector;
		}
	}
}