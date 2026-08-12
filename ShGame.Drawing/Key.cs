namespace ShGame.Drawing;

public enum Key {
	//
	// Zusammenfassung:
	//     An unknown key.
	Unknown = -1,
	//
	// Zusammenfassung:
	//     The spacebar key.
	Space = 32,
	//
	// Zusammenfassung:
	//     The apostrophe key.
	Apostrophe = 39,
	//
	// Zusammenfassung:
	//     The comma key.
	Comma = 44,
	//
	// Zusammenfassung:
	//     The minus key.
	Minus = 45,
	//
	// Zusammenfassung:
	//     The period key.
	Period = 46,
	//
	// Zusammenfassung:
	//     The slash key.
	Slash = 47,
	//
	// Zusammenfassung:
	//     The 0 key.
	Number0 = 48,
	//
	// Zusammenfassung:
	//     The 0 key; alias for Silk.NET.Input.Key.Number0
	D0 = 48,
	//
	// Zusammenfassung:
	//     The 1 key.
	Number1 = 49,
	//
	// Zusammenfassung:
	//     The 2 key.
	Number2 = 50,
	//
	// Zusammenfassung:
	//     The 3 key.
	Number3 = 51,
	//
	// Zusammenfassung:
	//     The 4 key.
	Number4 = 52,
	//
	// Zusammenfassung:
	//     The 5 key.
	Number5 = 53,
	//
	// Zusammenfassung:
	//     The 6 key.
	Number6 = 54,
	//
	// Zusammenfassung:
	//     The 7 key.
	Number7 = 55,
	//
	// Zusammenfassung:
	//     The 8 key.
	Number8 = 56,
	//
	// Zusammenfassung:
	//     The 9 key.
	Number9 = 57,
	//
	// Zusammenfassung:
	//     The semicolon key.
	Semicolon = 59,
	//
	// Zusammenfassung:
	//     The equal key.
	Equal = 61,
	//
	// Zusammenfassung:
	//     The A key.
	A = 65,
	//
	// Zusammenfassung:
	//     The B key.
	B = 66,
	//
	// Zusammenfassung:
	//     The C key.
	C = 67,
	//
	// Zusammenfassung:
	//     The D key.
	D = 68,
	//
	// Zusammenfassung:
	//     The E key.
	E = 69,
	//
	// Zusammenfassung:
	//     The F key.
	F = 70,
	//
	// Zusammenfassung:
	//     The G key.
	G = 71,
	//
	// Zusammenfassung:
	//     The H key.
	H = 72,
	//
	// Zusammenfassung:
	//     The I key.
	I = 73,
	//
	// Zusammenfassung:
	//     The J key.
	J = 74,
	//
	// Zusammenfassung:
	//     The K key.
	K = 75,
	//
	// Zusammenfassung:
	//     The L key.
	L = 76,
	//
	// Zusammenfassung:
	//     The M key.
	M = 77,
	//
	// Zusammenfassung:
	//     The N key.
	N = 78,
	//
	// Zusammenfassung:
	//     The O key.
	O = 79,
	//
	// Zusammenfassung:
	//     The P key.
	P = 80,
	//
	// Zusammenfassung:
	//     The Q key.
	Q = 81,
	//
	// Zusammenfassung:
	//     The R key.
	R = 82,
	//
	// Zusammenfassung:
	//     The S key.
	S = 83,
	//
	// Zusammenfassung:
	//     The T key.
	T = 84,
	//
	// Zusammenfassung:
	//     The U key.
	U = 85,
	//
	// Zusammenfassung:
	//     The V key.
	V = 86,
	//
	// Zusammenfassung:
	//     The W key.
	W = 87,
	//
	// Zusammenfassung:
	//     The X key.
	X = 88,
	//
	// Zusammenfassung:
	//     The Y key.
	Y = 89,
	//
	// Zusammenfassung:
	//     The Z key.
	Z = 90,
	//
	// Zusammenfassung:
	//     The left bracket(opening bracket) key.
	LeftBracket = 91,
	//
	// Zusammenfassung:
	//     The backslash.
	BackSlash = 92,
	//
	// Zusammenfassung:
	//     The right bracket(closing bracket) key.
	RightBracket = 93,
	//
	// Zusammenfassung:
	//     The grave accent key.
	GraveAccent = 96,
	//
	// Zusammenfassung:
	//     Non US keyboard layout key 1.
	World1 = 161,
	//
	// Zusammenfassung:
	//     Non US keyboard layout key 2.
	World2 = 162,
	//
	// Zusammenfassung:
	//     The escape key.
	Escape = 256,
	//
	// Zusammenfassung:
	//     The enter key.
	Enter = 257,
	//
	// Zusammenfassung:
	//     The tab key.
	Tab = 258,
	//
	// Zusammenfassung:
	//     The backspace key.
	Backspace = 259,
	//
	// Zusammenfassung:
	//     The insert key.
	Insert = 260,
	//
	// Zusammenfassung:
	//     The delete key.
	Delete = 261,
	//
	// Zusammenfassung:
	//     The right arrow key.
	Right = 262,
	//
	// Zusammenfassung:
	//     The left arrow key.
	Left = 263,
	//
	// Zusammenfassung:
	//     The down arrow key.
	Down = 264,
	//
	// Zusammenfassung:
	//     The up arrow key.
	Up = 265,
	//
	// Zusammenfassung:
	//     The page up key.
	PageUp = 266,
	//
	// Zusammenfassung:
	//     The page down key.
	PageDown = 267,
	//
	// Zusammenfassung:
	//     The home key.
	Home = 268,
	//
	// Zusammenfassung:
	//     The end key.
	End = 269,
	//
	// Zusammenfassung:
	//     The caps lock key.
	CapsLock = 280,
	//
	// Zusammenfassung:
	//     The scroll lock key.
	ScrollLock = 281,
	//
	// Zusammenfassung:
	//     The num lock key.
	NumLock = 282,
	//
	// Zusammenfassung:
	//     The print screen key.
	PrintScreen = 283,
	//
	// Zusammenfassung:
	//     The pause key.
	Pause = 284,
	//
	// Zusammenfassung:
	//     The F1 key.
	F1 = 290,
	//
	// Zusammenfassung:
	//     The F2 key.
	F2 = 291,
	//
	// Zusammenfassung:
	//     The F3 key.
	F3 = 292,
	//
	// Zusammenfassung:
	//     The F4 key.
	F4 = 293,
	//
	// Zusammenfassung:
	//     The F5 key.
	F5 = 294,
	//
	// Zusammenfassung:
	//     The F6 key.
	F6 = 295,
	//
	// Zusammenfassung:
	//     The F7 key.
	F7 = 296,
	//
	// Zusammenfassung:
	//     The F8 key.
	F8 = 297,
	//
	// Zusammenfassung:
	//     The F9 key.
	F9 = 298,
	//
	// Zusammenfassung:
	//     The F10 key.
	F10 = 299,
	//
	// Zusammenfassung:
	//     The F11 key.
	F11 = 300,
	//
	// Zusammenfassung:
	//     The F12 key.
	F12 = 301,
	//
	// Zusammenfassung:
	//     The F13 key.
	F13 = 302,
	//
	// Zusammenfassung:
	//     The F14 key.
	F14 = 303,
	//
	// Zusammenfassung:
	//     The F15 key.
	F15 = 304,
	//
	// Zusammenfassung:
	//     The F16 key.
	F16 = 305,
	//
	// Zusammenfassung:
	//     The F17 key.
	F17 = 306,
	//
	// Zusammenfassung:
	//     The F18 key.
	F18 = 307,
	//
	// Zusammenfassung:
	//     The F19 key.
	F19 = 308,
	//
	// Zusammenfassung:
	//     The F20 key.
	F20 = 309,
	//
	// Zusammenfassung:
	//     The F21 key.
	F21 = 310,
	//
	// Zusammenfassung:
	//     The F22 key.
	F22 = 311,
	//
	// Zusammenfassung:
	//     The F23 key.
	F23 = 312,
	//
	// Zusammenfassung:
	//     The F24 key.
	F24 = 313,
	//
	// Zusammenfassung:
	//     The F25 key.
	F25 = 314,
	//
	// Zusammenfassung:
	//     The 0 key on the key pad.
	Keypad0 = 320,
	//
	// Zusammenfassung:
	//     The 1 key on the key pad.
	Keypad1 = 321,
	//
	// Zusammenfassung:
	//     The 2 key on the key pad.
	Keypad2 = 322,
	//
	// Zusammenfassung:
	//     The 3 key on the key pad.
	Keypad3 = 323,
	//
	// Zusammenfassung:
	//     The 4 key on the key pad.
	Keypad4 = 324,
	//
	// Zusammenfassung:
	//     The 5 key on the key pad.
	Keypad5 = 325,
	//
	// Zusammenfassung:
	//     The 6 key on the key pad.
	Keypad6 = 326,
	//
	// Zusammenfassung:
	//     The 7 key on the key pad.
	Keypad7 = 327,
	//
	// Zusammenfassung:
	//     The 8 key on the key pad.
	Keypad8 = 328,
	//
	// Zusammenfassung:
	//     The 9 key on the key pad.
	Keypad9 = 329,
	//
	// Zusammenfassung:
	//     The decimal key on the key pad.
	KeypadDecimal = 330,
	//
	// Zusammenfassung:
	//     The divide key on the key pad.
	KeypadDivide = 331,
	//
	// Zusammenfassung:
	//     The multiply key on the key pad.
	KeypadMultiply = 332,
	//
	// Zusammenfassung:
	//     The subtract key on the key pad.
	KeypadSubtract = 333,
	//
	// Zusammenfassung:
	//     The add key on the key pad.
	KeypadAdd = 334,
	//
	// Zusammenfassung:
	//     The enter key on the key pad.
	KeypadEnter = 335,
	//
	// Zusammenfassung:
	//     The equal key on the key pad.
	KeypadEqual = 336,
	//
	// Zusammenfassung:
	//     The left shift key.
	ShiftLeft = 340,
	//
	// Zusammenfassung:
	//     The left control key.
	ControlLeft = 341,
	//
	// Zusammenfassung:
	//     The left alt key.
	AltLeft = 342,
	//
	// Zusammenfassung:
	//     The left super key.
	SuperLeft = 343,
	//
	// Zusammenfassung:
	//     The right shift key.
	ShiftRight = 344,
	//
	// Zusammenfassung:
	//     The right control key.
	ControlRight = 345,
	//
	// Zusammenfassung:
	//     The right alt key.
	AltRight = 346,
	//
	// Zusammenfassung:
	//     The right super key.
	SuperRight = 347,
	//
	// Zusammenfassung:
	//     The menu key.
	Menu = 348
}