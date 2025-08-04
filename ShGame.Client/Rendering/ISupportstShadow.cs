namespace ShGame;
using ShGame.Math;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISupportsShadow {

	public Vector3d GetRelativeVector();

	public Vector3d GetPointOfView();

	public void GetShadowOrigins(out Vector3d point1, out Vector3d point2, out Dir dir);
}