namespace ShGame.game.Client;
using ShGame.game.Client.Rendering;

using System;
using System.Runtime.InteropServices;

public class Bullet:Drawable {

	public const int BULLET_BYTE_LENGTH = 20;

	public Vector3d Pos;
	public Vector3d Dir;

	public Bullet(Vector3d pos_, byte type_):base(18) {
		client = client_;
		shadow = new Shadow(this);
		Pos = pos_??new Vector3d(0, 0, 0);
		type = type_;
		switch (type) {
			case 1:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 70;
				HEIGHT = 70;
				break;
			case 2:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 70;
				HEIGHT = 35;
				break;
			case 3:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 70;
				HEIGHT = 70;
				break;
			default:
				//logger.error("illegal type", new MessageParameter("type", type));
				WIDTH = 0;
				HEIGHT = 0;
				break;
		}
	}

	public override void Dispose() {
		GC.SuppressFinalize(this);
		base.Dispose();
		shadow?.Dispose();
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
        *ptr=(float)Pos.y+HEIGHT;
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
        *ptr=(float)Pos.y+HEIGHT;
        ptr++;
        *ptr=0;
        ptr++;
        *ptr=(float)Pos.x+WIDTH;
        ptr++;
        *ptr=(float)Pos.y+HEIGHT;
        ptr++;
        *ptr=0;
	}

	/// <summary>
	/// updates the bound objects of an obstacle to match its width and height.
	/// </summary>
	private static unsafe void UpdateBounds(Bullet* obstacle) {
		obstacle->WIDTH = obstacle->type switch {
			1 => 35,
			2 => 70,
			3 => 70,
			_ => 0,
		};
		obstacle->HEIGHT = obstacle->type switch {
			1 => 70,
			2 => 35,
			3 => 70,
			_ => 0,
		};
		obstacle->boundL.point1.Set(obstacle->Pos.x, obstacle->Pos.y, 0);
		obstacle->boundL.point2.Set(obstacle->Pos.x, obstacle->Pos.y+obstacle->HEIGHT, 0);
		obstacle->boundR.point1.Set(obstacle->Pos.x+obstacle->WIDTH, obstacle->Pos.y, 0);
		obstacle->boundR.point2.Set(obstacle->Pos.x+obstacle->WIDTH, obstacle->Pos.y+obstacle->HEIGHT, 0);
		obstacle->boundT.point1.Set(obstacle->boundT.point1);
		obstacle->boundT.point2.Set(obstacle->boundR.point1);
		obstacle->boundB.point1.Set(obstacle->boundL.point2);
		obstacle->boundB.point2.Set(obstacle->boundR.point2);
	}

	public static unsafe void SerializeObstacle(byte[]* input, Bullet* obstacle, int offset) {
		int offset_ = offset;
		fixed (byte* ptr = *input)
			if (obstacle==null) {
				*(ptr+offset_) = 0;
			} else {
				*(ptr+offset_) = obstacle->type;
				offset_ += 4;
				BitConverter.GetBytes((int)obstacle->Pos.x).CopyTo(*input, offset_);
				offset_ += 4;
				BitConverter.GetBytes((int)obstacle->Pos.y).CopyTo(*input, offset_);
				offset_ += 4;
				BitConverter.GetBytes(obstacle->WIDTH).CopyTo(*input, offset_);
				offset_ += 4;
				BitConverter.GetBytes(obstacle->HEIGHT).CopyTo(*input, offset_);
			}
	}
	/// <summary>
	/// reads the next 17 bytes after the offset from a bytearray. 
	/// </summary>

	public static unsafe void DeserializeObstacle(Bulltet client_, byte[]* input, Bullet* obstacle, int offset) {
		ArgumentNullException.ThrowIfNull(*input);
		int offset_ = offset;
		*obstacle ??= new Obstacle(client_, null, 0);
		fixed (byte* ptr = *input)
			obstacle->type = *(ptr+offset_);
		if (obstacle->type==0) {
			return;
		} else {
			offset_ += 4;
			obstacle->Pos.x=BitConverter.ToInt32(*input, offset_);
			offset_ += 4;
			obstacle->Pos.y=BitConverter.ToInt32(*input, offset_);
			offset_ += 4;
			obstacle->WIDTH=BitConverter.ToInt32(*input, offset_);
			offset_+=4;
			obstacle->HEIGHT=BitConverter.ToInt32(*input, offset_);
			UpdateBounds(obstacle);
		}
		
	}

	private unsafe int RelativeX(Vector3d* v) {
		if (v->x<=Pos.x)
			return 1;
		else if (v->x>Pos.x&&v->x<Pos.x+WIDTH)
			return 2;
		else
			return 3;
	}

	private unsafe int RelativeY(Vector3d* v) {
		if (v->y<=Pos.y)
			return 1;
		else if (v->y>Pos.y&&v->y<Pos.y+HEIGHT)
			return 2;
		else
			return 3;
	}

	public Vector3d GetPointOfView() => client!=null ? client.player.Pos:new Vector3d(0,0,0);

	public Vector3d GetRelativeVector() =>
		Pos.Cpy().Add(new Vector3d(WIDTH/2, HEIGHT/2, 0)).Sub(GetPointOfView()).Nor();

	public override string ToString() => "ShGame.Game.Client.Bullet[Pos:"+Pos.ToString()+", Type:"+Convert.ToString(type)+"]";
}