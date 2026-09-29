// ============================================================
// BlizzardIIIGcd - 无元素状态进入冰 III
// 优先级 #4：既不在火也不在冰 -> 暴雷进 UI
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class BlizzardIIIGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "在火阶段");
        if (BlmGcdHelper.Gauge.InUmbralIce)
            return new CheckResult(false, "在冰阶段");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.BlizzardIII);
}
