namespace ShGame.Drawing.Shapes;

using ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct RectShape {

	internal TriangleShape triangle1, triangle2;

	public const int SizeInBytes = 2 * TriangleShape.SizeInBytes + ColorF.SizeInBytes;

	internal RectShape(Rect rect, Color color) {
		var topLeft = rect.Pos;
		var topRight = topLeft.Add(rect.Width, 0, 0);
		var bottomLeft = topLeft.Add(0, rect.Height, 0);
		var bottomRight = bottomLeft.Add(rect.Width, 0, 0);
		triangle1 = new TriangleShape(new Triangle(topLeft, topRight, bottomRight), color);
		triangle2 = new TriangleShape(new Triangle(topLeft, bottomLeft, bottomRight), color);
	}
}
