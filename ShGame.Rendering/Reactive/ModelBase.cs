using ShGame.Rendering.Reactive.Interfaces;

namespace ShGame.Rendering.Reactive;

public class ModelBase : IObservable {
	
	public event ChangedEventHandler Changed;

	public ModelBase() {
		Changed = (arg) => { };
	}

	public void NotifyChanged(string property) {
		Changed.Invoke(new ChangedEventArgs(property));
	}
}
