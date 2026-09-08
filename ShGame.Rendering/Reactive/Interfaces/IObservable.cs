namespace ShGame.Rendering.Reactive.Interfaces;

public interface IObservable {

	event ChangedEventHandler Changed;

	void NotifyChanged(string property);

}

public record class ChangedEventArgs(string property);

public delegate void ChangedEventHandler(ChangedEventArgs e);
