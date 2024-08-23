using sh_game.game.client;
using sh_game.game.Logic;
using System;

namespace sh_game.game.net.protocoll {

	[Serializable]
	public readonly struct ParsableObstacle {
		public readonly Vector3d POS;
		public readonly int TYPE;
		public readonly int WIDTH, HEIGTH;

		public ParsableObstacle(Obstacle o) {
			this.POS = o.pos;
			this.TYPE = o.type;
			this.WIDTH = o.WIDTH;
			this.HEIGTH = o.HEIGHT;
		}
	}

	[Serializable]
	public readonly struct ParsablePlayer {
		public readonly Vector3d POS;
		public readonly Vector3d DIR;
		public readonly double HEALTH;

		public ParsablePlayer(Player p) {
			HEALTH = p.Health;
			POS = p.Pos;
			DIR = p.Dir;
		}
	}
}
