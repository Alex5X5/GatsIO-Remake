namespace ShGame.Rendering.Views.Components;

using ShGame.Game.Models;
using ShGame.Math;

public class BulletView : Componentbase<BulletViewModel> {

    public const int SIZE = 20, SIDES_COUNT = 50, FLOAT_COUNT = 9*SIDES_COUNT;

    public BulletView(BulletViewModel viewModel) : base(viewModel, FLOAT_COUNT) {

	}

	public override string ToString() => "ShGame.Rendering.Views.Components.Bullet[Pos:"+new Vector3d(1, 1, 1).ToString()+", Dir:"+Dir.T.ToString()+"]";
}