// ============================================================
// BlmOpener - 开场爆发器（Lv.100 标准 5+7 起手）
// 结构对齐 Los（The Balance 7.4 实战实现）+ The Balance 攻略：
//   预读爆炎(-4s) -> 高雷云 -> 迅捷 -> 详述 -> 炽炎#1 -> 黑魔纹
//     -> 炽炎#2#3#4#5 -> 魔泉 -> 炽炎#6 -> 耀星#1
//     -> 炽炎#7#8 -> 高雷云刷新(用魔泉给的雷首) -> 炽炎#9#10#11
//     -> 三连 -> 炽炎#12 -> 耀星#2 -> 绝望 -> 转冰 -> [交给常规循环]
// 要点：
//   - oGCD 绝不连续（每个前都有 GCD 缓冲，解决"一起摁卡手"）
//   - 迅捷给炽炎#1，三连给末端 炽炎#12/耀星#2/绝望（爆发瞬发）
//   - 魔泉：第 5 发后开，同时提供雷首给火中段高雷云刷新
//   - 异言：起手前有灵极魂才织入（防队列卡死），没有则跳过由决策器管
//   - 转冰后立即结束序列，资源循环（暴雷/冰澈/回蓝/转火）交还常规决策器
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

    public string OpenerName => "Lv.100 标准 5+7";

    public void InitializeCountdown(CountDownHandler handler)
    {
        // 雷首（Thunderhead）由"无元素 -> 火元素"切换自动获得，无需预读雷
        handler.AddAction(PrecastMs, new PAction(BlmSkills.FireIII, ActionType.Gcd, ActionTargetType.Target));
    }

    public List<PAction> InCombatSequence
    {
        get
        {
            var s = new List<PAction>();

            // 保底：开怪瞬间无雷首时先打一发雷系拿雷首
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

            // ---- 火阶段（魔泉前 5 发 + 魔泉后 7 发 = 12 发炽炎，2 次耀星）----
            s.Add(Gcd(BlmSkills.HighThunder));          // 高雷云 DoT（瞬发，耗开怪雷首）
            s.Add(OffGcd(BlmSkills.Swiftcast));         // 迅捷：给炽炎#1（迅捷 1 层）
            s.Add(OffGcd(BlmSkills.Amplifier));         // 详述：CD 60s 早开早转（前有迅捷后炽炎#1 缓冲）
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #1（迅捷瞬发）
            s.Add(OffGcd(BlmSkills.LeyLines));          // 黑魔纹（前有炽炎#1 缓冲）
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #2（读条）
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #3
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #4
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #5

            // 起手前已有灵极魂（战斗间隙积累）时，织入异言吃到团辅窗口；
            // 没有则跳过（防队列卡死），交给常规决策器防溢出
            if (me != null && BlmGcdHelper.Gauge.PolyglotStacks > 0)
                s.Add(OffGcd(BlmSkills.Xenoglossy));    // 异言（灵极魂）

            s.Add(OffGcd(BlmSkills.Manafont));          // 魔泉：回满 + 提供雷首
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #6（星魂 6 → 耀星可用）
            s.Add(Gcd(BlmSkills.FlareStar));            // 耀星 #1
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #7
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #8
            s.Add(Gcd(BlmSkills.HighThunder));          // 高雷云刷新（用魔泉给的雷首）
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #9
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #10
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #11
            s.Add(OffGcd(BlmSkills.Triplecast));        // 三连：给末端爆发（#12/耀星#2/绝望 瞬发）
            s.Add(Gcd(BlmSkills.FireIV));               // 炽炎 #12（星魂 6 → 耀星可用）
            s.Add(Gcd(BlmSkills.FlareStar));            // 耀星 #2
            s.Add(Despair());                            // 绝望收尾（对齐 Los：不验证直接放）
            s.Add(OffGcd(BlmSkills.Transpose));         // 转冰（前有绝望缓冲）
            // 序列到此结束：暴雷/冰澈/回蓝/转火/DoT 维护全部交还常规决策器

            return s;
        }
    }

    // 起手动作全部带施放验证：失败时框架等待重试（动画锁定/时机未到），不跳过
    private static PAction Gcd(uint id)
        => new(id, ActionType.Gcd, ActionTargetType.Target) { RequiresVerification = true };

    // 绝望：对齐 Los（level>=100 跳过验证，直接放；失败快速跳过不卡队列）
    private static PAction Despair()
        => new(BlmSkills.Despair, ActionType.Gcd, ActionTargetType.Target);

    private static PAction OffGcd(uint id)
        => new(id, ActionType.OffGcd, ActionTargetType.Self) { RequiresVerification = true };
}
