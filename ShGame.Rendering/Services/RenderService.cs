namespace ShGame.Rendering.Services;

using ShGame.Drawing;
using ShGame.Game;
using ShGame.Math;
using ShGame.Types;
using ShGame.Util;

public class RenderService {

	private GameService game;
	private ShadowService shadowService;

	private Player controlled;

	private Image fogImage;

	public RenderService(GameService game, ShadowService shadowService, Player controlled) {
		this.game = game;
		this.shadowService = shadowService;
		this.controlled = controlled;
	}

	public void UpdateControlledPlayer(Player controlled) {
		this.controlled = controlled;
	}

	public void OnLoad(LoadingContext context) {
		fogImage = context.LoadImage(Paths.AssetsPath("shadow.png"));
	}

	public void OnDraw(DrawingContext context) {

		foreach (var p in game.Players) {
			if (p.Health == -1)
				continue;
			var col = p.PlayerUUID == controlled.PlayerUUID ? Color.BLUE : Color.RED;
			context.DrawCircle(new Circle(p.Pos, 20), col);
			if(Constants.DEBUG_DRAW) {
				var mp = p.Pos.Add(p.MoveDir.Scl(30));
				context.DrawLine(p.Pos, mp, new Color(232, 44, 207));
				var lp = p.Pos.Add(p.LookDir.Scl(30));
				context.DrawLine(p.Pos, lp, new Color(49, 245, 39));
			}
		}

		double alpha = game.GetBulletTickAlpha();
		foreach (var b in game.Bullets) {
			if (b.Lifetime == -1)
				continue;
			var drawPos = b.Pos.Add(b.Dir.Scl(b.Speed * alpha));
			context.DrawRectangle(new Rect(drawPos, 10.0, 10.0), new Color(50, 50, 50));
		}

		shadowService.UpdateShadows(game.GetPlayerById(controlled.PlayerUUID));

		foreach (var s in shadowService.Shadows) {
			context.DrawImageQuad(s.Shape, fogImage);
		}

		foreach (var o in game.Map.Obstacles) {
			var col = o.type switch {
				0 => Color.DARK_GRAY,
				1 => Color.DARK_GRAY,
				2 => Color.DARK_GRAY,
				_ => Color.GREEN
			};
			context.DrawRectangle(new Rect(o.Pos.X, o.Pos.Y, o.WIDTH, o.HEIGHT), col);
		}
	}

}
