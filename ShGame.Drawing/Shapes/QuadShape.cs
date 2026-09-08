namespace ShGame.Drawing.Shapes;

using ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct QuadShape {

	internal TriangleShape triangle1, triangle2;

	public const uint SizeInBytes = 2 * TriangleShape.SizeInBytes;

	internal QuadShape(Quad quad, Color color) {
		triangle1 = new TriangleShape(new Triangle(quad.P1, quad.P2, quad.P4), color);
		triangle2 = new TriangleShape(new Triangle(quad.P1, quad.P3, quad.P4), color);
	}

	private static double Cross(Vector3d a, Vector3d b, Vector3d c) {
		return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
	}
}
