using Godot;

public partial class Grid : Node2D
{
    public int Row { get; set; }
    public int Line { get; set; }

    [Export] private Node2D _normal;
    [Export] private Node2D _highlight;
    [Export] private Node2D _flag;
    [Export] private Node2D _back;
    [Export] private Node2D _mine;
    [Export] private Label _number;

    public override void _Ready()
    {
        _normal.Show();
        _highlight.Hide();
        _flag.Hide();
        _back.Hide();
        _mine.Hide();
        _number.Hide();
    }

    public void ShowNumber(int num)
    {
        ShowBack(true);
        _number.Show();
        _number.Text = num != 0 ? num.ToString() : "";
    }

    public void ShowMark(bool isShow)
    {
        if (isShow) _flag.Show();
        else _flag.Hide();
    }

    public void ShowMine()
    {
        ShowBack(true);
        _mine.Show();
    }

    public void ShowBack(bool isShow)
    {
        if (isShow)
        {
            _back.Show();
        }
        else
        {
            _back.Hide();
        }
    }

    public void ShowHighLight(bool isShow)
    {
        if (isShow) _highlight.Show();
        else _highlight.Hide();
    }

    public void LoadTheme(ThemeUnit themeUnit)
    {
        GetNode<Sprite2D>("Normal").Texture = themeUnit.MineNormalTex;
        GetNode<Sprite2D>("Highlight").Texture = themeUnit.MineHighlightTex;
        GetNode<Sprite2D>("Flag").Texture = themeUnit.MineFlagTex;
        GetNode<Sprite2D>("Back").Texture = themeUnit.MineBackTex;
        GetNode<Sprite2D>("Mine").Texture = themeUnit.MineMineTex;
        GetNode<Label>("Number").Set("theme_override_colors/font_color", themeUnit.FontColor);
    }
}