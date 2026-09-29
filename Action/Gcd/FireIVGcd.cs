// ============================================================
// FireIVGcd - 炽炎（AF 单体）
// 优先级 #12：有灵极之心时 800MP，否则 1600MP；每 AF 阶段最多 6 发
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class FireIVGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "不在火阶段");
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        if (BlmState.FireIVCount >= 6)
            return new CheckResult(false, $"炽炎{BlmState.FireIVCount}/6已满");
        int cost = BlmGcdHelper.Gauge.UmbralHearts > 0 ? 800 : 1600;
        if (BlmGcdHelper.Mp < cost)
            return new CheckResult(false, $"MP不足({BlmGcdHelper.Mp}/{cost})");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction()
    {
        BlmState.CountFireIV();
        return BlmGcdHelper.Gcd(BlmSkills.FireIV);
    }
}
