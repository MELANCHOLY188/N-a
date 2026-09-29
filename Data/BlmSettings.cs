// ============================================================
// BlmSettings - 运行期设置（单例 + JSON 持久化，对齐 InitSettings）
// 开关类配置走 QT（BlmQT）；数值类配置走这里。
// 文件：PromeRotation 缓存目录 BlmAcr.Settings.json
// ============================================================
using System.Text.Json;
using ECommons.Logging;
using PromeRotation.Helpers;

namespace BlmAcr.Data;

public class BlmSettings
{
    private static BlmSettings? _instance;
    public static BlmSettings Instance => _instance ??= Load();

    // === 数值类设置 ===
    /// <summary>高难模式 / 日随模式</summary>
    public bool IsHighEnd = true;

    /// <summary>斩杀判断保守系数（越小越保守）</summary>
    public float KillSafety = 0.8f;

    /// <summary>高雷云 DoT 刷新阈值（秒）</summary>
    public float DotRefreshSeconds = 3.5f;

    /// <summary>斩杀无观测数据时的回退阈值（目标血量占比）</summary>
    public float KillFallbackShare = 0.02f;

    // === QT 默认值持久化（按模式） ===
    /// <summary>高难模式专用 QT 快照</summary>
    public Dictionary<string, bool> QtHighEndDefaults = new();

    /// <summary>日随模式专用 QT 快照</summary>
    public Dictionary<string, bool> QtDailyDefaults = new();

    // === QT 快照管理 ===
    private Dictionary<string, bool> GetModeDefaults()
        => IsHighEnd ? QtHighEndDefaults : QtDailyDefaults;

    /// <summary>保存当前 QT 状态到指定模式快照</summary>
    public void SaveQtSnapshot(bool toHighEnd)
    {
        var dict = toHighEnd ? QtHighEndDefaults : QtDailyDefaults;
        foreach (var key in BlmQT.All.Keys)
        {
            if (BlmQT.IsMetaKey(key)) continue;
            dict[key] = BlmQT.Enabled(key);
        }
    }

    /// <summary>从指定模式快照恢复 QT 状态</summary>
    public void RestoreQtSnapshot(bool fromHighEnd)
    {
        var dict = fromHighEnd ? QtHighEndDefaults : QtDailyDefaults;
        foreach (var key in BlmQT.All.Keys)
        {
            if (BlmQT.IsMetaKey(key)) continue;
            BlmQT.Set(key, dict.TryGetValue(key, out var v) ? v : BlmQT.Default(key));
        }
    }

    /// <summary>重置所有 QT 到默认值</summary>
    public void ResetQt()
    {
        foreach (var key in BlmQT.All.Keys)
        {
            if (BlmQT.IsMetaKey(key)) continue;
            BlmQT.Set(key, BlmQT.Default(key));
        }
    }

    /// <summary>模式切换（保存当前模式快照 -> 切换 -> 恢复新模式快照）</summary>
    public void SwitchMode(bool toHighEnd)
    {
        if (IsHighEnd == toHighEnd) return;
        SaveQtSnapshot(IsHighEnd);
        IsHighEnd = toHighEnd;
        BlmQT.Set(BlmQT.高难模式, toHighEnd);
        RestoreQtSnapshot(toHighEnd);
        Save();
    }

    // === JSON 持久化 ===
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
    };

    public static string FilePath
    {
        get
        {
            try
            {
                var root = CachePathHelper.EnsureAcrCacheRoot();
                return System.IO.Path.Combine(root, "BlmAcr.Settings.json");
            }
            catch
            {
                return System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                    "XIVLauncherCN", "pluginConfigs", "PromeRotation", "BlmAcr.Settings.json");
            }
        }
    }

    public static BlmSettings Load()
    {
        try
        {
            if (System.IO.File.Exists(FilePath))
            {
                var json = System.IO.File.ReadAllText(FilePath);
                var s = JsonSerializer.Deserialize<BlmSettings>(json, JsonOptions);
                if (s != null)
                {
                    BlmQT.Set(BlmQT.高难模式, s.IsHighEnd);
                    return s;
                }
            }
        }
        catch (Exception e)
        {
            PluginLog.Error($"[BlmAcr] 设置加载失败: {e.Message}");
        }
        return new BlmSettings();
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(this, JsonOptions);
            var dir = System.IO.Path.GetDirectoryName(FilePath);
            if (dir != null && !System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            System.IO.File.WriteAllText(FilePath, json);
        }
        catch { /* 写失败静默 */ }
    }
}
