// ============================================================
// ThunderDotGcd - 单体雷 DoT 维护
// 优先级 #2（单体，输出至上：DoT 不能掉，先于一切）
// 有雷首(3870) -> 高雷云(消耗雷首，瞬发，DoT 30s)
// 无雷首      -> 雷系技能拿雷首（等级对应闪雷/霹雷/震雷）
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class ThunderDotGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmQT.Enabled(BlmQT.雷DoT维持))
            return new CheckResult(false, "雷DoT维持关");
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景交给AOE填充");
        // 火阶段不做雷维护：无雷首时闪雷是读条，会打断爆发；转冰后第一时间补
        if (BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "火阶段优先爆发，转冰后补雷");
        if (BlmGcdHelper.DotNeedsRefresh(BlmGcdHelper.Target, BlmSkills.HighThunderDot))
            return new CheckResult(true, "就绪");
        return new CheckResult(false, "高雷云DoT健康");
    }

    public PromeRotation.Data.PAction GetAction()
    {
        // 有雷首 -> 高雷云；无雷首 -> 先打雷系技能拿雷首
        if (BlmGcdHelper.HasStatus(BlmGcdHelper.Me, BlmSkills.ThunderheadStatus))
            return BlmGcdHelper.Gcd(BlmSkills.HighThunder);

        var me = BlmGcdHelper.Me;
        uint thunderId = BlmSkills.Thunder;
        if (me != null)
        {
            thunderId = me.Level >= 62 ? BlmSkills.ThunderIV
                      : me.Level >= 40 ? BlmSkills.ThunderIII
                      : BlmSkills.Thunder;
        }
        return BlmGcdHelper.Gcd(thunderId);
    }
}
