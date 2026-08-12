namespace ShGame.Types;

using ShGame.Math;

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
	public const int SizeInBytesForNetwork = 17;

	public Player(Vector3d? newPos, int newHealth, short UUID) {
		Pos = newPos??new Vector3d(0, 0, 0);
		Health_ = newHealth;
		PlayerUUID = UUID; //!=0 ? UUID : new Random().Next();
		Visible=Health_ !=-1;
	}

	//the constructor for invalid players
	public Player() {
		Pos = new(0, 0, 0);
		//if the health of a player is -1 it is considered invalid and won't be processed
		Health_ = -1;
		PlayerUUID = 0;
		Visible = false;
	}

	public override string ToString() =>
		$"Game.graphics.client.Player[health:{Health}, speed:{Speed}, pos:{Pos}, dir:{Dir}, UUID:{PlayerUUID}]";

	public void Move() {
		Pos = Pos.Add(Dir.Nor().Scl(Speed));
	}
}
