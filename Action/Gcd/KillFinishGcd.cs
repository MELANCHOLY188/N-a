// ============================================================
// KillFinishGcd - 斩杀收尾（绝望）
// 优先级 #8（AF 单体）：这一发绝望能不能收掉目标
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class KillFinishGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InAstralFire)
            return new CheckResult(false, "不在火阶段");
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景");
        if (!BlmQT.Enabled(BlmQT.斩杀收尾))
            return new CheckResult(false, "斩杀开关关");
        if (BlmKillCheck.ShouldFinishWithDespair(BlmGcdHelper.Target, inAF: true, mp: BlmGcdHelper.Mp))
            return new CheckResult(true, "就绪");
        return new CheckResult(false, "目标血量不足以斩杀");
    }

    public PromeRotation.Data.PAction GetAction() => BlmGcdHelper.Gcd(BlmSkills.Despair);
}
