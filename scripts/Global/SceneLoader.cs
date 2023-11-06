using System.Threading.Tasks;
using Godot;

public partial class SceneLoader : CanvasLayer
{
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private Sprite2D _sprite2D;

    public override void _Ready()
    {
        _sprite2D.Hide();
    }

    public void LoadTheme()
    {
        var dataStation = GetNode<DataStation>("/root/DataStation");
        var currTheme = dataStation.Get<string>("curr_theme");
        var themeUnit = dataStation.Get<ThemeUnit>(currTheme);
        GetNode<Sprite2D>("Sprite2D").Texture = themeUnit.SceneLoadTex;
    }

    public async void LoadScene(string path)
    {
        LoadTheme();
        _sprite2D.Show();
        _animationPlayer.Play("StartLoad");
        await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        GetTree().ChangeSceneToFile(path);
        await Task.Delay(500);
        _animationPlayer.PlayBackwards("StartLoad");
        await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        _sprite2D.Hide();
    }
}