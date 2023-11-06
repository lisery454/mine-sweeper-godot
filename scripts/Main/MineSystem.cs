using System;
using System.Collections.Generic;
using Godot;

public partial class MineSystem : Node2D
{
    [Export] private InputManager _inputManager;
    [Export] private GridManager _gridManager;
    [Export] private Label _timeLabel;
    private DateTime _startTime;
    private MineParameter _mineParameter;
    private bool[,] _isMine = { };
    private int[,] _aroundMineNum = { };
    private bool[,] _isShowed = { };
    private bool[,] _isMarked = { };
    private bool _isFirstSweep;
    private bool _isGameOver;

    public override void _Ready()
    {
        _inputManager.SweepMine += SweepMine;
        _inputManager.MarkMine += MarkMine;
        _inputManager.DetectMine += DetectMine;
        _inputManager.HighlightGrid += HighLightGrid;
        _inputManager.ShowAroundGridsBack += ShowAroundGridsBack;
        _inputManager.ShowGridBack += ShowGridBack;
        var dataStation = GetNode<DataStation>("/root/DataStation");
        _mineParameter = dataStation.Get<MineParameter>("mine_parameter");
        _isFirstSweep = true;
        _isShowed = new bool[_mineParameter.row, _mineParameter.line];
        _isMarked = new bool[_mineParameter.row, _mineParameter.line];
        var theme = LoadTheme();
        _gridManager.InitGrid(_mineParameter.row, _mineParameter.line, theme);
        _startTime = DateTime.Now;
        _isGameOver = false;
    }

    private ThemeUnit LoadTheme()
    {
        var dataStation = GetNode<DataStation>("/root/DataStation");
        var currTheme = dataStation.Get<string>("curr_theme");
        var themeUnit = dataStation.Get<ThemeUnit>(currTheme);
        GetNode<ColorRect>("CanvasLayer/ColorRect").Color = themeUnit.BackColor;
        GetNode<Label>("CanvasLayer/Label").Set("theme_override_colors/font_color", themeUnit.FrontColor);
        return themeUnit;
    }

    private int GetSecond()
    {
        return (int)(DateTime.Now - _startTime).TotalSeconds;
    }

    public override void _Process(double delta)
    {
        if (!_isGameOver)
        {
            _timeLabel.Text = $"Time: {GetSecond()}s";
        }
    }

    private void InitGridState(int firstR, int firstL)
    {
        var rowNum = _mineParameter.row;
        var lineNum = _mineParameter.line;
        var mineNum = _mineParameter.mine;
        InitIsMine(rowNum, lineNum, mineNum, firstR, firstL);
        InitAroundMineNum(rowNum, lineNum);
    }

    private void InitIsMine(int rowNum, int lineNum, int mineNum, int firstR, int firstL)
    {
        _isMine = new bool[rowNum, lineNum];
        //用来随机获取mineNum个元素
        var randomMineSelector = new List<int>();
        var size = lineNum * rowNum;
        for (var i = 0; i < size; i++)
        {
            randomMineSelector.Add(i);
        }

        var random = new Random();
        for (var i = 0; i < size; i++)
        {
            var randomNum = random.Next(0, size);
            (randomMineSelector[i], randomMineSelector[randomNum]) =
                (randomMineSelector[randomNum], randomMineSelector[i]);
        }


        //设置值
        for (int i = 0, j = 0; i < mineNum; i++)
        {
            int r, l;
            do
            {
                var index = randomMineSelector[j];
                r = index / lineNum;
                l = index % lineNum;
                j++;
            } while (Distance(r, l, firstR, firstL) <= 2);

            _isMine[r, l] = true;
        }
    }

    private static int Distance(int r1, int l1, int r2, int l2)
    {
        return Mathf.Abs(r1 - r2) + Mathf.Abs(l1 - l2);
    }

    private void InitAroundMineNum(int rowNum, int lineNum)
    {
        _aroundMineNum = new int[rowNum, lineNum];
        var gridModelIsMine = _isMine;
        //对每个格子
        for (var r = 0; r < rowNum; r++)
        {
            for (var l = 0; l < lineNum; l++)
            {
                //如果是雷
                if (gridModelIsMine[r, l])
                {
                    //周围的格子全部加一雷数
                    for (var dr = -1; dr <= 1; dr++)
                    {
                        for (var dl = -1; dl <= 1; dl++)
                        {
                            if (r + dr >= 0 && r + dr < rowNum && l + dl >= 0 && l + dl < lineNum)
                                _aroundMineNum[r + dr, l + dl]++;
                        }
                    }

                    //自己本身不用加一
                    _aroundMineNum[r, l]--;
                }
            }
        }
    }

    private void SweepMine(int row, int line)
    {
        if (_isFirstSweep)
        {
            _isFirstSweep = false;
            InitGridState(row, line);
        }

        var gridsToBeShowed = new List<Tuple<int, int>>();
        RawSweep(row, line, _mineParameter.row, _mineParameter.line, gridsToBeShowed);
        if (gridsToBeShowed.Count != 0)
        {
            foreach (var (r, l) in gridsToBeShowed)
            {
                if (_isMine[r, l]) _gridManager.ShowMine(r, l);
                else _gridManager.ShowNum(r, l, _aroundMineNum[r, l]);
            }

            CheckWin();
        }
    }

    private void CheckWin()
    {
        var lineNum = _mineParameter.line;
        var rowNum = _mineParameter.row;

        //判断有没有雷踩到了
        for (var r = 0; r < rowNum; r++)
        for (var l = 0; l < lineNum; l++)
            if (_isMine[r, l] && _isShowed[r, l])
                GameOver(false, GetSecond());


        //判断有没有赢,把所有雷都找到了
        for (var r = 0; r < rowNum; r++)
        for (var l = 0; l < lineNum; l++)
            if (!_isMine[r, l] && !_isShowed[r, l])
                return;

        GameOver(true, GetSecond());
    }

    private void GameOver(bool isWin, int usedSeconds)
    {
        _isGameOver = true;
        _inputManager.SetInputAvailable(false);
        // GD.Print($"isWin: {isWin}, Use: {usedSeconds} s");
        var dataStation = GetNode<DataStation>("/root/DataStation");
        dataStation.Add("result", new GameResult { isWin = isWin, seconds = usedSeconds });
        GetNode<AudioManager>("/root/AudioManager").Play(isWin ? "success" : "fail");
        GetNode<SceneLoader>("/root/SceneLoader").LoadScene("res://scenes/EndUI.tscn");
    }

    private void RawSweep(int r, int l, int rowNum, int lineNum, List<Tuple<int, int>> gridsToBeShowed)
    {
        if (_isMarked[r, l]) return;
        if (_isShowed[r, l]) return;
        _isShowed[r, l] = true;

        gridsToBeShowed.Add(new Tuple<int, int>(r, l));

        if (_aroundMineNum[r, l] == 0 && !_isMine[r, l])
        {
            for (var dr = -1; dr <= 1; dr++)
            {
                for (var dl = -1; dl <= 1; dl++)
                {
                    if (dr == 0 && dl == 0) continue;

                    if (r + dr >= 0 && r + dr < rowNum && l + dl >= 0 && l + dl < lineNum)
                        RawSweep(r + dr, l + dl, rowNum, lineNum, gridsToBeShowed);
                }
            }
        }
    }

    private void MarkMine(int r, int l)
    {
        if (!_isShowed[r, l])
        {
            _isMarked[r, l] = !_isMarked[r, l];
            _gridManager.MarkGrid(r, l, _isMarked[r, l]);
        }
    }

    private void DetectMine(int r, int l)
    {
        var lineNum = _mineParameter.line;
        var rowNum = _mineParameter.row;

        if (_isShowed[r, l] && !_isMine[r, l])
        {
            var aroundMarkedNum = 0;
            for (var dr = -1; dr <= 1; dr++)
            {
                for (var dl = -1; dl <= 1; dl++)
                {
                    if (dr == 0 && dl == 0) continue;
                    var nr = r + dr;
                    var nl = l + dl;
                    if (nr >= 0 && nr < rowNum && nl >= 0 && nl < lineNum)
                        if (_isMarked[nr, nl] && !_isShowed[nr, nl])
                            aroundMarkedNum++;
                }
            }

            if (aroundMarkedNum == _aroundMineNum[r, l])
            {
                for (var dr = -1; dr <= 1; dr++)
                {
                    for (var dl = -1; dl <= 1; dl++)
                    {
                        if (dr == 0 && dl == 0) continue;
                        var nr = r + dr;
                        var nl = l + dl;
                        if (nr >= 0 && nr < rowNum && nl >= 0 && nl < lineNum)
                            if (!_isMarked[nr, nl] && !_isShowed[nr, nl])
                                SweepMine(nr, nl);
                    }
                }
            }
        }
    }

    private void HighLightGrid(int r, int l, bool isHighlight)
    {
        if (isHighlight)
        {
            if (!_isShowed[r, l]) _gridManager.HighlightGrid(r, l, true);
        }
        else _gridManager.HighlightGrid(r, l, false);
    }

    private void ShowGridBack(int r, int l, bool isShowBack)
    {
        if (!_isShowed[r, l] && !_isMarked[r, l])
            _gridManager.ShowGridBack(r, l, isShowBack);
    }

    private void ShowAroundGridsBack(int r, int l, bool isShowBack)
    {
        var lineNum = _mineParameter.line;
        var rowNum = _mineParameter.row;
        for (var dr = -1; dr <= 1; dr++)
        {
            for (var dl = -1; dl <= 1; dl++)
            {
                var nr = r + dr;
                var nl = l + dl;
                if (nr >= 0 && nr < rowNum && nl >= 0 && nl < lineNum)
                {
                    ShowGridBack(nr, nl, isShowBack);
                }
            }
        }
    }
}