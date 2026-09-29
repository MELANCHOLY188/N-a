// ============================================================
// DefenseOffGcd - 自动减伤（oGCD #0）
// 血 <30% 自动魔罩（日随专属：高难建议手动管理减伤）
// ============================================================
using PromeRotation.Resolvers;
using PromeRotation.Helpers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.OffGcd;

public class DefenseOffGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmQT.Enabled(BlmQT.自动减伤))
            return new CheckResult(false, "自动减伤关");
        var me = BlmGcdHelper.Me;
        if (me == null || me.MaxHp == 0)
            return new CheckResult(false, "无玩家");
        if ((double)me.CurrentHp / me.MaxHp > 0.3)
            return new CheckResult(false, "血量充足");
        if (!ActionHelper.IsReady(BlmSkills.Manaward))
            return new CheckResult(false, "魔罩CD");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Ocgd(BlmSkills.Manaward);
}
