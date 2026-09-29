// ============================================================
// AoeFillerGcd - AOE 场景瞬发填充
// 优先级 #1（AOE）：高雷二 DoT 快掉先刷新（需雷首3870）；灵极魂 >=2 用灵极魂·AOE
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
        // 高雷二需要雷首：有雷首才刷新 DoT（且雷DoT维持开关开启）
        if (BlmQT.Enabled(BlmQT.雷DoT维持)
            && BlmGcdHelper.DotNeedsRefresh(BlmGcdHelper.Target, BlmSkills.HighThunderIIDot)
            && BlmGcdHelper.HasStatus(BlmGcdHelper.Me, BlmSkills.ThunderheadStatus))
            return new CheckResult(true, "就绪");
        if (BlmGcdHelper.Gauge.PolyglotStacks >= 2)
            return new CheckResult(true, "就绪");
        return new CheckResult(false, "DoT健康(或无雷首)且灵极魂<2");
    }

    public PromeRotation.Data.PAction GetAction()
    {
        if (BlmGcdHelper.DotNeedsRefresh(BlmGcdHelper.Target, BlmSkills.HighThunderIIDot)
            && BlmGcdHelper.HasStatus(BlmGcdHelper.Me, BlmSkills.ThunderheadStatus))
            return BlmGcdHelper.Gcd(BlmSkills.HighThunderII);
        return BlmGcdHelper.Gcd(BlmSkills.Foul);
    }
}
