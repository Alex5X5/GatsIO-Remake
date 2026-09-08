namespace ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Circle {

	public Vector3d Pos;

	public double Radius;

	public const int SizeInBytes = 40;

	public Circle(double x, double y, double radius) : this(new Vector3d(x, y, 0), radius) { }
	
	public Circle(Vector3d pos, double radius) {
		Pos = pos;
		Radius = radius;
	}
}
