// ============================================================
// XenoglossyGcd - 单体异言防溢出
// 优先级 #3（单体）：灵极魂满 3 层 -> 异言
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class XenoglossyGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        if (BlmGcdHelper.Gauge.PolyglotStacks >= 3)
            return new CheckResult(true, "就绪");
        return new CheckResult(false, $"灵极魂{BlmGcdHelper.Gauge.PolyglotStacks}/3未满");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.Xenoglossy);
}
