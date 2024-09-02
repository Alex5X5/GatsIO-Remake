namespace ShGame.game.Logic;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)] // Ensures no extra padding is added
struct PrimitiveVector3I {
	public int X;
	public int Y;
	public int Z;
}