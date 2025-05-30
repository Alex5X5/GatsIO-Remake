﻿namespace ShGame.Game.Client;

using ShGame.Game.Client.Rendering;
using ShGame.Game.Logic.Math;

using System.Runtime.CompilerServices;

//#pragma warning disable CS8500 //a pointer is created to a variable of an unmanaged type

public class Player : Drawable {

	public const int PLAYER_BYTE_LENGTH = 54;

	public const int SIZE = 20, SIDES_COUNT = 50, FLOAT_COUNT = 9*SIDES_COUNT;


	public static readonly int[] CIRCLE_OFFSETS = CalcCircleOffsets();

	public short weaponCooldownTicks = 10;
	public short weaponCooldownTicksDone = 10;

	public byte shooting = 0x0;
	public byte default_shoot_speed = 0x10;

	public Vector3d Pos;
	public Vector3d Dir = new(0, 0, 0);

	public byte type;
	public double Speed = 2;
	private int Health_;
	public int Health {
		get => Health_;
		set => Health_ = value>100 ? 100 : value<-1 ? -1 : value;
	}

	public const int Radius = 10;
	public Int64 PlayerUUID = 0;
	public bool Visible;

	private static int[] CalcCircleOffsets() {

		int[] res = new int[FLOAT_COUNT];
		res[0] = 0;
		res[1] = 0;
		res[2] = 0;
		res[3] = 0;
		res[4] = SIZE;
		res[5] = 0;
		res[6] = (int)(Math.Sin(Math.PI*2/SIDES_COUNT*1)*SIZE);
		res[7] = (int)(Math.Cos(Math.PI*2/SIDES_COUNT*1)*SIZE);
		res[8] = 0;

		for (int i = 9; i<SIDES_COUNT*9; i+=9) {
			res[i] = res[0];
			res[i+1] = res[1];
			res[i+2] = 0;
			res[i+3] = res[i-3];
			res[i+4] = res[i-2];
			res[i+5] = 0;
			res[i+6] = (int)(Math.Sin(Math.PI*2/(SIDES_COUNT-1)*i/9)*SIZE);
			res[i+7] = (int)(Math.Cos(Math.PI*2/(SIDES_COUNT-1)*i/9)*SIZE);
			res[i+8] = 0;
		}

		return res;
	}

	public Player(Vector3d? newPos, int newHealth, Int64 UUID):base(FLOAT_COUNT) {
		Pos = newPos??new Vector3d(0, 0, 0);
		dirty = true;
		Health_ = newHealth;
		PlayerUUID = UUID!=0 ? UUID : new Random().Next();
		Visible=Health_ !=-1;
	}

	//the constructor for invalid players
	public Player():base(FLOAT_COUNT) {
		Pos = new(0, 0, 0);
		//if the health of a player is -1 it is considered invalid and won't be processed
		Health_ = -1;
		PlayerUUID = 0;
		Visible = false;
	}

	public override string ToString() => $"Game.graphics.client.Player2[health:{Health}, speed:{Speed}, pos:{Pos}, dir:{Dir}, UUID:{PlayerUUID}, VAO:{vaoHandle}, VBO:{vboHandle}]";

	public unsafe override void UpdateVertices() {
		//vertices ??= new float[FLOAT_COUNT];
		float* ptr = VertexDataPtr;
		for (int i=0; i<FLOAT_COUNT; i+=3) {
			*ptr=(int)Pos.x+CIRCLE_OFFSETS[i];
			ptr++;
			*ptr=(int)Pos.y+CIRCLE_OFFSETS[i+1];
			ptr++;
			*ptr=0;
			ptr++;
		}
	}

	public unsafe void Move() {
		Pos.Add(Dir.Cpy().Nor().Scl(Speed));
		dirty = true;
	}

	public void Damage(int damage) {
		if (damage>Health)
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

	public void OnKeyEvent(Client c) {
		if (c.keyUp) {
			if (c.keyLeft) {
				if (c.keyDown) {
					if (c.keyRight) {
						Dir.x=0; //wasd
						Dir.y=0;
					} else {
						Dir.x=-1; //was
						Dir.y=0;
					}
				} else {
					if (c.keyRight) {
						Dir.x=0; //wad
						Dir.y=1;
					} else {
						Dir.x=(-1)/Math.Sqrt(2); //wa
						Dir.y=1/Math.Sqrt(2);
					}
				}
			} else {
				if (c.keyDown) {
					if (c.keyRight) {
						Dir.x=1; //wsd
						Dir.y=0;
					} else {
						Dir.x=0; //ws
						Dir.y=0;
					}
				} else {
					if (c.keyRight) {
						Dir.x=1/Math.Sqrt(2); //wd
						Dir.y=(1)/Math.Sqrt(2);
					} else {
						Dir.x=0; //w
						Dir.y=1;
					}
				}
			}
		} else {
			if (c.keyLeft) {
				if (c.keyDown) {
					if (c.keyRight) {
						Dir.x=0; //asd
						Dir.y=-1;
					} else {
						Dir.x=(-1)/Math.Sqrt(2); //as
						Dir.y=(-1)/Math.Sqrt(2);
					}
				} else {
					if (c.keyRight) {
						Dir.x=0; //ad
						Dir.y=0;
					} else {
						Dir.x=(-1); //a
						Dir.y=0;
					}
				}
			} else {
				if (c.keyDown) {
					if (c.keyRight) {
						Dir.x=1/Math.Sqrt(2); //sd
						Dir.y=(-1)/Math.Sqrt(2);
					} else {
						Dir.x=0; //s
						Dir.y=-1;
					}
				} else {
					if (c.keyRight) {
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

	public static unsafe void SerializePlayer(byte* buffer, Player player, int offset) {
		byte* ptr = buffer;
		ptr+=offset;
		if (player==null) {
			Unsafe.Write(ptr, -1);
		} else {
			Unsafe.Write(ptr, player.Health);
			ptr += 4;
			Unsafe.Write(ptr, player.Pos.x);
			ptr += 8;
			Unsafe.Write(ptr, player.Pos.y);
			ptr += 8;
			Unsafe.Write(ptr, player.Dir.x);
			ptr += 8;
			Unsafe.Write(ptr, player.Dir.y);
			ptr += 8;
			Unsafe.Write(ptr, player.Speed);
			ptr += 4;
			Unsafe.Write(ptr, player.PlayerUUID);
		}
	}

	/// <summary>
	/// reads the next 17 bytes after the offset from a buffer and modifies a player 
	/// dependent on the values of te buffer.
	/// byte 1 to 4 are the health of the player,
	/// byte 5 to 13 are converted to an int and are set as the new x position of the player
	/// byte 6 to 9 are converted to an int and are set as the new y position of the player
	/// byte 10 to 13 are converted to an int and are set as the new width of the player
	/// byte 10 to 13 are converted to an int and are set as the new height of the player
	/// byte 10 to 13 are converted to an int and are set as the new height of the player
	/// </summary>
	public static unsafe void DeserializePlayer(byte* buffer, Player player, int offset) {
		//Console.WriteLine("Deserializing"+player.ToString());
		byte* ptr = buffer;
		ptr+=offset;
		//Console.WriteLine("player was:"+player.ToString());
		player ??= new Player(null, 0, 0);
		player.Health = Unsafe.Read<Int32>(ptr);
		if (player.Health_ ==-1) {
			player.Deactivate();
		} else {
			ptr += 4;
			player.Pos.x = Unsafe.Read<Int32>(ptr);
			ptr += 8;
			player.Pos.y = Unsafe.Read<Int32>(ptr);
			ptr += 8;
			player.Dir.x = Unsafe.Read<Int32>(ptr);
			ptr += 8;
			player.Dir.y = Unsafe.Read<Int32>(ptr);
			ptr += 8;
			player.Speed = Unsafe.Read<Int32>(ptr);
			ptr += 4;
			player.PlayerUUID = Unsafe.Read<Int64>(ptr);
			player.dirty=true;
		}
	}
}
