# The Conjuror 单机化改造 —— 阶段 0 + 阶段 1（不动 Unity 设置与 meta）

## 约束
- **不修改任何 Unity 工程设置文件**（ProjectSettings、packages、Addressables 配置等均不在代码侧改动）。
- **不增删改任何 .meta 文件** —— 只移动/删除资源与脚本本体；由此产生的 meta 孤儿/引用问题，由用户之后在 Unity 6000.6.0 编辑器里打开时让 Unity 自行处理（如需辅助说明，改造完成后给出在编辑器内的操作清单）。
- 只做：删除文件/目录、移动资源、编写/修改 C# 代码。

## 阶段 0：清理网络相关
1. 删除 `Server/` 目录。
2. 删除 `Client/Assets/SocketIO/` 插件目录（本体文件，不碰 meta）。
3. 删除 `NetworkController.cs` 及其他纯网络用途脚本；清理引用它们的代码（如 `BoardBehaviour` 中的调用），保证 C# 侧逻辑自洽（不依赖已被删除的类型）。

## 阶段 1：Addressables 资源加载骨架（代码与资源移动部分）
1. 盘点 `Resources/` 下预制体与所有 `Resources.Load` 调用点。
2. 编写 `AssetService`：基于 Unity Addressables API 的异步加载封装 —— `LoadAssetAsync` / `InstantiateAsync` / `Release`、句柄管理（实例销毁时释放）、key 常量或 ScriptableObject/JSON 配置映射。
3. 将现有 `Resources.Load` 调用点改写为走 `AssetService`。
4. 资源本体移出 `Resources` 目录到约定的新目录结构（如 `Assets/AddressableAssets/Cards|UI|Effects|Players`），只移动文件本体。
5. 输出一份“编辑器内后续操作清单”文档：安装 Addressables 包、标记资源为 Addressable、设置分组与 key（需与代码中 key 规范一致）、在 Unity 6000.6.0 下打开工程完成升级与编译修复。

## 不做（押后）
- Unity 版本升级的具体设置、Addressables 编辑器配置、规则引擎/玩法/AI（等核心玩法文档）。