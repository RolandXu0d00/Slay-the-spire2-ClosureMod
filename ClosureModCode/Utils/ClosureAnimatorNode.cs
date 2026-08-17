using Godot;

namespace ClosureMod.ClosureModCode.Utils;

/// <summary>
/// 逐帧动画播放器：把离线渲染好的透明 PNG 帧组装成 SpriteFrames，
/// 并根据游戏触发（Idle/Attack/Cast/Hit/Dead）切换动画。
/// </summary>
public sealed partial class ClosureAnimatorNode : Node
{
    /// <summary>
    /// 一段动画的帧来源：folder 为动画子目录名，count 为该目录下的帧数（frame_0000 起）。
    /// </summary>
    public sealed record AnimPart(string Folder, int Count);

    /// <summary>
    /// 一个完整动画状态的定义。
    /// </summary>
    public sealed record AnimSpec(string Name, AnimPart[] Parts, bool Loop, float Fps);

    private AnimatedSprite2D? _sprite;
    private readonly Dictionary<string, string> _stateToAnim = new()
    {
        ["Idle"] = "Idle",
        ["Relaxed"] = "Idle",
        ["Hit"] = "Idle",
        ["Attack"] = "Attack",
        ["Cast"] = "Skill",
        ["PowerUp"] = "Skill",
        ["Dead"] = "Die",
        ["Revive"] = "Idle",
        ["Start"] = "Start",
    };

    private bool _playingDie;

    /// <summary>
    /// 设置动画源并立即播放待机。
    /// </summary>
    public void Setup(AnimatedSprite2D sprite, string animRootResPath, IReadOnlyList<AnimSpec> specs)
    {
        _sprite = sprite;

        var frames = new SpriteFrames();
        foreach (AnimSpec spec in specs)
        {
            AddAnimation(frames, spec, animRootResPath);
        }

        sprite.SpriteFrames = frames;
        sprite.Animation = "Idle";
        sprite.Play();
        sprite.AnimationFinished += () =>
        {
            if (_playingDie)
            {
                if (sprite.Animation == "Die")
                {
                    // 死亡动画播完不隐藏，明确钉在最后一帧（蹲下跪地姿势），直到结算结束。
                    PinToDieLastFrame();
                }
                return;
            }
            if (sprite.Animation != "Idle")
            {
                sprite.Animation = "Idle";
                sprite.Play();
            }
        };
    }

    public void Play(string trigger)
    {
        if (_sprite == null || !_stateToAnim.TryGetValue(trigger, out string? anim))
        {
            return;
        }

        if (trigger == "Dead")
        {
            _playingDie = true;
            _sprite.Visible = true;
            _sprite.Frame = 0;
            _sprite.Animation = "Die";
            _sprite.Play();
            return;
        }

        // 死亡后锁定死亡动画：其它触发（Idle/Relaxed/Attack 等）不得打断倒地动作。
        if (_playingDie)
        {
            if (trigger == "Revive")
            {
                _playingDie = false;
                _sprite.Visible = true;
            }
            else
            {
                return;
            }
        }

        if (trigger == "Hit")
        {
            FlashWhite();
            return;
        }

        if (!_sprite.SpriteFrames.HasAnimation(anim))
        {
            return;
        }
        if (_sprite.Animation == anim && _sprite.IsPlaying())
        {
            return;
        }
        _sprite.Visible = true;
        _sprite.Animation = anim;
        _sprite.Play();
    }

    /// <summary>
    /// 结算界面兜底：若死亡信号没有传到逐帧模型（例如非战斗死亡时游戏重建了模型），
    /// 强制从头播放死亡动画，播完停在最后一帧；已经在播或已停在最后一帧时保持不变。
    /// </summary>
    public void EnterDeathPoseIfNeeded()
    {
        if (_sprite == null || _sprite.SpriteFrames == null || !_sprite.SpriteFrames.HasAnimation("Die"))
        {
            return;
        }
        if (_sprite.Animation == "Die")
        {
            // 死亡动画正在播：让它自然播完，播完会停在最后一帧。
            // 已停（定格）：确保确实停在最后一帧。
            if (!_sprite.IsPlaying())
            {
                PinToDieLastFrame();
            }
            return;
        }
        // 死亡信号没传到（例如模型被重建）：从头播放，播完停在最后一帧。
        Play("Dead");
    }

    /// <summary>
    /// 当前是否已处于死亡动画状态（用于结算兜底判断）。
    /// </summary>
    public bool IsInDeathPose =>
        _sprite != null
        && _sprite.SpriteFrames != null
        && _sprite.SpriteFrames.HasAnimation("Die")
        && _sprite.Animation == "Die";

    /// <summary>
    /// 强制定格在战败动画最后一帧（蹲下跪地姿势），不受任何后续触发影响。
    /// </summary>
    public void PinDeathPoseEnd()
    {
        if (_sprite == null || _sprite.SpriteFrames == null || !_sprite.SpriteFrames.HasAnimation("Die"))
        {
            return;
        }
        _playingDie = true;
        _sprite.Visible = true;
        PinToDieLastFrame();
    }

    private void PinToDieLastFrame()
    {
        if (_sprite == null || _sprite.SpriteFrames == null || !_sprite.SpriteFrames.HasAnimation("Die"))
        {
            return;
        }
        _sprite.Animation = "Die";
        _sprite.Frame = Math.Max(0, _sprite.SpriteFrames.GetFrameCount("Die") - 1);
        _sprite.Stop();
    }

    private void AddAnimation(SpriteFrames frames, AnimSpec spec, string animRootResPath)
    {
        var texturePaths = new List<string>();
        foreach (AnimPart part in spec.Parts)
        {
            string folder = $"{animRootResPath}/{part.Folder}";
            for (int i = 0; i < part.Count; i++)
            {
                texturePaths.Add($"{folder}/frame_{i:0000}.png");
            }
        }
        if (texturePaths.Count == 0)
        {
            MainFile.Logger.Warn($"[Animator] {spec.Name}: 帧数为 0，已跳过");
            return;
        }

        frames.AddAnimation(spec.Name);
        foreach (string texPath in texturePaths)
        {
            Texture2D? tex = GD.Load<Texture2D>(texPath);
            if (tex != null)
            {
                frames.AddFrame(spec.Name, tex);
            }
        }
        frames.SetAnimationLoop(spec.Name, spec.Loop);
        frames.SetAnimationSpeed(spec.Name, spec.Fps);
    }

    private void FlashWhite()
    {
        if (_sprite == null)
        {
            return;
        }
        _sprite.SelfModulate = new Color(2.2f, 2.2f, 2.2f, 1f);
        var tween = CreateTween();
        tween.TweenProperty(_sprite, "self_modulate", Colors.White, 0.18f);
    }
}
