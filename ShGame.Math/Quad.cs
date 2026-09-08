namespace ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Quad {

	public Vector3d P1, P2, P3, P4;

	public Quad(Vector3d p1, Vector3d p2, Vector3d p3, Vector3d p4) {
		P1 = p1;
		P2 = p2;
		P3 = p3;
		P4 = p4;
	}

}
