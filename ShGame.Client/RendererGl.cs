using Silk.NET.Windowing;

namespace ShGame.Client.Rendering;

public class RendererGl : RenderService {

	public RendererGl() : base() {
		
	}

	public void SetVisible() {
		var options = WindowOptions.Default;
		options.Size = new Silk.NET.Maths.Vector2D<int>(Constants.MAP_GRID_WIDTH, Constants.MAP_GRID_HEIGHT);
		options.Title = "ShGame";

		SetVisible(options);
	}
}