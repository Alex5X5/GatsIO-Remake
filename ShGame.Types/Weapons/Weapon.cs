namespace ShGame.Types.Weapons;

using ShGame.Math;

public abstract class Weapon {

	public readonly WeaponIdentifier Identifier;

	public readonly short CooldownTicks;
	public readonly short BulletSpeed;
	public short CooldownTicksDone = 0;

	public Weapon(WeaponIdentifier identifier, short cooldownTicks) {
		Identifier = identifier;
		CooldownTicks = cooldownTicks;
	}

	protected abstract Vector3d GetBulletSpawnPos(Vector3d pos, Vector3d dir);
}
