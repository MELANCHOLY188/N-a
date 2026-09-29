// ============================================================
// ManafontOffGcd - 魔泉（oGCD #1）
// 火阶段打不动炽炎时（MP < 1600）回满蓝 + 3心 + 雷首 + 悖论
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
        if (!BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "不在火阶段");
        if (BlmGcdHelper.Mp >= 1600)
            return new CheckResult(false, "MP充足");
        if (!ActionHelper.IsReady(BlmSkills.Manafont))
            return new CheckResult(false, "魔泉CD");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Ocgd(BlmSkills.Manafont);
}
