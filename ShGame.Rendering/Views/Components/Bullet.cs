namespace ShGame.Rendering.Views.Components;

using ShGame.Math;

public class Bullet : Componentbase {

    public const int SIZE = 20, SIDES_COUNT = 50, FLOAT_COUNT = 9*SIDES_COUNT;

    public Bullet() : base(FLOAT_COUNT) {

	}

	public override string ToString() => "ShGame.Rendering.Views.Components.Bullet[Pos:"+Pos.ToString()+", Dir:"+Dir.ToString()+"]";
}