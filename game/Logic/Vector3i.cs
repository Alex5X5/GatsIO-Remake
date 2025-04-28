using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShGame.game.Logic;
#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.

[StructLayout(LayoutKind.Sequential, Pack = 1)] // Ensures no extra padding is added
public struct Vector3i {
    public int X;
    public int Y;
    public int Z;
}

public static class Vector3iOperations {

    public static unsafe Vector3i Cpy(this Vector3i v) {
        Vector3i res = new();
        return *res.Set(&v.X, &v.Y, &v.Z);
    }

    public static unsafe Vector3i* Set(this Vector3i vec, int* x, int* y, int* z) {
        vec.X = *x;
        vec.Y = *y;
        vec.Z = *z;
        return &vec;
    }

    //public static unsafe void Set(ref Vector3i vec, int* x, int* y, int* z) {
    //	vec.X = (int)Math.Floor((double)*x);
    //	vec.Y = (int)Math.Floor((double)*y);
    //	vec.Z = (int)Math.Floor((double)*z);
    //}

    public static void Add(ref Vector3i vec, int x, int y, int z) {
        vec.X += x;
        vec.Y += y;
        vec.Z += z;
    }

    public static void Set(ref Vector3i vec, ref int[] values) {
        vec.X = values[0];
        vec.Y = values[1];
        vec.Z = values[2];
    }

    public static void Add(ref Vector3i vector1, ref Vector3i vector2) {
        Add(ref vector1, vector2.X, vector2.Y, vector2.Z);
    }

    public static Vector3i Cpy() {
        return new Vector3i();
    }

    public static void Sub(ref Vector3i vector, int x, int y, int z) {
        vector.X -= x;
        vector.Y -= y;
        vector.Z -= z;
    }

    public static void Sub(ref Vector3i vector, ref Vector3i vector2) {
        Sub(ref vector, vector2.X, vector2.Y, vector2.Z);
    }

    public static unsafe void Sub(ref Vector3i vector, int value) {
        vector.Set((int*)(vector.X-value), (int*)(vector.Y-value), (int*)(vector.Z-value));
    }

    public static unsafe void Scl(ref Vector3i vector, int value) {
        vector.Set((int*)(vector.X*value), (int*)(vector.Y*value), (int*)(vector.Z*value));
    }

    public unsafe static void Scl(ref Vector3i vector, int value1, int value2, int value3) {
        vector.Set((int*)(vector.X*value1), (int*)(vector.Y*value2), (int*)(vector.Z*value3));
    }

    public static double Len(double x, double y, double z)
        => Math.Sqrt(x*x+y*y+z*z);

    public static double Len(ref Vector3i vector)
        => Math.Sqrt(vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z);

    public static double Len2(ref Vector3i vector)
        => vector.X*vector.X+vector.Y*vector.Y+vector.Z*vector.Z;

    public static bool Idt(ref Vector3i vector1, ref Vector3i vector2)
        => vector1.X==vector2.X&&vector1.Y==vector2.Y&&vector1.Z==vector2.Z;

    public static double Dst(int x1, int y1, int z1, int x2, int y2, int z2) {
        double a = x2-x1;
        double b = y2-y1;
        double c = z2-z1;
        return (double)Math.Sqrt(a*a+b*b+c*c);
    }

    public static double Dst(ref Vector3i vector1, ref Vector3i vector2)
        => Dst(vector1.X, vector1.Y, vector1.Z, vector2.X, vector2.Y, vector2.Z);

    public static double Dst(ref Vector3i vector, int x, int y, int z)
        => Dst(vector.X, vector.Y, vector.Z, x, y, z);

    public static double Dst2(double x1, double y1, double z1, double x2, double y2, double z2) {
        double a = x2-x1;
        double b = y2-y1;
        double c = z2-z1;
        return a*a+b*b+c*c;
    }

    public static double Dst2(ref Vector3i vector1, Vector3i vector2)
        => Dst2(ref vector1, vector2.X, vector2.Y, vector2.Z);

    public static double Dst2(ref Vector3i vector, int x, int y, int z)
        => Dst2(vector.X, vector.Y, vector.Z, x, y, z);

    public static string ToString(ref Vector3i vector)
        => "("+vector.X+","+vector.Y+","+vector.Z+")";

    public static void SetZero(ref Vector3i vector) {
        vector.X = 0;
        vector.Y = 0;
        vector.Z = 0;
    }
}

