using sh_game.game.Logic;
using sh_game.game.net.protocoll;

using SimpleLogging.logging;
using System;

namespace sh_game.game.client {

	public class Player {
		
		public Vector3d Pos {set; get;} = new Vector3d(0,0,0);
		public Vector3d Dir {set; get;} = new Vector3d(0,0,0);
		public double Speed { set; get;} = 0.0;
		public double Health {set; get;} = 0.0;

		public Player(Vector3d p) {
			Pos=p;
		}

		public Player(ParsablePlayer p) {
			Health = p.HEALTH;
			Pos = p.POS;
			Dir = p.DIR;
		}

		public bool keyUp;
		public bool keyDown;
		public bool keyLeft;
		public bool keyRight;
	
		public static readonly int radius = 10;

		public readonly long PlayerUUID;
		public Vector3d pos;
		public Vector3d dir;
		public double speed = 10E-2;
		public int health;
		public int scheduleRemoval = 0;

		private readonly Logger logger;
	
	
//	private Panel panel;

		public override string ToString() {
			return "game.graphics.client.player[health:"+health+" speed:"+speed+" pos:"+pos.ToString()+" dir:"+dir.ToString()+" UUID:"+Convert.ToString(PlayerUUID)+"]";
		}

		public void Move() {
			//		logger.log("move!");
			pos.Add(dir.cpy().Nor().Scl(speed));
		}



		//	public bool checkEdges() {
		//		
		//		bool EdgeCollision= false;
		//		
		//		if (this.pos.x+radius >= panel.PANEL_WIDTH) EdgeCollision = true;
		//		if (this.pos.x-radius <=  0 ) EdgeCollision = true;
		//		if (this.pos.y+radius >= panel.PANEL_HEIGHT) EdgeCollision= true;
		//		if (this.pos.y-radius <= 0 )EdgeCollision= true;
		//		System.out.print("Player: CheckCollision(): "+EdgeCollision);		
		//		return EdgeCollision;
		//	}

		public void OnKeyEvent(Client c) {
			if(c.keyUp) {
				if(c.keyLeft) {
					if(c.keyDown) {
						if(c.keyRight) {
							dir.x=0; //wasd
							dir.y=0;
						} else {
							//						logger.log("was");
							dir.x=-1; //was
							dir.y=0;
						}
					} else {
						if(c.keyRight) {
							dir.x=0; //wad
							dir.y=-1;
						} else {
							dir.x=(-1)/Math.Sqrt(2); //wa
							dir.y=(-1)/Math.Sqrt(2);
						}
					}
				} else {
					if(c.keyDown) {
						if(c.keyRight) {
							dir.x=1; //wsd
							dir.y=0;
						} else {
							//						logger.log("ws");
							dir.x=0; //ws
							dir.y=0;
						}
					} else {
						if(c.keyRight) {
							dir.x=1/Math.Sqrt(2); //wd
							dir.y=(-1)/Math.Sqrt(2);
						} else {
							dir.x=0; //w
							dir.y=-1;
						}
					}
				}
			} else {
				if(c.keyLeft) {
					if(c.keyDown) {
						if(c.keyRight) {
							dir.x=0; //asd
							dir.y=1;
						} else {
							dir.x=(-1)/Math.Sqrt(2); //as
							dir.y=1/Math.Sqrt(2);
						}
					} else {
						if(c.keyRight) {
							dir.x=0; //ad
							dir.y=0;
						} else {
							dir.x=(-1); //a
							dir.y=0;
						}
					}
				} else {
					if(c.keyDown) {
						if(c.keyRight) {
							dir.x=1/Math.Sqrt(2); //sd
							dir.y=1/Math.Sqrt(2);
						} else {
							dir.x=0; //s
							dir.y=1;
						}
					} else {
						if(c.keyRight) {
							dir.x=1; //d
							dir.y=0;
						} else {
							dir.x=0; //
							dir.y=0;
						}
					}
				}
			}
			//		logger.log(dir.ToString());
		}
	}
}
