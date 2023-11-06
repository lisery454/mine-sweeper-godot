using Godot;

public partial class ThemeUnit : Resource
{
    [Export] public Color BackColor { get; set; }
    [Export] public Color FrontColor { get; set; }
    [Export] public Color FontColor { get; set; }
    [Export] public Texture2D ButtonTex { get; set; }
    [Export] public Texture2D ButtonDownTex { get; set; }
    [Export] public Texture2D MineMineTex { get; set; }
    [Export] public Texture2D MineBackTex { get; set; }
    [Export] public Texture2D MineFlagTex { get; set; }
    [Export] public Texture2D MineHighlightTex { get; set; }
    [Export] public Texture2D MineNormalTex { get; set; }
    [Export] public Texture2D SceneLoadTex { get; set; }
}
