// ============================================================
// BlmQT - QT 开关唯一数据源（对齐官方模板 InitQT）
// 设计原则：
//   1. 键名 + 默认值只在此定义（All 字典）
//   2. 新增 QT：加常量 + All 加一行 + ModeCategories 加一行
//   3. UI / 决策 / 模式管理自动同步
// ============================================================
using PromeRotation.Data;

namespace BlmAcr.Data;

/// <summary>QT 模式归属：通用 / 高难专属 / 日随专属</summary>
public enum BlmQtMode { Common, HighEndOnly, DailyOnly }

public static class BlmQT
{
    // === 键名常量 ===
    public const string 启用起手 = "启用起手";
    public const string 斩杀收尾 = "斩杀收尾";
    public const string 危险循环 = "危险循环";
    public const string AOE      = "AOE";
    public const string 雷DoT维持 = "雷DoT维持";
    public const string 自动爆发  = "自动爆发";
    public const string 移动瞬发  = "移动瞬发";
    public const string 自动减伤  = "自动减伤";
    public const string 停手      = "停手";
    public const string 高难模式  = "高难模式";

    // === 唯一数据源：键名 -> 默认值 ===
    public static readonly IReadOnlyDictionary<string, bool> All = new Dictionary<string, bool>
    {
        { 启用起手, true },    // 是否使用 5+7 起手
        { 斩杀收尾, true },    // 允许绝望斩杀
        { 危险循环, false },   // 赌暴击的激进循环
        { AOE,      true },    // 多目标自动 AOE
        { 雷DoT维持, true },   // 高雷云自动刷新
        { 自动爆发,  true },   // 魔泉/详述/黑魔纹 自动释放
        { 移动瞬发,  true },   // 移动中自动迅捷/三连
        { 自动减伤,  true },   // 血 <30% 自动魔罩
        { 停手,      false },  // 紧急停手（到时自动解除）
        { 高难模式,  true },   // 元数据键：模式切换（高难/日随）
    };

    // === 模式归属 ===
    public static readonly IReadOnlyDictionary<string, BlmQtMode> ModeCategories =
        new Dictionary<string, BlmQtMode>
        {
            { 启用起手, BlmQtMode.Common },
            { 斩杀收尾, BlmQtMode.Common },
            { 危险循环, BlmQtMode.Common },
            { AOE,      BlmQtMode.Common },
            { 雷DoT维持, BlmQtMode.Common },
            { 自动爆发,  BlmQtMode.Common },
            { 移动瞬发,  BlmQtMode.Common },
            { 自动减伤,  BlmQtMode.DailyOnly },   // 日随专属：高难手动管减伤
            { 停手,      BlmQtMode.Common },
            { 高难模式,  BlmQtMode.Common },
        };

    /// <summary>获取指定 key 的默认值（不存在返回 false）</summary>
    public static bool Default(string key) => All.TryGetValue(key, out var v) && v;

    /// <summary>是否是元数据 QT（非实际战斗开关，不参与保存/恢复）</summary>
    public static bool IsMetaKey(string key) => key == 高难模式;

    /// <summary>判断指定 QT 在当前模式下是否可见</summary>
    public static bool IsVisibleInMode(string key, bool isHighEnd)
        => ModeCategories.TryGetValue(key, out var cat) && cat switch
        {
            BlmQtMode.Common => true,
            BlmQtMode.HighEndOnly => isHighEnd,
            BlmQtMode.DailyOnly => !isHighEnd,
            _ => true,
        };

    /// <summary>获取指定 QT 的模式归属（不存在返回 Common）</summary>
    public static BlmQtMode GetModeCategory(string key)
        => ModeCategories.TryGetValue(key, out var cat) ? cat : BlmQtMode.Common;

    // === 注册 / 读取（框架交互） ===

    /// <summary>注册全部 QT（BlmRotation 构造时调用）</summary>
    public static void Register()
    {
        foreach (var (key, def) in All)
            PromeSettings.Instance.AddQt(key, def);
    }

    /// <summary>读取 QT 状态；失败回退默认值</summary>
    public static bool Enabled(string key)
    {
        bool def = Default(key);
        try { return PromeSettings.Instance.GetQt(key); }
        catch { return def; }
    }

    /// <summary>写入 QT 状态</summary>
    public static void Set(string key, bool value)
    {
        try { PromeSettings.Instance.SetQt(key, value); }
        catch { /* 静默 */ }
    }
}
