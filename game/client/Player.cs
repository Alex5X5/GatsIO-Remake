namespace ShGame.game.Client;

public class Player {

	public const int PLAYER_BYTE_LENGTH = 48;

	public Vector3d Pos;
	public Vector3d Dir = new(0, 0, 0);
	public int Speed = 1;
	public int Health { set; get; } = 100;
	public const int radius = 10;
	public Int64 PlayerUUID;
	public bool visible;

	public Player(Vector3d? pos, int health, Int64 UUID) {
		Pos=pos??new Vector3d(0, 0, 0);
		Health=health;
		PlayerUUID=UUID!=0 ? UUID : new Random().Next();
		visible=Health!=-1;
	}

	public Player() {
		Pos = new(0, 0, 0);
		Health = -1;
		PlayerUUID = 0;
		visible=false;
	}

	public override string ToString() => $"game.graphics.client.player[health:{Health}, speed:{Speed}, pos:{Pos} dir:{Dir} UUID:{PlayerUUID}]";

	public void Move() {
		Pos.Add(Dir.Cpy().Nor().Scl(Speed));
	}

	public void Damage(int damage) {
		if(damage>Health)
			Health=0;
	}

	public void Deactivate() {
		visible = false;
		Pos.x=0;
		Pos.y=0;
		Pos.z=0;
		Dir.x=0;
		Dir.y=0;
		Dir.z=0;
		Health=0;
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
						//logger.log("was");
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
						//Console.WriteLine(Dir.ToString());
					}
				}
			} else {
				if(c.keyDown) {
					if(c.keyRight) {
						Dir.x=1; //wsd
						Dir.y=0;
					} else {
						//logger.log("ws");
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
	}

	public static void SerializePlayer(ref byte[] input, ref Player p, int offset) {
		int offset_ = offset;
		SerializePlayerCountable(ref input, ref p, ref offset_);
	}

	public static void SerializePlayerCountable(ref byte[] input, ref Player p, ref int offset) {
		if(p==null) {
			BitConverter.GetBytes(-1).CopyTo(input, offset);
			offset+=PLAYER_BYTE_LENGTH;
			return;
		}
		BitConverter.GetBytes(p.Health).CopyTo(input, offset);
		offset+=4;
		BitConverter.GetBytes(p.Pos==null ? 0 : p.Pos.x).CopyTo(input, offset);
		offset+=8;
		BitConverter.GetBytes(p.Pos==null ? 0 : p.Pos.y).CopyTo(input, offset);
		offset+=8;
		BitConverter.GetBytes(p.Dir==null ? 0 : p.Dir.x).CopyTo(input, offset);
		offset+=8;
		BitConverter.GetBytes(p.Dir==null ? 0 : p.Dir.y).CopyTo(input, offset);
		offset+=8;
		BitConverter.GetBytes(p.Speed).CopyTo(input, offset);
		offset+=4;
		BitConverter.GetBytes(p.PlayerUUID).CopyTo(input, offset);
		offset+=8;
	}

	public static void DeserializePlayer(ref byte[] input, ref Player player, int offset) {
		int offset_ = offset;
		DeserializePlayerCountable(ref input, ref player, ref offset_);
	}
	
	public static void DeserializePlayerCountable(ref byte[] input, ref Player player, ref int offset) {
		if(input==null)
			return;
		player ??= new Player(null, 0, 0);
		player.Health = BitConverter.ToInt32(input, offset);
		if(player.Health==-1) {
			player.Deactivate();
			offset+=PLAYER_BYTE_LENGTH;
		} else {
			offset+=4;
			player.Pos.x=BitConverter.ToDouble(input, offset);
			offset+=8;
			player.Pos.y=BitConverter.ToDouble(input, offset);
			offset+=8;
			player.Dir.x=BitConverter.ToDouble(input, offset);
			offset+=8;
			player.Dir.y=BitConverter.ToDouble(input, offset);
			offset+=8;
			player.Speed=BitConverter.ToInt32(input, offset);
			offset+=4;
			player.PlayerUUID = BitConverter.ToInt64(input, offset);
			offset+=8;
		}
	}
}
