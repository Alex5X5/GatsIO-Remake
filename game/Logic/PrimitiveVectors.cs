namespace ShGame.game.Logic;

using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Security.Permissions;

[StructLayout(LayoutKind.Sequential, Pack = 1)] // Ensures no extra padding is added
public struct PrimitiveVector3i {
	public int X;
	public int Y;
	public int Z;
}

public struct PrimitiveVector3d {
	public double X;
	public double Y;
	public double Z;
}

public static class Vector3iOperations {

	public static PrimitiveVector3i Cpy(ref PrimitiveVector3i v) {
		PrimitiveVector3i res = new();	
		Set(ref res, v.X, v.Y, v.Z);
		return res;
	}

	public static void Set(ref PrimitiveVector3i vec, int x, int y, int z) {
		vec .X = x;
		vec .Y = y;
		vec .Z = z;
	}

	public static void Set(ref PrimitiveVector3i vec, double x, double y, double z) {
		vec.X = (int)Math.Floor(x);
		vec.Y = (int)Math.Floor(y);
		vec.Z = (int)Math.Floor(z);
	}

	public static void Add(ref PrimitiveVector3i vec, int x, int y, int z) {
		vec.X += x;
		vec.Y += y;
		vec.Z += z;
	}

	public static void Set(ref PrimitiveVector3i vec, ref int[] values){
		vec .X = values[0];
		vec .Y = values[1];
		vec .Z = values[2];
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
	
	public static void Sub(ref PrimitiveVector3i vector, int value) {
		Set(ref vector, vector.X-value, vector.Y-value, vector.Z-value);
	}

	public static void Scl(ref PrimitiveVector3i vector, int value) {
		Set(ref vector, vector.X*value, vector.Y*value,vector.Z*value);
	}

	public static void Scl(ref PrimitiveVector3i vector, int value1, int value2, int value3) {
		Set(ref vector, vector.X*value1, vector.Y*value2,vector.Z*value3);
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
		=> Dst(vector.X, vector.Y, vector.Z, x ,y ,z);

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

public static class PrimitiveVector3dOperations {
    public static ref PrimitiveVector3d Set(ref PrimitiveVector3d vector, double x, double y, double z) {
        vector.X=x;
        vector.Y=y;
        vector.Z=z;
        return ref vector;
    }

    public static ref PrimitiveVector3d Set(ref PrimitiveVector3d vector, ref PrimitiveVector3d vector2) {
        return ref Set(ref vector, vector2.X, vector2.Y, vector2.Z);
    }

    public static PrimitiveVector3d Cpy(ref PrimitiveVector3d vector) {
        PrimitiveVector3d result = new();
        return Set(ref result, ref vector); ;
    }

    public static ref PrimitiveVector3d Add(ref PrimitiveVector3d vector, double x, double y, double z) {
        return ref Set(ref vector, vector.X+x, vector.Y+y, vector.Z+z);
    }

    public static ref PrimitiveVector3d Add(ref PrimitiveVector3d vector, PrimitiveVector3d vector2) {
        return ref Add(ref vector, vector2.X, vector2.Y, vector2.Z);
    }

    public static ref PrimitiveVector3d Add(ref PrimitiveVector3d vector, double value) {
        return ref Set(ref vector, vector.X+value, vector.Y+value, vector.Z+value);
    }

    public static ref PrimitiveVector3d Sub(ref PrimitiveVector3d vector, double x, double y, double z) {
        return ref Set(ref vector, vector.X-x, vector.Y-y, vector.Z-z);
    }

    public static ref PrimitiveVector3d Sub(ref PrimitiveVector3d vector, double value) {
        return ref Set(ref vector, vector.X-value, vector.Y-value, vector.Z-value);
    }

    public static ref PrimitiveVector3d Sub(ref PrimitiveVector3d vector, PrimitiveVector3d vector2) {
        return ref Sub(ref vector, vector2.X, vector2.Y, vector2.Z);
    }

    public static ref PrimitiveVector3d Scl(ref PrimitiveVector3d vector, double scalar) {
        return ref Set(ref vector, vector.X*scalar, vector.Y*scalar, vector.Z*scalar);
    }

    public static ref PrimitiveVector3d Scl(ref PrimitiveVector3d vector, PrimitiveVector3d other) {
        return ref Set(ref vector, vector.X*other.X, vector.Y*other.Y, vector.Z*other.Z);
    }

    public static ref PrimitiveVector3d Scl(ref PrimitiveVector3d vector, double vx, double vy, double vz) {
        return ref Set(ref vector, vector.X*vx, vector.Y*vy, vector.Z*vz);
    }

    public static double Len(double x, double y, double z) {
        return Math.Sqrt(x*x+y*y+z*z);
    }

    public static double Len(ref PrimitiveVector3d vector) {
        return Math.Sqrt(vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z);
    }

    public static double Len2(double x, double y, double z) {
        return x*x+y*y+z*z;
    }

    public static double Len2(ref PrimitiveVector3d vector) {
        return vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z;
    }

    public static bool Idt(ref PrimitiveVector3d vector, ref PrimitiveVector3d vector2) {
        return vector.X==vector2.X&&vector.Y==vector2.Y&&vector.Z==vector2.Z;
    }

    public static double Dst(double x1, double y1, double z1, double x2, double y2, double z2) {
        double a = x2-x1;
        double b = y2-y1;
        double c = z2-z1;
        return (double)Math.Sqrt(a*a+b*b+c*c);
    }

    public static double Dst(ref PrimitiveVector3d vector, ref PrimitiveVector3d vector2) {
        double a = vector2.X-vector.X;
        double b = vector2.Y-vector.Y;
        double c = vector2.Z-vector.Z;
        return Math.Sqrt(a*a+b*b+c*c);
    }

    public static double Dst(ref PrimitiveVector3d vector, double x, double y, double z) {
        double a = x-vector.X;
        double b = y-vector.Y;
        double c = z-vector.Z;
        return Math.Sqrt(a*a+b*b+c*c);
    }

    public static double Dst2(double x1, double y1, double z1, double x2, double y2, double z2) {
        double a = x2-x1;
        double b = y2-y1;
        double c = z2-z1;
        return a*a+b*b+c*c;
    }

    public static double Dst2(ref PrimitiveVector3d vector, double x, double y, double z) {
        double a = x-vector.X;
        double b = y-vector.Y;
        double c = z-vector.Z;
        return a*a+b*b+c*c;
    }

    public static double Dst2(ref PrimitiveVector3d vector, ref PrimitiveVector3d vector2) {
        double a = vector.X-vector2.X;
        double b = vector.Y-vector2.Y;
        double c = vector.Z-vector2.Z;
        return a*a+b*b+c*c;
    }

    public static ref PrimitiveVector3d Nor(ref PrimitiveVector3d vector) {
        double len2 = Len2(ref vector);
        if (len2==0f||len2==1f)
            return ref vector;
        return ref Scl(ref vector, 1f/(double)Math.Sqrt(len2));
    }

    public static double Dot(double x1, double y1, double z1, double x2, double y2, double z2) {
        return x1*x2+y1*y2+z1*z2;
    }

    public static double Dot(ref PrimitiveVector3d vector, ref PrimitiveVector3d vector2) {
        return vector.X*vector2.X+vector.Y*vector2.Y+vector.Z*vector2.Z;
    }

    public static double Dot(ref PrimitiveVector3d vector, double x, double y, double z) {
        return vector.X*x+vector.Y*y+vector.Z*z;
    }

    public static ref PrimitiveVector3d Crs(ref PrimitiveVector3d vector, ref PrimitiveVector3d vector2) {
        return ref Set(ref vector, vector.Y*vector2.Z-vector.Z*vector2.Y, vector.Z*vector2.X-vector.X*vector2.Z, vector.X*vector2.Y-vector.Y*vector2.X);
    }

    public static ref PrimitiveVector3d Crs(ref PrimitiveVector3d vector, double x, double y, double z) {
        return ref Set(ref vector, vector.Y*z-vector.Z*y, vector.Z*x-vector.X*z, vector.X*y-vector.Y*x);
    }
    public static string ToString(PrimitiveVector3d v) {
        return "("+v.X+","+v.Y+","+v.Z+")";
    }
    public static ref PrimitiveVector3d Limit(ref PrimitiveVector3d vector, double limit) {
        return ref Limit2(ref vector, limit*limit);
    }
    public static ref PrimitiveVector3d Limit2(ref PrimitiveVector3d vector, double limit2) {
        double len2 = Len2(ref vector);
        if (len2>limit2) {
            Scl(ref vector, (double)Math.Sqrt(limit2/len2));
        }
        return ref vector;
    }
    public static ref PrimitiveVector3d SetLength(ref PrimitiveVector3d vector, double len) {
        return ref SetLength2(ref vector, len*len);
    }
    public static ref PrimitiveVector3d SetLength2(ref PrimitiveVector3d vector, double len2) {
        double oldLen2 = Len2(ref vector);
        return ref oldLen2==0||oldLen2==len2 ? ref vector : ref Scl(ref vector, (double)Math.Sqrt(len2/oldLen2));
    }

    public static ref PrimitiveVector3d SetZero(ref PrimitiveVector3d vector) {
        vector.X=0;
        vector.Y=0;
        vector.Z=0;
        return ref vector;
    }
}