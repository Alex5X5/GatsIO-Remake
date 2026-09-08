namespace ShGame.Types;

using ShGame.Math;

public struct Bullet {

	public const int BULLET_BYTE_LENGTH = 38;

	public Vector3d Pos;
	public Vector3d Dir;

	private byte WIDTH, LENGHT;

	public short Speed;
	public short Lifetime;
	public short OwnerHandle;

	public Bullet() : this(null, null, 0, 0) { }

	public Bullet(Vector3d? _pos, Vector3d? _dir, int _width, int _length) {
		Pos = _pos??new Vector3d(10, 10, 0);
		Dir = _dir??new Vector3d(0, 1, 0);
		WIDTH = (byte)(_width>0 ? _width : 5);
		LENGHT = (byte)(_length>0 ? _length : 5);
	}

	public void Move() {
		Pos = Pos.Add(
			Dir.Scl(Speed)
		);
		//if (Pos.x<0|Pos.y<0|Pos.x>Constants.MAP_GRID_WIDTH|Pos.y>Constants.MAP_GRID_HEIGHT) {
		//	Dealloc();
		//}
		//dirty = true;
	}

	public void CheckObstacleCollision(Obstacle[] obstacles) {
		foreach (Obstacle obstacle in obstacles) {
			Vector3d bulletCorner1 = Pos;
			Vector3d bulletCorner2 = Pos.Cpy().Add(WIDTH, LENGHT, 0);

			Vector3d obstacleCorner1 = obstacle.Pos;
			Vector3d obstacleCorner2 = obstacle.Pos.Cpy().Add(obstacle.WIDTH, obstacle.HEIGHT, 0);

			//if (
			//	bulletCorner1.x <= obstacleCorner2.x && bulletCorner1.y <= obstacleCorner2.y &&
			//	bulletCorner2.x >= obstacleCorner1.x && bulletCorner2.y >= obstacleCorner1.y
			//) {
			//	Dealloc();
			//}
		}
	}

	public override string ToString() => "ShGame.Types.Bullet[Pos:"+Pos.ToString()+", Dir:"+Dir.ToString()+"]";
}