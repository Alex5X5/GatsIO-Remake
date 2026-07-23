namespace ShGame.Drawing.Shapes;

using ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct QuadShape {

	internal TriangleShape triangle1, triangle2;

	public const int SizeInBytes = 2 * TriangleShape.SizeInBytes;

	internal QuadShape(Quad quad, Color color) {

		triangle1 = new TriangleShape(new Triangle(quad.P1, quad.P2, quad.P4), color);
		triangle2 = new TriangleShape(new Triangle(quad.P1, quad.P3, quad.P4), color);

		//double cross1 = Cross(quad.P1, quad.P2, quad.P3);
		//double cross2 = Cross(quad.P1, quad.P3, quad.P4);

		//bool sameSignAC = System.Math.Sign(cross1) == System.Math.Sign(cross2);

		//if (sameSignAC) {
		//	// A-C is a valid internal diagonal
		//	triangle1 = new TriangleShape(new Triangle(quad.P1, quad.P2, quad.P3), color);
		//	triangle2 = new TriangleShape(new Triangle(quad.P1, quad.P3, quad.P4), color);
		//} else {
		//	// A-C would cut outside the quad -> use B-D instead
		//	triangle1 = new TriangleShape(new Triangle(quad.P1, quad.P3, quad.P4), color);
		//	triangle2 = new TriangleShape(new Triangle(quad.P2, quad.P3, quad.P4), color);
		//}

	}

	private static double Cross(Vector3d a, Vector3d b, Vector3d c) {
		return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
	}
}
