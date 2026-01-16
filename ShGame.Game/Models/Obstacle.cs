namespace ShGame.Game.Models;

using System;
using System.Runtime.CompilerServices;
using ShGame.Math;

public class Obstacle {

	public const int OBSTACLE_BYTE_LENGTH = 17;
	public int WIDTH, HEIGHT;
	public byte type;

	public readonly LineSection3d boundL, boundT, boundR, boundB;
	public Vector3d Pos;

	public Obstacle() : this(null, 0){
	
	}

	public Obstacle(Vector3d? pos_, byte type_) {
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
        obstacle.boundR.point1.Set(obstacle.Pos.x + obstacle.WIDTH, obstacle.Pos.y, 0);//bottom right corner
        obstacle.boundR.point2.Set(obstacle.Pos.x + obstacle.WIDTH, obstacle.Pos.y+obstacle.HEIGHT, 0);//top right corner
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
    }

    /// <summary>
    /// reads the next 17 bytes after the offset from a buffer.
	/// byte 1 is the type of the obstackle,
    /// byte 2 to 5 are converted to an int and are set as the new x position of the obstacle
	/// byte 6 to 9 are converted to an int and are set as the new y position of the obstacle
    /// byte 10 to 13 are converted to an int and are set as the new width of the obstacle
    /// byte 10 to 13 are converted to an int and are set as the new height of the obstacle
    /// </summary>
    public static unsafe void DeserializeObstacle(byte* buffer, ref Obstacle obstacle, int offset) {
		Console.WriteLine("Deserializing"+obstacle.ToString());
		byte* ptr = buffer;
        obstacle ??= new Obstacle(null, 0);
		obstacle.type = *buffer;
		if (obstacle.type ==0) {
			return;
		} else {
            buffer += 1;//change to 1 because of type len = 1
            obstacle.Pos.x = Unsafe.Read<int>(buffer);
            buffer += 4;
            obstacle.Pos.y = Unsafe.Read<int>(buffer);
            buffer += 4;
            obstacle.WIDTH = Unsafe.Read<int>(buffer);
            buffer += 4;
            obstacle.HEIGHT = Unsafe.Read<int>(buffer);
            UpdateBounds(obstacle);
		}
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

	public override string ToString() => "ShGame.Game.Client.Obstacle[Pos:"+Pos.ToString()+", Type:"+Convert.ToString(type)+"]";
}