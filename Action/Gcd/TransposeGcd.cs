// ============================================================
// TransposeGcd - 星灵移位（双向：转火 / 转冰）
// 优先级 #14：对齐 Los（CheckSingleTargetTranspose）：
//   火阶段：MP<800 且星魂≠6（先耀星）-> 转冰回蓝
//   冰阶段：UI3 + 心满 + MP≥95%（或上次冰澈）-> 转火（省 2000MP，输出至上！）
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

        // ---- 火 -> 冰（Los：InFire && MP<800 && 星魂≠6 -> 转冰）----
        if (gauge.InAstralFire)
        {
            if (mp >= 800)
                return new CheckResult(false, $"还有MP({mp})打绝望");
            if (gauge.AstralSoulStacks >= 6)
                return new CheckResult(false, "星魂满先耀星");
            return new CheckResult(true, "MP耗尽转冰回蓝");
        }

        // ---- 冰 -> 火（Los：IsSingleTargetIceReadyToTranspose）----
        if (gauge.InUmbralIce)
        {
            // UI 未满 3 或 心未满：先暴雷/冰澈攒好再转
            if (gauge.UmbralIceStacks < 3)
                return new CheckResult(false, "UI层未满");
            if (gauge.UmbralHearts < 3)
                return new CheckResult(false, "心未攒满");
            // 悖论在手：Los 先打悖论再转
            if (gauge.IsParadoxActive)
                return new CheckResult(false, "先打悖论");
            // MP 回满才转火（Los：MP >= 95% MaxMp；转火后满蓝打满 6 发炽炎）
            var me = BlmGcdHelper.Me;
            uint maxMp = me?.MaxMp ?? 10000;
            if (mp < maxMp * 0.95f)
                return new CheckResult(false, $"MP未回满({mp}/{maxMp})");
            return new CheckResult(true, "心满蓝满转火");
        }

        return new CheckResult(false, "无元素阶段");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.Transpose);
}
