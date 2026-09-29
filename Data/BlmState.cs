// ============================================================
// BlmState - 决策共享运行状态
// 每帧由 BlmRotation.NextGcd 调用 UpdatePhase 同步 AF 阶段，
// FireIVCount 在每次进入新 AF 阶段时归零。
// LastOffGcdTime 用于 oGCD 节流（防两个能力技卡手）
// ============================================================
using System;

namespace BlmAcr.Data;

public static class BlmState
{
    private static int _fireIVCount;
    private static bool _afActive;

    /// <summary>上次成功返回 oGCD 的时刻（Environment.TickCount）</summary>
    public static int LastOffGcdTime { get; private set; }

    /// <summary>上次成功返回的 GCD 技能 id（Los PreviousGcdMatches 用）</summary>
    public static uint LastGcdId { get; private set; }

    /// <summary>当前 AF 阶段已打出的炽炎数量</summary>
    public static int FireIVCount => _fireIVCount;

    /// <summary>记录最近一次 GCD 动作（用于转火"上一发冰澈"判断）</summary>
    public static void MarkGcd(uint actionId) => LastGcdId = actionId;

    /// <summary>每帧同步 AF 阶段，进入新阶段时重置炽炎计数</summary>
    public static void UpdatePhase(bool inAF)
    {
        if (inAF && !_afActive) { _afActive = true; _fireIVCount = 0; }
        if (!inAF && _afActive) _afActive = false;
    }

    /// <summary>记录打出一发炽炎</summary>
    public static void CountFireIV() => _fireIVCount++;

    /// <summary>标记一个 oGCD 已发出（用于节流）</summary>
    public static void MarkOffGcd() => LastOffGcdTime = Environment.TickCount;
}
