namespace ShGame.Net.Shared.Exceptions;

internal class ConnectException:Exception {

	public ConnectException():base() {}

	public ConnectException(string message):base(message) {}	
}
