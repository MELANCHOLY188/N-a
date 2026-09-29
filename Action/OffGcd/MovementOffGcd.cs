// ============================================================
// MovementOffGcd - 移动瞬发（oGCD #1）
// 移动中自动开三连/迅捷，保 GCD 不因读条断
// ============================================================
using PromeRotation.Resolvers;
using PromeRotation.Helpers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.OffGcd;

public class MovementOffGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmQT.Enabled(BlmQT.移动瞬发))
            return new CheckResult(false, "移动瞬发关");
        var me = BlmGcdHelper.Me;
        if (me == null || !BlmGcdHelper.IsMovingNow)
            return new CheckResult(false, "未移动");
        if (ActionHelper.IsReady(BlmSkills.Triplecast))
            return new CheckResult(true, "就绪");
        if (ActionHelper.IsReady(BlmSkills.Swiftcast))
            return new CheckResult(true, "就绪");
        return new CheckResult(false, "瞬发技能CD中");
    }

    public PromeRotation.Data.PAction GetAction()
    {
        if (ActionHelper.IsReady(BlmSkills.Triplecast))
            return BlmGcdHelper.Ocgd(BlmSkills.Triplecast);
        return BlmGcdHelper.Ocgd(BlmSkills.Swiftcast);
    }
}
