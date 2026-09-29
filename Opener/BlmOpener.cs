// ============================================================
// BlmOpener - 开场爆发器（Standard 5+7 起手）
// 对齐 The Balance 7.4 / Los BlmOpener57Definition：
// 倒计时 3.5s 预读爆炎 -> 开怪后 5 炽炎 + 魔泉 + 7 炽炎
//   -> 两次耀星 -> 绝望 -> 转冰回正常循环
// ============================================================
using PromeRotation.Core;
using PromeRotation.Data;
using PromeRotation.Rotation;

namespace BlmAcr.Opener;

public sealed class BlmOpener : IOpener
{
    private const int PrecastMs = 3500; // 预读提前量（毫秒）

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

        // 倒计时剩余 3.5 秒预读爆炎，开战瞬间处于 AF3
        handler.AddAction(PrecastMs, new PAction(BlmSkills.FireIII, ActionType.Gcd, ActionTargetType.Target));
    }

    public List<PAction> InCombatSequence => new()
    {
        // 进战后按序列逐发执行（oGCD 由框架在织入窗口自动放）
        Gcd(BlmSkills.HighThunder),          // 1. 开高雷云 DoT
        OffGcd(BlmSkills.Swiftcast),         // 2. 迅捷
        OffGcd(BlmSkills.Amplifier),         // 3. 详述
        Gcd(BlmSkills.FireIV),               // 4. 炽炎 #1
        OffGcd(BlmSkills.LeyLines),          // 5. 黑魔纹
        Gcd(BlmSkills.FireIV),               // 6. 炽炎 #2
        Gcd(BlmSkills.FireIV),               // 7. 炽炎 #3
        Gcd(BlmSkills.FireIV),               // 8. 炽炎 #4
        Gcd(BlmSkills.FireIV),               // 9. 炽炎 #5
        Gcd(BlmSkills.Xenoglossy),           // 10. 异言（织入窗口）
        OffGcd(BlmSkills.Manafont),          // 11. 魔泉：回满蓝，续后 7 发
        Gcd(BlmSkills.FireIV),               // 12. 炽炎 #6
        Gcd(BlmSkills.FlareStar),            // 13. 耀星 #1
        Gcd(BlmSkills.FireIV),               // 14. 炽炎 #7
        Gcd(BlmSkills.FireIV),               // 15. 炽炎 #8
        Gcd(BlmSkills.HighThunder),          // 16. 刷新高雷云 DoT（团辅末）
        Gcd(BlmSkills.FireIV),               // 17. 炽炎 #9
        Gcd(BlmSkills.FireIV),               // 18. 炽炎 #10
        Gcd(BlmSkills.FireIV),               // 19. 炽炎 #11
        OffGcd(BlmSkills.Triplecast),        // 20. 三连（瞬发后段）
        Gcd(BlmSkills.FireIV),               // 21. 炽炎 #12
        Gcd(BlmSkills.FlareStar),            // 22. 耀星 #2
        Gcd(BlmSkills.Despair),              // 23. 绝望收尾
        OffGcd(BlmSkills.Transpose),         // 24. 转冰，回到正常循环
    };

    private static PAction Gcd(uint id)
        => new(id, ActionType.Gcd, ActionTargetType.Target);

    private static PAction OffGcd(uint id)
        => new(id, ActionType.OffGcd, ActionTargetType.Self);
}
