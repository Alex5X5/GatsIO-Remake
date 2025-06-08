namespace ShGame.Game.Logic;

using SimpleLogging.logging;

using System;
using System.Threading;
using System.Threading.Tasks;

public class GameInstance {

	private bool Idle = true;
	private bool Run = false;
	private long TargetClockCounter;

	private CancellationTokenSource InterruptSource;

	public ReaderWriterLockSlim PlayersAccessLock;
	public ReaderWriterLockSlim ObstaclesAccessLock;
	public ReaderWriterLockSlim BulletsAccessLock;

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
	private readonly TimeSpan LOOP_FRAGMENT_SLEEP_TIMESPAN = TimeSpan.FromTicks(LOOP_FRAGMENT_SLEEP_TICKS);

	private readonly Logger logger;

	public Player[] Players;
	public Obstacle[] Obstacles;
	public Bullet[] Bullets;

	public GameInstance(Client.Client? client) {
		logger = new(new LoggingLevel("Game"));
		Players = new Player[PLAYER_COUNT];
		for (int i = 0; i<PLAYER_COUNT; i++)
			Players[i]=new();
		Bullets = new Bullet[BULLET_COUNT];
		for (int i = 0; i<BULLET_COUNT; i++)
			Bullets[i]=new();
		Obstacles = new Obstacle[OBSTACLE_COUNT];
		for (int i = 0; i<OBSTACLE_COUNT; i++)
			Obstacles[i]=new(client, null, 0);
		InterruptSource = new CancellationTokenSource();
		PlayersAccessLock = new ReaderWriterLockSlim();
		ObstaclesAccessLock = new ReaderWriterLockSlim();
		BulletsAccessLock = new ReaderWriterLockSlim();
	}

	#region flow controll

	public void StartNewLoop(Action loop) {
		new Thread(
			() => {
				logger.Log("start loop");
				long nextExecution = DateTime.Now.Ticks + TARGET_LOOP_DELAY_TICKS;
				while (Run) {
					while (DateTime.Now.Ticks<nextExecution && Run)
						Thread.Sleep(LOOP_FRAGMENT_SLEEP_TIMESPAN);
					nextExecution = DateTime.Now.Ticks + TARGET_LOOP_DELAY_TICKS;
					loop();
				}
			}
			//,
			//InterruptSource.Token
		).Start();
	}

	//public void StartClock() {
	//	if (!Run) {
	//		Run = true;
	//		new Thread(ClockLoop).Start();
	//		//StartNewLoop(ClockLoop);
	//	}
	//}

	public void StartAllLoops() {
		//StartClock();
		Run=true;
		StartNewLoop(PlayerMoveLoop);
		StartNewLoop(PlayerShootLoop);
		StartNewLoop(BulletMoveLoop);
	}

	public void Stop() {
		Run = false;
		InterruptSource.Cancel();
		Task.Delay(1000);
		InterruptSource.Dispose();
		PlayersAccessLock.Dispose();
		ObstaclesAccessLock.Dispose();
		BulletsAccessLock.Dispose();
	}

	#endregion flow controll

	#region game loops

	public Action? PrePlayerMoveLoop;
	public Action? PastPlayerMoveLoop;

	public void PlayerMoveLoop() {
		PrePlayerMoveLoop?.Invoke();
		//for (int i = 0; i<200; i++) {
		foreach (Player p in Players) {
			if (p!=null)
				if (p.Health!=-1) {
					p.Pos.Add(p.Dir.Cpy().Scl(p.Speed));
					//logger.Log("moved player ", new MessageParameter("player ", p.ToString()));
				}
		}
		PastPlayerMoveLoop?.Invoke();
	}

	public Action? PreBulletMoveLoop;
	public Action? AfterBulletMoveLoop;

	public void BulletMoveLoop() {
		PreBulletMoveLoop?.Invoke();
		foreach (Bullet b in Bullets) {
			b.Move();
			b.CheckObstacleCollision(Obstacles);
		}
		AfterBulletMoveLoop?.Invoke();
	}

	public Action? PrePlayerShootLoop = ()=> { };
	public Action? PastPlayerShootLoop;

	public void PlayerShootLoop() {
		PrePlayerShootLoop?.Invoke();
		foreach (Player p in Players) {
			if (p.IsShooting == 0x1 && p.weaponCooldownTicksDone==0) {
				AllocBullet(p);
				p.weaponCooldownTicksDone = p.WeaponCooldownTicks;
			}
			if (p.weaponCooldownTicksDone>0)
				p.weaponCooldownTicksDone--;
		}
		PastPlayerShootLoop?.Invoke();
	}

	#endregion game loops

	#region obstacle generation

	public void SpreadObstacles() {
		logger.Log("generating Obstacles");
		int c = 0;
		//spreading obstacles over OBSTACKLE_ROWS rows
		for (int row = 0; row<OBSTACKLE_ROWS; row++)
			//spreading obstacles over OBSTACKLE_LINES lines so there are OBSTACKLE_ROWS*OBSTACKLE_LINES obstacles all together
			for (int line = 0; line<OBSTACKLE_LINES; line++) {
				PlaceObstacles(1 + row, 1 + line, c);
				//c is the position of the obstacle in the arary
				c++;
			}
	}

	public void PlaceObstacles(int row, int line, int offset) {
		//since there are OBSTACLE_ROWS rows the distance between the rows has to be MAP_WIDTH/OBSTACLE_ROWS
		row = MAP_GRID_WIDTH / OBSTACKLE_ROWS * row;
		//substract half of the distance between the rows so the obstakles get placed in the middle of each row
		row -= (int)(0.5 * MAP_GRID_WIDTH / OBSTACKLE_ROWS);
		//since there are OBSTACKLE_LINES lines the distance between the lines has to be MAP_HEIGHT/OBSTACKLE_LINES
		line = MAP_GRID_HEIGHT / OBSTACKLE_LINES * line;
		//substract half of the distance between the lines so the obstakles get placed in the middle of each line
		line -= (int)(0.5 * MAP_GRID_HEIGHT / (OBSTACKLE_LINES));
		Random r = new();
		Obstacles[offset] = new Obstacle(
			null,
			new Vector3d(
				//the obstacles may also be offset by half the distance to the next row/line
				//first add half of the distance between the rows to x
				//then substract a random number between 0 and OBSTACLE_ROW_DISANCE from it
				row + OBSTACLE_ROW_DISANCE / 2 - r.Next(0, OBSTACLE_ROW_DISANCE),
				//first add half of the distance between the lines to y
				//then substract a random number between 0 and OBSTACLE_LINE_DISTANCE from it
				line + OBSTACLE_LINE_DISTANCE /2 + r.Next(0, OBSTACLE_LINE_DISTANCE),
				0
			),
			//the upper bound of the type must be 4 becuase 3 ist the maxumum possible tytpe but the upper bound is not included
			(byte)r.Next(1, 4)
		);
		logger.Log("generated new Obstacle ", new MessageParameter("obstacle", Obstacles[offset]));
	}

	#endregion obstacle placement

	private void AllocBullet(Player p) {
		logger.Log("alloc bullet");
		for (int i = 0; i<BULLET_COUNT; i++) {
			logger.Log(Bullets[i].Speed.ToString());
			if (Bullets[i].Lifetime==-1) {
				Bullets[i].Pos.Set(p.Pos.Cpy().Add(new Vector3d(Player.SIZE/2, Player.SIZE/2, 0)));
				Bullets[i].Dir.Set(p.Dir);
				Bullets[i].Speed = p.InitialBulletSpeed;
				Bullets[i].OwnerUUID = p.PlayerUUID;
				break;
			}
		}
	}
}
