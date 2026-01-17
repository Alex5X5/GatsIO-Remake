using ShGame.Math;

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ShGame.Util.Services;

public class ConfigurationService {

	public bool NoGUI { get; private set; }

	public bool StartClient { get; private set; }
	public bool StartServer { get; private set; }

	public IPAddress Address { get; private set; }

	public ConfigurationService(string[] args) {
		List<string> args_ = args.ToList();
		NoGUI = args_.Contains("-nogui");
		StartClient = args_.Contains("--client");
		StartServer = args_.Contains("--server");

		IPAddress? address = null;
		try {
			address = IPAddress.Parse(args_.Contains("-ip") ? args_[args_.IndexOf("-ip")+1] : "");
		} catch {
			address = NetUtil.GetLocalIP().MapToIPv4();
		}
		Address = address;
		

		int port = 1;
		try {
			port = args_.Contains("-port") ? Convert.ToInt32(args_[args_.IndexOf("-port")+1]) : 5000;
		} catch {
			port = 5000;
		}
	}
}
