namespace ShGame.Drawing.Shapes;

using ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct LineShape {

	internal ColoredPointF p1, p2;

	public const int SizeInBytes = 2 * ColoredPointF.SizeInBytes;

	internal LineShape(Line line, Color color) {
		var color_ = new ColorF(color);
		p1 = new ColoredPointF(new PointF(line.A), color_);
		p2 = new ColoredPointF(new PointF(line.B), color_);
	}
}
