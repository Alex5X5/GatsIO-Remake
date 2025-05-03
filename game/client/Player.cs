namespace ShGame.game.Client;

using ShGame.game.Client.Rendering;
using ShGame.game.Logic;

public class Player {

	public const int PLAYER_BYTE_LENGTH = 48;

    public Vector3d Pos;
    public int WIDTH, HEIGHT;

    public byte type;

    public Vector3d Dir = new(0, 0, 0);
	public int Speed = 1;

	//private int Health_ = 0;// property

	private int Health_;
	public int Health {
		get => Health_;
		set => Health_ = value>100 ? 100 : value<-1 ? -1 : value;
	}

	public const int Radius = 10;
	public Int64 PlayerUUID = 0;
	public bool Visible;

	public Player(Vector3d? newPos, int newHealth, Int64 UUID) {
		Pos = newPos??new Vector3d(0, 0, 0);
		Health_ =newHealth;
		PlayerUUID = UUID!=0 ? UUID : new Random().Next();
		Visible=Health_ !=-1;
	}

	//the constructor for invalid players
	public Player() {
		Pos = new(0, 0, 0);
		//if the health of a player is -1 it is considered invalid and won't be processed
		Health_ = -1;
		PlayerUUID = 0;
		Visible = false;
	}

	public override string ToString() => $"game.graphics.client.player[health:{Health}, speed:{Speed}, pos:{Pos} dir:{Dir} UUID:{PlayerUUID}]";

	public unsafe void Move() {
		Pos.Add(Dir.Cpy().Nor().Scl(Speed));
	}

	public void Damage(int damage) {
		if(damage>Health)
			Health=0;
	}

	public void Deactivate() {
		Visible = false;
		Pos = new(0, 0, 0);
		Dir.x = 0;	
		Dir.y = 0;
		Dir.z = 0;
		Health_ = -1;
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

	public void OnKeyEvent(Client2 c) {
		if(c.keyUp) {
			if(c.keyLeft) {
				if(c.keyDown) {
					if(c.keyRight) {
						Dir.x=0; //wasd
						Dir.y=0;
					} else {
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

	public static unsafe void SerializePlayer(byte[]* input, Player* player, int offset) {
		//Console.WriteLine("serializing"+player->ToString());
		int offset_ = offset;
		if(player==null) {
			BitConverter.GetBytes(-1).CopyTo(*input, offset_);
			return;
		}
		BitConverter.GetBytes(player->Health_).CopyTo(*input, offset_);
		offset_+=4;
		BitConverter.GetBytes(player->Pos.x).CopyTo(*input, offset_);
		offset_+=8;
		BitConverter.GetBytes(player->Pos.y).CopyTo(*input, offset_);
		offset_+=8;
		BitConverter.GetBytes(player->Dir.x).CopyTo(*input, offset_);
		offset_+=8;
		BitConverter.GetBytes(player->Dir.y).CopyTo(*input, offset_);
		offset_+=8;
		BitConverter.GetBytes(player->Speed).CopyTo(*input, offset_);
		offset_+=4;
		BitConverter.GetBytes(player->PlayerUUID).CopyTo(*input, offset_);
		//Console.WriteLine("finished serializing"+player->ToString());
	}

	public static unsafe void DeserializePlayer(byte[]* input, Player* player, int offset) {
		int offset_ = offset;
		if (input==null)
			return;
		*player ??= new Player(null, 0, 0);
		//Console.WriteLine("writing "+ *input + " at "+offset_);
		player->Health_ = BitConverter.ToInt32(*input, offset_);
		if (player->Health_ ==-1) {
			player->Deactivate();
			//offset_+=PLAYER_BYTE_LENGTH;
		} else {
			//Console.WriteLine("writing "+ *input + " at "+offset_);
			offset_+=4;
			player->Pos.x=BitConverter.ToDouble(*input, offset_);
			//Console.WriteLine("writing "+ *input + " at "+offset_ );
			offset_+=8;
			player->Pos.y=BitConverter.ToDouble(*input, offset_);
			//Console.WriteLine("writing "+ *input + " at "+offset_);
			offset_+=8;
			player->Dir.x=BitConverter.ToDouble(*input, offset_);
			//Console.WriteLine("writing "+ *input + " at "+offset_);
			offset_+=8;
			player->Dir.y=BitConverter.ToDouble(*input, offset_);
			//Console.WriteLine("writing "+ *input + " at "+offset_);
			offset_+=8;
			player->Speed=BitConverter.ToInt32(*input, offset_);
			//Console.WriteLine("writing "+ *input + " at "+offset_);
			offset_+=4;
			player->PlayerUUID = BitConverter.ToInt64(*input, offset_);
		}
	}
}