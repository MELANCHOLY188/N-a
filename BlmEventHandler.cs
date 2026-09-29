// ============================================================
// BlmEventHandler - 战斗事件处理器
// ============================================================
using PromeRotation.Core;
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
    }

    public void OnTerritoryChanged(ushort territoryType) { }
}
