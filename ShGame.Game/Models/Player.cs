namespace ShGame.Game.Models;

using ShGame.Drawing.Models;
using ShGame.Game;
using ShGame.Math;

using System.Runtime.CompilerServices;


//#pragma warning disable CS8500 //a pointer is created to a variable of an unmanaged type

public class Player {

	public const int PLAYER_BYTE_LENGTH = 56;

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
		set => Health_ = value>100 ? 100 : value<-1 ? -1 : value;
	}

	public const int Radius = 10;
	public short PlayerUUID = 0;
	public bool Visible;

    public Player() : this(new(0, 0, 0), -1, 0) {
	}

    public Player(Vector3d? newPos, int newHealth, short UUID) {
		Pos = newPos??new Vector3d(0, 0, 0);
		Health_ = newHealth;
		PlayerUUID = UUID;
		Visible=Health_ !=-1;
	}

	public override string ToString() =>
		$"Game.graphics.client.Player[health:{Health}, speed:{Speed}, pos:{Pos}, dir:{Dir}, UUID:{PlayerUUID}]";

	public unsafe void Move() {
		Pos.Add(Dir.Cpy().Nor().Scl(Speed));
	}

	public void Deactivate() {
		Visible = false;
		Pos = new(0, 0, 0);
		Dir.x = 0;
		Dir.y = 0;
		Dir.z = 0;
		Health_ = -1;
	}

	public void OnKeyEvent(IKeySupplier c) {
		Console.WriteLine(Pos.ToString());
		if (c.keyUp) {
			if (c.keyLeft) {
				if (c.keyDown) {
					if (c.keyRight) {
						Dir.x=0; //wasd
						Dir.y=0;
					} else {
						Dir.x=-1; //was
						Dir.y=0;
					}
				} else {
					if (c.keyRight) {
						Dir.x=0; //wad
						Dir.y=1;
					} else {
						Dir.x=-1/System.Math.Sqrt(2); //wa
						Dir.y=1/System.Math.Sqrt(2);
					}
				}
			} else {
				if (c.keyDown) {
					if (c.keyRight) {
						Dir.x=1; //wsd
						Dir.y=0;
					} else {
						Dir.x=0; //ws
						Dir.y=0;
					}
				} else {
					if (c.keyRight) {
						Dir.x=1/System.Math.Sqrt(2); //wd
						Dir.y=1/System.Math.Sqrt(2);
					} else {
						Dir.x=0; //w
						Dir.y=1;
					}
				}
			}
		} else {
			if (c.keyLeft) {
				if (c.keyDown) {
					if (c.keyRight) {
						Dir.x=0; //asd
						Dir.y=-1;
					} else {
						Dir.x=-1/System.Math.Sqrt(2); //as
						Dir.y=-1/System.Math.Sqrt(2);
					}
				} else {
					if (c.keyRight) {
						Dir.x=0; //ad
						Dir.y=0;
					} else {
						Dir.x=-1; //a
						Dir.y=0;
					}
				}
			} else {
				if (c.keyDown) {
					if (c.keyRight) {
						Dir.x=1/System.Math.Sqrt(2); //sd
						Dir.y=-1/System.Math.Sqrt(2);
					} else {
						Dir.x=0; //s
						Dir.y=-1;
					}
				} else {
					if (c.keyRight) {
						Dir.x=1; //d
						Dir.y=0;
					} else {
						Dir.x=0; //
						Dir.y=0;
					}
				}
			}
		}
		Console.WriteLine(Dir.ToString());
		Console.WriteLine(Pos.ToString());
    }

	public static unsafe void SerializePlayer(byte* buffer, Player player, int offset) {
		byte* ptr = buffer;
		ptr+=offset;
		if (player==null) {
			Unsafe.Write(ptr, -1);
		} else {
			Unsafe.Write(ptr, player.Health);
			ptr += 4;
			Unsafe.Write(ptr, player.Pos.x);
			ptr += 8;
			Unsafe.Write(ptr, player.Pos.y);
			ptr += 8;
			Unsafe.Write(ptr, player.Dir.x);
			ptr += 8;
			Unsafe.Write(ptr, player.Dir.y);
			ptr += 8;
			Unsafe.Write(ptr, (int)player.Speed);
			ptr += 4;
			Unsafe.Write(ptr, player.PlayerUUID);
			ptr += 2;
			Unsafe.Write(ptr, player.WeaponCooldownTicks);
			ptr += 2;
			Unsafe.Write(ptr, player.WeaponCooldownTicks);
			ptr += 2;
			Unsafe.Write(ptr, player.InitialBulletSpeed);
			ptr += 2;
			*ptr = player.IsShooting;
		}
	}

	public static unsafe void DeserializePlayer(byte* buffer, Player player, int offset) {
		byte* ptr = buffer;
		ptr+=offset;
		player ??= new Player(null, 0, 0);
		player.Health = Unsafe.Read<int>(ptr);
		if (player.Health_ == -1) {
			player.Deactivate();
		} else {
			ptr += 4;
			player.Pos.x = Unsafe.Read<double>(ptr);
			ptr += 8;
			player.Pos.y = Unsafe.Read<double>(ptr);
			ptr += 8;
			player.Dir.x = Unsafe.Read<double>(ptr);
			ptr += 8;
			player.Dir.y = Unsafe.Read<double>(ptr);
			ptr += 8;
			player.Speed = Unsafe.Read<int>(ptr);
			ptr += 4;
			player.PlayerUUID = Unsafe.Read<short>(ptr);
			ptr += 2;
			player.WeaponCooldownTicks = Unsafe.Read<short>(ptr);
			ptr += 2;
			player.weaponCooldownTicksDone = Unsafe.Read<short>(ptr);
			ptr += 2;
			player.IsShooting = *ptr;
		}
	}

	public static unsafe short DeserializePlayerId(byte* buffer, int offset) {
		byte* ptr = buffer;
		ptr+=offset;
		ptr+=40;
		//new Logger(new LoggingLevel("Player")).Log("read player uuid ",new MessageParameter("id", Unsafe.Read<Int16>(ptr)));
		return Unsafe.Read<short>(ptr);
	}
}
