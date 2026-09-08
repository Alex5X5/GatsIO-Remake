namespace ShGame.Net.Shared.Services;

using ShGame.Math;
using ShGame.Types;

using System.Runtime.CompilerServices;

public static class SerializerService {

	private static unsafe byte* PtrWithOffset(byte* buffer, int offset) {
		byte* ptr = buffer;
		ptr+=offset;
		return ptr;
	}

	public static unsafe void SerializePlayer(byte* buffer, Player player, int offset = 0) {
		byte* ptr = PtrWithOffset(buffer, offset);
		Unsafe.Write(ptr, player.Health);
		ptr += 4;
		Unsafe.Write(ptr, player.Pos.X);
		ptr += 8;
		Unsafe.Write(ptr, player.Pos.Y);
		ptr += 8;
		Unsafe.Write(ptr, player.Dir.X);
		ptr += 8;
		Unsafe.Write(ptr, player.Dir.Y);
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

	public static unsafe Player DeserializePlayer(byte* buffer, int offset = 0) {
		byte* ptr = PtrWithOffset(buffer, offset);
		int health = Unsafe.Read<int>(ptr);
		ptr += 4;
		double px = Unsafe.Read<double>(ptr);
		ptr += 8;
		double py = Unsafe.Read<double>(ptr);
		ptr += 8;
		double dx = Unsafe.Read<double>(ptr);
		ptr += 8;
		double dy = Unsafe.Read<double>(ptr);
		ptr += 8;
		int speed = Unsafe.Read<int>(ptr);
		ptr += 4;
		short uuid = Unsafe.Read<short>(ptr);
		ptr += 2;
		short cooldown = Unsafe.Read<short>(ptr);
		ptr += 2;
		short cooldownDone = Unsafe.Read<short>(ptr);
		ptr += 2;
		byte shooting = *ptr;
		return new Player() {
			Health = health,
			Pos = new Vector3d(px, py, 0.0),
			Dir = new Vector3d(dx, dy, 0.0),
			Speed = speed,
			PlayerUUID = uuid,
			WeaponCooldownTicks = cooldown,
			weaponCooldownTicksDone = cooldownDone,
			IsShooting = shooting
		};
	}

	public static unsafe short DeserializePlayerId(byte* buffer, int offset) {
		byte* ptr = PtrWithOffset(buffer, offset);
		ptr+=40;
		return Unsafe.Read<short>(ptr);
	}

	/// <summary>
	/// Takes the information from an obstackle and
	/// writes it as 17 bytes to a buffer.
	/// byte 1 is the type of the obstackle,
	/// byte 2 to 8 is the x position of the obstacle
	/// byte 9 to 17 is the y position of the obstacle
	/// </summary>
	public static unsafe void SerializeObstacle(Obstacle obstacle, byte* buffer, int offset) {
		byte* ptr = PtrWithOffset(buffer, offset);
		Unsafe.Write(ptr, obstacle.type);
		ptr += 1;
		Unsafe.Write(ptr, obstacle.Pos.X);
		ptr += 8;
		Unsafe.Write(ptr, obstacle.Pos.Y);
	}

	/// <summary>
	/// reads the next 17 bytes after the offset from a buffer
	/// and constructs an obstacle object from it.
	/// byte 1 is the type of the obstackle,
	/// byte 2 to 8 is the x position of the obstacle
	/// byte 9 to 17 is the y position of the obstacle
	/// </summary>
	public static unsafe Obstacle DeserializeObstacle(byte* buffer, int offset) {
		byte* ptr = PtrWithOffset(buffer, offset);
		byte type = *ptr;
		ptr += 1;
		double x = Unsafe.Read<double>(ptr);
		ptr += 8;
		double y = Unsafe.Read<double>(ptr);
		return new Obstacle(new Vector3d(x, y, 0.0), type);
	}

	public static unsafe void SerializeBullet(Bullet bullet, byte* buffer, int offset) {
		byte* ptr = PtrWithOffset(buffer, offset);
		Unsafe.Write(ptr, bullet.Lifetime);
		ptr += 2;
		Unsafe.Write(ptr, bullet.Pos.X);
		ptr += 8;
		Unsafe.Write(ptr, bullet.Pos.Y);
		ptr += 8;
		Unsafe.Write(ptr, bullet.Dir.X);
		ptr += 8;
		Unsafe.Write(ptr, bullet.Dir.Y);
		ptr += 8;
		Unsafe.Write(ptr, bullet.Speed);
		ptr += 2;
		Unsafe.Write(ptr, bullet.OwnerHandle);
	}

	/// <summary>
	/// </summary>
	public static unsafe Bullet DeserializeBullet(byte* buffer, int offset) {
		byte* ptr = PtrWithOffset(buffer, offset);
		short lifetime = Unsafe.Read<short>(ptr);
		ptr += 2;
		double px = Unsafe.Read<double>(ptr);
		ptr += 8;
		double py = Unsafe.Read<double>(ptr);
		ptr += 8;
		double dx = Unsafe.Read<double>(ptr);
		ptr += 8;
		double dy = Unsafe.Read<double>(ptr);
		ptr += 8;
		double speed = Unsafe.Read<short>(ptr);
		ptr += 2;
		short owner = Unsafe.Read<short>(ptr);
		var pos = new Vector3d(px, py, 0.0);
		var dir = new Vector3d(dx, dy, 0.0);
		return new Bullet(pos, dir, 5, 10);
	}

}
