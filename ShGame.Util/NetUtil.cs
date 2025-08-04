namespace ShGame;

using System.Net.NetworkInformation;
using System.Net;




public static class NetUtil {

	public static IPAddress GetLocalIPv4() =>
		NetworkInterface.GetAllNetworkInterfaces()[0].
			GetIPProperties().UnicastAddresses[^1].
				Address;

	public static IPAddress GetLocalIPv6() =>
		NetworkInterface.GetAllNetworkInterfaces()[0].
			GetIPProperties().UnicastAddresses[^1].
				Address;

	public static IPAddress GetLocalIP() =>
		NetworkInterface.GetAllNetworkInterfaces()[1].
			GetIPProperties().UnicastAddresses[^1].
				Address;
}
