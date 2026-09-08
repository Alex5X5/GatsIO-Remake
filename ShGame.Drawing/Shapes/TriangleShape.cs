namespace ShGame.Drawing.Shapes;

using System.Runtime.InteropServices;

using ShGame.Math;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct TriangleShape {

	internal ColoredPointF p1, p2, p3;

	internal const uint SizeInBytes = 3 * ColoredPointF.SizeInBytes;

	internal TriangleShape(Triangle triangle, Color color) {
		var color_ = new ColorF(color);
		p1 = new ColoredPointF(new PointF(triangle.P1), color_);
		p2 = new ColoredPointF(new PointF(triangle.P2), color_);
		p3 = new ColoredPointF(new PointF(triangle.P3), color_);
	}

	internal TriangleShape(PointF p1, PointF p2, PointF p3, Color color) {
		var color_ = new ColorF(color);
		this.p1 = new ColoredPointF(p1, color_);
		this.p2 = new ColoredPointF(p2, color_);
		this.p3 = new ColoredPointF(p3, color_);
	}

	internal TriangleShape(ColoredPointF p1, ColoredPointF p2, ColoredPointF p3) {
		this.p1 = p1;
		this.p2 = p2;
		this.p3 = p3;
	}
}