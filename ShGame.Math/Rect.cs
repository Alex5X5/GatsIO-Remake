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

	public Vector3d TopLeft => new Vector3d(Pos.X, Pos.Y, 0);
	public Vector3d BottomLeft => new Vector3d(Pos.X, Pos.Y+Height, 0);
	public Vector3d TopRight => new Vector3d(Pos.X+Width, Pos.Y, 0);
	public Vector3d BottomRight => new Vector3d(Pos.X+Width, Pos.Y+Height, 0);

	public const int SizeInBytes = 40;

	public Rect(double x, double y, double width, double height) : this(new Vector3d(x, y, 0), width, height) { }

	public Rect(Vector3d pos, Vector3d bounds) : this(pos, bounds.X, bounds.Y) { }

	public Rect(Vector3d pos, double width, double height) {
		Pos = pos;
		Width = width;
		Height = height;
	}
}
