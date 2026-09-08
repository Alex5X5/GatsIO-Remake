namespace ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Triangle {

	public Vector3d P1, P2, P3;

	public const int SizeInBytes = 72;

	public Triangle(double x1, double y1, double x2, double y2, double x3, double y3)
		: this(new Vector3d(x1, y1, 0), new Vector3d(x2, y2, 0), new Vector3d(x3, y3, 0)) { }

	public Triangle(Vector3d p1, Vector3d p2, Vector3d p3) {
		P1 = p1;
		P2 = p2;
		P3 = p3;
	}
}