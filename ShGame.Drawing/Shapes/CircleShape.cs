namespace ShGame.Drawing.Shapes;

using ShGame.Math;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct CircleShape {

	internal readonly TriangleShape[] triangles;

	internal readonly int SidesCount;

	public const uint SizeInBytes = 2 * TriangleShape.SizeInBytes + ColorF.SizeInBytes;

	internal CircleShape(Circle circle, Color color) {
		SidesCount = (int)System.Math.Ceiling(System.Math.Sqrt((float)circle.Radius * 2 * System.Math.PI) * 2);
		triangles = new TriangleShape[SidesCount];
		float[] offsets = CalcCircleOffsets(SidesCount, circle.Radius);
		for (int i = 1; i<SidesCount; i++) {
			int i_ = i * 6;
			triangles[i] = new TriangleShape(
				new PointF(offsets[i_ + 0] + (float)circle.Pos.X, offsets[i_ + 1] + (float)circle.Pos.Y),
				new PointF(offsets[i_ + 2] + (float)circle.Pos.X, offsets[i_ + 3] + (float)circle.Pos.Y),
				new PointF(offsets[i_ + 4] + (float)circle.Pos.X, offsets[i_ + 5] + (float)circle.Pos.Y),
				color
			);
		}
	}

	private static float[] CalcCircleOffsets(int count, double radius) {
		float[] res = new float[(count)*6];
		float radius_ = (float)radius;
		res[0] = 0;
		res[1] = 0;
		res[2] = 0;
		res[3] = radius_;
		res[4] = (float)System.Math.Sin(System.Math.PI*2.0/(count-2))*radius_;
		res[5] = (float)System.Math.Cos(System.Math.PI*2.0/(count-2))*radius_;

		for (int i = 1; i<count; i++) {
			int i_ = i*6;
			res[i_] = res[0];
			res[i_+1] = res[1];
			res[i_+2] = res[i_-6+4];
			res[i_+3] = res[i_-6+5];
			res[i_+4] = (float)System.Math.Sin(System.Math.PI*2.0/(count-2)*i)*radius_;
			res[i_+5] = (float)System.Math.Cos(System.Math.PI*2.0/(count-2)*i)*radius_;
		}

		return res;
	}
}
