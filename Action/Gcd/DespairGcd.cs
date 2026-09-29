// ============================================================
// DespairGcd - 绝望（AF 单体）
// 优先级 #13：对齐 Los（SelectFireGcd 末尾）：
//   星魂满先耀星（FlareStar 接管）；MP<800 由暴雷/转冰接管；
//   否则 -> 绝望清空 MP 收尾
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class DespairGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "不在火阶段");
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        if (BlmGcdHelper.Gauge.AstralFireStacks < 3)
            return new CheckResult(false, "火层未满");
        if (BlmGcdHelper.Mp < 800)
            return new CheckResult(false, $"MP不足({BlmGcdHelper.Mp}/800)");
        // 星魂满 6：先耀星，绝望让位（对齐 Los：flag2 -> 耀星优先）
        if (BlmGcdHelper.Gauge.AstralSoulStacks >= 6)
            return new CheckResult(false, "星魂满打耀星");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.Despair);
}
