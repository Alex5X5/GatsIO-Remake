namespace ShGame.Drawing;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct ColoredPointF {

	internal PointF point;
	internal ColorF color;

	public const int SizeInBytes = PointF.SizeInBytes + ColorF.SizeInBytes;

	public ColoredPointF(PointF point, ColorF color) {
		this.point = point;
		this.color = color;
	}
}
