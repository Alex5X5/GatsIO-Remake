namespace ShGame.Game;

public interface IKeySupplier {

	public bool keyUp { set; get; }
	public bool keyDown { set; get; }
	public bool keyLeft { set; get; }
	public bool keyRight { set; get; }
}
