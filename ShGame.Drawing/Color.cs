namespace ShGame.Drawing;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Color {

	public byte Red, Green, Blue, Alpha;

	public Color(byte r, byte g, byte b) : this(r, g, b, 255) { }

	public Color(byte r, byte g, byte b, byte a) {
		Red = r;
		Green = g;
		Blue = b;
		Alpha = a;
	}

	public static readonly Color WHITE = new Color(255, 255, 255);
	public static readonly Color BLACK = new Color(0, 0, 0);

	public static readonly Color GRAY = new Color(115, 115, 115);
	public static readonly Color LIGHT_GRAY = new Color(158, 158, 158);
	public static readonly Color DARK_GRAY = new Color(78, 78, 78);

	public static readonly Color RED = new Color(220, 38, 27);
	public static readonly Color LIGHT_RED = new Color(239, 83, 80);
	public static readonly Color DARK_RED = new Color(128, 0, 0);

	public static readonly Color GREEN = new Color(21, 140, 16);
	public static readonly Color LIGHT_GREEN = new Color(109, 198, 32);
	public static readonly Color DARK_GREEN = new Color(0, 73, 0);

	public static readonly Color BLUE = new Color(0, 0, 235);
	public static readonly Color LIGHT_BLUE = new Color(60, 180, 255);
	public static readonly Color DARK_BLUE = new Color(10, 10, 133);

	public static readonly Color YELLOW = new Color(255, 255, 0);
	public static readonly Color ORANGE = new Color(239, 108, 0);
	public static readonly Color PINK = new Color(255, 0, 255);
	public static readonly Color CYAN = new Color(0, 255, 255);
}
