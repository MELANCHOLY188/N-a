// ============================================================
// BlmAcr - Black Mage Action Rotation for PromeRotation
// 技能 ID 常量表（来自游戏数据 / Los.dll BLMSkill 枚举）
// ============================================================
namespace BlmAcr;

public static class BlmSkills
{
    // ---- 火系 ----
    public const uint Fire          = 141;   // 火炎
    public const uint FireII        = 147;   // 烈炎
    public const uint FireIII       = 152;   // 爆炎
    public const uint FireIV        = 3577;  // 炽炎
    public const uint Flare         = 162;   // 核爆
    public const uint Despair       = 16505; // 绝望
    public const uint FlareStar     = 36989; // 耀星
    public const uint HighFireII    = 25794; // 高烈炎

    // ---- 冰系 ----
    public const uint Blizzard      = 142;   // 冰结
    public const uint BlizzardII    = 154;   // 冰封
    public const uint BlizzardIII   = 153;   // 暴雷
    public const uint BlizzardIV    = 3576;  // 冰澈
    public const uint Freeze        = 159;   // 玄冰

    // ---- 雷系 ----
    public const uint Thunder       = 144;   // 闪雷
    public const uint ThunderIII    = 7420;  // 霹雷
    public const uint ThunderIV     = 7447;  // 震雷
    public const uint HighThunder   = 36986; // 高雷云（单体 DoT，消耗雷首）
    public const uint HighThunderII = 36987; // 高雷二（AOE DoT）

    // ---- 通用/功能 ----
    public const uint Transpose     = 149;   // 星灵移位
    public const uint Manafont      = 158;   // 魔泉
    public const uint Manaward      = 157;   // 魔罩（减伤30%）
    public const uint LeyLines      = 3573;  // 黑魔纹
    public const uint Triplecast    = 7421;  // 三连咏唱
    public const uint Swiftcast     = 7561;  // 迅捷咏唱
    public const uint UmbralSoul    = 16506; // 灵极魂
    public const uint Xenoglossy    = 16507; // 异言
    public const uint Amplifier     = 25796; // 详述
    public const uint Foul          = 7422;  // 灵极魂·AOE（AOE 异言）
    public const uint Paradox       = 25797; // 悖论
    public const uint Retrace       = 36988; // 魔纹重置

    // ---- 状态 ----
    public const ushort FirestarterStatus = 165;  // 火苗（Firestarter，Los/官方实证 165，非 481）
    public const ushort SwiftcastBuff = 167;      // 迅捷咏唱
    public const ushort TriplecastBuff = 1211;    // 三连咏唱
    public const ushort ThunderheadStatus = 3870; // 雷首（Thunderhead，自己身上 30s）
    public const ushort HighThunderDot    = 3871; // 高雷云 DoT（目标身上 30s）
    public const ushort HighThunderIIDot  = 3872; // 高雷二 DoT（目标身上 24s）
}
