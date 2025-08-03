namespace ShGame.Game.Net;

internal class ConnectException:Exception {

	public ConnectException():base() {}

	public ConnectException(string message):base(message) {}	
}
