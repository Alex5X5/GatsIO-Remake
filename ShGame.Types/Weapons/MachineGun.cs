using ShGame.Math;

namespace ShGame.Types.Weapons;

public class MachineGun : Weapon {

	public MachineGun() : base(WeaponIdentifier.MachineGun, 18) {
		
	}

	protected override Vector3d GetBulletSpawnPos(Vector3d pos, Vector3d dir) {
		return pos.Add(dir.Scl(Player.RADIUS));
	}
}
