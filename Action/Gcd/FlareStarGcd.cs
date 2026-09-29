// ============================================================
// FlareStarGcd - 耀星（AF 单体）
// 优先级 #10：灵极魂满 6 -> 耀星
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class FlareStarGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "不在火阶段");
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        if (BlmGcdHelper.Gauge.AstralSoulStacks < 6)
            return new CheckResult(false, $"耀星{BlmGcdHelper.Gauge.AstralSoulStacks}/6未满");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.FlareStar);
}
