namespace ShGame.game.Client;

using ShGame.game.Net;
using ShGame.game.Net.protocoll;
using System;

using static ShGame.game.Client.Obstacle;

#pragma warning disable CS8500 //insert spaces instead of tabs

public class Obstacle {

	public Vector3d Pos;
    public readonly LineSection3d boundL, boundT, boundR, boundB;

    public const int OBSTACLE_BYTE_LENGTH = 20;

	public int WIDTH, HEIGHT;
	public int type;

	public Obstacle() {
		Pos = new(0, 0, 0);
		WIDTH = 0;
		HEIGHT = 0;
		boundL = new LineSection3d(Pos, Pos.Cpy().Add(0, HEIGHT, 0));
		boundT = new LineSection3d(Pos, Pos.Cpy().Add(WIDTH, 0, 0));
		boundB = new LineSection3d(boundL.point2, boundL.point2.Cpy().Add(WIDTH, 0, 0));
		boundR = new LineSection3d(boundT.point2, boundB.point2);
	}

	public Obstacle(Vector3d? pos_, int type_) {
		Pos = pos_??new Vector3d(0, 0, 0);
		type = type_;
		switch(type) {
			case 1:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 35;
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
	}

	public Obstacle(ParsableObstacle obstacle) {
		Pos = obstacle.POS;
		type = obstacle.TYPE;

		switch(type) {
			case 1:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH=35;
				HEIGHT=70;
				break;
			case 2:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH=70;
				HEIGHT=35;
				break;
			case 3:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH=70;
				HEIGHT=70;
				break;
			default:
				//logger.error("illegal type", new MessageParameter("type", type));
				WIDTH=0;
				HEIGHT=0;
				break;
		}
		boundL=new LineSection3d(Pos, Pos.Cpy().Add(0, HEIGHT, 0));
		boundT=new LineSection3d(Pos, Pos.Cpy().Add(WIDTH, 0, 0));
		boundB=new LineSection3d(boundL.point2, boundL.point2.Cpy().Add(WIDTH, 0, 0));
		boundR=new LineSection3d(boundT.point2, boundB.point2);
	}

	public unsafe LineSection3d[]? GetVisibleSides(Vector3d* v) {
		if(RelativeX(v)==1&&RelativeY(v)==1) {
			return [boundL, boundT];
		} else if(RelativeX(v)==2&&RelativeY(v)==1) {
			return [boundT];
		} else if(RelativeX(v)==3&&RelativeY(v)==1) {
			return [boundT, boundR];
		} else if(RelativeX(v)==1&&RelativeY(v)==2) {
			return [boundL];
		} else if(RelativeX(v)==3&&RelativeY(v)==2) {
			return [boundR];
		} else if(RelativeX(v)==1&&RelativeY(v)==3) {
			return [boundL, boundB];
		} else if(RelativeX(v)==2&&RelativeY(v)==3) {
			return [boundB];
		} else if(RelativeX(v)==3&&RelativeY(v)==3) {
			return [boundR, boundB];
		}
		return null;
	}

	public unsafe void GetShadowPoints(Vector3d* pos, Vector3d* point1, Vector3d* point2) {
		if(RelativeX(pos)==1&&RelativeY(pos)==1) {
			//
			point1->x=boundR.point1.x;
			point1->y=boundR.point1.y;
			point2->x=boundL.point2.x;
			point2->y=boundL.point2.y;
		} else if(RelativeX(pos)==2&&RelativeY(pos)==1) {
			point1->x=boundL.point1.x;
			point1->y=boundL.point1.y;
			point2->x=boundR.point1.x;
			point2->y=boundR.point1.y;
		} else if(RelativeX(pos)==3&&RelativeY(pos)==1) {
			point1->x=boundL.point1.x;
			point1->y=boundL.point1.y;
			point2->x=boundR.point2.x;
			point2->y=boundR.point2.y;
		} else if(RelativeX(pos)==1&&RelativeY(pos)==2) {
			point1->x=boundL.point1.x;
			point1->y=boundL.point1.y;
			point2->x=boundL.point2.x;
			point2->y=boundL.point2.y;
		} else if(RelativeX(pos)==3&&RelativeY(pos)==2) {
			point1->x=boundR.point1.x;
			point1->y=boundR.point1.y;
			point2->x=boundR.point2.x;
			point2->y=boundR.point2.y;
		} else if(RelativeX(pos)==1&&RelativeY(pos)==3) {
			point1->x=boundL.point1.x;
			point1->y=boundL.point1.y;
			point2->x=boundR.point2.x;
			point2->y=boundR.point2.y;
		} else if(RelativeX(pos)==2&&RelativeY(pos)==3) {
			point1->x=boundL.point2.x;
			point1->y=boundL.point2.y;
			point2->x=boundR.point2.x;
			point2->y=boundR.point2.y;
		} else if(RelativeX(pos)==3&&RelativeY(pos)==3) {
			point1->x=boundR.point1.x;
			point1->y=boundR.point1.y;
			point2->x=boundL.point2.x;
			point2->y=boundL.point2.y;
		}
	}

	private unsafe int RelativeX(Vector3d* v) {
		if(v->x<=Pos.x)
			return 1;
		else if(v->x>Pos.x&&v->x<Pos.x+WIDTH)
			return 2;
		else
			return 3;
	}

	private unsafe int RelativeY(Vector3d* v) {
		if(v->y<=Pos.y)
			return 1;
		else if(v->y>Pos.y&&v->y<Pos.y+HEIGHT)
			return 2;
		else
			return 3;
	}

	public override string ToString() {
		return "game.client.graphics.Obstacle[type:" + Convert.ToString(type) + " posX:" + Convert.ToString(Pos.x) + " posY:" + Convert.ToString(Pos.y) + "]";
	}

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

	public static unsafe void SerializeObstacle(byte[]* input, Obstacle* obstacle, int offset) {
		int offset_ = offset;
		if (obstacle==null) {
			BitConverter.GetBytes(-1).CopyTo(*input, offset_);
		} else {
            BitConverter.GetBytes(obstacle->type).CopyTo(*input, offset_);
            offset_ += 4;
            BitConverter.GetBytes((int)(obstacle->Pos==null ? 0 : obstacle->Pos.x)).CopyTo(*input, offset_);
            offset_ += 4;
            BitConverter.GetBytes((int)(obstacle->Pos==null ? 0 : obstacle->Pos.y)).CopyTo(*input, offset_);
            offset_ += 4;
            BitConverter.GetBytes(obstacle->WIDTH).CopyTo(*input, offset_);
            offset_ += 4;
            BitConverter.GetBytes(obstacle->HEIGHT).CopyTo(*input, offset_);
        }
	}

    public static unsafe void DeserializeObstacle(byte[]* input, Obstacle* obstacle, int offset) {
        ArgumentNullException.ThrowIfNull(*input);
        int offset_ = offset;
		*obstacle ??= new Obstacle(null, 0);
        obstacle->type = BitConverter.ToInt32(*input, offset_);
        if (obstacle->type==-1) {
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

	public enum Type : byte {
		Wide,
		High,
		WideHigh
	}
}
