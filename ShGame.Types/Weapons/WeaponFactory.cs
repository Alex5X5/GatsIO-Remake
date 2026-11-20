using System;

namespace ShGame.Types.Weapons;

public static class WeaponFactory {

	public static Weapon BuildFromIdentifier(WeaponIdentifier identifier) {
		return identifier switch {
			WeaponIdentifier.MachineGun => new MachineGun(),
			WeaponIdentifier.Pistol => new Pistol(),
			_ => throw new InvalidOperationException($"unknown weapon type {identifier}")
		};
	}
}
