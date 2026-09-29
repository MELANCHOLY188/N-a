# BlmAcr — 黑魔法师 ACR（PromeRotation）

基于 **PromeRotation** 框架的 FFXIV 黑魔法师（BLM）外部 ACR。
定位：**输出至上**，循环策略依据 The Balance 7.4 Black Mage 理论，采用官方 Resolver 架构，决策逻辑为原创实现。

## 功能特性

- **官方 Resolver 架构**：`IDecisionResolver` 决策器链（14 GCD + 3 oGCD），每个技能一个类，优先级 = 注册顺序
- **游戏内实时调试**：`UpdateDebugStatus` 将每个决策器的生效/失效原因实时显示在框架面板
- **输出至上循环**：
  - 高雷云 DoT 无缝维持（剩余 <3.5s 自动刷新，阈值可在设置面板调）
  - 异言防溢出（灵极魂满 3 层自动打出）
  - oGCD 管理：魔泉（火阶段续蓝）、详述（防溢出）、黑魔纹（CD 好就铺）
  - 转冰优化：暴雷升 UI3 → 冰澈攒心 → 灵极魂回蓝，替代长时间空转
  - 危险循环模式（QT）：跳过攒心直接回火赌火苗暴击
- **斩杀收尾**：12s 滑动窗口观测每 GCD 平均伤害 → 估算绝望伤害（380×1.8/450 等效威力比 × 保守系数），判定能否收掉目标；无观测数据回退目标血量 2%
- **AOE 自适应**：5 码内 ≥2 敌人自动切换 AOE 分支（高雷二 DoT / 玄冰攒心 / 核爆清蓝 / 耀星）
- **5+7 起手**：3.5s 预读爆炎 → 5 炽炎 + 魔泉 + 7 炽炎 → 双耀星 → 绝望 → 转冰
- **QT/设置面板**（ImGui）：斩杀收尾 / 危险循环 / AOE / 启用起手开关，斩杀系数与 DoT 阈值滑块
- **高难/日随模式**：两套 QT 快照独立持久化（JSON），切换自动保存/恢复
- **设置持久化**：`BlmAcr.Settings.json` 落盘于 PromeRotation 缓存目录

## 目录结构

```
BlmAcr/
├── BlmAcr.csproj            # net10.0-windows + PromeRotation.SDK.API15 (NuGet)
├── BlmRotation.cs           # 主类：Resolver 注册 + 循环出口 + 调试状态 + UI 委托
├── BlmSkills.cs             # 技能 / 状态 ID 常量
├── BlmEventHandler.cs       # 战斗事件（斩杀伤害采样）
├── BlmDamageWatcher.cs      # 12s 滑动窗口伤害观测器
├── BlmKillCheck.cs          # 斩杀判断器（读设置面板参数）
├── Data/
│   ├── BlmQT.cs             # QT 唯一数据源（键名 + 默认值 + 模式归属）
│   ├── BlmSettings.cs       # 设置持久化 + 模式切换 + QT 快照
│   └── BlmState.cs          # AF 炽炎计数（每帧同步）
├── Action/
│   ├── Gcd/                 # 14 个 GCD 决策器
│   └── OffGcd/              # 3 个 oGCD 决策器
├── Opener/BlmOpener.cs      # Standard 5+7 起手
├── UI/
│   ├── BlmQTUI.cs           # QT 面板
│   └── BlmSettingsUI.cs     # 设置面板
└── Helper/BlmGcdHelper.cs   # 决策器共享工具
```

## 构建与部署

```powershell
cd BlmAcr
dotnet build -c Release
# 输出: bin\Release\net10.0-windows\BlmAcr.dll
```

部署（复制 DLL 到游戏 ACR 目录，覆盖已有文件，**不要动**目录里的 deps.json）：

```powershell
Copy-Item bin\Release\net10.0-windows\BlmAcr.dll `
  "C:\Users\<你的用户名>\AppData\Roaming\XIVLauncherCN\pluginConfigs\PromeRotation\ACR\BlmAcr\BlmAcr.dll" -Force
```

进游戏 → `/pr reload` → 切黑魔法师 → 打开 PromeRotation 面板选择 BlmAcr。

## 依赖

- .NET 10.0 Windows
- [PromeRotation](https://github.com/PromeRotation)（插件本体 + `PromeRotation.SDK.API15` NuGet 包）
- Dalamud / ECommons / Lumina（SDK 包内附带）

## GCD 决策链优先级

```
AOE填充 → 单体DoT → 异言 → 无元素进冰 → 火苗爆炎
→ UI·AOE玄冰 → UI·单体(危险/正常) → 斩杀 → AF·AOE
→ 耀星 → 悖论 → 炽炎 → 绝望 → 转冰
```

## 状态 ID 参考

| 状态 | ID | 说明 |
|---|---|---|
| 火苗 Firestarter | 481 | 免费爆炎 |
| 雷首 Thunderhead | 3870 | 自己身上 30s |
| 高雷云 DoT | 3871 | 目标身上 30s |
| 高雷二 DoT | 3872 | 目标身上 24s |

## 说明

- 循环决策为原创设计，仅参考框架 API 机制与 The Balance 公开攻略
- 斩杀模型为近似估算（威力比 × 观测均伤 × 保守系数），可通过设置面板调参
