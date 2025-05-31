namespace ShGame.Game.Logic;

using ShGame.Game.Client;
using ShGame.Game.Logic.Math;
using ShGame.Game.Net;

using SimpleLogging.logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


class Game {

	private bool idle = true;
	private bool run = true;

	public const int PLAYER_COUNT = 20;
	public const int BULLET_COUNT = 200;

	public const int OBSTACKLE_ROWS = 5, OBSTACKLE_LINES = 8;
	public const int OBSTACLE_ROW_DISANCE = MAP_WIDTH / OBSTACKLE_ROWS;
	public const int OBSTACLE_COUNT = OBSTACKLE_ROWS*OBSTACKLE_LINES;
	public const int OBSTACLE_LINE_DISTANCE = MAP_HEIGHT / OBSTACKLE_LINES;

	public const int TARGET_TPS = 50;

	public const int MAP_WIDTH = 2100, MAP_HEIGHT = 1400;

	private readonly Logger logger;

	public readonly Player[] Players;
	public readonly Obstacle[] Obstacles;
	public readonly Bullet[] Bullets;

	public Game() {
		logger = new(new LoggingLevel("Game"));
		Players = new Player[PLAYER_COUNT];
		Bullets = new Bullet[BULLET_COUNT];
		Obstacles = new Obstacle[OBSTACLE_COUNT];
	}

	#region flow controll

	public void Stop() {
		run = false;
	}

	#endregion flow controll

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
		row = MAP_WIDTH / OBSTACKLE_ROWS * row;
		//substract half of the distance between the rows so the obstakles get placed in the middle of each row
		row -= (int)(0.5 * MAP_WIDTH / OBSTACKLE_ROWS);
		//since there are OBSTACKLE_LINES lines the distance between the lines has to be MAP_HEIGHT/OBSTACKLE_LINES
		line = MAP_HEIGHT / OBSTACKLE_LINES * line;
		//substract half of the distance between the lines so the obstakles get placed in the middle of each line
		line -= (int)(0.5 * MAP_HEIGHT / (OBSTACKLE_LINES));
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
				Bullets[i].Speed = p.default_shoot_speed;
				Bullets[i].OwnerUUID = p.PlayerUUID;
				break;
			}
		}
	}

	private void DeallocBullet() {
		
	}

	#region game loops

	public void PlayerMoveLoop() {
		while (run) {
			foreach (Player p in Players) {
				if (p!=null)
					if (p.Health!=-1)
						p.Move();
			}
			Thread.Sleep(1000/GameServer.TARGET_TPS);
		}
	}

	public void BulletMoveLoop() {
		while (run) {
			foreach (Bullet b in Bullets) {
				b.Move();
				b.CheckObstacleCollision(Obstacles);
			}
			Thread.Sleep(1000/GameServer.TARGET_TPS);
		}
	}

	public void PlayerShootLoop() {
		foreach (Player p in Players) {
			if (p.shooting == 0x1 && p.weaponCooldownTicksDone==0) {
				AllocBullet(p);
				p.weaponCooldownTicksDone = p.weaponCooldownTicks;
			}
			if (p.weaponCooldownTicksDone>0)
				p.weaponCooldownTicksDone--;
		}
	}

	#endregion game loops
}
