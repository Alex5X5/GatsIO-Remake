namespace ShGame.Game.Client;
using ShGame.Game.Client.Rendering;
using ShGame.Game.Net;

using System;

public class Bullet : Drawable {

	public const int BULLET_BYTE_LENGTH = 20;

	public Vector3d Pos;
	public Vector3d Dir;
	private byte WIDTH, LENGHT;
	public short Speed;

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
			Pos.Set(10, 10, 0);
			Dir.Set(0, 1, 0);
			//SCREEN_PIXEL_WIDTH=0;
			//LENGHT=0;
			Speed=0;
			Console.WriteLine("dealloc bullet");
			Console.WriteLine(this);
		}
		dirty = true;
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
				Pos.Set(10, 10, 0);
				Dir.Set(0, 1, 0);
				//WIDTH=0;
				//LENGHT=0;
				Speed = 0;
				Console.WriteLine("dealloc bullet");
				Console.WriteLine(this);
			}
		}
	}

	/// <summary>
	/// updates the bound objects of an obstacle to match its width and height.
	/// </summary>
	public static unsafe void SerializeBullet(byte[]* input, Bullet* bullet, int offset) {
		int offset_ = offset;
		fixed (byte* ptr = *input)
			if (bullet==null) {
				*(ptr+offset_) = 0;
			} else {
				offset_ += 4;
				BitConverter.GetBytes((int)bullet->Pos.x).CopyTo(*input, offset_);
				offset_ += 4;
				BitConverter.GetBytes((int)bullet->Pos.y).CopyTo(*input, offset_);
			}
	}

	/// <summary>
	/// reads the next 17 bytes after the offset from a bytearray. 
	/// </summary>
	public static unsafe void DeserializeBullet(Client client, byte* input, Bullet* bullet) {
		//ArgumentNullException.ThrowIfNull(*input);
		byte* _input = input;
		int offset = *_input;
		_input++;
		client.bullets[offset].Pos.x = *_input;
		_input+=4;
		client.bullets[offset].Pos.x = *_input;
		_input+=4;
		client.bullets[offset].Dir.x = *_input;
		_input+=4;
		client.bullets[offset].Dir.x = *_input;
		_input+=4;
		client.bullets[offset].WIDTH = *_input;
		_input++;
		client.bullets[offset].LENGHT = *_input;
	}

	public override string ToString() => "ShGame.Game.Client.Bullet[Pos:"+Pos.ToString()+", Dir:"+Dir.ToString()+"]";
}