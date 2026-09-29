// ============================================================
// BlmRotation - 黑魔法师 ACR 主旋转类（官方 Resolver 架构）
// 按优先级注册 GCD/oGCD 决策器链 -> 遍历第一个 Check 成功的执行
// 游戏内调试：UpdateDebugStatus 实时显示每个决策器状态
// ============================================================
using BlmAcr.Action.Gcd;
using BlmAcr.Action.OffGcd;
using BlmAcr.Data;
using BlmAcr.Helper;
using BlmAcr.Opener;
using BlmAcr.UI;
using ECommons.ExcelServices;
using PromeRotation.Core;
using PromeRotation.Data;
using PromeRotation.Managers;
using PromeRotation.Resolvers;
using PromeRotation.Rotation;

namespace BlmAcr;

[RotationMetadata((uint)Job.BLM, "BlmAcr", "BlmAcr", "1.0.0.0")]
public sealed class BlmRotation : IRotation, IRotationMeta, IRotationLifecycle
{
    public string RotationName => "BlmAcr";

    public uint JobId => (uint)Job.BLM;

    private readonly IRotationEventHandler _eventHandler = new BlmEventHandler();
    public IRotationEventHandler GetEventHandler() => _eventHandler;

    private readonly List<IDecisionResolver> _gcdResolvers = new();
    private readonly List<IDecisionResolver> _offGcdResolvers = new();

    // ---- IRotationMeta（静态）----
    public static IReadOnlyDictionary<string, bool> QtList => BlmQT.All;

    public static IReadOnlyDictionary<string, Type> Openers { get; } = new Dictionary<string, Type>
    {
        { "Standard 5+7", typeof(BlmOpener) },
    };

    public BlmRotation()
    {
        // GCD 决策器链（优先级 = 注册顺序，先注册先判定）
        _gcdResolvers.Add(new AoeFillerGcd());    // 1  AOE 填充（高雷二 DoT / 灵极魂·AOE）
        _gcdResolvers.Add(new ThunderDotGcd());   // 2  单体高雷云 DoT 刷新
        _gcdResolvers.Add(new XenoglossyGcd());   // 3  单体异言防溢出
        _gcdResolvers.Add(new BlizzardIIIGcd());  // 4  无元素 -> 暴雷进冰
        _gcdResolvers.Add(new FirestarterGcd());  // 5  UI 火苗 -> 免费爆炎
        _gcdResolvers.Add(new AoeIceGcd());       // 6  UI AOE 玄冰攒心
        _gcdResolvers.Add(new IceUmbralGcd());    // 7  UI 单体（危险/正常）
        _gcdResolvers.Add(new KillFinishGcd());   // 8  AF 斩杀收尾
        _gcdResolvers.Add(new AoeFireGcd());      // 9  AF AOE（耀星/核爆/转冰）
        _gcdResolvers.Add(new FlareStarGcd());    // 10 AF 耀星
        _gcdResolvers.Add(new ParadoxGcd());      // 11 AF 悖论
        _gcdResolvers.Add(new FireIVGcd());       // 12 AF 炽炎
        _gcdResolvers.Add(new DespairGcd());      // 13 AF 绝望
        _gcdResolvers.Add(new TransposeGcd());    // 14 AF 转冰

        // oGCD 决策器链
        _offGcdResolvers.Add(new DefenseOffGcd());     // 0  自动减伤（魔罩）
        _offGcdResolvers.Add(new MovementOffGcd());    // 1  移动瞬发（三连/迅捷）
        _offGcdResolvers.Add(new ManafontOffGcd());    // 2  魔泉
        _offGcdResolvers.Add(new AmplifierOffGcd());  // 3  详述
        _offGcdResolvers.Add(new LeyLinesOffGcd());   // 4  黑魔纹

        // 注册 QT + 恢复当前模式快照
        BlmQT.Register();
        BlmSettings.Instance.RestoreQtSnapshot(BlmSettings.Instance.IsHighEnd);
    }

    public PAction? NextAlways() => null;

    public PAction? NextGcd()
    {
        // 停手：紧急机制期间完全停手
        if (BlmQT.Enabled(BlmQT.停手))
            return null;

        // 每帧同步 AF 阶段（新 AF 阶段重置炽炎计数）
        BlmState.UpdatePhase(BlmGcdHelper.Gauge.InAstralFire);

        foreach (var r in _gcdResolvers)
        {
            if (r.Check().Success)
                return r.GetAction();
        }
        return null;
    }

    public PAction? NextOffGcd()
    {
        // 停手：紧急机制期间完全停手
        if (BlmQT.Enabled(BlmQT.停手))
            return null;

        // oGCD 节流：距离上一个 oGCD 不足 700ms 不再发（动画锁定，防两个能力技卡手）
        if (System.Environment.TickCount - BlmState.LastOffGcdTime < 700)
            return null;

        foreach (var r in _offGcdResolvers)
        {
            if (r.Check().Success)
            {
                BlmState.MarkOffGcd();
                return r.GetAction();
            }
        }
        return null;
    }

    public void UpdateDebugStatus()
    {
        RotationManager.GcdSolverStatus.Clear();
        RotationManager.OffGcdSolverStatus.Clear();

        BlmState.UpdatePhase(BlmGcdHelper.Gauge.InAstralFire);

        // ★ 量谱实时概览（第一行）：确认 ACR 读到的资源
        var g = BlmGcdHelper.Gauge;
        RotationManager.GcdSolverStatus.Add(new SolverStatus
        {
            Name = "量谱",
            Success = true,
            Message = $"AF{g.AstralFireStacks} UI{g.UmbralIceStacks} 心{g.UmbralHearts} 星魂{g.AstralSoulStacks}/6 灵极魂{g.PolyglotStacks}/2 MP{BlmGcdHelper.Mp} 雷首{BlmGcdHelper.HasStatus(BlmGcdHelper.Me, BlmSkills.ThunderheadStatus)}",
        });

        foreach (var r in _gcdResolvers)
        {
            var x = r.Check();
            RotationManager.GcdSolverStatus.Add(new SolverStatus
            {
                Name = r.GetType().Name.Replace("Gcd", ""),
                Success = x.Success,
                Message = x.Message,
            });
        }

        foreach (var r in _offGcdResolvers)
        {
            var x = r.Check();
            RotationManager.OffGcdSolverStatus.Add(new SolverStatus
            {
                Name = r.GetType().Name.Replace("OffGcd", ""),
                Success = x.Success,
                Message = x.Message,
            });
        }
    }

    public IOpener? GetOpener()
    {
        if (!BlmQT.Enabled(BlmQT.启用起手)) return null;
        if (Core.Me == null) return null;

        // 优先从框架注册表加载；失败兜底直接新建
        var openers = RotationManager.GetOpenersByJob((int)Job.BLM);
        if (openers != null && openers.TryGetValue("Standard 5+7", out var t))
        {
            if (Activator.CreateInstance(t) is IOpener opener)
                return opener;
        }
        return new BlmOpener();
    }

    public void DrawQTs() => BlmQTUI.Draw();

    public void DrawSettings() => BlmSettingsUI.Draw();

    // ---- IRotationLifecycle ----
    public void OnEnterAcr() { }

    public void OnExitAcr() { }
}
