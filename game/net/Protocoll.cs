namespace ShGame.game.Net;

public static class Protocoll {

	public const byte PING_HEADER = 1, PLAYER_HEADER = 11, MAP_HEADER = 3;
	public const short PACKET_BYTE_LENGTH = 1000, PAYLOAD_OFFSET = 1;
	

	public static byte AnalyzePacket(byte[] packet) {
		return packet[0];
	}

	public static byte[] PreparePacket(byte typeID) {
		byte[] packet = new byte[PACKET_BYTE_LENGTH];
		packet[0] = typeID;
		return packet;
	}

	public static int UnloadMessage(byte[] packet) {
		byte[] temp = new byte[8];
		Array.ConstrainedCopy(packet, 0, temp, 0, 8);
		return BitConverter.ToInt32(temp);
	}

	public static byte[] LoadPing(bool answer) {
		byte[] result = PreparePacket(Headers.PING);
		BitConverter.GetBytes(answer).CopyTo(result, 5);
		return result;
	}

	public static bool UnloadPing(byte[] packet) =>
		AnalyzePacket(packet)==Headers.PING;
}

public class Headers {
	public const byte PING = 1;
	public const byte ABORT_CONNECTION = 2;
	public const byte MAP = 5;
	public const byte PLAYER = 6;
	public const byte ABILITY = 7;
	public const byte PAYER_LIMIT = 11;
}
