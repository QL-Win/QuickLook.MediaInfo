---
name: update-mediainfo-languages
description: "验证并更新 QuickLook.MediaInfo 项目的多语言支持列表。从 GitHub MediaInfo 仓库 Source/Resource/Plugin/Language 目录获取所有 .csv 文件列表，与 Language_ISO639.cs 和 LanguageExtension.cs 中的语言列表进行比对，增删以保持同步。Use when: updating language support, adding new languages, checking if Language_ISO639 enum is up to date with MediaInfo upstream."
argument-hint: "可选：本地 MediaInfo 仓库路径，如 D:\\GitHub\\MediaInfo"
---

# 更新 MediaInfo 多语言支持

## 何时使用

- 需要检查 `Language_ISO639` 枚举是否与 MediaInfo 上游同步
- MediaInfo 发布新版本后更新语言列表
- 发现语言代码缺失或多余时

## 权威来源

MediaInfo 实际支持的语言由其源码中的 CSV 文件决定，位于：
- **GitHub（上游）**：`https://github.com/MediaArea/MediaInfo/tree/master/Source/Resource/Plugin/Language`
- **本地克隆（首选）**：`<MediaInfo仓库根目录>/Source/Resource/Plugin/Language/`

每个 CSV 文件的文件名（不含扩展名）就是对应的语言代码（如 `zh-CN.csv` → `zh-CN`）。

## 步骤

### 1. 获取权威语言列表

**优先使用本地仓库**（速度快、无网络依赖）：

```powershell
# 替换为实际的 MediaInfo 本地路径
$langDir = "D:\GitHub\MediaInfo\Source\Resource\Plugin\Language"
$csvCodes = Get-ChildItem $langDir -Filter "*.csv" |
    Select-Object -ExpandProperty BaseName | Sort-Object
$csvCodes
```

**若无本地仓库，则抓取 GitHub API**：

```powershell
$resp = Invoke-RestMethod `
    "https://api.github.com/repos/MediaArea/MediaInfo/contents/Source/Resource/Plugin/Language" `
    -Headers @{ "User-Agent" = "QuickLook.MediaInfo" }
$csvCodes = $resp | Where-Object { $_.name -like "*.csv" } |
    ForEach-Object { [System.IO.Path]::GetFileNameWithoutExtension($_.name) } |
    Sort-Object
$csvCodes
```

### 2. 比对现有枚举

读取 `src/QuickLook.MediaInfo/Core/Language_ISO639.cs`，提取所有 `[Description("...")]` 属性值，与步骤 1 的列表比对：

```powershell
$file = "src\QuickLook.MediaInfo\Core\Language_ISO639.cs"
$existing = (Select-String -Path $file -Pattern '\[Description\("([^"]+)"\)\]').Matches |
    ForEach-Object { $_.Groups[1].Value } | Sort-Object

$toAdd    = $csvCodes | Where-Object { $_ -notin $existing }
$toRemove = $existing | Where-Object { $_ -notin $csvCodes }

"需要新增: $toAdd"
"需要删除: $toRemove"
```

### 3. 更新 Language_ISO639.cs

- **新增**：在枚举末尾（`Albanian` 之前或之后）插入新条目，格式：
  ```csharp
  [Description("xx")] XxxLanguageName,
  ```
  枚举名使用英文语言名（Pascal Case）。
- **删除**：移除对应的枚举条目。

### 4. 更新 LanguageExtension.cs

同步更新 `SupportedCodes` 字符串，保持与枚举一致（分号分隔、按字母排序）：

```csharp
private static readonly string[] SupportedCodes =
    "ar;be;bg;...所有代码按字母排序..."
    .Split(';');
```

### 5. 更新 GetInformTests.cs 中的语言测试（可选）

`[InlineData(Language_ISO639.XxxLanguageName)]` 测试用例可根据新增语言补充。

### 6. 构建验证

```powershell
dotnet build QuickLook.MediaInfo.slnx
```

确认 0 错误 0 警告。

## 注意事项

- `gr` 是 MediaInfo 对希腊语的代码（标准 ISO 639 为 `el`），`LanguageExtension.NormalizeCode` 中有专门映射，勿删。
- `pt-BR`、`zh-CN`、`zh-HK`、`zh-TW` 是多段代码，`NormalizeCode` 中有特殊处理，勿改为两字母。
- 枚举名与语言代码之间没有强制对应规则，但约定使用英文语言名（如 `ChineseSimplified` 对应 `zh-CN`）。
