namespace ShGame.Game.Client;

using ShGame.Game.Client.Rendering;
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public class Obstacle:Drawable, ISupportsShadow {

	public const int OBSTACLE_BYTE_LENGTH = 17;
	public int WIDTH, HEIGHT;
	public byte type;

	public readonly LineSection3d boundL, boundT, boundR, boundB;
	public Vector3d Pos;

	public Shadow? shadow;

	private Client? client;

	public Obstacle(Client? client_, Vector3d? pos_, byte type_):base(18) {
		client = client_;
		shadow = new Shadow(this);
		Pos = pos_??new Vector3d(0, 0, 0);
		type = type_;
		switch (type) {
			case 1:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 150;
				HEIGHT = 150;
				break;
			case 2:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 150;
				HEIGHT = 75;
				break;
			case 3:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 75;
				HEIGHT = 150;
				break;
			default:
				//logger.error("illegal type", new MessageParameter("type", type));
				WIDTH = 0;
				HEIGHT = 0;
				break;
		}
		boundL = new LineSection3d(Pos, Pos.Cpy().Add(0, HEIGHT, 0));
		boundT = new LineSection3d(Pos, Pos.Cpy().Add(WIDTH, 0, 0));
		boundB = new LineSection3d(boundL.point2, boundL.point2.Cpy().Add(WIDTH, 0, 0));
		boundR = new LineSection3d(boundT.point2, boundB.point2);
		//Console.WriteLine(vertices);
	}

	public override void Dispose() {
		GC.SuppressFinalize(this);
		base.Dispose();
		shadow?.Dispose();
	}

	public unsafe override void UpdateVertices() {
		float* ptr = VertexDataPtr;
		*ptr=(float)Pos.x;
		ptr++;
        *ptr=(float)Pos.y;
        ptr++;
        *ptr=0;
        ptr++;
        *ptr=(float)Pos.x+WIDTH;
        ptr++;
        *ptr=(float)Pos.y;
        ptr++;
        *ptr=0;
        ptr++;
        *ptr=(float)Pos.x+WIDTH;
        ptr++;
        *ptr=(float)Pos.y+HEIGHT;
        ptr++;
        *ptr=0;
        ptr++;
        *ptr=(float)Pos.x;
        ptr++;
        *ptr=(float)Pos.y;
        ptr++;
        *ptr=0;
        ptr++;
        *ptr=(float)Pos.x;
        ptr++;
        *ptr=(float)Pos.y+HEIGHT;
        ptr++;
        *ptr=0;
        ptr++;
        *ptr=(float)Pos.x+WIDTH;
        ptr++;
        *ptr=(float)Pos.y+HEIGHT;
        ptr++;
        *ptr=0;
	}

	/// <summary>
	/// updates the bound objects of an obstacle to match its width and height.
	/// </summary>
	private static unsafe void UpdateBounds(Obstacle obstacle) {
		obstacle.WIDTH = obstacle.type switch {
			1 => 35,
			2 => 70,
			3 => 70,
			_ => 0,
		};
		obstacle.HEIGHT = obstacle.type switch {
			1 => 70,
			2 => 35,
			3 => 70,
			_ => 0,
		};
        obstacle.boundL.point1.Set(obstacle.Pos.x, obstacle.Pos.y, 0); //bottom left corner
        obstacle.boundL.point2.Set(obstacle.Pos.x, obstacle.Pos.y+obstacle.HEIGHT, 0);//top left corner
        obstacle.boundR.point1.Set(obstacle.Pos.x+obstacle.WIDTH, obstacle.Pos.y, 0);//bottom right corner
        obstacle.boundR.point2.Set(obstacle.Pos.x+obstacle.WIDTH, obstacle.Pos.y+obstacle.HEIGHT, 0);//top right corner
        obstacle.boundT.point1.Set(obstacle.boundL.point1);//bottom left corner
        obstacle.boundT.point2.Set(obstacle.boundR.point1);//bottom right corner
        obstacle.boundB.point1.Set(obstacle.boundL.point2);//top left corner
        obstacle.boundB.point2.Set(obstacle.boundR.point2);//top right corner
    }

	public static unsafe void SerializeObstacle(byte* buffer, Obstacle obstacle, int offset) {
        byte* ptr = buffer;
		ptr+=offset;
		if (obstacle==null) {
			Unsafe.Write(ptr, 0);
		} else {
            Unsafe.Write(ptr, obstacle.type);
			ptr += 1;
            Unsafe.Write(ptr, (int)obstacle.Pos.x);            
			ptr += 4;
			Unsafe.Write(ptr, (int)obstacle.Pos.y);
            ptr += 4;
            Unsafe.Write(ptr, obstacle.WIDTH);
            ptr += 4;
            Unsafe.Write(ptr, obstacle.HEIGHT);
        }
        obstacle.dirty = true;
    }

    /// <summary>
    /// reads the next 17 bytes after the offset from a buffer.
	/// byte 1 is the type of the obstackle,
    /// byte 2 to 5 are converted to an int and are set as the new x position of the obstacle
	/// byte 6 to 9 are converted to an int and are set as the new y position of the obstacle
    /// byte 10 to 13 are converted to an int and are set as the new width of the obstacle
    /// byte 10 to 13 are converted to an int and are set as the new height of the obstacle
    /// </summary>
    public static unsafe void DeserializeObstacle(Client? client_, byte* buffer, Obstacle obstacle, int offset) {
		Console.WriteLine("Deserializing"+obstacle.ToString());
		byte* ptr = buffer;
        obstacle ??= new Obstacle(client_, null, 0);
		obstacle.type = *buffer;
		if (obstacle.type ==0) {
			return;
		} else {
            buffer += 1;//change to 1 because of type len = 1
            obstacle.Pos.x = Unsafe.Read<Int32>(buffer);
            buffer += 4;
            obstacle.Pos.y = Unsafe.Read<Int32>(buffer);
            buffer += 4;
            obstacle.WIDTH = Unsafe.Read<Int32>(buffer);
            buffer += 4;
            obstacle.HEIGHT = Unsafe.Read<Int32>(buffer);
            UpdateBounds(obstacle);
		}
		obstacle.dirty = true;
	}

	private unsafe int RelativeX(Vector3d* v) {
		if (v->x<Pos.x)
			return 1;
		else if (v->x>Pos.x+WIDTH)
			return 3;
		else
			return 2;
	}

	private unsafe int RelativeY(Vector3d* v) {
        if (v->y<Pos.y)
            return 1;
        else if (v->y>Pos.y+HEIGHT)
            return 3;
        else
            return 2;
    }

    public Vector3d GetPointOfView() => client!=null ? client.player.Pos.Cpy().Sub(Player.SIZE/2.0, Player.SIZE/2.0, 0.0):new Vector3d(0,0,0);

	public Vector3d GetRelativeVector() =>
		Pos.Cpy().Add(new Vector3d(WIDTH/2, HEIGHT/2, 0)).Sub(GetPointOfView()).Nor();

	public unsafe void GetShadowOrigins(out Vector3d point1, out Vector3d point2, out Dir dir) {
		point1 = new(0, 0, 0);
		point2 = new(0, 0, 0);
		dir=Dir.B;
		if (client==null) 
			return;
		fixed (Vector3d* pos = &client.player.Pos) {
			//relative is to the left and to the top
			if (RelativeX(pos)==1&&RelativeY(pos)==1) {
				point1.x=boundR.point1.x;
				point1.y=boundR.point1.y;
				point2.x=boundL.point2.x;
				point2.y=boundL.point2.y;
				dir = Dir.T;
			//relative is in the middle on x and to the top
			} else if (RelativeX(pos)==2&&RelativeY(pos)==1) {
				point1.x=boundL.point1.x;
				point1.y=boundL.point1.y;
				point2.x=boundR.point1.x;
				point2.y=boundR.point1.y;
				dir = Dir.T;
            } else if (RelativeX(pos)==3&&RelativeY(pos)==1) {
				point1.x=boundL.point1.x;
				point1.y=boundL.point1.y;
				point2.x=boundR.point2.x;
				point2.y=boundR.point2.y;
				dir = Dir.T;
            } else if (RelativeX(pos)==1&&RelativeY(pos)==2) {
				point1.x=boundL.point1.x;
				point1.y=boundL.point1.y;
				point2.x=boundL.point2.x;
				point2.y=boundL.point2.y;
				dir = Dir.L;
            } else if (RelativeX(pos)==3&&RelativeY(pos)==2) {
				point1.x=boundR.point1.x;
				point1.y=boundR.point1.y;
				point2.x=boundR.point2.x;
				point2.y=boundR.point2.y;
                dir = Dir.R;
            } else if (RelativeX(pos)==1&&RelativeY(pos)==3) {
				point1.x=boundL.point1.x;
				point1.y=boundL.point1.y;
				point2.x=boundR.point2.x;
				point2.y=boundR.point2.y;
                dir = Dir.B;
            } else if (RelativeX(pos)==2&&RelativeY(pos)==3) {
				point1.x=boundL.point2.x;
				point1.y=boundL.point2.y;
				point2.x=boundR.point2.x;
				point2.y=boundR.point2.y;
				dir = Dir.B;
			} else if (RelativeX(pos)==3&&RelativeY(pos)==3) {
				point1.x=boundR.point1.x;
				point1.y=boundR.point1.y;
				point2.x=boundL.point2.x;
				point2.y=boundL.point2.y;
                dir = Dir.B;
            }
        }
	}

	public override string ToString() => "ShGame.Game.Client.Obstacle[Pos:"+Pos.ToString()+", Type:"+Convert.ToString(type)+"]";
}