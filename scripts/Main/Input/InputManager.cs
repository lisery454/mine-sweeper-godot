using System.Collections.Generic;
using Godot;

public partial class InputManager : Node
{
    private bool _isCheckInput;

    private Grid _hitGrid;

    private IInputState _currentInputState;

    private List<Grid> _highLightedGrids;
    private List<Grid> _pressedDownGrids;
    private List<Grid> _aroundHighLightedGrids;

    [Export] private RayCast2D _rayCast2D;

    [Signal]
    public delegate void MarkMineEventHandler(int row, int line);

    [Signal]
    public delegate void SweepMineEventHandler(int row, int line);

    [Signal]
    public delegate void DetectMineEventHandler(int row, int line);

    [Signal]
    public delegate void HighlightGridEventHandler(int row, int line, bool isHighlight);

    [Signal]
    public delegate void ShowAroundGridsBackEventHandler(int row, int line, bool isShowAroundBack);

    [Signal]
    public delegate void ShowGridBackEventHandler(int row, int line, bool isShowBack);

    public override void _Ready()
    {
        _currentInputState = new NormalInputState();
        _highLightedGrids = new List<Grid>();
        _pressedDownGrids = new List<Grid>();
        _aroundHighLightedGrids = new List<Grid>();
        
        _isCheckInput = true;
    }

    public void SetInputAvailable(bool isAvailable)
    {
        _isCheckInput = isAvailable;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isCheckInput) return;
        
        // 获取鼠标位置的Grid
        _rayCast2D.Position = GetViewport().GetMousePosition();
        if (_rayCast2D.IsColliding() && _rayCast2D.GetCollider() is Area2D area2D)
            _hitGrid = area2D.GetParent<Grid>();

        else _hitGrid = null;
        
        if (_hitGrid != null)
        {
            
            switch (_currentInputState)
            {
                case NormalInputState:
                    MakeGridHighLight(_hitGrid);
                    break;
                case LeftMouseInputState:
                    MakeGridPressDown(_hitGrid);
                    break;
                case RightMouseInputState:
                    MakeGridHighLight(_hitGrid);
                    break;
                case LeftRightMouseInputState:
                    MakeAroundGridsDown(_hitGrid);
                    break;
            }
        }
        else
        {
            ClearHighLightGrids();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isCheckInput) return;

        // 处理输入
        if (@event is InputEventMouseButton e)
        {
            // GD.Print($"Press: {e.Pressed}, {e.ButtonIndex}");

            if (e.IsPressed() && e.ButtonIndex == MouseButton.Left) OnLeftMouseDown();

            if (e.IsPressed() && e.ButtonIndex == MouseButton.Right) OnRightMouseDown();

            if (e.IsReleased() && e.ButtonIndex == MouseButton.Left) OnLeftMouseUp();

            if (e.IsReleased() && e.ButtonIndex == MouseButton.Right) OnRightMouseUp();
        }
    }

    #region Deal Input

    private void OnLeftMouseDown()
    {
        switch (_currentInputState)
        {
            case NormalInputState:
            {
                ClearHighLightGrids();
                // if (_hitGrid != null) MakeGridPressDown(_hitGrid);
                break;
            }
            // case LeftMouseInputState:
            // {
            //     ClearPressDownGrids();
            //     break;
            // }
        }

        _currentInputState = _currentInputState.OnLeftMouseDown();
    }

    private void OnRightMouseDown()
    {
        switch (_currentInputState)
        {
            // case NormalInputState:
            // {
            //     MarkMineGrid();
            //     break;
            // }
            // case LeftMouseInputState:
            // {
            //     ClearPressDownGrids();
            //     break;
            // }
        }

        _currentInputState = _currentInputState.OnRightMouseDown();
    }

    private void OnLeftMouseUp()
    {
        switch (_currentInputState)
        {
            case LeftMouseInputState:
            {
                ClearPressDownGrids();
                SweepMineGrid();
                break;
            }
            case LeftRightMouseInputState:
            {
                ClearAroundDownGrids();
                DetectMineGrid();
                break;
            }
            // case NormalInputState:
            // {
            //     ClearHighLightGrids();
            //     break;
            // }
        }

        _currentInputState = _currentInputState.OnLeftMouseUp();
    }

    private void OnRightMouseUp()
    {
        switch (_currentInputState)
        {
            case LeftRightMouseInputState:
            {
                ClearAroundDownGrids();
                DetectMineGrid();
                break;
            }
            case RightMouseInputState:
            {
                MarkMineGrid();
                break;
            }
            // case NormalInputState:
            // {
            //     ClearHighLightGrids();
            //     break;
            // }
            // case LeftMouseInputState:
            // {
            //     ClearPressDownGrids();
            //     break;
            // }
        }

        _currentInputState = _currentInputState.OnRightMouseUp();
    }

    #endregion

    #region Commands

    private void MarkMineGrid()
    {
        if (_hitGrid != null) EmitSignal(SignalName.MarkMine, _hitGrid.Row, _hitGrid.Line);
    }

    private void SweepMineGrid()
    {
        if (_hitGrid != null) EmitSignal(SignalName.SweepMine, _hitGrid.Row, _hitGrid.Line);
    }

    private void DetectMineGrid()
    {
        if (_hitGrid != null) EmitSignal(SignalName.DetectMine, _hitGrid.Row, _hitGrid.Line);
    }

    private void MakeGridHighLight(Grid hitGrid)
    {
        ClearHighLightGrids();
        _highLightedGrids.Add(hitGrid);
        EmitSignal(SignalName.HighlightGrid, hitGrid.Row, hitGrid.Line, true);
    }

    private void ClearHighLightGrids()
    {
        foreach (var grid in _highLightedGrids)
        {
            EmitSignal(SignalName.HighlightGrid, grid.Row, grid.Line, false);
        }

        _highLightedGrids.Clear();
    }

    private void MakeAroundGridsDown(Grid hitGrid)
    {
        ClearAroundDownGrids();
        _aroundHighLightedGrids.Add(hitGrid);
        EmitSignal(SignalName.ShowAroundGridsBack, hitGrid.Row, hitGrid.Line, true);
    }

    private void ClearAroundDownGrids()
    {
        foreach (var grid in _aroundHighLightedGrids)
        {
            EmitSignal(SignalName.ShowAroundGridsBack, grid.Row, grid.Line, false);
        }

        _aroundHighLightedGrids.Clear();
    }

    private void MakeGridPressDown(Grid hitGrid)
    {
        ClearPressDownGrids();
        _pressedDownGrids.Add(hitGrid);
        EmitSignal(SignalName.ShowGridBack, hitGrid.Row, hitGrid.Line, true);
    }

    private void ClearPressDownGrids()
    {
        foreach (var grid in _pressedDownGrids)
        {
            EmitSignal(SignalName.ShowGridBack, grid.Row, grid.Line, false);
        }

        _pressedDownGrids.Clear();
    }

    #endregion
}