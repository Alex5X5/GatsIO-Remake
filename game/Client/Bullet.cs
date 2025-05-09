namespace ShGame.game.Client;
using ShGame.game.Client.Rendering;

using System;
using System.Runtime.InteropServices;

public class Bullet:Drawable {

	public const int BULLET_BYTE_LENGTH = 20;

	public Vector3d Pos;
	public Vector3d Dir;
	private int WIDTH, LENGHT;

	public Bullet(Vector3d? pos_, Vector3d? dir_, int width_, int length_) :base(18) {
		Pos = pos_??new Vector3d(10, 10, 0);
		Dir = dir_??new Vector3d(0, 1, 0);
		WIDTH = width_>0 ? width_ : 5;
		LENGHT = length_>0 ? length_ : 5;
    }

	public override void Dispose() {
		GC.SuppressFinalize(this);
		base.Dispose();
	}

	public unsafe override void UpdateVertices() {
		//float* ptr = VertexDataPtr;
		//*ptr=(float)Pos.x;
		//ptr++;
		//*ptr=(float)Pos.y;
		//ptr++;
		//*ptr=0;
		//ptr++;
		//*ptr=(float)Pos.x+WIDTH;
		//ptr++;
		//*ptr=(float)Pos.y;
		//ptr++;
		//*ptr=0;
		//ptr++;
		//*ptr=(float)Pos.x+WIDTH;
		//ptr++;
		//*ptr=(float)Pos.y+HEIGHT;
		//ptr++;
		//*ptr=0;
		//ptr++;
		//*ptr=(float)Pos.x;
		//ptr++;
		//*ptr=(float)Pos.y;
		//ptr++;
		//*ptr=0;
		//ptr++;
		//*ptr=(float)Pos.x;
		//ptr++;
		//*ptr=(float)Pos.y+HEIGHT;
		//ptr++;
		//*ptr=0;
		//ptr++;
		//*ptr=(float)Pos.x+WIDTH;
		//ptr++;
		//*ptr=(float)Pos.y+HEIGHT;
		//ptr++;
		//*ptr=0;
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
		client.Bullets[offset].Pos.x = *input_;
		input_+=4;
        client.Bullets[offset].Pos.x = *input_;
        input_+=4;
		client.Bullets[offset].Dir.x = *input_;
        input_+=4;
        client.Bullets[offset].Dir.x = *input_;
        input_+=4;
        client.Bullets[offset].WIDTH = *input_;
		input_++;
        client.Bullets[offset].LENGHT = *input_;
    }

    public override string ToString() => "ShGame.Game.Client.Bullet[Pos:"+Pos.ToString()+"]";
}