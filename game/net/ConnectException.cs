namespace ShGame.game.Net;

internal class ConnectException:Exception {

	public ConnectException():base() {}

	public ConnectException(string message):base(message) {}	
}
