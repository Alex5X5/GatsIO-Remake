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
				0 => Color.DARK_GRAY,
				1 => Color.DARK_GRAY,
				2 => Color.DARK_GRAY,
				_ => Color.GREEN
			};
			context.DrawRectangle(new Rect(o.Pos.X, o.Pos.Y, o.WIDTH, o.HEIGHT), col);
		}

		foreach (var p in game.Players) {
			var col = p.PlayerUUID != controlled.PlayerUUID ? Color.RED : Color.BLUE;
			context.DrawCircle(new Circle(p.Pos, 20), col);
			var mp = p.Pos.Add(p.MoveDir.Scl(30));
			context.DrawLine(p.Pos, mp, new Color(232, 44, 207));
			var lp = p.Pos.Add(p.LookDir.Scl(30));
			context.DrawLine(p.Pos, lp, new Color(49, 245, 39));
		}

		double alpha = game.GetBulletTickAlpha();
		foreach (var b in game.Bullets) {
			if (b.Lifetime == -1)
				continue;
			var drawPos = b.Pos.Add(b.Dir.Scl(b.Speed * alpha));
			context.DrawRectangle(new Rect(drawPos, 10.0, 10.0), new Color(50, 50, 50));
		}
	}

}
