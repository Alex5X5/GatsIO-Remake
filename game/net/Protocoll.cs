namespace ShGame.game.Net;

public static class Protocoll {

	public const int PING_HEADER = 1, PLAYER_HEADER = 11, MAP_HEADER = 3;
	public const int PACKET_BYTE_LENGTH = 1000, PAYLOAD_OFFSET = 1;

	public static ProtocollType AnalyzePacket(byte[] packet) {
		return (ProtocollType)packet[0];
	}

	public static byte[] PreparePacket(ProtocollType typeID) {
		byte[] packet = new byte[PACKET_BYTE_LENGTH];
		packet[0] = (byte)typeID;
		return packet;
	}

	public static int UnloadMessage(byte[] packet) {
		byte[] temp = new byte[8];
		Array.ConstrainedCopy(packet, 0, temp, 0, 8);
		
		return 0;
	}

	public static byte[] LoadPing(bool answer) {
		byte[] result = PreparePacket(ProtocollType.Communication);
		BitConverter.GetBytes(answer).CopyTo(result, 5);
		return result;
	}

	public static bool UnloadPing(byte[] packet) {
		if(AnalyzePacket(packet)==ProtocollType.Communication) {
			return BitConverter.ToBoolean(packet, 5);
		} else {
			return false;
		}
	}
}

public enum ProtocollType:byte{
	Communication,
	Player,
	Map
}

public static class Messages {
	public const byte ping = 1;
	public const byte abortConnection = 10;
    public const byte playerLimitreached = 11;
}

public static class Headers {
    public const byte ping = 1;
	public const byte PLAYER = 11;
	public const byte MAP = 12;
	public const byte ABILITY = 13;
    public const byte abortConnection = 10;
    public const byte PAYER_LIMIT_REACHED = 11;
}
