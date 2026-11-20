using ShGame.Math;

namespace ShGame.Types.Weapons;

public class Pistol : Weapon {
	
	public Pistol() : base(WeaponIdentifier.Pistol, 100) {
		
	}

	protected override Vector3d GetBulletSpawnPos(Vector3d pos, Vector3d dir) {
		return pos.Add(dir.Scl(Player.RADIUS));
	}
}
