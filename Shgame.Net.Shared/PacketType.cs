namespace ShGame.Net.Shared;

public enum PacketType : byte {
	Invalid = 0,
	Ping = 1,
	Time = 2,
	AbortConnection = 3,
	PlayerLimit = 4,
	Map = 10,
	UpdatePlayer = 11,
	GetPlayers = 12,
	Register = 13,
	Ability = 14,
	Bullets = 15,
	Empty = 254
}