using Godot;
using System.Collections.Generic;

public partial class DataStation : Node
{
    private Dictionary<string, object> _data;

    public override void _Ready()
    {
        _data = new Dictionary<string, object>();

        LoadData();
    }

    public override void _ExitTree()
    {
        SaveData();
    }

    private void SaveData()
    {
        var file = new ConfigFile();
        var mineParameter = Get<MineParameter>("mine_parameter");
        var currTheme = Get<string>("curr_theme");
        
        file.SetValue("parameter", "row", mineParameter.row);
        file.SetValue("parameter", "line", mineParameter.line);
        file.SetValue("parameter", "mine", mineParameter.mine);
        file.SetValue("theme", "theme_name", currTheme);
        file.Save("user://config.cfg");
    }

    private void LoadData()
    {
        var fileExists = FileAccess.FileExists("user://config.cfg");
        var file = new ConfigFile();
        if (!fileExists)
        {
            var configFile = file;
            configFile.SetValue("parameter", "row", 10);
            configFile.SetValue("parameter", "line", 20);
            configFile.SetValue("parameter", "mine", 30);
            configFile.Save("user://config.cfg");
        }

        var error = file.Load("user://config.cfg");
        if (error != Error.Ok)
        {
            GD.PrintErr("load config fail!");
            return;
        }

        var row = (int)file.GetValue("parameter", "row");
        var line = (int)file.GetValue("parameter", "line");
        var mine = (int)file.GetValue("parameter", "mine");
        var currTheme = (string)file.GetValue("theme", "theme_name");

        Add("mine_parameter", new MineParameter { row = row, line = line, mine = mine });
        Add("curr_theme", currTheme);

        var seaThemeUnit = ResourceLoader.Load<ThemeUnit>("res://themes/Sea.tres");
        var flowerThemeUnit = ResourceLoader.Load<ThemeUnit>("res://themes/Flower.tres");
        var zenThemeUnit = ResourceLoader.Load<ThemeUnit>("res://themes/Zen.tres");
        Add("theme_sea", seaThemeUnit);
        Add("theme_flower", flowerThemeUnit);
        Add("theme_zen", zenThemeUnit);
    }

    public enum DataAddMode
    {
        Keep,
        Replace
    }

    public void Add(string name, object element, DataAddMode mode = DataAddMode.Replace)
    {
        if (mode == DataAddMode.Replace)
        {
            _data[name] = element;
        }
        else if (mode == DataAddMode.Keep)
        {
            _data.TryAdd(name, element);
        }
    }

    public void Clear()
    {
        _data = new Dictionary<string, object>();
    }

    public void Remove(string name)
    {
        _data.Remove(name);
    }

    public enum DataGetMode
    {
        Copy,
        Unique
    }

    public T Get<T>(string name, DataGetMode mode = DataGetMode.Copy)
    {
        var foo = (T)_data[name];
        if (mode == DataGetMode.Unique)
            Remove(name);
        return foo;
    }

    public bool TryGet<T>(string name, out T result, DataGetMode mode = DataGetMode.Copy)
    {
        if (_data.TryGetValue(name, out var value))
        {
            result = (T)value;
            if (mode == DataGetMode.Unique)
                Remove(name);
            return true;
        }

        result = default;
        return false;
    }
}