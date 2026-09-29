// ============================================================
// BlmOpener - 开场爆发器（Standard 5+7 起手）
// 严格对齐 The Balance 7.4：
//   倒计时 -6s 雷（拿雷首）+ -4s 预读爆炎
//   开怪 高雷云 -> 三连/详述 -> 炽炎×5 -> 魔泉
//     -> 炽炎×1 -> 耀星#1 -> 炽炎×6 -> 耀星#2 -> 绝望 -> 转冰
//     -> 暴雷 -> 冰澈 -> 异言 -> 刷新高雷云 -> 常规循环
// 注：5+7 = 魔泉前 5 发 + 魔泉后 7 发共 12 发炽炎 = 两次耀星
// ============================================================
using PromeRotation.Core;
using PromeRotation.Data;
using PromeRotation.Rotation;

namespace BlmAcr.Opener;

public sealed class BlmOpener : IOpener
{
    private const int PrecastMs = 4000; // The Balance：开怪前 4 秒预读爆炎

    public string OpenerName => "Standard 5+7";

    public void InitializeCountdown(CountDownHandler handler)
    {
        // 倒计时 6 秒预读雷系技能：命中获得"雷首"(3870)，开怪后才能放高雷云
        var me = Core.Me;
        uint thunderId = BlmSkills.Thunder;
        if (me != null)
        {
            thunderId = me.Level >= 62 ? BlmSkills.ThunderIV
                      : me.Level >= 40 ? BlmSkills.ThunderIII
                      : BlmSkills.Thunder;
        }
        handler.AddAction(6000, new PAction(thunderId, ActionType.Gcd, ActionTargetType.Target));

        // 倒计时剩余 4 秒预读爆炎（The Balance：4s prepull，落地进 AF）
        handler.AddAction(PrecastMs, new PAction(BlmSkills.FireIII, ActionType.Gcd, ActionTargetType.Target));
    }

    public List<PAction> InCombatSequence => new()
    {
        // ---- 火阶段（5+7 = 12 发炽炎 + 2 次耀星 + 1 次绝望）----
        Gcd(BlmSkills.HighThunder),          // 1. 高雷云 DoT（瞬发，耗雷首）
        OffGcd(BlmSkills.Triplecast),        // 2. 三连（The Balance 起手用三连，迅捷留给移动）
        OffGcd(BlmSkills.Amplifier),         // 3. 详述
        Gcd(BlmSkills.FireIV),               // 4. 炽炎 #1（三连1）
        OffGcd(BlmSkills.LeyLines),          // 5. 黑魔纹（瞬发后织入）
        Gcd(BlmSkills.FireIV),               // 6. 炽炎 #2（三连2）
        Gcd(BlmSkills.FireIV),               // 7. 炽炎 #3（三连3）
        Gcd(BlmSkills.FireIV),               // 8. 炽炎 #4（读条）
        Gcd(BlmSkills.FireIV),               // 9. 炽炎 #5（读条）
        OffGcd(BlmSkills.Manafont),          // 10. 魔泉：第 5 发后，回满续后 7 发
        Gcd(BlmSkills.FireIV),               // 11. 炽炎 #6（星魂 6 → 耀星可用）
        Gcd(BlmSkills.FlareStar),            // 12. 耀星 #1
        Gcd(BlmSkills.FireIV),               // 13. 炽炎 #7
        Gcd(BlmSkills.FireIV),               // 14. 炽炎 #8
        Gcd(BlmSkills.FireIV),               // 15. 炽炎 #9
        Gcd(BlmSkills.FireIV),               // 16. 炽炎 #10
        Gcd(BlmSkills.FireIV),               // 17. 炽炎 #11
        Gcd(BlmSkills.FireIV),               // 18. 炽炎 #12（星魂 12 → 耀星可用）
        Gcd(BlmSkills.FlareStar),            // 19. 耀星 #2
        Gcd(BlmSkills.Despair),              // 20. 绝望收尾
        OffGcd(BlmSkills.Transpose),         // 21. 转冰

        // ---- 冰阶段（回蓝补资源，交给常规循环接管前的最小序列）----
        Gcd(BlmSkills.BlizzardIII),          // 22. 暴雷进 UI3（UI1 中读条无 30% 惩罚）
        Gcd(BlmSkills.BlizzardIV),           // 23. 冰澈拿 3 心 + 满蓝
        Gcd(BlmSkills.Xenoglossy),           // 24. 异言（灵极魂，起手团辅窗口内）
        Gcd(BlmSkills.HighThunder),          // 25. 刷新高雷云 DoT
        // 之后交给常规决策器循环
    };

    private static PAction Gcd(uint id)
        => new(id, ActionType.Gcd, ActionTargetType.Target);

    private static PAction OffGcd(uint id)
        => new(id, ActionType.OffGcd, ActionTargetType.Self);
}
