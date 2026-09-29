// ============================================================
// ThunderDotGcd - 单体高雷云 DoT 刷新
// 优先级 #2（单体，输出至上：DoT 不能掉，先于一切）
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class ThunderDotGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景交给AOE填充");
        if (BlmGcdHelper.DotNeedsRefresh(BlmGcdHelper.Target, BlmSkills.HighThunderDot))
            return new CheckResult(true, "就绪");
        return new CheckResult(false, "高雷云DoT健康");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.HighThunder);
}
