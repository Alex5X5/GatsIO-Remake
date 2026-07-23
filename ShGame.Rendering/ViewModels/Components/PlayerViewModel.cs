namespace ShGame.Rendering.ViewModels.Components;

using ShGame.Game.Models;
using ShGame.Rendering.ViewModels;

public class PlayerViewModel : ViewModelBase {

	public Player Player { get; }


	public PlayerViewModel(Player player) {
		Player = player;
	}
}
