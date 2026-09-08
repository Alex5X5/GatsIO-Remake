namespace ShGame.Types;

using ShGame.Util;

public class Map {

	public Obstacle[] Obstacles;

	public Map() {
		Obstacles = new Obstacle[Constants.OBSTACLE_COUNT];
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++) {
			Obstacles[i] = new();
		}
	}
}
