// ============================================================
// AoeIceGcd - UI 阶段 AOE 分支
// 优先级 #6：玄冰攒心回蓝（代替冰澈/灵极魂；UI1 一发够支撑双核爆）
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class AoeIceGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InUmbralIce)
            return new CheckResult(false, "不在冰阶段");
        if (!BlmGcdHelper.AoeScene)
            return new CheckResult(false, "非AOE场景");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction()
    {
        var gauge = BlmGcdHelper.Gauge;
        if (gauge.UmbralHearts < 3 || BlmGcdHelper.Mp < 2000)
            return BlmGcdHelper.Gcd(BlmSkills.Freeze);
        if (gauge.IsParadoxActive)
            return BlmGcdHelper.Gcd(BlmSkills.Paradox);
        return BlmGcdHelper.Gcd(BlmSkills.FireIII);
    }
}
