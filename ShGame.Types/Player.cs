namespace ShGame.Types;

using ShGame.Math;

using System;

public class Player {

	public short WeaponCooldownTicks = 10;
	public short weaponCooldownTicksDone = 10;

	public byte IsShooting = 0x0;
	public short InitialBulletSpeed = 0x30;

	public Vector3d Pos;
	public Vector3d MoveDir = new(0, 0, 0);
	public Vector3d LookDir = new(0, 0, 0);

	public double Speed = 2;
	private int Health_;
	public int Health {
		get => Health_;
		set => Health_ = value > 100 ? 100 : value < -1 ? -1 : value;
	}

	public const int Radius = 10;
	public short PlayerUUID = 0;
	public bool Visible;

	public const int SizeInBytes = 100;
	public const int SizeInBytesForNetwork = 63;

	public Player(Vector3d pos, int health, short UUID) {
		Pos = pos;
		Health = health;
		PlayerUUID = UUID; //!=0 ? UUID : new Random().Next();
		Visible = Health !=-1;
	}

	//the constructor for invalid players
	public Player() {
		Pos = new(0, 0, 0);
		//if the health of a player is -1 it is considered invalid and won't be processed
		Health = -1;
		PlayerUUID = 0;
		Visible = false;
	}

	public override string ToString() =>
		$"Game.graphics.client.Player[health:{Health}, speed:{Speed}, pos:{Pos}, dir:{MoveDir}, UUID:{PlayerUUID}]";

	public void Move() {
		Pos = Pos.Add(MoveDir.Nor().Scl(Speed));
	}

	public void UpdateDir(bool up, bool down, bool left, bool right) {
		//Console.WriteLine(Pos.ToString());
		if (up) {
			if (left) {
				if (down) {
					if (right) {
						MoveDir.X=0; //wasd
						MoveDir.Y=0;
					} else {
						MoveDir.X=-1; //was
						MoveDir.Y=0;
					}
				} else {
					if (right) {
						MoveDir.X=0; //wad
						MoveDir.Y=-1;
					} else {
						MoveDir.X=-1/System.Math.Sqrt(2); //wa
						MoveDir.Y=-1/System.Math.Sqrt(2);
					}
				}
			} else {
				if (down) {
					if (right) {
						MoveDir.X=1; //wsd
						MoveDir.Y=0;
					} else {
						MoveDir.X=0; //ws
						MoveDir.Y=0;
					}
				} else {
					if (right) {
						MoveDir.X=1/System.Math.Sqrt(2); //wd
						MoveDir.Y=-1/System.Math.Sqrt(2);
					} else {
						MoveDir.X=0; //w
						MoveDir.Y=-1;
					}
				}
			}
		} else {
			if (left) {
				if (down) {
					if (right) {
						MoveDir.X=0; //asd
						MoveDir.Y=1;
					} else {
						MoveDir.X=-1/System.Math.Sqrt(2); //as
						MoveDir.Y=1/System.Math.Sqrt(2);
					}
				} else {
					if (right) {
						MoveDir.X=0; //ad
						MoveDir.Y=0;
					} else {
						MoveDir.X=-1; //a
						MoveDir.Y=0;
					}
				}
			} else {
				if (down) {
					if (right) {
						MoveDir.X=1/System.Math.Sqrt(2); //sd
						MoveDir.Y=1/System.Math.Sqrt(2);
					} else {
						MoveDir.X=0; //s
						MoveDir.Y=1;
					}
				} else {
					if (right) {
						MoveDir.X=1; //d
						MoveDir.Y=0;
					} else {
						MoveDir.X=0; //
						MoveDir.Y=0;
					}
				}
			}
		}
		Console.WriteLine(MoveDir.ToString());
		Console.WriteLine(Pos.ToString());
	}
}
