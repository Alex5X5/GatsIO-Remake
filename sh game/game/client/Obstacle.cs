using sh_game.game.Logic;
using sh_game.game.net.protocoll;

using SimpleLogging.logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sh_game.game.client {
	[Serializable]
	public class Obstacle {

		//public readonly long UUID = new Random().NextInt64();

		public readonly Vector3d pos;

		//private readonly LoggingLevel mLvl = new LoggingLevel("Obstacle"+UUID);
		//private readonly Logger logger = new Logger(mLvl);

		public readonly int WIDTH, HEIGHT;
		private readonly LineSection3d boundL, boundT, boundR, boundB;
		public readonly int type;

		//	public Obsticle(int x, int y) {
		//		this.pos = new Vector3d((double)x,(double)x,0);
		//	}

		public Obstacle(Vector3d position, int type) {
			//new Random().;
			this.pos=position;
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
			boundL=new LineSection3d(pos, pos.cpy().Add(0, HEIGHT, 0));
			boundT=new LineSection3d(pos, pos.cpy().Add(WIDTH, 0, 0));
			boundB=new LineSection3d(boundL.point2, boundL.point2.cpy().Add(WIDTH, 0, 0));
			boundR=new LineSection3d(boundT.point2, boundB.point2);
			//logger.log("set bounds");
		}

		public Obstacle(ParsableObstacle obstacle) {
			pos = obstacle.POS;
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
			boundL=new LineSection3d(pos, pos.cpy().Add(0, HEIGHT, 0));
			boundT=new LineSection3d(pos, pos.cpy().Add(WIDTH, 0, 0));
			boundB=new LineSection3d(boundL.point2, boundL.point2.cpy().Add(WIDTH, 0, 0));
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
			if(v.x<=pos.x)
				return 1;
			else if(v.x>pos.x&&v.x<pos.x+WIDTH)
				return 2;
			else
				return 3;
		}

		private int RelativeY(Vector3d v) {
			if(v.y<=pos.y)
				return 1;
			else if(v.y>pos.y&&v.y<pos.y+HEIGHT)
				return 2;
			else
				return 3;
		}

		public override String ToString() {
			return "game.client.graphics.Obstacle[type:"+Convert.ToString(type)+" posX:"+pos.x+" posY"+pos.y+"]";
		}
	}
}
