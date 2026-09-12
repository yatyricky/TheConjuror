# Unity 编辑器内后续操作清单

> 本次代码侧改造已完成单机化清理与 Addressables 加载骨架。以下步骤需要在
> Unity 6000.6.0 编辑器内完成（代码侧未动任何 ProjectSettings / Packages 配置）。

## 1. 打开工程并升级

1. 用 Unity 6000.6.0 打开 `Client` 工程，按提示完成版本升级与 API Updater。
2. 已删除的 `Client/Assets/SocketIO/`、`Server/`、`NetworkController.cs` 会留下孤儿
   `.meta`（如 `Client/Assets/SocketIO.meta`、`scripts/NetworkController.cs.meta`），
   Unity 会自动清理或直接忽略；如提示缺失脚本引用，确认删除即可。
3. `Scenes` 中引用 `SocketIOComponent` 的 GameObject（Login/Main 场景里的 "SocketIO"
   对象）已失效，请在编辑器里删除这些 GameObject，并删除场景中挂着的
   `NetworkController` 组件。

## 2. 安装并初始化 Addressables

1. Package Manager 安装 `com.unity.addressables`。
2. 菜单 `Window > Asset Management > Addressables > Groups > Create Addressables Settings`。

## 3. 标记资源为 Addressable

资源已移动到 `Client/Assets/AddressableAssets/`（prefabs / sprites / data），
key 规范见 `scripts/AssetLoading/AssetKeys.cs`，约定 key = 旧 Resources 相对路径。

勾选资源的 Addressable 选项，并把地址（Address）设置为对应 key：

| 资源 | 地址 key |
|---|---|
| Prefabs/CardObject.prefab | `prefabs/CardObject` |
| Prefabs/CardSlotObject.prefab | `prefabs/CardSlotObject` |
| Prefabs/HandObject.prefab | `prefabs/HandObject` |
| Prefabs/DeckObject.prefab | `prefabs/DeckObject` |
| Prefabs/Player.prefab | `prefabs/Player` |
| Prefabs/Buff.prefab | `prefabs/Buff` |
| Prefabs/Target.prefab | `prefabs/Target` |
| Prefabs/SlashEffect.prefab | `prefabs/SlashEffect` |
| Prefabs/CardEffectParticle.prefab | `prefabs/CardEffectParticle` |
| Prefabs/AvatarObject.prefab | `prefabs/AvatarObject` |
| Data/config.txt | `data/config`（TextAsset） |
| Sprites/card_images/ 下所有卡面图 | `sprites/card_images/<文件名>` |
| Sprites/buffs/ 下所有图标 | `sprites/buffs/<文件名>` |
| Sprites/card_ui/ 下所有 UI 图 | `sprites/card_ui/<文件名>` |

分组建议：Prefabs / Sprites / Data 三个组（命名随意，key 才是寻址依据）。
注意：地址是含子目录的相对 key（如 `sprites/card_ui/frame_white`），不是文件名。

## 4. 场景挂载入口

1. 在 Login 场景创建一个空 GameObject（如 `GameController`），挂上
   `scripts/GameController.cs`。它负责启动时初始化 Addressables 与 GameConfig。
2. Login 场景点 Start 后会直接进入 Main 场景（单机桩行为，无对战逻辑）。

## 5. 冒烟验证

1. Play Login 场景，输入名字点开始，应能进入 Main 场景。
2. Console 无 `[AssetService] ... failed` 报错即说明 key 配置正确。
   （当前无本地引擎，卡牌/战斗流程不会推进，属预期。）

## 后续（本次未做）

- 本地规则引擎（纯 C# Core 层）、AI 对手、事件流接线 —— 等核心玩法文档。
