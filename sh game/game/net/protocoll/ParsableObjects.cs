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
			this.POS = o.Pos;
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

	public class Serializer {
		public static readonly int OBSTACKLE_LENGTH = 20;

		

		 

		public static void SerializeObstacle(ref byte[] buffer, ref Obstacle o, ref int offset) {
			BitConverter.GetBytes(o.Pos.x).CopyTo(buffer, offset);
			offset += 8;
			BitConverter.GetBytes(o.Pos.y).CopyTo(buffer, offset);
			offset += 8;
			BitConverter.GetBytes(o.type).CopyTo(buffer, offset);
			offset += 4;
		}
	}
}
