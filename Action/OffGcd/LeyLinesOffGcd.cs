// ============================================================
// LeyLinesOffGcd - 黑魔纹（oGCD #3）
// CD 好就铺（不严格对齐爆发，影响所有 GCD 含瞬发）
// ============================================================
using PromeRotation.Resolvers;
using PromeRotation.Helpers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.OffGcd;

public class LeyLinesOffGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmQT.Enabled(BlmQT.自动爆发))
            return new CheckResult(false, "自动爆发关");
        if (!ActionHelper.IsReady(BlmSkills.LeyLines))
            return new CheckResult(false, "黑魔纹CD");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Ocgd(BlmSkills.LeyLines);
}
