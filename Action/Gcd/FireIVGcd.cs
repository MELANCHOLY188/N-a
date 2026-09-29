// ============================================================
// FireIVGcd - 炽炎（AF 单体）★核心输出
// 优先级 #12：对齐 Los（SelectFireGcd）：
//   星魂<6 且 MP 预留足够（能留出绝望的 MP）才打；
//   星魂满 6 -> 耀星接管；MP 低 -> 绝望/暴雷接管
// Los：IsLowMpForFire4 = MP < (无心?2400:1600)（有心 800/发，无心 1600/发，均留 800 给绝望）
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
        if (BlmGcdHelper.Gauge.AstralFireStacks < 3)
            return new CheckResult(false, "火层未满（转火中）");
        // 星魂满 6：先耀星，炽炎让位（对齐 Los flag2）
        if (BlmGcdHelper.Gauge.AstralSoulStacks >= 6)
            return new CheckResult(false, "星魂满打耀星");
        // Los 低MP判断：炽炎要留出绝望的 800MP
        bool hasHearts = BlmGcdHelper.Gauge.UmbralHearts > 0;
        uint minMp = hasHearts ? 1600u : 2400u; // 有心 800/发留800；无心 1600/发留800
        if (BlmGcdHelper.Mp < minMp)
            return new CheckResult(false, $"MP预留不足({BlmGcdHelper.Mp}/{minMp})");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.FireIV);
}
