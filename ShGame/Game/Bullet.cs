namespace ShGame.Game;
using ShGame.Game.Client;
using ShGame.Game.Client.Rendering;
using ShGame.Game.Logic.Math;
using ShGame.Game.Net;

using System;
using System.Runtime.CompilerServices;

public class Bullet : Drawable {

	public const int BULLET_BYTE_LENGTH = 52;

	public Vector3d Pos;
	public Vector3d Dir;
	private byte WIDTH, LENGHT;
	public short Speed;
	public short Lifetime;
	public short OwnerHandle;

	public Bullet() : this(null, null, 0, 0) { }

	public Bullet(Vector3d? _pos, Vector3d? _dir, int _width, int _length) : base(18) {
		Pos = _pos??new Vector3d(10, 10, 0);
		Dir = _dir??new Vector3d(0, 1, 0);
		WIDTH = (byte)(_width>0 ? _width : 5);
		LENGHT = (byte)(_length>0 ? _length : 5);
	}

	public override void Dispose() {
		GC.SuppressFinalize(this);
		base.Dispose();
	}

	public unsafe override void UpdateVertices() {
		float* ptr = VertexDataPtr;
		*ptr=(float)Pos.x;
		ptr++;
		*ptr=(float)Pos.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)Pos.x+WIDTH;
		ptr++;
		*ptr=(float)Pos.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)Pos.x+WIDTH;
		ptr++;
		*ptr=(float)Pos.y+LENGHT;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)Pos.x;
		ptr++;
		*ptr=(float)Pos.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)Pos.x;
		ptr++;
		*ptr=(float)Pos.y+LENGHT;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)Pos.x+WIDTH;
		ptr++;
		*ptr=(float)Pos.y+LENGHT;
	}

	public void Move() {
		Pos.Add(
			Dir.Cpy().Scl(Speed)
		);
		if (Pos.x<0|Pos.y<0|Pos.x>GameServer.MAP_WIDTH|Pos.y>GameServer.MAP_HEIGHT) {
			Dealloc();
		}
		dirty = true;
	}

	public void Dealloc() {
		Pos.Set(10, 10, 0);
		Dir.Set(0, 1, 0);
		Speed = 0;
		Lifetime =- 1;
		dirty = false;
		Console.WriteLine("dealloc bullet");
		Console.WriteLine(this);
	}

	public void CheckObstacleCollision(Obstacle[] obstacles) {
		foreach (Obstacle obstacle in obstacles) {
			Vector3d bulletCorner1 = Pos;
			Vector3d bulletCorner2 = Pos.Cpy().Add(WIDTH, LENGHT, 0);

			Vector3d obstacleCorner1 = obstacle.Pos;
			Vector3d obstacleCorner2 = obstacle.Pos.Cpy().Add(obstacle.WIDTH, obstacle.HEIGHT, 0);

			if (
				bulletCorner1.x <= obstacleCorner2.x && bulletCorner1.y <= obstacleCorner2.y &&
				bulletCorner2.x >= obstacleCorner1.x && bulletCorner2.y >= obstacleCorner1.y
			) {
				Dealloc();
			}
		}
	}

	public static unsafe void SerializeBullet(byte* buffer, Bullet bullet, int offset) {
		byte* ptr = buffer;
		ptr+=offset;
		if (bullet==null) {
			Unsafe.Write<short>(ptr, -1);
		} else {
			Unsafe.Write(ptr, bullet.Lifetime);
			ptr += 2;
			Unsafe.Write(ptr, (int)bullet.Pos.x);
			ptr += 8;
			Unsafe.Write(ptr, (int)bullet.Pos.y);
			ptr += 8;
			Unsafe.Write(ptr, (int)bullet.Dir.x);
			ptr += 8;
			Unsafe.Write(ptr, (int)bullet.Dir.y);
			ptr += 8;
			Unsafe.Write(ptr, bullet.Speed);
			ptr += 2;
			Unsafe.Write(ptr, bullet.OwnerHandle);
		}
	}

	/// <summary>
	/// reads the next 17 bytes after the offset from a buffer and modifies a player 
	/// dependent on the values of te buffer.
	/// byte 1 to 4 are the health of the player,
	/// byte 5 to 13 are converted to an int and are set as the new x position of the player
	/// byte 6 to 9 are converted to an int and are set as the new y position of the player
	/// byte 10 to 13 are converted to an int and are set as the new width of the player
	/// byte 10 to 13 are converted to an int and are set as the new height of the player
	/// byte 10 to 13 are converted to an int and are set as the new height of the player
	/// </summary>
	public static unsafe void DeserializeBullet(byte* buffer, Bullet bullet, int offset) {
		byte* ptr = buffer;
		ptr+=offset;
		bullet ??= new Bullet(null, null, 0, 0);
		bullet.Lifetime = Unsafe.Read<short>(ptr);
		if (bullet.Lifetime ==-1) {
			bullet.Dealloc();
		} else {
			ptr += 2;
			bullet.Pos.x = Unsafe.Read<int>(ptr);
			ptr += 4;
			bullet.Pos.y = Unsafe.Read<int>(ptr);
			ptr += 4;
			bullet.Dir.x = Unsafe.Read<int>(ptr);
			ptr += 4;
			bullet.Dir.y = Unsafe.Read<int>(ptr);
			ptr += 4;
			bullet.Speed = Unsafe.Read<short>(ptr);
			ptr += 2;
			bullet.OwnerHandle = Unsafe.Read<short>(ptr);
			bullet.dirty=true;
		}
	}

	public override string ToString() => "ShGame.Game.Client.Bullet[Pos:"+Pos.ToString()+", Dir:"+Dir.ToString()+"]";
}