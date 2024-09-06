namespace ShGame.game.Logic;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)] // Ensures no extra padding is added
public struct PrimitiveVector3I {
	public int X;
	public int Y;
	public int Z;
}

public struct PrimitiveVector3D {
	public int X;
	public int Y;
	public int Z;
}

public static class Vector3IOperations {

	public static PrimitiveVector3I Cpy(ref PrimitiveVector3I v) {
		PrimitiveVector3I res = new();	
		Set(ref res, v.X, v.Y, v.Z);
		return res;
	}

	public static void Set(ref PrimitiveVector3I vec, int x, int y, int z) {
		vec .X = x;
		vec .Y = y;
		vec .Z = z;
	}

	public static void Set(ref PrimitiveVector3I vec, double x, double y, double z) {
		vec.X = (int)Math.Floor(x);
		vec.Y = (int)Math.Floor(y);
		vec.Z = (int)Math.Floor(z);
	}

	public static void Add(ref PrimitiveVector3I vec, int x, int y, int z) {
		vec.X += x;
		vec.Y += y;
		vec.Z += z;
	}

	public static void Set(ref PrimitiveVector3I vec, ref int[] values){
		vec .X = values[0];
		vec .Y = values[1];
		vec .Z = values[2];
	}

	public static void Add(ref PrimitiveVector3I vector1, ref PrimitiveVector3I vector2) {
		Add(ref vector1, vector2.X, vector2.Y, vector2.Z);
	}

	public static PrimitiveVector3I Cpy() {
		return new PrimitiveVector3I();
	}

	public static void Sub(ref PrimitiveVector3I vector, int x, int y, int z) {
		vector.X -= x;
		vector.Y -= y;
		vector.Z -= z;
	}

	public static void Sub(ref PrimitiveVector3I vector, ref PrimitiveVector3I vector2) {
		Sub(ref vector, vector2.X, vector2.Y, vector2.Z);
	}
	
	public static void Sub(ref PrimitiveVector3I vector, int value) {
		Set(ref vector, vector.X-value, vector.Y-value, vector.Z-value);
	}

	public static void Scl(ref PrimitiveVector3I vector, int value) {
		Set(ref vector, vector.X*value, vector.Y*value,vector.Z*value);
	}

	public static void Scl(ref PrimitiveVector3I vector, int value1, int value2, int value3) {
		Set(ref vector, vector.X*value1, vector.Y*value2,vector.Z*value3);
	}

	public static double Len(double x, double y, double z)
		=> Math.Sqrt(x*x+y*y+z*z);

	public static double Len(ref PrimitiveVector3I vector) 
		=> Math.Sqrt(vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z);

	public static double Len2(ref PrimitiveVector3I vector) 
		=> vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z;

	public static bool Idt(ref PrimitiveVector3I vector1, ref PrimitiveVector3I vector2)
		=> vector1.X==vector2.X&&vector1.Y==vector2.Y&&vector1.Z==vector2.Z;

	public static double Dst(int x1, int y1, int z1, int x2, int y2, int z2) {
		double a = x2-x1;
		double b = y2-y1;
		double c = z2-z1;
		return (double)Math.Sqrt(a*a+b*b+c*c);
	}

	public static double Dst(ref PrimitiveVector3I vector1, ref PrimitiveVector3I vector2)
		=> Dst(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);

	public static double Dst(ref PrimitiveVector3I vector, int x, int y, int z)
		=> Dst(vector.X, vector.Y, vector.Z, x ,y ,z);

	public static double Dst2(double x1, double y1, double z1, double x2, double y2, double z2) {
		double a = x2-x1;
		double b = y2-y1;
		double c = z2-z1;
		return a*a+b*b+c*c;
	}

	public static double Dst2(ref PrimitiveVector3I vector1, PrimitiveVector3I vector2)
		=> Dst2(ref vector1, vector2.X, vector2.Y, vector2.Z);

	public static double Dst2(ref PrimitiveVector3I vector, int x, int y, int z)
		=> Dst2(vector.X, vector.Y, vector.Z, x, y, z);

	public static string ToString(ref PrimitiveVector3I vector)
		=> "("+vector.X+","+vector.Y+","+vector.Z+")";

	public static void SetZero(ref PrimitiveVector3I vector) {
		vector.X = 0;
		vector.Y = 0;
		vector.Z = 0;
	}
}