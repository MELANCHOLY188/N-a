// ============================================================
// AoeFireGcd - AF 阶段 AOE 分支
// 优先级 #9：灵极魂满 6 -> 耀星；核爆清 MP；打不动转冰补蓝
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class AoeFireGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "不在火阶段");
        if (!BlmGcdHelper.AoeScene)
            return new CheckResult(false, "非AOE场景");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction()
    {
        var gauge = BlmGcdHelper.Gauge;
        // 灵极魂满 6 -> 耀星（AOE 场景也打，600 威力值得）
        if (gauge.AstralSoulStacks >= 6)
            return BlmGcdHelper.Gcd(BlmSkills.FlareStar);
        // 核爆：耗 MP（3 心减 1/3 消耗），双发由 UI 玄冰回满支撑
        if (BlmGcdHelper.Mp >= 1000)
            return BlmGcdHelper.Gcd(BlmSkills.Flare);
        // MP 打不动 -> 回冰补蓝
        return BlmGcdHelper.Gcd(BlmSkills.Transpose);
    }
}
