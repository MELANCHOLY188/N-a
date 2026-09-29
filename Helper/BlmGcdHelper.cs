// ============================================================
// BlmGcdHelper - 决策器共享静态工具
// 量谱 / 玩家 / 目标 / DoT / 敌数 / AOE 场景判断
// ============================================================
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

    /// <summary>目标指定 DoT（3871 高雷云 / 3872 高雷二）是否需要刷新</summary>
    public static bool DotNeedsRefresh(IBattleChara? target, ushort dotStatusId)
    {
        if (target == null || target.IsDead) return false;
        float threshold = BlmSettings.Instance.DotRefreshSeconds;
        foreach (var s in target.StatusList)
        {
            if (s.StatusId == dotStatusId)
                return s.RemainingTime < threshold;
        }
        return true; // 目标身上没有该 DoT
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
