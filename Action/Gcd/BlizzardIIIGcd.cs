// ============================================================
// BlizzardIIIGcd - 进入冰 III（转冰）
// 优先级 #4：照抄 Los/The Balance 转冰路径：
//   无元素（死亡复活/起手）-> 暴雷进 UI
//   火阶段 MP<800 且无瞬发保证 -> 暴雷读条转冰（兜底，Los SelectFireGcd MP<800 -> 冰系）
//   有瞬发保证时由 TransposeGcd 星灵移位转冰（免 AF3 暴雷 30% 惩罚）
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class BlizzardIIIGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        var gauge = BlmGcdHelper.Gauge;

        // 无元素（脱战/死亡复活/起手）：暴雷进冰
        if (!gauge.InAstralFire && !gauge.InUmbralIce)
            return new CheckResult(true, "无元素进冰");

        // 火阶段 MP 打干 + 无瞬发保证：暴雷读条转冰（兜底，避免卡死在火阶段）
        if (gauge.InAstralFire && gauge.AstralFireStacks >= 3)
        {
            if (BlmGcdHelper.Mp >= 800)
                return new CheckResult(false, $"还有MP({BlmGcdHelper.Mp})");
            if (gauge.AstralSoulStacks >= 6)
                return new CheckResult(false, "星魂满先耀星");
            // 有瞬发保证：让 TransposeGcd 星灵移位转冰（免 AF3 暴雷 30% 惩罚）
            if (BlmGcdHelper.FireToIceInstantAssurance)
                return new CheckResult(false, "有瞬发由星灵移位转冰");
            // 有火苗：先让 FirestarterGcd 免费爆炎（伤害白赚）再转冰
            if (BlmGcdHelper.HasStatus(BlmGcdHelper.Me, BlmSkills.FirestarterStatus))
                return new CheckResult(false, "火苗先爆炎");
            return new CheckResult(true, "MP耗尽无瞬发暴雷转冰");
        }

        // 冰阶段/其他：交给 IceUmbral/Transpose 处理
        return new CheckResult(false, "不在火阶段");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.BlizzardIII);
}
