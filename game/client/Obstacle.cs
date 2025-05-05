namespace ShGame.game.Client;

using ShGame.game.Client.Rendering;
using System;

public class Obstacle:Drawable, ISupportsShadow {

    public const int OBSTACLE_BYTE_LENGTH = 20;
	public int WIDTH, HEIGHT;
	public byte type;

	public readonly LineSection3d boundL, boundT, boundR, boundB;
	public Vector3d Pos;

    public Shadow? shadow;

	private Client? client;

	public Obstacle(Client? client_, Vector3d? pos_, byte type_):base(18) {
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
		boundL = new LineSection3d(Pos, Pos.Cpy().Add(0, HEIGHT, 0));
		boundT = new LineSection3d(Pos, Pos.Cpy().Add(WIDTH, 0, 0));
		boundB = new LineSection3d(boundL.point2, boundL.point2.Cpy().Add(WIDTH, 0, 0));
		boundR = new LineSection3d(boundT.point2, boundB.point2);
		//Console.WriteLine(vertices);
	}

    public override void UpdateVertices() {
        vertices[0]=(float)Pos.x;
        vertices[1]=(float)Pos.y;
        vertices[2]=0;
        vertices[3]=(float)Pos.x+WIDTH;
        vertices[4]=(float)Pos.y;
        vertices[5]=0;
        vertices[6]=(float)Pos.x+WIDTH;
        vertices[7]=(float)Pos.y+HEIGHT;
        vertices[8]=0;
        vertices[9]=(float)Pos.x;
        vertices[10]=(float)Pos.y;
        vertices[11]=0;
        vertices[12]=(float)Pos.x;
        vertices[13]=(float)Pos.y+HEIGHT;
        vertices[14]=0;
        vertices[15]=(float)Pos.x+WIDTH;
        vertices[16]=(float)Pos.y+HEIGHT;
        vertices[17]=0;
    }

    /// <summary>
    /// updates the bound objects of an obstacle to match its width and height.
    /// </summary>
    private static unsafe void UpdateBounds(Obstacle* obstacle) {
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

    /// <summary>
    /// This class performs an important function.
    /// </summary>

    public static unsafe void SerializeObstacle(byte[]* input, Obstacle* obstacle, int offset) {
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

	public static unsafe void DeserializeObstacle(Client? client_, byte[]* input, Obstacle* obstacle, int offset) {
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
		Pos.Cpy().Add(new Vector3d(WIDTH, HEIGHT, 0)).Sub(GetPointOfView()).Nor();

	public unsafe void GetShadowOrigins(out Vector3d point1, out Vector3d point2) {
		point1 = new(0, 0, 0);
		point2 = new(0, 0, 0);
		if (client==null) 
			return;
		fixed (Vector3d* pos = &client.player.Pos) {
			if (RelativeX(pos)==1&&RelativeY(pos)==1) {
				point1.x=boundR.point1.x;
				point1.y=boundR.point1.y;
				point2.x=boundL.point2.x;
				point2.y=boundL.point2.y;
			} else if (RelativeX(pos)==2&&RelativeY(pos)==1) {
				point1.x=boundL.point1.x;
				point1.y=boundL.point1.y;
				point2.x=boundR.point1.x;
				point2.y=boundR.point1.y;
			} else if (RelativeX(pos)==3&&RelativeY(pos)==1) {
				point1.x=boundL.point1.x;
				point1.y=boundL.point1.y;
				point2.x=boundR.point2.x;
				point2.y=boundR.point2.y;
			} else if (RelativeX(pos)==1&&RelativeY(pos)==2) {
				point1.x=boundL.point1.x;
				point1.y=boundL.point1.y;
				point2.x=boundL.point2.x;
				point2.y=boundL.point2.y;
			} else if (RelativeX(pos)==3&&RelativeY(pos)==2) {
				point1.x=boundR.point1.x;
				point1.y=boundR.point1.y;
				point2.x=boundR.point2.x;
				point2.y=boundR.point2.y;
			} else if (RelativeX(pos)==1&&RelativeY(pos)==3) {
				point1.x=boundL.point1.x;
				point1.y=boundL.point1.y;
				point2.x=boundR.point2.x;
				point2.y=boundR.point2.y;
			} else if (RelativeX(pos)==2&&RelativeY(pos)==3) {
				point1.x=boundL.point2.x;
				point1.y=boundL.point2.y;
				point2.x=boundR.point2.x;
				point2.y=boundR.point2.y;
			} else if (RelativeX(pos)==3&&RelativeY(pos)==3) {
				point1.x=boundR.point1.x;
				point1.y=boundR.point1.y;
				point2.x=boundL.point2.x;
				point2.y=boundL.point2.y;
			}
		}
	}

	public override string ToString() => "ShGame.Game.Client.Obstacle[Pos:"+Pos.ToString()+", Type:"+Convert.ToString(type)+"]";
}