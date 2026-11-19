namespace ShGame.Types;

using ShGame.Math;

using System;

public class Player {

	public short WeaponCooldownTicks = 20;
	public short weaponCooldownTicksDone = 0;

	public byte IsShooting = 0x0;
	public short InitialBulletSpeed = 0x5;

	public Vector3d Pos;
	public Vector3d MoveDir = new(0, 0, 0);
	public Vector3d LookDir = new(0, 0, 0);

	public Vector3d CenterPos => Pos.Add(RADIUS*0.5, RADIUS*0.5, 0);

	public double Speed = 2;

	public const int MAX_HEALTH = 100;

	private int Health_;
	public int Health {
		get => Health_;
		set => Health_ = value > MAX_HEALTH ? MAX_HEALTH : value < -1 ? -1 : value;
	}

	public const double RADIUS = 20.0;
	public short PlayerUUID = 0;
	public bool Visible;

	public const int SizeInBytes = 100;
	public const int SizeInBytesForNetwork = 65;

	//the constructor for invalid players
	public Player() : this(new(0, 0, 0), -1, 0) {
		Visible = false;
	}

	public Player(Vector3d pos, int health, short UUID) {
		Pos = pos;
		Health = health;
		PlayerUUID = UUID;
		Visible = Health !=-1;
		IsShooting = 0x0;
	}

	public override string ToString() =>
		$"Game.graphics.client.Player[health:{Health}, speed:{Speed}, pos:{Pos}, dir:{MoveDir}, UUID:{PlayerUUID}, IsShooting:{IsShooting}]";

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
