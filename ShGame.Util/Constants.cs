using ShGame.Math;

namespace ShGame;

public class Constants {


	public static readonly Line3d BORDER_TOP = Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(Constants.MAP_GRID_WIDTH, 0, 0));
	public static readonly Line3d BORDER_BOTTOM = Line3d.FromPoints(new Vector3d(0, Constants.MAP_GRID_HEIGHT, 0), new Vector3d(Constants.MAP_GRID_WIDTH, Constants.MAP_GRID_HEIGHT, 0));
	public static readonly Line3d BORDER_LEFT = Line3d.FromPoints(new Vector3d(0, 0, 0), new Vector3d(0, Constants.MAP_GRID_HEIGHT, 0));
	public static readonly Line3d BORDER_RIGHT = Line3d.FromPoints(new Vector3d(Constants.MAP_GRID_WIDTH, 0, 0), new Vector3d(Constants.MAP_GRID_WIDTH, Constants.MAP_GRID_HEIGHT, 0));

	public const int PLAYER_COUNT = 20;
	public const int BULLET_COUNT = 200;

	public const int OBSTACKLE_ROWS = 5, OBSTACKLE_LINES = 8;
	public const int OBSTACLE_ROW_DISANCE = MAP_GRID_WIDTH / OBSTACKLE_ROWS;
	public const int OBSTACLE_COUNT = OBSTACKLE_ROWS*OBSTACKLE_LINES;
	public const int OBSTACLE_LINE_DISTANCE = MAP_GRID_HEIGHT / OBSTACKLE_LINES;

	public const int MAP_GRID_WIDTH = 2100, MAP_GRID_HEIGHT = 1400;

	public const int TARGET_TPS = 110;
	public const long TARGET_LOOP_DELAY_TICKS = TimeSpan.TicksPerSecond/TARGET_TPS;
	public const long LOOP_FRAGMENT_SLEEP_TICKS = TARGET_LOOP_DELAY_TICKS/20;
	public static readonly TimeSpan LOOP_FRAGMENT_SLEEP_TIMESPAN = TimeSpan.FromTicks(LOOP_FRAGMENT_SLEEP_TICKS);

}
