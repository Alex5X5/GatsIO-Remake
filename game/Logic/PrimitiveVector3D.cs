namespace ShGame.game.Logic;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)] // Ensures no extra padding is added
struct PrimitiveVector3 {
	public double X;
	public double Y;
	public double Z;
}