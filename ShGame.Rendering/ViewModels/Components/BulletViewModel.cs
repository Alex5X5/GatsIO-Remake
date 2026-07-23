namespace ShGame.Game.Models;

using ShGame.Math;
using ShGame.Rendering.ViewModels;

public class BulletViewModel : ViewModelBase {

    private Bullet bullet;

    public Vector3d Pos => bullet.Pos;
    public Vector3d Dir => bullet.Dir;

    public byte Width => bullet.WIDTH;
    public byte Length => bullet.LENGHT;

	public BulletViewModel(Bullet bullet) {
        this.bullet = bullet;
    }
}