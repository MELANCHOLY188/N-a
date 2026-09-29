// ============================================================
// BlmSettingsUI - 设置面板（ImGui）
// 数值设置（斩杀/DoT 阈值）+ QT 快照管理
// ============================================================
using BlmAcr.Data;
using Dalamud.Bindings.ImGui;

namespace BlmAcr.UI;

public static class BlmSettingsUI
{
    public static void Draw()
    {
        if (!ImGui.BeginTabBar("Blm_Settings")) return;

        if (ImGui.BeginTabItem("数值"))
        {
            var s = BlmSettings.Instance;
            float kill = s.KillSafety;
            float dot = s.DotRefreshSeconds;

            if (ImGui.SliderFloat("斩杀保守系数", ref kill, 0.5f, 1.0f))
                s.KillSafety = kill;
            if (ImGui.SliderFloat("DoT刷新阈值(秒)", ref dot, 0.5f, 10f))
                s.DotRefreshSeconds = dot;

            ImGui.Separator();
            if (ImGui.Button("保存设置"))
                s.Save();
            ImGui.SameLine();
            if (ImGui.Button("重置QT为默认"))
                s.ResetQt();

            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("QT快照"))
        {
            var s = BlmSettings.Instance;
            if (ImGui.Button("保存当前QT到高难快照"))
                s.SaveQtSnapshot(true);
            ImGui.SameLine();
            if (ImGui.Button("保存当前QT到日随快照"))
                s.SaveQtSnapshot(false);

            if (ImGui.Button("恢复高难快照"))
                s.RestoreQtSnapshot(true);
            ImGui.SameLine();
            if (ImGui.Button("恢复日随快照"))
                s.RestoreQtSnapshot(false);

            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();
    }
}
