namespace ShGame.game.Net.protocoll;

using ShGame.game.Client;

[Serializable]
public readonly struct ParsableObstacle {
	public readonly Vector3d POS;
	public readonly int TYPE;
	public readonly int WIDTH, HEIGTH;

	public ParsableObstacle(Obstacle o) {
		POS=o.Pos;
		TYPE=o.type;
		WIDTH=o.WIDTH;
		HEIGTH=o.HEIGHT;
	}
}

[Serializable]
public readonly struct ParsablePlayer {
	public readonly Vector3d POS;
	public readonly Vector3d DIR;
	public readonly int HEALTH;

	public ParsablePlayer(Player p) {
		HEALTH=p.Health;
		POS=p.Pos;
		DIR=p.Dir;
	}
}

public class Serializer {
	public static readonly int OBSTACKLE_LENGTH = 20;





	public static void SerializeObstacle(ref byte[] buffer, ref Obstacle o, ref int offset) {
		BitConverter.GetBytes(o.Pos.x).CopyTo(buffer, offset);
		offset+=8;
		BitConverter.GetBytes(o.Pos.y).CopyTo(buffer, offset);
		offset+=8;
		//BitConverter.GetBytes(o.type).CopyTo(buffer, offset);
		//offset+=4;
	}
}
