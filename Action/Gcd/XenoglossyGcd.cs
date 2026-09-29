// ============================================================
// XenoglossyGcd - 单体异言（灵极魂防溢出）
// 优先级 #3：对齐 Los（CheckXenoglossy）：
//   灵极魂满 2 层 -> 异言（防溢出；上限 2 层，3 层是 98+ 悖论体系的写法，
//   实测 100 级上限 2，故 >=2 即打）
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
        if (BlmGcdHelper.Gauge.PolyglotStacks >= 2)
            return new CheckResult(true, $"就绪({BlmGcdHelper.Gauge.PolyglotStacks}/2满)");
        return new CheckResult(false, $"灵极魂{BlmGcdHelper.Gauge.PolyglotStacks}/2未满");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.Xenoglossy);
}
