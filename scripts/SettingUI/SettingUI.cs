using Godot;

public partial class SettingUI : Node2D
{
    [Export] private LineEdit _rowEdit;
    [Export] private LineEdit _lineEdit;
    [Export] private LineEdit _mineEdit;
    [Export] private TextureButton _backButton;
    [Export] private TextureButton _setButton;
    [Export] private Label _hintLabel;

    public override void _Ready()
    {
        LoadTheme();

        var dataStation = GetNode<DataStation>("/root/DataStation");
        var mineParameter = dataStation.Get<MineParameter>("mine_parameter");
        _rowEdit.Text = mineParameter.row.ToString();
        _lineEdit.Text = mineParameter.line.ToString();
        _mineEdit.Text = mineParameter.mine.ToString();


        _backButton.Pressed += () =>
        {
            GetNode<SceneLoader>("/root/SceneLoader").LoadScene("res://scenes/StartUI.tscn");
            GetNode<AudioManager>("/root/AudioManager").Play("cancel");
        };

        _setButton.Pressed += () =>
        {
            var testIfInputOk = TestIfInputOk(_rowEdit.Text, _lineEdit.Text, _mineEdit.Text,
                out var row, out var line, out var mine);
            if (testIfInputOk)
                GetNode<DataStation>("/root/DataStation").Add("mine_parameter",
                    new MineParameter { row = row, line = line, mine = mine });
            GetNode<AudioManager>("/root/AudioManager").Play("confirm");
        };

        GetNode<TextureButton>("CanvasLayer/SeaButton").Pressed += () =>
        {
            GetNode<DataStation>("/root/DataStation").Add("curr_theme", "theme_sea");
            LoadTheme();
            GetNode<AudioManager>("/root/AudioManager").Play("confirm");
        };
        
        GetNode<TextureButton>("CanvasLayer/FlowerButton").Pressed += () =>
        {
            GetNode<DataStation>("/root/DataStation").Add("curr_theme", "theme_flower");
            LoadTheme();
            GetNode<AudioManager>("/root/AudioManager").Play("confirm");
        };
        
        GetNode<TextureButton>("CanvasLayer/ZenButton").Pressed += () =>
        {
            GetNode<DataStation>("/root/DataStation").Add("curr_theme", "theme_zen");
            LoadTheme();
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
        GetNode<Label>("CanvasLayer/RowLabel").Set("theme_override_colors/font_color", themeUnit.FrontColor);
        GetNode<Label>("CanvasLayer/LineLabel").Set("theme_override_colors/font_color", themeUnit.FrontColor);
        GetNode<Label>("CanvasLayer/MineLabel").Set("theme_override_colors/font_color", themeUnit.FrontColor);
        GetNode<TextureButton>("CanvasLayer/SetButton").TextureNormal = themeUnit.ButtonTex;
        GetNode<TextureButton>("CanvasLayer/SetButton").TexturePressed = themeUnit.ButtonDownTex;
        GetNode<TextureButton>("CanvasLayer/BackButton").TextureNormal = themeUnit.ButtonTex;
        GetNode<TextureButton>("CanvasLayer/BackButton").TexturePressed = themeUnit.ButtonDownTex;
        GetNode<Label>("CanvasLayer/SetButton/Label").Set("theme_override_colors/font_color", themeUnit.FontColor);
        GetNode<Label>("CanvasLayer/BackButton/Label").Set("theme_override_colors/font_color", themeUnit.FontColor);
        GetNode<TextureRect>("CanvasLayer/TextureRect").Texture = themeUnit.ButtonDownTex;
        GetNode<TextureRect>("CanvasLayer/TextureRect2").Texture = themeUnit.ButtonDownTex;
        GetNode<TextureRect>("CanvasLayer/TextureRect3").Texture = themeUnit.ButtonDownTex;
        GetNode<LineEdit>("CanvasLayer/TextureRect/RowLineEdit")
            .Set("theme_override_colors/font_color", themeUnit.FontColor);
        GetNode<LineEdit>("CanvasLayer/TextureRect2/LineLineEdit")
            .Set("theme_override_colors/font_color", themeUnit.FontColor);
        GetNode<LineEdit>("CanvasLayer/TextureRect3/MineLineEdit")
            .Set("theme_override_colors/font_color", themeUnit.FontColor);
        GetNode<Label>("CanvasLayer/HintLabel").Set("theme_override_colors/font_color", themeUnit.FontColor);
        GetNode<Label>("CanvasLayer/ThemeLabel").Set("theme_override_colors/font_color", themeUnit.FrontColor);
    }

    private bool TestIfInputOk(string rowText, string lineText, string mineText, out int row, out int line,
        out int mine)
    {
        if (int.TryParse(rowText, out row))
        {
            if (int.TryParse(lineText, out line))
            {
                if (int.TryParse(mineText, out mine))
                {
                    if (row * line <= mine + 20)
                    {
                        _hintLabel.Text = "<!>the mine is too big<!>";
                        return false;
                    }

                    if (row <= 0 || line <= 0 || mine <= 0)
                    {
                        _hintLabel.Text = "<!>must be positive number<!>";
                        return false;
                    }

                    if (row >= 26)
                    {
                        _hintLabel.Text = "<!>the row is too big<!>";
                        return false;
                    }

                    if (line >= 36)
                    {
                        _hintLabel.Text = "<!>the line is too big<!>";
                        return false;
                    }

                    if (mine >= 800)
                    {
                        _hintLabel.Text = "<!>the mine is too big<!>";
                        return false;
                    }

                    _hintLabel.Text = "<!>set success<!>";

                    return true;
                }
            }
        }

        _hintLabel.Text = "<!>type is not correct<!>";

        row = 0;
        line = 0;
        mine = 0;
        return false;
    }
}