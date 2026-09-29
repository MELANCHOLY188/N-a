// ============================================================
// ManafontOffGcd - 魔泉（oGCD #2）
// 火阶段已打 2 发炽炎后开启：回满蓝，让火阶段从 6 发变 12 发
// （对齐 The Balance：魔泉=延长火阶段，不是等 MP 打空）
// ============================================================
using PromeRotation.Resolvers;
using PromeRotation.Helpers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.OffGcd;

public class ManafontOffGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmQT.Enabled(BlmQT.自动爆发))
            return new CheckResult(false, "自动爆发关");
        if (!BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "不在火阶段");
        // 至少打满 2 发炽炎再开（保证后半段 6 发能吃完回蓝）
        if (BlmState.FireIVCount < 2)
            return new CheckResult(false, $"炽炎{BlmState.FireIVCount}/2未到开启时机");
        if (BlmGcdHelper.Mp < 8000 && ActionHelper.IsReady(BlmSkills.Manafont))
            return new CheckResult(true, "就绪");
        return new CheckResult(false, "MP充足或魔泉CD");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Ocgd(BlmSkills.Manafont);
}
