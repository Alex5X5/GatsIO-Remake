namespace ShGame.Net;

public static class Protocoll {

	public const int PACKET_BYTE_LENGTH = 2000;
	public const int TYPE_OFFSET = 0;
	public const int PAGE_OFFSET = 1;
	public const int PAYLOAD_OFFSET = 2;

	public static byte AnalyzePacket(byte[] packet) => packet[0];

	public static byte[] PreparePacket(byte typeID) {
		byte[] packet = new byte[PACKET_BYTE_LENGTH];
		packet[0] = typeID;
		return packet;
	}
}
