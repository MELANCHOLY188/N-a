// ============================================================
// AmplifierOffGcd - 详述（oGCD #2）
// CD 好且灵极魂 <2 层（+1 不溢出）
// ============================================================
using PromeRotation.Resolvers;
using PromeRotation.Helpers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.OffGcd;

public class AmplifierOffGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmQT.Enabled(BlmQT.自动爆发))
            return new CheckResult(false, "自动爆发关");
        if (BlmGcdHelper.Gauge.PolyglotStacks >= 2)
            return new CheckResult(false, $"灵极魂{BlmGcdHelper.Gauge.PolyglotStacks}/2会溢出");
        if (!ActionHelper.IsReady(BlmSkills.Amplifier))
            return new CheckResult(false, "详述CD");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Ocgd(BlmSkills.Amplifier);
}
