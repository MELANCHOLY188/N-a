// ============================================================
// BlmDamageWatcher - 伤害观测器
// 通过监听目标 HP 下降，估算「每个 GCD 的平均伤害」。
// 原理：滑动时间窗口（12 秒）记录目标 HP 样本，
//       用窗口内的 HP 下降量 / 时间，换算每秒 DPS，
//       再 × 2.5s 得到每 GCD 伤害。
// 注意：多人战斗时 HP 下降包含队友贡献，属于高估；
//       因此在消费方会乘以保守系数。
// ============================================================
using Dalamud.Game.ClientState.Objects.Types;

namespace BlmAcr;

public static class BlmDamageWatcher
{
    private const double WindowMs = 12000;   // 滑动窗口 12 秒
    private const double GcdSeconds = 2.5;   // 标准 GCD

    private sealed record HpSample(double TimeMs, uint Hp);

    private static readonly Queue<HpSample> _samples = new();
    private static readonly DateTime _start = DateTime.UtcNow;

    /// <summary>每帧采样目标 HP（战斗更新时调用）</summary>
    public static void Sample(IBattleChara? target)
    {
        if (target == null || target.MaxHp == 0) return;

        double now = (DateTime.UtcNow - _start).TotalMilliseconds;
        _samples.Enqueue(new HpSample(now, target.CurrentHp));

        while (_samples.Count > 0 && now - _samples.Peek().TimeMs > WindowMs)
            _samples.Dequeue();
    }

    /// <summary>估算每个 GCD 的平均伤害（血/2.5s），数据不足时返回 0</summary>
    public static double GcdDamageEstimate()
    {
        if (_samples.Count < 2) return 0;

        double firstT = _samples.Peek().TimeMs;
        double lastT = _samples.Last().TimeMs;
        uint firstHp = _samples.Peek().Hp;
        uint lastHp = _samples.Last().Hp;

        double spanSec = (lastT - firstT) / 1000.0;
        if (spanSec <= 0.5) return 0;

        // HP 下降量为正
        double hpLost = (double)firstHp - lastHp;
        if (hpLost <= 0) return 0;

        double dps = hpLost / spanSec;
        return dps * GcdSeconds;
    }

    /// <summary>清空观测（出战/换目标时）</summary>
    public static void Reset()
    {
        _samples.Clear();
    }
}
