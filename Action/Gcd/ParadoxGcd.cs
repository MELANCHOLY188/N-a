// ============================================================
// ParadoxGcd - 悖论（AF 单体，瞬发，耗 1600MP，产出火苗）
// 优先级 #11：对齐 Los（SelectFireGcd）：
//   AF<3（转火初期）：有悖论 -> 先打悖论开场（生成火苗，The Balance 火苗赤字）
//   AF3：Fire4Count 3~5（炽炎中段）-> 悖论压缩（打完继续炽炎，不打断循环）
//   其余位置不打（避免开局悖论抢炽炎窗口）
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class ParadoxGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "不在火阶段");
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        if (!BlmGcdHelper.Gauge.IsParadoxActive)
            return new CheckResult(false, "无悖论");
        if (BlmGcdHelper.Mp < 1600)
            return new CheckResult(false, $"MP不足({BlmGcdHelper.Mp}/1600)");
        // Los 位置限制：AF3 时只在炽炎中段（3~5 发）打，避免抢开局/结尾窗口
        if (BlmGcdHelper.Gauge.AstralFireStacks >= 3)
        {
            int f4 = BlmState.FireIVCount;
            if (f4 < 3 || f4 >= 6)
                return new CheckResult(false, $"炽炎计数{f4}不在中段(3-5)");
        }
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.Paradox);
}
