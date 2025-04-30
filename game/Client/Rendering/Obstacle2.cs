namespace ShGame.game.Client.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Obstacle2:Drawable {
	
	private Vector3d Pos_;
	public Vector3d Pos {get => Pos_; set => SetPos(value);}
	public int WIDTH, HEIGHT;

	public byte type;

	private void SetPos(Vector3d value) {
		Console.WriteLine("set!");
		Pos_ = value;
		vertices[0] = (float)value.x;
		vertices[1] = (float)value.y;
		vertices[2] = 0f;
		vertices[3] = (float)value.x+WIDTH;
		vertices[4] = (float)value.y;
		vertices[5] = 0f;
		vertices[6] = (float)value.x+WIDTH;
		vertices[7] = (float)value.y+HEIGHT;
		vertices[8] = 0f;
		vertices[9] = (float)value.x;
		vertices[10] = (float)value.y;
		vertices[11] = 0f;
		vertices[12] = (float)value.x;
		vertices[13] = (float)value.y+HEIGHT;
		vertices[14] = 0f;
		vertices[15] = (float)value.x+WIDTH;
		vertices[16] = (float)value.y+HEIGHT;
		vertices[17] = 0f;
		Console.WriteLine("data:");
		for (int i = 0; i<17; i++) {
			Console.WriteLine(vertices[i]);
		}
			BindVBO();
			Console.WriteLine("gl is not null:");
			unsafe {
			fixed (float* ptr = &vertices[0]) ;
					//Drawable.BufferTriangles(ptr, 18, RendererGl.Gl);
			}
			UnbindVBO();
	}

	public Obstacle2(Vector3d? pos__, byte type_):base() {
		//      Pos = pos_??new Vector3d(0, 0, 0);
		type = type_;
		switch (type) {
			case 1:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 100;
				HEIGHT = 300;
				break;
			case 2:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 70;
				HEIGHT = 35;
				break;
			case 3:
				//logger.log("setting bounds", new MessageParameter("type", type));
				WIDTH = 70;
				HEIGHT = 70;
				break;
			default:
				//logger.error("illegal type", new MessageParameter("type", type));
				WIDTH = 0;
				HEIGHT = 0;
				break;
		}

		FLOAT_COUNT = 18;
		Console.WriteLine(vertices);
		vertices = new float[FLOAT_COUNT];
		Pos = pos__??new Vector3d(0, 0, 0);
		Console.WriteLine(vertices);
	}
}