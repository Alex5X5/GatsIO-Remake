using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sh_game.game.server {
	internal class ConnectException:Exception{
	
		public ConnectException():base() {}

		public ConnectException(string message):base(message) {}	
	}
}
