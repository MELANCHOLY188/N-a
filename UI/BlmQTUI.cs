// ============================================================
// BlmQTUI - QT 面板（ImGui 自绘）
// 模式切换（高难/日随）+ 战斗开关
// ============================================================
using BlmAcr.Data;
using Dalamud.Bindings.ImGui;
using PromeRotation.Data;

namespace BlmAcr.UI;

public static class BlmQTUI
{
    public static void Draw()
    {
        SyncModeFromQt();

        if (!ImGui.BeginTabBar("Blm_QT")) return;

        if (ImGui.BeginTabItem("模式"))
        {
            ModeToggle();
            ImGui.Separator();
            QtToggle(BlmQT.启用起手, "启用起手");
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("战斗"))
        {
            QtToggle(BlmQT.斩杀收尾, "斩杀收尾");
            QtToggle(BlmQT.危险循环, "危险循环");
            QtToggle(BlmQT.AOE, "AOE 模式");
            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();
    }

    /// <summary>模式感知的 QT 开关</summary>
    public static void QtToggle(string key, string label)
    {
        if (!BlmQT.IsVisibleInMode(key, BlmSettings.Instance.IsHighEnd)) return;

        var v = BlmQT.Enabled(key);
        var display = v ? $"☑ {label}" : $"☐ {label}";
        if (ImGui.Selectable($"{display}##{key}"))
        {
            v = !v;
            var qt = PromeSettings.Instance.QuickToggles;
            qt[key] = v;
        }
    }

    /// <summary>高难/日随模式切换</summary>
    private static void ModeToggle()
    {
        var v = BlmSettings.Instance.IsHighEnd;
        var display = v ? "☑ 高难模式" : "☐ 日随模式";
        if (ImGui.Selectable($"{display}##Mode"))
            BlmSettings.Instance.SwitchMode(!v);
    }

    /// <summary>面板渲染前同步框架 QT 与设置的模式一致</summary>
    private static void SyncModeFromQt()
    {
        var qt = PromeSettings.Instance.QuickToggles;
        if (!qt.TryGetValue(BlmQT.高难模式, out var modeQt)) return;
        if (modeQt == BlmSettings.Instance.IsHighEnd) return;
        BlmSettings.Instance.IsHighEnd = modeQt;
    }
}
