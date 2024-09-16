namespace ShGame.game.Net;

public static class Protocoll {

	public const int PING_HEADER = 1, PLAYER_HEADER = 2, MAP_HEADER = 3;
	public const int PACKET_BYTE_LENGTH = 1000, PAYLOAD_OFFSET = 1;

	public static ProtocollType AnalyzePacket(byte[] packet) {
		return (ProtocollType)packet[0];
	}

	public static byte[] PreparePacket(ProtocollType typeID) {
		byte[] packet = new byte[PACKET_BYTE_LENGTH];
		packet[0] = (byte)typeID;
		return packet;
	}

	public static byte[] LoadPing(bool answer) {
		byte[] result = PreparePacket(ProtocollType.Ping);
		BitConverter.GetBytes(answer).CopyTo(result, 5);
		return result;
	}

	public static bool UnloadPing(byte[] packet) {
		if(AnalyzePacket(packet)==ProtocollType.Ping) {
			return BitConverter.ToBoolean(packet, 5);
		} else {
			return false;
		}
	}
}

public enum ProtocollType:byte{
	Ping,
	Player,
	Map
}
