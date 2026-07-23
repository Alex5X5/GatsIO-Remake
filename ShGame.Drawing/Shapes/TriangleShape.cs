namespace ShGame.Drawing.Shapes;

using System.Runtime.InteropServices;

using ShGame.Math;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct TriangleShape {

	internal ColoredPointF p1, p2, p3;

	internal const int SizeInBytes = 3 * ColoredPointF.SizeInBytes;

	internal TriangleShape(Triangle triangle, Color color) {
		var color_ = new ColorF(color);
		p1 = new ColoredPointF(new PointF(triangle.P1), color_);
		p2 = new ColoredPointF(new PointF(triangle.P2), color_);
		p3 = new ColoredPointF(new PointF(triangle.P3), color_);
	}
}