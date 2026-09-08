namespace ShGame.Rendering.Interfaces;

using System.Collections.Generic;

using ShGame.Drawing;

public interface IHasChildren {

	IEnumerable<Control> Children { get; }

}
