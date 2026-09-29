// ============================================================
// XenoglossyGcd - 单体异言（灵极魂防溢出）
// 优先级 #3：照抄 Los（CheckXenoglossy）：
//   灵极魂满层（<98 上限 2；>=98 上限 3）-> 异言防溢出
//   （The Balance：Polyglot Lv80+ 上限 2，Lv98+ 上限 3）
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
        int stacks = BlmGcdHelper.Gauge.PolyglotStacks;
        // Los：<98 上限 2 防溢出；>=98 上限 3（等级不够 98 时按 2 算）
        int max = BlmGcdHelper.Level >= 98 ? 3 : 2;
        if (stacks >= max)
            return new CheckResult(true, $"就绪({stacks}/{max}满)");
        return new CheckResult(false, $"灵极魂{stacks}/{max}未满");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.Xenoglossy);
}
