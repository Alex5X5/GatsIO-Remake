namespace ShGame.Rendering.Views.Components;

using ShGame.Drawing.Models;
using ShGame.Game.Models;
using ShGame.Math;

public class BulletView : Drawable<BulletViewModel> {

    public const int SIZE = 20, SIDES_COUNT = 50, FLOAT_COUNT = 9*SIDES_COUNT;

    public BulletView(BulletViewModel viewModel) : base(viewModel, FLOAT_COUNT) {

	}

	public override string ToString() => "ShGame.Rendering.Views.Components.Bullet[Pos:"+new Vector3d(1, 1, 1).ToString()+", Dir:"+Dir.T.ToString()+"]";

	public unsafe override void UpdateVertices() {
		float* ptr = VertexDataPtr;
		*ptr=(float)viewModel.Pos.x;
		ptr++;
		*ptr=(float)viewModel.Pos.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)viewModel.Pos.x+viewModel.Width;
		ptr++;
		*ptr=(float)viewModel.Pos.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)viewModel.Pos.x+viewModel.Width;
		ptr++;
		*ptr=(float)viewModel.Pos.y+viewModel.Length;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)viewModel.Pos.x;
		ptr++;
		*ptr=(float)viewModel.Pos.y;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)viewModel.Pos.x;
		ptr++;
		*ptr=(float)viewModel.Pos.y+viewModel.Length;
		ptr++;
		*ptr=0;
		ptr++;
		*ptr=(float)viewModel.Pos.x+viewModel.Width;
		ptr++;
		*ptr=(float)viewModel.Pos.y+viewModel.Length;
	}
}