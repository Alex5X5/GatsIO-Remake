using ShGame.Game.Logic.Math;

namespace ShGame.Game.Client.Rendering;
public unsafe class DebugDrawable : Drawable {
	Vector3d Point1, Point2;

	public DebugDrawable(Vector3d _Point1) : this(null, _Point1) {
	}

	public DebugDrawable(Vector3d? _Point1, Vector3d _Point2) : base(18) {
		Point1 = _Point1 ?? new Vector3d(0, 0, 0);
		Point2 = _Point2;
	}

	public override unsafe void UpdateVertices() {
		float* ptr = VertexDataPtr;
		*ptr = (float)Point1.x;
		ptr++;
		*ptr = (float)Point1.y + 1;
		ptr++;
		*ptr = 0;
		ptr++;
		*ptr = (float)Point1.x;
		ptr++;
		*ptr = (float)Point1.y - 1;
		ptr++;
		*ptr = 0;
		ptr++;
		*ptr = (float)Point2.x;
		ptr++;
		*ptr = (float)Point2.y;
		ptr++;
		*ptr = 0;
		ptr++;
		*ptr = (float)Point1.x;
		ptr++;
		*ptr = (float)Point1.y - 1;
		ptr++;
		*ptr = 0;
		ptr++;
		*ptr = (float)Point2.x;
		ptr++;
		*ptr = (float)Point2.y - 1;
		ptr++;
		*ptr = 0;
		ptr++;
		*ptr = (float)Point2.x;
		ptr++;
		*ptr = (float)Point2.y;
		ptr++;
		*ptr = 0;
	}
}
