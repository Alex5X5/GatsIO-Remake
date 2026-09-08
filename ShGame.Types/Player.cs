namespace ShGame.Types;

using ShGame.Math;

using System;

public class Player {

	public short WeaponCooldownTicks = 10;
	public short weaponCooldownTicksDone = 10;

	public byte IsShooting = 0x0;
	public short InitialBulletSpeed = 0x30;

	public Vector3d Pos;
	public Vector3d Dir = new(0, 0, 0);

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
	public const int SizeInBytesForNetwork = 47;

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
		$"Game.graphics.client.Player[health:{Health}, speed:{Speed}, pos:{Pos}, dir:{Dir}, UUID:{PlayerUUID}]";

	public void Move() {
		Pos = Pos.Add(Dir.Nor().Scl(Speed));
	}

	public void UpdateDir(bool up, bool down, bool left, bool right) {
		//Console.WriteLine(Pos.ToString());
		if (up) {
			if (left) {
				if (down) {
					if (right) {
						Dir.X=0; //wasd
						Dir.Y=0;
					} else {
						Dir.X=-1; //was
						Dir.Y=0;
					}
				} else {
					if (right) {
						Dir.X=0; //wad
						Dir.Y=-1;
					} else {
						Dir.X=-1/System.Math.Sqrt(2); //wa
						Dir.Y=-1/System.Math.Sqrt(2);
					}
				}
			} else {
				if (down) {
					if (right) {
						Dir.X=1; //wsd
						Dir.Y=0;
					} else {
						Dir.X=0; //ws
						Dir.Y=0;
					}
				} else {
					if (right) {
						Dir.X=1/System.Math.Sqrt(2); //wd
						Dir.Y=-1/System.Math.Sqrt(2);
					} else {
						Dir.X=0; //w
						Dir.Y=-1;
					}
				}
			}
		} else {
			if (left) {
				if (down) {
					if (right) {
						Dir.X=0; //asd
						Dir.Y=1;
					} else {
						Dir.X=-1/System.Math.Sqrt(2); //as
						Dir.Y=1/System.Math.Sqrt(2);
					}
				} else {
					if (right) {
						Dir.X=0; //ad
						Dir.Y=0;
					} else {
						Dir.X=-1; //a
						Dir.Y=0;
					}
				}
			} else {
				if (down) {
					if (right) {
						Dir.X=1/System.Math.Sqrt(2); //sd
						Dir.Y=1/System.Math.Sqrt(2);
					} else {
						Dir.X=0; //s
						Dir.Y=1;
					}
				} else {
					if (right) {
						Dir.X=1; //d
						Dir.Y=0;
					} else {
						Dir.X=0; //
						Dir.Y=0;
					}
				}
			}
		}
		Console.WriteLine(Dir.ToString());
		Console.WriteLine(Pos.ToString());
	}
}
