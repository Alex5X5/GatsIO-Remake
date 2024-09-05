namespace ShGame.game.Net;

public static class Protocoll {

	public const int PING_HEADER = 1, PLAYER_HEADER = 2, MAP_HEADER = 3;
	public const int PACKET_BYTE_LENGTH = 1000, PAYLOAD_OFFSET = 5;

	public static int AnalyzePacket(byte[] packet) {
		return BitConverter.ToInt32(packet, 0);
	}

	public static byte[] PreparePacket(int typeID) {
		byte[] packet = new byte[PACKET_BYTE_LENGTH];
		BitConverter.GetBytes(typeID).CopyTo(packet, 0);
		return packet;
	}

	public static byte[] LoadPing(bool answer) {
		byte[] result = PreparePacket(PING_HEADER);
		BitConverter.GetBytes(answer).CopyTo(result, 5);
		return result;
	}

	public static bool UnloadPing(byte[] packet) {
		if(AnalyzePacket(packet)==1) {
			return BitConverter.ToBoolean(packet, 5);
		} else {
			return false;
		}
	}
}

//[Serializable]
//public enum ProtocollType {
//	Ping,
//	Player,
//	Map
//}
