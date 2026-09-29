// ============================================================
// FirestarterGcd - UI 阶段火苗免费爆炎
// 优先级 #5：冰阶段攒好资源后，火苗 -> 免费爆炎回 AF
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class FirestarterGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InUmbralIce)
            return new CheckResult(false, "不在冰阶段");
        if (!BlmGcdHelper.HasStatus(BlmGcdHelper.Me, BlmSkills.FirestarterStatus))
            return new CheckResult(false, "无火苗");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.FireIII);
}
