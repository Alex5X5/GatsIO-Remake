namespace ShGame.Game.Client;
using ShGame.Game.Client.Rendering;
using ShGame.Game.Net;

using System;

public class Bullet:Drawable {

	public const int BULLET_BYTE_LENGTH = 20;

	public Vector3d Pos;
	public Vector3d Dir;
	private byte WIDTH, LENGHT;
	public short Speed;

	public Bullet(Vector3d? pos_, Vector3d? dir_, int width_, int length_) :base(18) {
		Pos = pos_??new Vector3d(10, 10, 0);
		Dir = dir_??new Vector3d(0, 1, 0);
		WIDTH = (byte)(width_>0 ? width_ : 5);
		LENGHT = (byte)(length_>0 ? length_ : 5);
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
			//WIDTH=0;
			//LENGHT=0;
			Speed=0;
			Console.WriteLine("dealloc bullet");
			Console.WriteLine(this);
        }
		dirty = true;
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
		byte* input_ = input;
		int offset = *input_;
		input_++;
		client.bullets[offset].Pos.x = *input_;
		input_+=4;
		client.bullets[offset].Pos.x = *input_;
		input_+=4;
		client.bullets[offset].Dir.x = *input_;
		input_+=4;
		client.bullets[offset].Dir.x = *input_;
		input_+=4;
		client.bullets[offset].WIDTH = *input_;
		input_++;
		client.bullets[offset].LENGHT = *input_;
	}

	public override string ToString() => "ShGame.Game.Client.Bullet[Pos:"+Pos.ToString()+", Dir:"+Dir.ToString()+"]";
}