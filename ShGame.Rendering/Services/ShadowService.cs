using ShGame.Game;
using ShGame.Types;
using ShGame.Util;

namespace ShGame.Rendering.Services;

public class ShadowService {

	private GameService game;

	public Shadow[] Shadows;

	public ShadowService(GameService game) {
		this.game = game;

		Shadows = new Shadow[Constants.OBSTACLE_COUNT];
		for (int i=0; i<Constants.OBSTACLE_COUNT; i++) {
			Shadows[i] = new(game.Map.Obstacles[i]);
		}
	}

	public void UpdateShadows(Player controlled) {
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
			Shadows[i].Update(controlled);
		}
	}
}
