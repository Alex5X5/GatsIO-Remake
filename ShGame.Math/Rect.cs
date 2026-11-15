namespace ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Rect {

	public Vector3d Pos;

	public double Width, Height;

	public double X {
		set => Pos.X = value;
		get => Pos.X;
	}

	public double Y {
		set => Pos.Y = value;
		get => Pos.Y;
	}

	public const int SizeInBytes = 40;

	public Rect(double x, double y, double width, double height) : this(new Vector3d(x, y, 0), width, height) { }

	public Rect(Vector3d pos, Vector3d bounds) : this(pos, bounds.X, bounds.Y) { }

	public Rect(Vector3d pos, double width, double height) {
		Pos = pos;
		Width = width;
		Height = height;
	}
}
