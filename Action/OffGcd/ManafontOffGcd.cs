// ============================================================
// ManafontOffGcd - 魔泉（oGCD #2）
// 常规循环续命用（对齐 Los/The Balance）：
//   火阶段 MP 见底(<800)且星魂未满时开启，多打 1-2 发炽炎；
//   起手爆发魔泉由起手序列负责（5+7 第 5 发后），这里不重复开。
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
        // 星魂满 6：不需要续（该转冰/耀星了）
        if (BlmGcdHelper.Gauge.AstralSoulStacks >= 6)
            return new CheckResult(false, "星魂已满无需续");
        // 火层未满 3（转火中）不开
        if (BlmGcdHelper.Gauge.AstralFireStacks < 3)
            return new CheckResult(false, "火层未满");
        // MP 见底才开（续命多打 1-2 发），MP 充足时留给转冰
        if (BlmGcdHelper.Mp >= 800)
            return new CheckResult(false, $"MP充足({BlmGcdHelper.Mp})无需魔泉");
        if (!ActionHelper.IsReady(BlmSkills.Manafont))
            return new CheckResult(false, "魔泉CD");
        return new CheckResult(true, "MP见底续命");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Ocgd(BlmSkills.Manafont);
}
