// ============================================================
// BlmQTUI - QT 面板（ImGui 自绘）
// Tab 分组：基础 / 循环 / 爆发
// 高难专属=金色，日随专属=绿色；停手开->HoldTime 后自动解除
// ============================================================
using System.Numerics;
using System.Timers;
using BlmAcr.Data;
using Dalamud.Bindings.ImGui;
using PromeRotation.Data;
using Timer = System.Timers.Timer;

namespace BlmAcr.UI;

public static class BlmQTUI
{
    private static Timer? _holdTimer;

    public static void Draw()
    {
        SyncModeFromQt();

        if (!ImGui.BeginTabBar("Blm_QT")) return;

        if (ImGui.BeginTabItem("基础"))
        {
            ModeToggle();
            ImGui.Separator();
            QtToggle(BlmQT.启用起手, "启用起手");
            QtToggle(BlmQT.停手, "停手", OnHoldChanged);
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("循环"))
        {
            QtToggle(BlmQT.斩杀收尾, "斩杀收尾");
            QtToggle(BlmQT.危险循环, "危险循环");
            QtToggle(BlmQT.AOE, "AOE 模式");
            QtToggle(BlmQT.雷DoT维持, "雷DoT维持");
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("爆发"))
        {
            QtToggle(BlmQT.自动爆发, "自动爆发");
            QtToggle(BlmQT.移动瞬发, "移动瞬发");
            QtToggle(BlmQT.自动减伤, "自动减伤（日随）");
            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();

        SyncModeFromQt();
    }

    /// <summary>模式感知的 QT 开关（高难金 / 日随绿）</summary>
    public static void QtToggle(string key, string label, Action<bool>? onChanged = null)
    {
        if (!BlmQT.IsVisibleInMode(key, BlmSettings.Instance.IsHighEnd)) return;

        var v = BlmQT.Enabled(key);
        var cat = BlmQT.GetModeCategory(key);

        if (cat == BlmQtMode.HighEndOnly)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0.8f, 0.2f, 1f));
        else if (cat == BlmQtMode.DailyOnly)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.2f, 1f, 0.4f, 1f));

        var display = v ? $"☑ {label}" : $"☐ {label}";
        if (ImGui.Selectable($"{display}##{key}"))
        {
            v = !v;
            var qt = PromeSettings.Instance.QuickToggles;
            qt[key] = v;
            onChanged?.Invoke(v);
        }

        if (cat != BlmQtMode.Common)
            ImGui.PopStyleColor();
    }

    /// <summary>高难/日随模式切换</summary>
    private static void ModeToggle()
    {
        var v = BlmSettings.Instance.IsHighEnd;
        ImGui.PushStyleColor(ImGuiCol.Text, v
            ? new Vector4(1f, 0.8f, 0.2f, 1f)
            : new Vector4(0.2f, 1f, 0.4f, 1f));
        var display = v ? "☑ 高难模式" : "☐ 日随模式";
        if (ImGui.Selectable($"{display}##Mode"))
            BlmSettings.Instance.SwitchMode(!v);
        ImGui.PopStyleColor();
    }

    /// <summary>面板渲染前同步框架 QT 与设置的模式一致</summary>
    private static void SyncModeFromQt()
    {
        var qt = PromeSettings.Instance.QuickToggles;
        if (!qt.TryGetValue(BlmQT.高难模式, out var modeQt)) return;
        if (modeQt == BlmSettings.Instance.IsHighEnd) return;
        BlmSettings.Instance.IsHighEnd = modeQt;
    }

    /// <summary>停手回调：开启后 HoldTime 毫秒自动解除</summary>
    public static void OnHoldChanged(bool isSet)
    {
        if (!isSet) return;
        _holdTimer?.Stop();
        _holdTimer?.Dispose();
        _holdTimer = new Timer(BlmSettings.Instance.HoldTime) { AutoReset = false };
        _holdTimer.Elapsed += (_, _) =>
        {
            BlmQT.Set(BlmQT.停手, false);
            _holdTimer?.Dispose();
            _holdTimer = null;
        };
        _holdTimer.Start();
    }
}
