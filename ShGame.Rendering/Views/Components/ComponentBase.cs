namespace ShGame.Rendering.Views.Components;

using ShGame.Drawing.Models;

public abstract class Componentbase<ModelT> : Drawable<ModelT> {

	public Componentbase(ModelT viewModel, uint verticesCount) : base(viewModel, verticesCount) {
		
	}

}
