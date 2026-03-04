using Godot;

namespace Mummoth;

public partial class MainMenu : Node
{
	[Export]
	private Button _NewGameBtn;
	[Export]
	private Button _QuitBtn;


	public override void _Ready()
	{
		_NewGameBtn.Pressed += () =>
		{
			GetTree().ChangeSceneToPacked(
					GD.Load<PackedScene>("res://scenes/main.tscn"));
		};
		_QuitBtn.Pressed += () => { GetTree().Quit(); };
	}
}
