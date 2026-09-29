// ============================================================
// BlmEventHandler - 战斗事件处理器
// ============================================================
using PromeRotation.Core;
using PromeRotation.Data;
using PromeRotation.Rotation;

namespace BlmAcr;

public sealed class BlmEventHandler : IRotationEventHandler
{
    public void OnUpdate() { }

    public void OnOutOfBattleUpdate() { }

    public void OnBattleStarted()
    {
        // 新战斗开始：重置伤害观测
        BlmDamageWatcher.Reset();
    }

    public void OnBattleUpdate()
    {
        // 每帧采样目标 HP，供斩杀判断器校准
        BlmDamageWatcher.Sample(Core.Target);
    }

    public void OnNoTarget() { }

    public void OnBattleEnded()
    {
        BlmDamageWatcher.Reset();
        // ★ 关键：重置起手已执行标志，否则下一次进战框架会跳过起手序列
        // （官方 SimpleRotationEventHandler 标准做法）
        PromeSettings.Instance.OpenerHasBeenExecuted = false;
    }

    public void OnTerritoryChanged(ushort territoryType) { }
}
