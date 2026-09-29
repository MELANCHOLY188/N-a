// ============================================================
// BlmOpener - 开场爆发器（Standard 5+7 起手）
// 严格对齐 The Balance 7.4：
//   倒计时 -4s 预读爆炎（雷首由"无元素->AF"自动获得，无需预读雷）
//   开怪 高雷云 -> 三连/详述 -> 炽炎×5 -> 魔泉
//     -> 炽炎×1 -> 耀星#1 -> 炽炎×6 -> 耀星#2 -> 绝望 -> 转冰
//     -> 暴雷 -> 冰澈 -> 异言 -> 刷新高雷云 -> 常规循环
// 注：5+7 = 魔泉前 5 发 + 魔泉后 7 发共 12 发炽炎 = 两次耀星
// ============================================================
using System.Collections.Generic;
using BlmAcr.Helper;
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
        // The Balance 7.4：起手不需要预读雷。
        // 雷首（Thunderhead）在"无元素 -> 获得火元素"时自动获得，
        // 预读爆炎落地即带雷首，开怪直接高雷云。
        handler.AddAction(PrecastMs, new PAction(BlmSkills.FireIII, ActionType.Gcd, ActionTargetType.Target));
    }

    public List<PAction> InCombatSequence
    {
        get
        {
            var s = new List<PAction>();

            // 保底：若开怪瞬间无雷首（爆炎未落地/异常），先打一发雷系拿雷首
            var me = Core.Me;
            bool hasThunderhead = me != null && BlmGcdHelper.HasStatus(me, BlmSkills.ThunderheadStatus);
            if (!hasThunderhead)
            {
                uint thunderId = BlmSkills.Thunder;
                if (me != null)
                {
                    thunderId = me.Level >= 62 ? BlmSkills.ThunderIV
                              : me.Level >= 40 ? BlmSkills.ThunderIII
                              : BlmSkills.Thunder;
                }
                s.Add(Gcd(thunderId));
            }

            // ---- 火阶段（5+7 = 12 发炽炎 + 2 次耀星 + 1 次绝望）----
            s.Add(Gcd(BlmSkills.HighThunder));          // 高雷云 DoT（瞬发，耗雷首）
            s.Add(OffGcd(BlmSkills.Triplecast));        // 三连（The Balance 起手用三连，迅捷留给移动）
            s.Add(OffGcd(BlmSkills.Amplifier));         // 详述
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #1（三连1）
            s.Add(OffGcd(BlmSkills.LeyLines));          // 黑魔纹（瞬发后织入）
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #2（三连2）
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #3（三连3）
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #4（读条）
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #5（读条）
            s.Add(OffGcd(BlmSkills.Manafont));          // 魔泉：第 5 发后，回满续后 7 发
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #6（星魂 6 → 耀星可用）
            s.Add(Gcd(BlmSkills.FlareStar));            // 耀星 #1
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #7
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #8
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #9
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #10
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #11
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #12（星魂 12 → 耀星可用）
            s.Add(Gcd(BlmSkills.FlareStar));            // 耀星 #2
            s.Add(Gcd(BlmSkills.Despair));              // 绝望收尾
            s.Add(OffGcd(BlmSkills.Transpose));         // 转冰

            // ---- 冰阶段（回蓝补资源，交给常规循环接管前的最小序列）----
            s.Add(Gcd(BlmSkills.BlizzardIII));          // 暴雷进 UI3（UI1 中读条无 30% 惩罚）
            s.Add(Gcd(BlmSkills.BlizzardIV));           // 冰澈拿 3 心 + 满蓝
            s.Add(Gcd(BlmSkills.Xenoglossy));           // 异言（灵极魂，起手团辅窗口内）
            s.Add(Gcd(BlmSkills.HighThunder));          // 刷新高雷云 DoT
            // 之后交给常规决策器循环

            return s;
        }
    }

    // 起手 GCD 带施放验证（RequiresVerification = true，官方模板标准做法）
    private static PAction Gcd(uint id)
        => new(id, ActionType.Gcd, ActionTargetType.Target) { RequiresVerification = true };

    private static PAction OffGcd(uint id)
        => new(id, ActionType.OffGcd, ActionTargetType.Self);
}
