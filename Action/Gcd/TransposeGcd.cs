// ============================================================
// TransposeGcd - 星灵移位（双向：转火 / 转冰）
// 优先级 #14：照抄 Los（CheckSingleTargetTranspose）：
//   火->冰：MP<800 且星魂≠6 且【转冰后有瞬发保证】(迅捷/三连)
//            否则不转（The Balance：没瞬发就用暴雷读条转冰，由 Despair/暴雷兜底）
//   冰->火：UI3 + 心3 + （上一发是冰澈【直接转】 或 MP≥95%）
//            有悖论先打悖论再转
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class TransposeGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        var gauge = BlmGcdHelper.Gauge;
        uint mp = BlmGcdHelper.Mp;

        // ---- 火 -> 冰（Los：MP<800 && 星魂≠6 && 瞬发保证 -> 转冰）----
        if (gauge.InAstralFire)
        {
            if (mp >= 800)
                return new CheckResult(false, $"还有MP({mp})打绝望");
            if (gauge.AstralSoulStacks >= 6)
                return new CheckResult(false, "星魂满先耀星");
            // Los HasFireToIceInstantAssurance：转冰后要能瞬发冰系（暴雷免 AF3 惩罚）
            if (!BlmGcdHelper.FireToIceInstantAssurance)
                return new CheckResult(false, "无瞬发保证(迅捷/三连)不转");
            return new CheckResult(true, "MP耗尽+瞬发保证转冰");
        }

        // ---- 冰 -> 火（Los：IsSingleTargetIceReadyToTranspose）----
        if (gauge.InUmbralIce)
        {
            if (gauge.UmbralIceStacks < 3)
                return new CheckResult(false, "UI层未满");
            if (gauge.UmbralHearts < 3)
                return new CheckResult(false, "心未攒满");
            // 悖论在手：Los 先打悖论再转（悖论冰阶段打 -> 转火+火苗）
            if (gauge.IsParadoxActive)
                return new CheckResult(false, "先打悖论");
            // Los：上一发是冰澈（心刚好满）-> 直接转火；否则要 MP≥95%
            bool lastWasBlizzardIV = BlmState.LastGcdId == BlmSkills.BlizzardIV;
            if (!lastWasBlizzardIV)
            {
                var me = BlmGcdHelper.Me;
                uint maxMp = me?.MaxMp ?? 10000;
                if (mp < maxMp * 0.95f)
                    return new CheckResult(false, $"MP未回满({mp}/{maxMp})");
            }
            return new CheckResult(true, lastWasBlizzardIV ? "冰澈后心满直接转火" : "心满蓝满转火");
        }

        return new CheckResult(false, "无元素阶段");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.Transpose);
}
