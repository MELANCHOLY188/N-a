// ============================================================
// BlmGcdHelper - 决策器共享静态工具
// 量谱 / 玩家 / 目标 / DoT / 敌数 / AOE 场景判断
// ============================================================
using System;
using System.Numerics;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using PromeRotation.Core;
using PromeRotation.Data;
using BlmAcr.Data;

namespace BlmAcr.Helper;

public static class BlmGcdHelper
{
    public static BLMGauge Gauge => Svc.Gauges.Get<BLMGauge>();
    public static IBattleChara? Me => Core.Me;
    public static IBattleChara? Target => Core.Target;

    public static uint Mp => Me?.CurrentMp ?? 0;

    private static Vector3 _lastPos;
    private static bool _hasLastPos;

    /// <summary>移动检测：与上一帧位置比较，位移超过阈值视为移动中</summary>
    public static bool IsMovingNow
    {
        get
        {
            var me = Me;
            if (me == null) return false;
            var p = me.Position;
            if (!_hasLastPos)
            {
                _lastPos = p;
                _hasLastPos = true;
                return false;
            }
            bool moving = (p - _lastPos).LengthSquared() > 0.0001f; // 每帧位移 >1cm
            _lastPos = p;
            return moving;
        }
    }

    public static PAction Gcd(uint id) => new(id, ActionType.Gcd, ActionTargetType.Target);
    public static PAction Ocgd(uint id) => new(id, ActionType.OffGcd, ActionTargetType.Self);

    /// <summary>玩家身上是否有指定状态</summary>
    public static bool HasStatus(IBattleChara? c, ushort id)
    {
        if (c?.StatusList == null) return false;
        foreach (var s in c.StatusList)
            if (s.StatusId == id) return true;
        return false;
    }

    /// <summary>目标指定 DoT（3871 高雷云 / 3872 高雷二）是否需要刷新。
    /// 完全照抄 Los：RemainingMs = Abs(RemainingTime) * 1000f，阈值 3000ms（3 秒）；
    /// 单位无论秒/毫秒都成立；多条重复状态取最大剩余。</summary>
    public static bool DotNeedsRefresh(IBattleChara? target, ushort dotStatusId)
    {
        if (target == null || target.IsDead) return false;
        float thresholdMs = BlmSettings.Instance.DotRefreshSeconds * 1000f; // 3.0s -> 3000ms
        float maxRemMs = 0f;
        foreach (var s in target.StatusList)
        {
            if (s.StatusId == dotStatusId)
            {
                float remMs = Math.Abs(s.RemainingTime) * 1000f;
                if (remMs > maxRemMs) maxRemMs = remMs;
            }
        }
        if (maxRemMs <= 0f) return true; // 目标身上没有有效 DoT
        return maxRemMs < thresholdMs;   // 剩余低于 3 秒才刷新
    }

    /// <summary>以自身为圆心 radius 码内的可攻击敌人数量</summary>
    public static int EnemyCountNear(float radius)
    {
        var me = Me;
        if (me == null) return 0;
        try
        {
            return ObjectFunctions.GetAttackableEnemyCountAroundPoint(me.Position, radius);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>AOE 场景：QT 开启 + 5 码内 2 个以上敌人</summary>
    public static bool AoeScene => BlmQT.Enabled(BlmQT.AOE) && EnemyCountNear(5f) >= 2;
}
