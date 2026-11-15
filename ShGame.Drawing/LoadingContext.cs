namespace ShGame.Drawing;

using Silk.NET.OpenGL;

public class LoadingContext {
	
	private GL Gl;

	public LoadingContext(GL gl) {
		Gl = gl;
	}

	public Image LoadImage(string path) {
		var image = new Image(path);

		image.Load(Gl);

		return image;
	}
}
