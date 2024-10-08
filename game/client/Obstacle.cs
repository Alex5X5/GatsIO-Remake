﻿namespace ShGame.game.Client;

using ShGame.game.Net.protocoll;
using System;

#pragma warning disable CS8500 //insert spaces instead of tabs

public class Obstacle {

	//public readonly long UUID = new Random().NextInt64();

	public Vector3d Pos;

	//private readonly LoggingLevel mLvl = new LoggingLevel("Obstacle"+UUID);
	//private readonly Logger logger = new Logger(mLvl);

	public const int OBSTACLE_BYTE_LENGTH = 36;

	public int WIDTH, HEIGHT;
	public readonly LineSection3d boundL, boundT, boundR, boundB;
	public int type;

	//public Obsticle(int x, int y) {
	//	this.pos=new Vector3d((double)x, (double)x, 0);
	//}
	public Obstacle() {
		Pos = new(0, 0, 0);
		WIDTH = 0;
		HEIGHT = 0;
		boundL = new LineSection3d(Pos, Pos.Cpy().Add(0, HEIGHT, 0));
		boundT = new LineSection3d(Pos, Pos.Cpy().Add(WIDTH, 0, 0));
		boundB = new LineSection3d(boundL.point2, boundL.point2.Cpy().Add(WIDTH, 0, 0));
		boundR = new LineSection3d(boundT.point2, boundB.point2);
	}

	public Obstacle(Vector3d pos_, int type_) {
		Pos=pos_??new Vector3d(0, 0, 0);
		type=type_;
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
		//logger.log("set bounds");
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
			//return new Vector3d[] { boundL.point1, boundR.point1 };
			point1->x=boundL.point1.x;
			point1->y=boundL.point1.y;
			point2->x=boundR.point1.x;
			point2->y=boundR.point1.y;
		} else if(RelativeX(pos)==3&&RelativeY(pos)==1) {
			//return new Vector3d[] { boundL.point1, boundR.point2 };
			point1->x=boundL.point1.x;
			point1->y=boundL.point1.y;
			point2->x=boundR.point2.x;
			point2->y=boundR.point2.y;
		} else if(RelativeX(pos)==1&&RelativeY(pos)==2) {
			//return new Vector3d[] { boundL.point1, boundL.point2 };
			point1->x=boundL.point1.x;
			point1->y=boundL.point1.y;
			point2->x=boundL.point2.x;
			point2->y=boundL.point2.y;
		} else if(RelativeX(pos)==3&&RelativeY(pos)==2) {
			//return new Vector3d[] { boundR.point1, boundR.point2 };
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
			//return new Vector3d[] { boundR.point1, boundL.point2 };
		}
		//return null;
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
		return "game.client.graphics.Obstacle[type:" + Convert.ToString(type) + " posX:" + Convert.ToString(Pos.x) + " Convert.ToString(posY:" + Convert.ToString(Pos.y) + "]";
	}

	private static unsafe void UpdateBounds(Obstacle* obstacle) {
		obstacle->boundL.point1.Set(obstacle->Pos.x, obstacle->Pos.y, 0);
		obstacle->boundL.point2.Set(obstacle->Pos.x, obstacle->Pos.y+obstacle->HEIGHT, 0);
		obstacle->boundR.point1.Set(obstacle->Pos.x+obstacle->WIDTH, obstacle->Pos.y, 0);
		obstacle->boundR.point2.Set(obstacle->Pos.x+obstacle->WIDTH, obstacle->Pos.y+obstacle->HEIGHT, 0);
		obstacle->boundT.point1.Set(obstacle->boundT.point1);
		obstacle->boundT.point2.Set(obstacle->boundR.point1);
		obstacle->boundB.point1.Set(obstacle->boundL.point2);
		obstacle->boundB.point2.Set(obstacle->boundR.point2);
	}

	public static unsafe void SerializeObstacle(ref byte[] input, Obstacle* obstacle, int offset) {
		int offset_ = offset;
		SerializeObstacleCountable(ref input, obstacle, ref offset_);
	}

	public static unsafe void SerializeObstacleCountable(ref byte[] input, Obstacle* obstacle, ref int offset) {
		if(obstacle==null) {
			BitConverter.GetBytes(-1).CopyTo(input, offset);
			offset+=OBSTACLE_BYTE_LENGTH;
		} else {
			BitConverter.GetBytes(obstacle->type).CopyTo(input, offset);
			offset+=4;
			BitConverter.GetBytes(obstacle->Pos==null ? 0 : obstacle->Pos.x).CopyTo(input, offset);
			offset+=8;
			BitConverter.GetBytes(obstacle->Pos==null ? 0 : obstacle->Pos.y).CopyTo(input, offset);
			offset+=8;
			BitConverter.GetBytes(obstacle->WIDTH==0 ? 0 : obstacle->WIDTH).CopyTo(input, offset);
			offset+=4;
			BitConverter.GetBytes(obstacle->HEIGHT==0 ? 0 : obstacle->HEIGHT).CopyTo(input, offset);
			offset+=4;
		}
	}

	public static unsafe void DeserializeObstacle(ref byte[] input, Obstacle* obstacle, int offset) {
		int offset_ = offset;
		DeserializeObstacleCountable(ref input, obstacle, ref offset_);
	}

	public static unsafe void DeserializeObstacleCountable(ref byte[] input, Obstacle* obstacle, ref int offset) {
		ArgumentNullException.ThrowIfNull(input);
		ArgumentNullException.ThrowIfNull(obstacle);

		int type_ = BitConverter.ToInt32(input, offset);
		if(type_==-1) {
			//obstacle=null;
			offset+=OBSTACLE_BYTE_LENGTH;
		} else {
			if(*obstacle==null)
				*obstacle=new Obstacle(new Vector3d(0, 0, 0), type_);
			else
				obstacle->type=type_;
			offset+=4;
			obstacle->Pos.x=BitConverter.ToDouble(input, offset);
			offset+=8;
			obstacle->Pos.y=BitConverter.ToDouble(input, offset);
			offset+=8;
			obstacle->WIDTH=BitConverter.ToInt32(input, offset);
			offset+=4;
			obstacle->HEIGHT=BitConverter.ToInt32(input, offset);
			offset+=4;
			UpdateBounds(obstacle);
		}
	}

	public enum Type : byte {
		Wide,
		High,
		WideHigh
	}
}
