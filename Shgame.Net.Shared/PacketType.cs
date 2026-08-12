namespace ShGame.Net.Shared;

public enum PacketType : byte {
	Ping = 1,
	AbortConnection = 2,
	PlayerLimit = 3,
	Map = 10,
	Player = 11,
	Register = 12,
	Ability = 13,
	Bullets = 14,
	Invalid = 255
}