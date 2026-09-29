// ============================================================
// IceUmbralGcd - UI 阶段单体分支（危险循环 / 正常循环）
// 优先级 #7：
//   危险循环：跳过攒心，直接回火赌火苗暴击续命
//   正常循环：暴雷升 UI3 回蓝 -> 冰澈攒心 -> 灵极魂回蓝 -> UI悖论 -> 爆炎进 AF3
// ============================================================
using PromeRotation.Resolvers;
using BlmAcr.Data;
using BlmAcr.Helper;

namespace BlmAcr.Action.Gcd;

public class IceUmbralGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (!BlmGcdHelper.Gauge.InUmbralIce)
            return new CheckResult(false, "不在冰阶段");
        if (BlmGcdHelper.AoeScene)
            return new CheckResult(false, "AOE场景交给AOE冰");
        return new CheckResult(true, "就绪");
    }

    public PromeRotation.Data.PAction GetAction()
    {
        var gauge = BlmGcdHelper.Gauge;
        uint mp = BlmGcdHelper.Mp;

        if (BlmQT.Enabled(BlmQT.危险循环))
        {
            // 危险循环：悖论不耗蓝先打掉；有蓝回火；没蓝灵极魂
            if (gauge.IsParadoxActive)
                return BlmGcdHelper.Gcd(BlmSkills.Paradox);
            if (mp >= 2000)
                return BlmGcdHelper.Gcd(BlmSkills.FireIII);
            return BlmGcdHelper.Gcd(BlmSkills.UmbralSoul);
        }

        // 转冰初期（UI1/2、MP 低）-> 暴雷升 UI3 回蓝（避免长时间灵极魂）
        if (gauge.UmbralIceStacks < 3 && mp < 3000)
            return BlmGcdHelper.Gcd(BlmSkills.BlizzardIII);

        // 攒三颗灵极之心（冰澈耗 800MP）
        if (gauge.UmbralHearts < 3 && mp >= 800)
            return BlmGcdHelper.Gcd(BlmSkills.BlizzardIV);

        // MP 不足 -> 灵极魂回蓝
        if (mp < 2000)
            return BlmGcdHelper.Gcd(BlmSkills.UmbralSoul);

        // UI 悖论不耗蓝，先打掉
        if (gauge.IsParadoxActive)
            return BlmGcdHelper.Gcd(BlmSkills.Paradox);

        // 准备就绪 -> 爆炎进 AF3
        return BlmGcdHelper.Gcd(BlmSkills.FireIII);
    }
}
