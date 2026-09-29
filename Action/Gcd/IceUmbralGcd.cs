// ============================================================
// IceUmbralGcd - UI 阶段单体分支（危险循环 / 正常循环）
// 优先级 #7：对齐 Los（SelectLevel72SingleTargetGcd）：
//   正常循环：转冰后 心<3 -> 冰澈攒心；心满 -> 爆炎直接转火（不灵极魂、不悖论）
//   危险循环：跳过攒心，直接回火赌火苗暴击续命
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

        // 转冰初期（UI1/2、MP 低）-> 暴雷升 UI3 回蓝（对齐 Los：UI<3 打暴雷）
        if (gauge.UmbralIceStacks < 3 && mp < 3000)
            return BlmGcdHelper.Gcd(BlmSkills.BlizzardIII);

        // 心已满 -> 爆炎直接转火（对齐 Los：152u，不灵极魂不悖论）
        if (gauge.UmbralHearts >= 3)
            return BlmGcdHelper.Gcd(BlmSkills.FireIII);

        // 攒心：冰澈（耗 800MP），有蓝就打
        if (mp >= 800)
            return BlmGcdHelper.Gcd(BlmSkills.BlizzardIV);

        // 实在没蓝才灵极魂回蓝
        return BlmGcdHelper.Gcd(BlmSkills.UmbralSoul);
    }
}
