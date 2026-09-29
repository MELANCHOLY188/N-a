// ============================================================
// FirestarterGcd - 火苗免费爆炎（进 AF3 主循环）
// 优先级 #5：对齐 Los（SelectFireGcd AF<3：HasFirestarter -> 152）：
//   火苗在手（165）且不在 AF3 -> 免费爆炎进 AF3（悖论转火后 AF1 火苗 / UI 残留火苗）
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class FirestarterGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.HasStatus(BlmGcdHelper.Me, BlmSkills.FirestarterStatus))
            return new CheckResult(false, "无火苗");
        // AF3 已就绪：火苗留给下轮（Los AF<3 才用火苗爆炎）
        if (BlmGcdHelper.Gauge.InAstralFire && BlmGcdHelper.Gauge.AstralFireStacks >= 3)
            return new CheckResult(false, "AF3已满火苗待用");
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        return new CheckResult(true, "火苗就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.FireIII);
}
