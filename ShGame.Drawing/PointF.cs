namespace ShGame.Drawing;

using ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct PointF {

	internal float x, y;

	public const int SizeInBytes = 8;

	public PointF(float x, float y) {
		this.x = x;
		this.y = y;
	}

	public PointF(Vector3d vector) {
		x = (float)vector.X;
		y = (float)vector.Y;
	}
}
