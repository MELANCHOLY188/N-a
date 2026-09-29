// ============================================================
// ParadoxGcd - AF 悖论（火阶段）
// 优先级 #11：耗 1600MP，产出火苗（免费爆炎）
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class ParadoxGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "不在火阶段");
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        if (!BlmGcdHelper.Gauge.IsParadoxActive)
            return new CheckResult(false, "无悖论");
        if (BlmGcdHelper.Mp < 1600)
            return new CheckResult(false, $"MP不足({BlmGcdHelper.Mp}/1600)");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.Paradox);
}
