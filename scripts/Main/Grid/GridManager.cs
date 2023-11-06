using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GridManager : Node
{
    [Export] private PackedScene _gridPrefab;
    private List<List<Grid>> _grids;

    [Export] private float _borderHeight;
    [Export] private float _borderWidth;
    [Export] private float _gridDistanceFactor;


    public void InitGrid(float rowNum, float lineNum, ThemeUnit themeUnit)
    {
        //清空之前的格子
        if (_grids != null && _grids.Count != 0)
        {
            foreach (var grid in _grids.SelectMany(gridRow => gridRow))
            {
                grid.QueueFree();
            }
        }

        _grids = new List<List<Grid>>();

        var gridRoot = new Node2D();
        AddChild(gridRoot);

        // 计算grid的scale,interval
        float interval;
        var windowSize = GetViewport().GetVisibleRect().Size;
        var alpha = Mathf.Atan2(windowSize.Y - _borderHeight, windowSize.X - _borderWidth);
        var beta = Mathf.Atan2(rowNum, lineNum);
        if (alpha > beta) interval = (windowSize.X - _borderWidth) / lineNum;
        else interval = (windowSize.Y - _borderHeight) / rowNum;
        var scale = (interval * (1 / (1 + _gridDistanceFactor))) / 16f;

        //生成新的格子
        gridRoot.Position = windowSize * 0.5f -
                            new Vector2(lineNum - 1, rowNum - 1) * interval * 0.5f;

        for (var r = 0; r < rowNum; r++)
        {
            _grids.Add(new List<Grid>());
            for (var l = 0; l < lineNum; l++)
            {
                var grid = _gridPrefab.Instantiate<Grid>();
                grid.LoadTheme(themeUnit);
                gridRoot.AddChild(grid);
                grid.Position = new Vector2(l, r) * interval;
                // GD.Print($"l:{l} r:{r} pos:{grid.GlobalPosition}");
                grid.Line = l;
                grid.Row = r;
                grid.Scale = scale * Vector2.One;
                _grids[r].Add(grid);
            }
        }
    }

    public void ShowMine(int r, int l)
    {
        _grids[r][l].ShowMine();
    }

    public void ShowNum(int r, int l, int aroundMineNum)
    {
        _grids[r][l].ShowNumber(aroundMineNum);
    }

    public void MarkGrid(int r, int l, bool isMark)
    {
        _grids[r][l].ShowMark(isMark);
    }

    public void HighlightGrid(int r, int l, bool isHighlight)
    {
        _grids[r][l].ShowHighLight(isHighlight);
    }

    public void ShowGridBack(int r, int l, bool isShowBack)
    {
        _grids[r][l].ShowBack(isShowBack);
    }
}