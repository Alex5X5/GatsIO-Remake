namespace ShGame.Rendering.Views.Components;

using ShGame.Drawing.Interfaces;
using ShGame.Rendering.ViewModels;

public class ShadowViewModel : ViewModelBase {

    public ISupportsShadow attatch;

    public ShadowViewModel(ISupportsShadow attatch) { 
        this.attatch = attatch;
    }

}