namespace ShGame.Drawing;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct ColorF {

	public float Red, Green, Blue, Alpha;

	public const int SizeInBytes = 16;

	public ColorF(Color color) {
		Red = (float)color.Red / 255.0f;
		Green = (float)color.Green / 255.0f;
		Blue = (float)color.Blue / 255.0f;
		Alpha = (float)color.Alpha / 255.0f;
	}

}