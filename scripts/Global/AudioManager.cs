using Godot;
using System.Collections.Generic;

public partial class AudioManager : Node2D
{
    private Dictionary<string, AudioUnit> _audios;

    private AudioStreamPlayer _bgmPlayer;
    private AudioStreamPlayer _soundPlayer;

    public override void _Ready()
    {
        _bgmPlayer = GetNode<AudioStreamPlayer>("BGMPlayer");
        _soundPlayer = GetNode<AudioStreamPlayer>("SoundPlayer");
        _audios = new Dictionary<string, AudioUnit>
        {
            { "confirm", ResourceLoader.Load<AudioUnit>("res://audios/unit/confirm.tres") },
            { "cancel", ResourceLoader.Load<AudioUnit>("res://audios/unit/cancel.tres") },
            { "success", ResourceLoader.Load<AudioUnit>("res://audios/unit/success.tres") },
            { "fail", ResourceLoader.Load<AudioUnit>("res://audios/unit/fail.tres") },
            { "bgm", ResourceLoader.Load<AudioUnit>("res://audios/unit/bgm.tres") },
        };

        PlayBgm("bgm");
    }

    public void PlayBgm(string name)
    {
        if (_audios.TryGetValue(name, out var audioUnit))
        {
            _bgmPlayer.Stream = audioUnit.audio;
            _bgmPlayer.VolumeDb = audioUnit.velocity;
            _bgmPlayer.Play();
        }
    }

    public void Play(string name)
    {
        if (_audios.TryGetValue(name, out var audioUnit))
        {
            _soundPlayer.Stream = audioUnit.audio;
            _soundPlayer.VolumeDb = audioUnit.velocity;
            _soundPlayer.Play();
        }
    }
}