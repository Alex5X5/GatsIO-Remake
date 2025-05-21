using System.Drawing;
using System.Runtime.InteropServices;

namespace ShGame.Game.Logic;

[StructLayout(LayoutKind.Sequential, Pack = 1)][Serializable]// Ensures no extra padding is added
public unsafe struct Vector3f {

	public float x;
	public float y;
	public float z;

	public const int Size = 12;
	public Vector3f():this(0,0,0) {
	}

	public Vector3f(float x, float y, float z) {
		Set(x, y, z);
	}

	public Vector3f(Vector3f vector) {
		Set(vector);
	}

	public unsafe Vector3f Set(float x_, float y_, float z_) {
		this.x=x_;
		this.y=y_;
		this.z=z_;
		fixed (float* ptr = &this.x)
		return this;
	}

	public Vector3f Set(Vector3f vector) {
		return Set(vector.x, vector.y, vector.z);
	}

	public Vector3f Cpy() {
		return new Vector3f(this);
	}

	public Vector3f Add(Vector3f vector) {
		return Add(vector.x, vector.y, vector.z);
	}

	public Vector3f Add(float x, float y, float z) {
		return Set(this.x+x, this.y+y, this.z+z);
	}

	public Vector3f Add(float values) {
		return Set(x+values, y+values, z+values);
	}

	public Vector3f Sub(Vector3f a_vec) {
		return Sub(a_vec.x, a_vec.y, a_vec.z);
	}

	public Vector3f Sub(float x, float y, float z) {
		return Set(this.x-x, this.y-y, this.z-z);
	}

	public Vector3f Sub(float value) {
		return Set(x-value, y-value, z-value);
	}

	public Vector3f Scl(float scalar) {
		return Set(x*scalar, y*scalar, z*scalar);
	}

	public Vector3f Scl(Vector3f other) {
		return Set(x*other.x, y*other.y, z*other.z);
	}

	public Vector3f Scl(float vx, float vy, float vz) {
		return Set(x*vx, y*vy, z*vz);
	}

	public Vector3f MulAdd(Vector3f vec, float scalar) {
		x+=vec.x*scalar;
		y+=vec.y*scalar;
		z+=vec.z*scalar;
		return this;
	}

	public Vector3f MulAdd(Vector3f vec, Vector3f mulVec) {
		x+=vec.x*mulVec.x;
		y+=vec.y*mulVec.y;
		z+=vec.z*mulVec.z;
		return this;
	}

	public static float Len(float x, float y, float z) {
		return (float)Math.Sqrt(x*x+y*y+z*z);
	}

	public readonly float Len() {
		return (float)Math.Sqrt(x*x+y*y+z*z);
	}

	public static float Len2(float x, float y, float z) {
		return x*x+y*y+z*z;
	}

	public readonly float Len2() {
		return x*x+y*y+z*z;
	}

	public readonly bool Idt(Vector3f vector) {
		return x==vector.x&&y==vector.y&&z==vector.z;
	}

	public static float Dst(float x1, float y1, float z1, float x2, float y2, float z2) {
		float a = x2-x1;
		float b = y2-y1;
		float c = z2-z1;
		return (float)Math.Sqrt(a*a+b*b+c*c);
	}

	public readonly float Dst(Vector3f vector) {
		float a = vector.x-x;
		float b = vector.y-y;
		float c = vector.z-z;
		return (float)Math.Sqrt(a*a+b*b+c*c);
	}

	public readonly float Dst(float x, float y, float z) {
		float a = x-this.x;
		float b = y-this.y;
		float c = z-this.z;
		return (float)Math.Sqrt(a*a+b*b+c*c);
	}

	public static float Dst2(float x1, float y1, float z1, float x2, float y2, float z2) {
		float a = x2-x1;
		float b = y2-y1;
		float c = z2-z1;
		return a*a+b*b+c*c;
	}

	public readonly float Dst2(Vector3f point) {
		float a = point.x-x;
		float b = point.y-y;
		float c = point.z-z;
		return a*a+b*b+c*c;
	}

	public readonly float Dst2(float x, float y, float z) {
		float a = x-this.x;
		float b = y-this.y;
		float c = z-this.z;
		return a*a+b*b+c*c;
	}

	public Vector3f Nor() {
		float len2 = Len2();
		if(len2==0f||len2==1f)
			return this;
		return Scl(1f/(float)Math.Sqrt(len2));
	}

	public static float Dot(float x1, float y1, float z1, float x2, float y2, float z2) {
		return x1*x2+y1*y2+z1*z2;
	}

	public readonly float Dot(Vector3f vector) {
		return x*vector.x+y*vector.y+z*vector.z;
	}

	public readonly float Dot(float x, float y, float z) {
		return this.x*x+this.y*y+this.z*z;
	}

	public Vector3f Crs(Vector3f vector) {
		return Set(y*vector.z-z*vector.y, z*vector.x-x*vector.z, x*vector.y-y*vector.x);
	}

	public Vector3f Crs(float x, float y, float z) {
		return Set(this.y*z-this.z*y, this.z*x-this.x*z, this.x*y-this.y*x);
	}

	public override string ToString() {
		return "("+x+","+y+","+z+")";
	}

	public Vector3f Limit(float limit) {
		return Limit2(limit*limit);
	}

	public Vector3f Limit2(float limit2) {
		float len2 = Len2();
		if(len2>limit2) {
			Scl((float)Math.Sqrt(limit2/len2));
		}
		return this;
	}

	public Vector3f SetLength(float len) {
		return SetLength2(len*len);
	}

	public Vector3f SetLength2(float len2) {
		float oldLen2 = Len2();
		return oldLen2==0||oldLen2==len2 ? this : Scl((float)Math.Sqrt(len2/oldLen2));
	}

	public Vector3f SetZero() {
		x=0;
		y=0;
		z=0;
		return this;
	}
}