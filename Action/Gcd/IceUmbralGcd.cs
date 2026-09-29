// ============================================================
// IceUmbralGcd - UI 阶段单体分支（危险循环 / 正常循环）
// 优先级 #7：对齐 Los（SelectIceGcd）：
//   UI<3 -> 暴雷升层回蓝；心<3 -> 冰澈攒心；
//   心满 -> 让位（交 Transpose 转火，省 2000MP）
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
        // 心已攒满：交给 Transpose 转火（对齐 Los：IsSingleTargetIceReadyToTranspose）
        if (BlmGcdHelper.Gauge.UmbralHearts >= 3)
            return new CheckResult(false, "心满待转火");
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

        // 转冰初期（UI1/2、MP 低）-> 暴雷升 UI3 回蓝（对齐 Los：UI<3 打暴雷 154）
        if (gauge.UmbralIceStacks < 3)
            return BlmGcdHelper.Gcd(BlmSkills.BlizzardIII);

        // 极端低 MP：灵极魂回蓝（保底；正常 UI 回蓝已足够）
        if (mp < 800)
            return BlmGcdHelper.Gcd(BlmSkills.UmbralSoul);

        // 心未满 -> 冰澈攒心（对齐 Los：3576）
        return BlmGcdHelper.Gcd(BlmSkills.BlizzardIV);
    }
}
