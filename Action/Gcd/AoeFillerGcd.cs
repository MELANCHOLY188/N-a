// ============================================================
// AoeFillerGcd - AOE 场景瞬发填充
// 优先级 #1（AOE）：高雷二 DoT 快掉先刷新；灵极魂 >=2 用灵极魂·AOE
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class AoeFillerGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.AoeScene)
            return new CheckResult(false, "非AOE场景");
        if (BlmGcdHelper.DotNeedsRefresh(BlmGcdHelper.Target, BlmSkills.HighThunderIIDot))
            return new CheckResult(true, "就绪");
        if (BlmGcdHelper.Gauge.PolyglotStacks >= 2)
            return new CheckResult(true, "就绪");
        return new CheckResult(false, "DoT健康且灵极魂<2");
    }

    public PromeRotation.Data.PAction GetAction()
    {
        if (BlmGcdHelper.DotNeedsRefresh(BlmGcdHelper.Target, BlmSkills.HighThunderIIDot))
            return BlmGcdHelper.Gcd(BlmSkills.HighThunderII);
        return BlmGcdHelper.Gcd(BlmSkills.Foul);
    }
}
