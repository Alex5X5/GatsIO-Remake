namespace ShGame.Net.Shared;

public enum PacketType : byte {
	Ping = 1,
	AbortConnection = 2,
	PlayerLimit = 3,
	Map = 10,
	UpdatePlayer = 11,
	GetPlayers = 12,
	Register = 13,
	Ability = 14,
	Bullets = 15,
	Empty = 254,
	Invalid = 255
}