using Godot;

public partial class StartUI : Node2D
{
    public override void _Ready()
    {
        LoadTheme();

        GetNode<TextureButton>("CanvasLayer/StartButton").Pressed += () =>
        {
            GetNode<AudioManager>("/root/AudioManager").Play("confirm");
            var sceneLoader = GetNode<SceneLoader>("/root/SceneLoader");
            sceneLoader.LoadScene("res://scenes/Main.tscn");
        };

        GetNode<TextureButton>("CanvasLayer/SettingButton").Pressed += () =>
        {
            GetNode<AudioManager>("/root/AudioManager").Play("confirm");
            GetNode<SceneLoader>("/root/SceneLoader").LoadScene("res://scenes/SettingUI.tscn");
        };


        GetNode<TextureButton>("CanvasLayer/QuitButton").Pressed += () =>
        {
            GetNode<AudioManager>("/root/AudioManager").Play("confirm");
            GetTree().Quit();
        };
    }

    private void LoadTheme()
    {
        var dataStation = GetNode<DataStation>("/root/DataStation");
        var currTheme = dataStation.Get<string>("curr_theme");
        var themeUnit = dataStation.Get<ThemeUnit>(currTheme);
        GetNode<ColorRect>("CanvasLayer/ColorRect").Color = themeUnit.BackColor;
        GetNode<Label>("CanvasLayer/Title").Set("theme_override_colors/font_color", themeUnit.FrontColor);
        GetNode<TextureButton>("CanvasLayer/StartButton").TextureNormal = themeUnit.ButtonTex;
        GetNode<TextureButton>("CanvasLayer/StartButton").TexturePressed = themeUnit.ButtonDownTex;
        GetNode<TextureButton>("CanvasLayer/SettingButton").TextureNormal = themeUnit.ButtonTex;
        GetNode<TextureButton>("CanvasLayer/SettingButton").TexturePressed = themeUnit.ButtonDownTex;
        GetNode<TextureButton>("CanvasLayer/QuitButton").TextureNormal = themeUnit.ButtonTex;
        GetNode<TextureButton>("CanvasLayer/QuitButton").TexturePressed = themeUnit.ButtonDownTex;
        GetNode<Label>("CanvasLayer/StartButton/Label").Set("theme_override_colors/font_color", themeUnit.FontColor);
        GetNode<Label>("CanvasLayer/SettingButton/Label").Set("theme_override_colors/font_color", themeUnit.FontColor);
        GetNode<Label>("CanvasLayer/QuitButton/Label").Set("theme_override_colors/font_color", themeUnit.FontColor);
    }
}