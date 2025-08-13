namespace ShGame.Game;

using ShGame.Game.GameObjects;
using ShGame.Math;
using ShGame.Util;

using SimpleLogging.logging;

using System;
using System.Reflection;
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

	private readonly Logger logger;

	public Player[] Players;
	public Obstacle[] Obstacles;
	public Bullet[] Bullets;

	public GameInstance(Player? pov) {
		logger = new(new LoggingLevel("Game"));
		Players = new Player[Constants.PLAYER_COUNT];
		for (int i = 0; i<Constants.PLAYER_COUNT; i++)
			Players[i]=new();
		if (pov!=null)
			Players[0]=pov;
		Bullets = new Bullet[Constants.BULLET_COUNT];
		for (int i = 0; i<Constants.BULLET_COUNT; i++)
			Bullets[i]=new();
		Obstacles = new Obstacle[Constants.OBSTACLE_COUNT];
		for (int i = 0; i<Constants.OBSTACLE_COUNT; i++)
			Obstacles[i]=new(pov, null, 0);
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
				long nextExecution = DateTime.Now.Ticks + Constants.TARGET_LOOP_DELAY_TICKS;
				while (Run) {
					while (DateTime.Now.Ticks<nextExecution && Run)
						Thread.Sleep(Constants.LOOP_FRAGMENT_SLEEP_TIMESPAN);
					nextExecution = DateTime.Now.Ticks + Constants.TARGET_LOOP_DELAY_TICKS;
					loop();
				}
			}
			//,
			//InterruptSource.Token
		).Start();
	}

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
		//Assembly ass = Assembly.GetEntryAssembly();
  //      if (ass.FullName.Contains(".Server")) 
		//	Console.Write("");
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
		for (int row = 0; row<Constants.OBSTACKLE_ROWS; row++)
			//spreading obstacles over OBSTACKLE_LINES lines so there are OBSTACKLE_ROWS*OBSTACKLE_LINES obstacles all together
			for (int line = 0; line<Constants.OBSTACKLE_LINES; line++) {
				PlaceObstacles(1 + row, 1 + line, c);
				//c is the position of the obstacle in the arary
				c++;
			}
	}

	public void PlaceObstacles(int row, int line, int offset) {
		//since there are OBSTACLE_ROWS rows the distance between the rows has to be MAP_WIDTH/OBSTACLE_ROWS
		row = Constants.MAP_GRID_WIDTH / Constants.OBSTACKLE_ROWS * row;
		//substract half of the distance between the rows so the obstakles get placed in the middle of each row
		row -= (int)(0.5 * Constants.MAP_GRID_WIDTH / Constants.OBSTACKLE_ROWS);
		//since there are OBSTACKLE_LINES lines the distance between the lines has to be MAP_HEIGHT/OBSTACKLE_LINES
		line = Constants.MAP_GRID_HEIGHT / Constants.OBSTACKLE_LINES * line;
		//substract half of the distance between the lines so the obstakles get placed in the middle of each line
		line -= (int)(0.5 * Constants.MAP_GRID_HEIGHT / Constants.OBSTACKLE_LINES);
		Random r = new();
		Obstacles[offset] = new Obstacle(
			null,
			new Vector3d(
				//the obstacles may also be offset by half the distance to the next row/line
				//first add half of the distance between the rows to x
				//then substract a random number between 0 and OBSTACLE_ROW_DISANCE from it
				row + Constants.OBSTACLE_ROW_DISANCE / 2 - r.Next(0, Constants.OBSTACLE_ROW_DISANCE),
				//first add half of the distance between the lines to y
				//then substract a random number between 0 and OBSTACLE_LINE_DISTANCE from it
				line + Constants.OBSTACLE_LINE_DISTANCE /2 + r.Next(0, Constants.OBSTACLE_LINE_DISTANCE),
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
		for (int i = 0; i<Constants.BULLET_COUNT; i++) {
			logger.Log(Bullets[i].Speed.ToString());
			if (Bullets[i].Lifetime==-1) {
				Bullets[i].Pos.Set(p.Pos.Cpy().Add(new Vector3d(Player.SIZE/2, Player.SIZE/2, 0)));
				Bullets[i].Dir.Set(p.Dir);
				Bullets[i].Speed = p.InitialBulletSpeed;
				Bullets[i].OwnerHandle = p.PlayerUUID;
				break;
			}
		}
	}
}
