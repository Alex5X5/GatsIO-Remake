namespace ShGame.Drawing.Interfaces;
using ShGame.Math;

public interface ISupportsShadow {

	public Vector3d GetRelativeVector();

	public Vector3d GetPointOfView();

	public void GetShadowOrigins(out Vector3d point1, out Vector3d point2, out Dir dir);
}