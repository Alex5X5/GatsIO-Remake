using sh_game.game.Logic;
using sh_game.game.net.protocoll;

using SimpleLogging.logging;
using System;

namespace sh_game.game.client {

	public class Player {
		
		public Vector3d Pos {set; get;} = new Vector3d(0,0,0);
		public Vector3d Dir {set; get;} = new Vector3d(0,0,0);
		public double Speed { set; get;} = 10E-2;
		public double Health {set; get;} = 100;

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
		public int scheduleRemoval = 0;

		private readonly Logger logger;
	
	
//	private Panel panel;

		public override string ToString() {
			return "game.graphics.client.player[health:"+Health+" speed:"+Speed+" pos:"+Pos.ToString()+" dir:"+Dir.ToString()+" UUID:"+Convert.ToString(PlayerUUID)+"]";
		}

		public void Move() {
			//		logger.log("move!");
			Pos.Add(Dir.cpy().Nor().Scl(Speed));
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
							Dir.x=0; //wasd
							Dir.y=0;
						} else {
							//						logger.log("was");
							Dir.x=-1; //was
							Dir.y=0;
						}
					} else {
						if(c.keyRight) {
							Dir.x=0; //wad
							Dir.y=-1;
						} else {
							Dir.x=(-1)/Math.Sqrt(2); //wa
							Dir.y=(-1)/Math.Sqrt(2);
						}
					}
				} else {
					if(c.keyDown) {
						if(c.keyRight) {
							Dir.x=1; //wsd
							Dir.y=0;
						} else {
							//						logger.log("ws");
							Dir.x=0; //ws
							Dir.y=0;
						}
					} else {
						if(c.keyRight) {
							Dir.x=1/Math.Sqrt(2); //wd
							Dir.y=(-1)/Math.Sqrt(2);
						} else {
							Dir.x=0; //w
							Dir.y=-1;
						}
					}
				}
			} else {
				if(c.keyLeft) {
					if(c.keyDown) {
						if(c.keyRight) {
							Dir.x=0; //asd
							Dir.y=1;
						} else {
							Dir.x=(-1)/Math.Sqrt(2); //as
							Dir.y=1/Math.Sqrt(2);
						}
					} else {
						if(c.keyRight) {
							Dir.x=0; //ad
							Dir.y=0;
						} else {
							Dir.x=(-1); //a
							Dir.y=0;
						}
					}
				} else {
					if(c.keyDown) {
						if(c.keyRight) {
							Dir.x=1/Math.Sqrt(2); //sd
							Dir.y=1/Math.Sqrt(2);
						} else {
							Dir.x=0; //s
							Dir.y=1;
						}
					} else {
						if(c.keyRight) {
							Dir.x=1; //d
							Dir.y=0;
						} else {
							Dir.x=0; //
							Dir.y=0;
						}
					}
				}
			}
			//		logger.log(dir.ToString());
		}
	}
}
