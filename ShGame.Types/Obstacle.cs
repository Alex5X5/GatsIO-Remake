namespace ShGame.Types;

using System;
using ShGame.Math;

public struct Obstacle {

	public const int SizeInBytes = 29;
	public const int SizeInBytesForNetwork = 17;

	public byte type;
	public Vector3d Pos;
	public int WIDTH, HEIGHT;
	
	public Obstacle() : this(new Vector3d(0.0, 0.0, 0.0), 1){
	
	}

	public Obstacle(Vector3d? pos_, byte type_) {
		Pos = pos_ ?? new Vector3d(0, 0, 0);
		type = type_;
		switch (type) {
			case 1:
				WIDTH = 150;
				HEIGHT = 150;
				break;
			case 2:
				WIDTH = 150;
				HEIGHT = 75;
				break;
			case 3:
				WIDTH = 75;
				HEIGHT = 150;
				break;
			default:
				WIDTH = 0;
				HEIGHT = 0;
				break;
		}
	}

	public override string ToString() => "ShGame.Game.Client.Obstacle[Pos:"+Pos.ToString()+", Type:"+Convert.ToString(type)+"]";
}