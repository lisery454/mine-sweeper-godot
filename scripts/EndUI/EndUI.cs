using Godot;

public partial class EndUI : Node2D
{
    [Export] private TextureButton _backButton;
    [Export] private Label _timeLabel;
    [Export] private Label _titleLabel;

    public override void _Ready()
    {
        LoadTheme();
        var dataStation = GetNode<DataStation>("/root/DataStation");
        var gameResult = dataStation.Get<GameResult>("result");
        _timeLabel.Text = $"time: {gameResult.seconds.ToString()} s";
        _titleLabel.Text = gameResult.isWin ? "You Win!" : "You Fail!";

        _backButton.Pressed += () =>
        {
            GetNode<SceneLoader>("/root/SceneLoader").LoadScene("res://scenes/StartUI.tscn");
            GetNode<AudioManager>("/root/AudioManager").Play("confirm");
        };
    }

    private void LoadTheme()
    {
        var dataStation = GetNode<DataStation>("/root/DataStation");
        var currTheme = dataStation.Get<string>("curr_theme");
        var themeUnit = dataStation.Get<ThemeUnit>(currTheme);
        GetNode<ColorRect>("CanvasLayer/ColorRect").Color = themeUnit.BackColor;
        GetNode<Label>("CanvasLayer/TitleLabel").Set("theme_override_colors/font_color", themeUnit.FrontColor);
        GetNode<Label>("CanvasLayer/TimeLabel").Set("theme_override_colors/font_color", themeUnit.FrontColor);
        GetNode<Label>("CanvasLayer/BackButton/Label").Set("theme_override_colors/font_color", themeUnit.FontColor);
        GetNode<TextureButton>("CanvasLayer/BackButton").TextureNormal = themeUnit.ButtonTex;
        GetNode<TextureButton>("CanvasLayer/BackButton").TexturePressed = themeUnit.ButtonDownTex;
    }
}