// ============================================================
// BlmKillCheck - 斩杀判断器
// 判断「当前这一发绝望能不能收掉 boss」。
//
// 伤害模型（7.x 近似威力，按 The Balance 思路）：
//   Despair 威力 380 × AF3 加成 1.8 = 684 等效威力
//   黑魔循环平均等效威力 ≈ 450（炽炎310×1.8=558 / 悖论500×1.8=900 加权）
//   因此 绝望伤害 ≈ 每 GCD 平均伤害 × (684 / 450) ≈ × 1.5
//   再乘 0.8 保守系数，避免「差一点没打死」浪费整个火阶段。
// 当观测数据不足时，回退到「目标血量 2%」的静态阈值。
// ============================================================
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using BlmAcr.Data;

namespace BlmAcr;

public static class BlmKillCheck
{
    private const double DespairPotency = 380.0;      // 绝望基础威力
    private const double Af3Multiplier = 1.8;          // AF3 火系加成
    private const double AvgGcdPotency = 450.0;        // 黑魔循环平均等效威力（近似）
    private static double _despairEquiv = DespairPotency * Af3Multiplier;

    /// <summary>
    /// 是否应当用绝望收尾：
    /// 火阶段 + 有 MP + 目标剩余血量 ≤ 一发绝望的估算伤害。
    /// </summary>
    public static bool ShouldFinishWithDespair(IBattleChara? target, bool inAF, uint mp)
    {
        if (!inAF || mp <= 0) return false;
        if (target == null || target.MaxHp == 0) return false;

        // 保守系数 / 回退阈值来自设置面板（可游戏内调）
        double safety = BlmSettings.Instance.KillSafety;
        double fallback = BlmSettings.Instance.KillFallbackShare;

        double gcdDmg = BlmDamageWatcher.GcdDamageEstimate();
        double despairDmg;

        if (gcdDmg > 0)
        {
            // 动态估算：绝望 ≈ GCD 平均伤害 × 等效威力比 × 保守系数
            despairDmg = gcdDmg * (_despairEquiv / AvgGcdPotency) * safety;
        }
        else
        {
            // 回退：目标总血量的百分比
            despairDmg = target.MaxHp * fallback;
        }

        double hpLeft = target.CurrentHp;
        bool canKill = hpLeft <= despairDmg;

        // 调试输出：方便校准模型
        Svc.Log.Debug(
            $"[BlmKill] target={target.Name} hpLeft={hpLeft} ({hpLeft * 100.0 / target.MaxHp:F2}%) " +
            $"despairEst={despairDmg:F0} gcdAvg={gcdDmg:F0} => {(canKill ? "KILL" : "keep-loop")}");

        return canKill;
    }
}
