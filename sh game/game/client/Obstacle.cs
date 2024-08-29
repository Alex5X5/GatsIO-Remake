using sh_game.game.Logic;
using sh_game.game.net.protocoll;
using System;

namespace sh_game.game.client {
	public class Obstacle {

		//public readonly long UUID = new Random().NextInt64();

		public Vector3d Pos;

		//private readonly LoggingLevel mLvl = new LoggingLevel("Obstacle"+UUID);
		//private readonly Logger logger = new Logger(mLvl);

		public const int OBSTACLE_BYTE_LENGTH = 36;

		public int WIDTH, HEIGHT;
		private LineSection3d boundL, boundT, boundR, boundB;
		public int type;

		//public Obsticle(int x, int y) {
		//	this.pos=new Vector3d((double)x, (double)x, 0);
		//}

		public Obstacle(Vector3d position, int type) {
			//new Random().;
			Pos=position;
			this.type=type;
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

		public LineSection3d[] GetVisibleSides(Vector3d v) {
			if(RelativeX(v)==1&&RelativeY(v)==1) {
				return new LineSection3d[] { boundL, boundT };
			} else if(RelativeX(v)==2&&RelativeY(v)==1) {
				return new LineSection3d[] { boundT };
			} else if(RelativeX(v)==3&&RelativeY(v)==1) {
				return new LineSection3d[] { boundT, boundR };
			} else if(RelativeX(v)==1&&RelativeY(v)==2) {
				return new LineSection3d[] { boundL };
			} else if(RelativeX(v)==3&&RelativeY(v)==2) {
				return new LineSection3d[] { boundR };
			} else if(RelativeX(v)==1&&RelativeY(v)==3) {
				return new LineSection3d[] { boundL, boundB };
			} else if(RelativeX(v)==2&&RelativeY(v)==3) {
				return new LineSection3d[] { boundB };
			} else if(RelativeX(v)==3&&RelativeY(v)==3) {
				return new LineSection3d[] { boundR, boundB };
			}
			return null;
		}

		public Vector3d[] GetShadowPoints(Vector3d v) {
			if(RelativeX(v)==1&&RelativeY(v)==1) {
				return new Vector3d[] { boundR.point1, boundL.point2 };
			} else if(RelativeX(v)==2&&RelativeY(v)==1) {
				return new Vector3d[] { boundL.point1, boundR.point1 };
			} else if(RelativeX(v)==3&&RelativeY(v)==1) {
				return new Vector3d[] { boundL.point1, boundR.point2 };
			} else if(RelativeX(v)==1&&RelativeY(v)==2) {
				return new Vector3d[] { boundL.point1, boundL.point2 };
			} else if(RelativeX(v)==3&&RelativeY(v)==2) {
				return new Vector3d[] { boundR.point1, boundR.point2 };
			} else if(RelativeX(v)==1&&RelativeY(v)==3) {
				return new Vector3d[] { boundL.point1, boundR.point2 };
			} else if(RelativeX(v)==2&&RelativeY(v)==3) {
				return new Vector3d[] { boundL.point2, boundR.point2 };
			} else if(RelativeX(v)==3&&RelativeY(v)==3) {
				return new Vector3d[] { boundR.point1, boundL.point2 };
			}
			return null;
		}

		private int RelativeX(Vector3d v) {
			if(v.x<=Pos.x)
				return 1;
			else if(v.x>Pos.x&&v.x<Pos.x+WIDTH)
				return 2;
			else
				return 3;
		}

		private int RelativeY(Vector3d v) {
			if(v.y<=Pos.y)
				return 1;
			else if(v.y>Pos.y&&v.y<Pos.y+HEIGHT)
				return 2;
			else
				return 3;
		}

		public override string ToString() {
			return "game.client.graphics.Obstacle[type:"+Convert.ToString(type)+" posX:"+Pos.x+" posY"+Pos.y+"]";
		}

		private static void UpdateBounds(ref Obstacle obstacle) {
			obstacle.boundL.point1.Set(obstacle.Pos.x, obstacle.Pos.y, 0);
			obstacle.boundL.point2.Set(obstacle.Pos.x, obstacle.Pos.y+obstacle.HEIGHT, 0);
			obstacle.boundR.point1.Set(obstacle.Pos.x+obstacle.WIDTH, obstacle.Pos.y, 0);
			obstacle.boundR.point2.Set(obstacle.Pos.x+obstacle.WIDTH, obstacle.Pos.y+obstacle.HEIGHT, 0);
			obstacle.boundT.point1.Set(obstacle.boundT.point1);
			obstacle.boundT.point2.Set(obstacle.boundR.point1);
			obstacle.boundB.point1.Set(obstacle.boundL.point2);
			obstacle.boundB.point2.Set(obstacle.boundR.point2);
		}

		public static void SerializeObstacle(ref byte[] input, ref Obstacle o, ref int offset) {
			if(o==null) {
				BitConverter.GetBytes(-1).CopyTo(input, offset);
				offset+=OBSTACLE_BYTE_LENGTH;
			} else {
				BitConverter.GetBytes(o.type).CopyTo(input, offset);
				offset+=4;
				BitConverter.GetBytes(o.Pos==null ? 0 : o.Pos.x).CopyTo(input, offset);
				offset+=8;
				BitConverter.GetBytes(o.Pos==null ? 0 : o.Pos.y).CopyTo(input, offset);
				offset+=8;
				BitConverter.GetBytes(o.WIDTH==0 ? 0 : o.WIDTH).CopyTo(input, offset);
				offset+=8;
				BitConverter.GetBytes(o.HEIGHT==0 ? 0 : o.HEIGHT).CopyTo(input, offset);
				offset+=8;
			}
		}

		public static void DeserializeObstacle(ref byte[] input, ref Obstacle obstacle, ref int offset) {
			int type_ = BitConverter.ToInt32(input, offset);
			if(type_==-1) {
				obstacle=null;
				offset+=OBSTACLE_BYTE_LENGTH;
			} else {
				if(obstacle==null)
					obstacle=new Obstacle(new Vector3d(0, 0, 0), type_);
				offset+=4;
				obstacle.Pos.x=BitConverter.ToDouble(input, offset);
				offset+=8;
				obstacle.Pos.y=BitConverter.ToDouble(input, offset);
				offset+=8;
				obstacle.WIDTH=BitConverter.ToInt32(input, offset);
				offset+=4;
				obstacle.HEIGHT=BitConverter.ToInt32(input, offset);
				offset+=4;
				UpdateBounds(ref obstacle);
			}
		}
	}
}
