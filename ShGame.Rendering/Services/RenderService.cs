namespace ShGame.Rendering.Services;

using ShGame.Drawing;
using ShGame.Game;
using ShGame.Math;
using ShGame.Types;

public class RenderService {

	private GameService game;

	private Player controlled;

	public RenderService(GameService game, Player controlled) {
		this.game = game;
		this.controlled = controlled;
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
			var col = p.PlayerUUID != controlled.PlayerUUID ? Color.RED : Color.BLUE;
			context.DrawRectangle(new Rect(p.Pos.X, p.Pos.Y, 10, 10), col);
		}
	}

}
