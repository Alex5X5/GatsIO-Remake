using sh_game.game.Logic;
using sh_game.game.net.protocoll;

namespace sh_game.game.client {

	public class Player {
		
		public Vector3d pos {set; get;} = new Vector3d(0,0,0);
		public Vector3d dir {set; get;} = new Vector3d(0,0,0);
		public double speed { set; get;} = 0.0;
		public double health {set; get;} = 0.0;

		public Player(Vector3d p) {
			pos=p;
		}

		public Player(ParsablePlayer p) {
			health=p.HEALTH;
			pos = p.POS;
			dir = p.DIR;
		}
	}
}
