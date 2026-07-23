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
	public int Port { get; private set; }

	public ConfigurationService(string[] args) {
		List<string> args_ = args.ToList();
		NoGUI = args_.Contains("-nogui");

		StartClient = args_.Contains("--client");
		StartServer = args_.Contains("--server");

		try {
			if(args_.Contains("-ip"))
				Address = IPAddress.Parse(args_[args_.IndexOf("-ip")+1]);
			Address = NetUtil.GetLocalIP().MapToIPv4();
		} catch {
			Address = NetUtil.GetLocalIP().MapToIPv4();
		}

		try {
			if(args_.Contains("-port"))
				Port = Convert.ToInt32(args_[args_.IndexOf("-port")+1]);
		} catch {
			Port = 5000;
		}
	}
}
