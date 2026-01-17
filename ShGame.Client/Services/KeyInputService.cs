using Silk.NET.Input;
using Silk.NET.Windowing;

using System.Numerics;

namespace ShGame.Client.Services;

public class KeyInputService {

	
	public KeyInputService() {
			
	}


	private void KeyUp_(IKeyboard keyboard, Key key, int keyCode) {
		//logger.Log("key "+key+" up");
		switch (key) {
			case Key.W:
				keyUp=false;
				break;
			case Key.S:
				keyDown=false;
				break;
			case Key.A:
				keyLeft=false;
				break;
			case Key.D:
				keyRight=false;
				break;
		}
		if (ControlledPlayer!=null)
			ControlledPlayer.OnKeyEvent(this);
		//Console.WriteLine("key up, p:"+player.ToString());
	}

	private void KeyDown_(IKeyboard keyboard, Key key, int keyCode) {
		//logger.Log("key "+key+" down");
		switch (key) {
			case Key.W:
				keyUp=true;
				break;
			case Key.S:
				keyDown=true;
				break;
			case Key.A:
				keyLeft=true;
				break;
			case Key.D:
				keyRight=true;
				break;
			case Key.Escape:
				Stop();
				break;
		}
		if (ControlledPlayer!=null)
			ControlledPlayer.OnKeyEvent(this);
		//Console.WriteLine("key up, p:"+player.ToString());
	}

	private void OnMouseDown(IMouse cursor, MouseButton button) {
		Console.WriteLine("Mouse Down! "+mousePos);
		if (button==MouseButton.Left) {
			mouseLeftDown=true;
			if (ControlledPlayer!=null)
				ControlledPlayer.IsShooting = 0x1;
		}
		if (button==MouseButton.Right)
			mouseRightDown=true;
	}

	private void OnMouseUp(IMouse cursor, MouseButton button) {
		Console.WriteLine("Mouse Up! "+mousePos);
		if (button==MouseButton.Left) {
			mouseLeftDown=false;
			if (ControlledPlayer!=null)
				ControlledPlayer.IsShooting = 0x0;
		}
		if (button==MouseButton.Right) {
			mouseRightDown=false;
		}
	}

	private void OnMouseMove(IMouse cursor, Vector2 pos) {
		mousePos.X = pos.X*(Constants.MAP_GRID_WIDTH/window.Size.X);
		mousePos.Y = Constants.MAP_GRID_HEIGHT-pos.Y*(Constants.MAP_GRID_HEIGHT/window.Size.Y);
		//mousePos = pos-new Vector2(window.Position.X,window.Size.Y-window.Position.Y);
		//Console.WriteLine("I Moved! "+mousePos);
	}
}
