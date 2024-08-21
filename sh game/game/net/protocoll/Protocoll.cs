using sh_game.game.client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace sh_game.game.net.protocoll {

	[Serializable]	
	public class Protocoll{

		public readonly bool answer;
		public readonly ProtocollType type;

		public Protocoll(bool answer, ProtocollType type) {
			this.answer = answer;
			this.type = type;
		}
	}

	[Serializable]
	public class PingProtocoll:Protocoll {

		public PingProtocoll(bool answer) : base(answer, ProtocollType.Ping) {
		}
	}

	[Serializable]
	public class PlayerProtocoll:Protocoll {

		public readonly ParsablePlayer[] players;

		public PlayerProtocoll(bool answer, Player[] players_) : base(answer, ProtocollType.Player) {
			List<ParsablePlayer> list = new List<ParsablePlayer>();
			foreach(Player p in players_)
				list.Add(new ParsablePlayer(p));
			players = list.ToArray();
		}

		public PlayerProtocoll(bool answer, Player player) : base(answer, ProtocollType.Player) {
			players = new ParsablePlayer[] { new ParsablePlayer(player) };
		}
	}

	[Serializable]
	public class MapProtocoll:Protocoll {

		public readonly ParsableObstacle[] obstacles;

		public MapProtocoll(bool answer, ParsableObstacle[] obstacles_) : base(answer, ProtocollType.Map) {
			if(obstacles_!=null) {
				obstacles = obstacles_;
			}
		}

		public MapProtocoll(bool answer, Obstacle[] obstacles_) : base(answer, ProtocollType.Map) {
			if(obstacles_!=null) {
				List<ParsableObstacle> list = new List<ParsableObstacle>();
				foreach(Obstacle o in obstacles_)
					list.Add(new ParsableObstacle(o));
				obstacles = list.ToArray();
			}
		}



		public override string ToString() {
			String s = "";
			if(obstacles!=null)
				foreach(ParsableObstacle o in obstacles) {
					s+=o.ToString();
					s+=",";
				}
			return "sh_game.game.net.protocoll.MapProtocoll("+s+")";
		}
	}

	[Serializable]
	public enum ProtocollType {
		Ping,
		Player,
		Map
	}
}
