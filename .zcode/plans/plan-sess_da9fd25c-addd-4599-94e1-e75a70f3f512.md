# UEClient → TheConjuror 全量改名清单（基于 2026-09-12 21:37 现状扫描）

## 一、重命名文件（7 处）
| 旧 | 新 |
|---|---|
| `UEClient.uproject` | `TheConjuror.uproject` |
| `Source/UEClient.Target.cs` | `TheConjuror.Target.cs` |
| `Source/UEClientEditor.Target.cs` | `TheConjurorEditor.Target.cs` |
| `Source/UEClient/`（目录） | `Source/TheConjuror/` |
| `…/UEClient.Build.cs` | `TheConjuror.Build.cs` |
| `…/UEClient.h` | `TheConjuror.h` |
| `…/UEClient.cpp` | `TheConjuror.cpp` |

## 二、文件内容替换
1. **TheConjuror.uproject**：`"Name": "UEClient"` → `"TheConjuror"`（其余不动：5.8 关联、VisualStudioTools/ModelingTools 插件保留）。
2. **TheConjuror.Build.cs**：`class UEClient : ModuleRules` 及构造函数名 → `TheConjuror`（依赖列表 Core/CoreUObject/Engine/InputCore/EnhancedInput 不动）。
3. **TheConjuror.h**：仅 `#include "CoreMinimal.h"`，无需内容修改。
4. **TheConjuror.cpp**：`#include "UEClient.h"` → `"TheConjuror.h"`；`IMPLEMENT_PRIMARY_GAME_MODULE(FDefaultGameModuleImpl, UEClient, "UEClient")` → `(FDefaultGameModuleImpl, TheConjuror, "TheConjuror")`。
5. **TheConjuror.Target.cs**：类 `UEClientTarget` → `TheConjurorTarget`；`ExtraModuleNames.Add("UEClient")` → `"TheConjuror"`（BuildSettingsVersion.V7 / Unreal5_8 保持）。
6. **TheConjurorEditor.Target.cs**：类 `UEClientEditorTarget` → `TheConjurorEditorTarget`；`ExtraModuleNames` 同上。
7. **Config/DefaultEngine.ini**（本次新发现的关键点）：两行 GameName 重定向改目标
   `+ActiveGameNameRedirects=(OldGameName="TP_Blank",NewGameName="/Script/UEClient")` 与 `…"/Script/TP_Blank"…` → `NewGameName="/Script/TheConjuror"`（OldGameName 保留）。

## 三、删除生成物（重新生成，不入库）
`UEClient.sln`、`UEClient.slnx`、`Automation_UEClient.sln`、`Automation_UEClient.slnx`、`Binaries/`、`Intermediate/`、`DerivedDataCache/`、`Saved/`、`.vs/`

## 四、确认无需改动（已扫描验证）
- `Content/`（.umap、__ExternalActors__、textures、Collections/Developers）：grep 无 UEClient 引用，路径全部是 /Game/ 对象路径，与模块名解耦。
- `.vsconfig`、`.editorconfig`、`README.md`、`docs/`：无引用（docs 历史文档本就不动）。
- 模块头文件无 UECLIENT_API/日志类别（5.8 空模板比预期干净）。

## 五、验证
- 定位 UE 5.8 安装路径（预计 C:\Program Files\Epic Games\UE_5.8），命令行 GenerateProjectFiles + Build `TheConjurorEditor Win64 Development` 确认编译通过；若 VS 工具链仍未就绪则交付“改名完成、待重试”清单。

## 前置条件（你侧）
改名开始前关闭 UE 编辑器和 Visual Studio。

## 改名后建议（本次不含）
补 `.gitignore`/`.gitattributes`（LFS）、`Content/TheConjuror/` 命名空间目录。