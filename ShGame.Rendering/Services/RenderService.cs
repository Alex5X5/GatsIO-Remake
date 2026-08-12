namespace ShGame.Rendering.Services;

using ShGame.Drawing;
using ShGame.Game;
using ShGame.Math;

public class RenderService {

	private GameService game;

	public RenderService(GameService game) {
		this.game = game;
	}

	public void OnDraw(DrawingContext context) {

		foreach (var o in game.Map.Obstacles) {
			var col = o.type switch {
				0 => Color.BLUE,
				1 => Color.BLUE,
				2 => Color.YELLOW,
				_ => Color.GREEN
			};
			context.DrawRectangle(new Rect(o.Pos.X, o.Pos.Y, o.WIDTH, o.HEIGHT), col);
		}

		foreach (var p in game.Players) {
			MappingService.ToModel(p).Draw(context);
		}
	}

}
