namespace ShGame.Rendering.Models;

using ShGame.Rendering.Reactive.Interfaces;

internal class BulletModel : IObservable {
	public event ChangedEventHandler Changed;

	public void NotifyChanged(string property) {
		throw new System.NotImplementedException();
	}
}
