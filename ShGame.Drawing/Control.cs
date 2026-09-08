namespace ShGame.Drawing;

using ShGame.Rendering.Interfaces;

using System.Collections.Generic;

public class Control : IHasBounds, IHasChildren {

	public Rect Bounds { set; get; }

	public IEnumerable<Control> Children { set; get; }

	public Control() {
		
	}
}
