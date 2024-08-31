namespace sh_game.game.Logic;
using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential, Pack = 1)] // Ensures no extra padding is added
struct MyStruct {
	public int Field1 = NativeMemory.Alloc();
	public int Field2;
	public int Field3;
}