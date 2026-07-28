---
name: update-mediainfo-languages
description: >-
  Verify and update QuickLook.MediaInfo multilanguage support against a MediaInfo
  GitHub release (latest by default, or a user-specified version). Syncs
  MediaInfoLanguage enum, LanguageExtension.SupportedCodes, and embedded
  Resource/Plugin/Language/*.csv files. Use when: updating language support,
  syncing language CSVs after a MediaInfo release, checking if languages are
  up to date with upstream.
argument-hint: "Optional MediaInfo version (e.g. 26.05 / v26.05). Default: latest release."
---

# Update MediaInfo language support

## When to use

- Check whether `MediaInfoLanguage` matches upstream MediaInfo
- After a MediaInfo release, refresh language codes and CSV resources
- Fix missing/extra language codes or stale embedded language CSVs

## Version selection (required)

**Always pin to a MediaInfo GitHub release tag. Never use `master` / HEAD tip.**

1. If the user specified a version (e.g. `26.05`, `v26.05`, `26.5`), normalize it to a tag:
   - Accept `YY.MM`, `YY.M`, or `vYY.MM`
   - Canonical tag form: `v` + zero-padded month → `v26.05` (not `v26.5`)
2. If the user did **not** specify a version, resolve the **latest** release:

```powershell
$latest = gh release view --repo MediaArea/MediaInfo --json tagName -q .tagName
# e.g. "v26.05"
$tag = $latest
```

Fallback without `gh`:

```powershell
$rel = Invoke-RestMethod `
    "https://api.github.com/repos/MediaArea/MediaInfo/releases/latest" `
    -Headers @{ "User-Agent" = "QuickLook.MediaInfo" }
$tag = $rel.tag_name
```

3. Confirm the resolved tag with the user only if ambiguous; otherwise state it clearly in the summary (e.g. “syncing against MediaInfo `v26.05`”).

### Resolve language directory for that tag

**Preferred — local clone at the release tag** (fast, no network for file content):

```powershell
$mediaInfoRoot = "D:\GitHub\MediaInfo"   # or path provided by user
Push-Location $mediaInfoRoot
git fetch --tags origin
git checkout $tag
Pop-Location
$langDir = Join-Path $mediaInfoRoot "Source\Resource\Plugin\Language"
```

**If no local clone — GitHub Contents API with `?ref=`**:

```powershell
$resp = Invoke-RestMethod `
    "https://api.github.com/repos/MediaArea/MediaInfo/contents/Source/Resource/Plugin/Language?ref=$tag" `
    -Headers @{ "User-Agent" = "QuickLook.MediaInfo" }
```

Do **not** call the API (or read local files) without pinning `ref` / checkout to `$tag`.

## Authoritative source

MediaInfo languages are defined by CSV files under:

`Source/Resource/Plugin/Language/*.csv`

Each CSV basename is the language code (`zh-CN.csv` → `zh-CN`).

## Project files to update

| File | Role |
| ---- | ---- |
| `src/QuickLook.MediaInfo/Core/MediaInfoLanguage.cs` | `MediaInfoLanguage` enum + `[Description("code")]` |
| `src/QuickLook.MediaInfo/Core/LanguageExtension.cs` | `SupportedCodes` (semicolon-separated, **alphabetically sorted**) |
| `src/QuickLook.MediaInfo/Resource/Plugin/Language/*.csv` | Embedded language resources (must match release CSVs **byte-for-byte**) |
| `tests/QuickLook.MediaInfo.Tests/GetInformTests.cs` | Optional: add `[InlineData(MediaInfoLanguage.…)]` for new languages |

> Note: older docs may say `Language_ISO639.cs` — the current type is `MediaInfoLanguage` in `MediaInfoLanguage.cs`.

## Steps

### 1. List upstream codes for `$tag`

```powershell
$csvCodes = Get-ChildItem $langDir -Filter "*.csv" |
    Select-Object -ExpandProperty BaseName | Sort-Object
$csvCodes
```

Or from the GitHub API `$resp` above: take entries whose `name` ends with `.csv`, strip extension, sort.

### 2. Diff enum and local resources

```powershell
$enumFile = "src\QuickLook.MediaInfo\Core\MediaInfoLanguage.cs"
$existing = (Select-String -Path $enumFile -Pattern '\[Description\("([^"]+)"\)\]').Matches |
    ForEach-Object { $_.Groups[1].Value } | Sort-Object

$localResDir = "src\QuickLook.MediaInfo\Resource\Plugin\Language"
$localCsv = Get-ChildItem $localResDir -Filter "*.csv" |
    Select-Object -ExpandProperty BaseName | Sort-Object

$toAddEnum    = $csvCodes | Where-Object { $_ -notin $existing }
$toRemoveEnum = $existing | Where-Object { $_ -notin $csvCodes }
$toAddRes     = $csvCodes | Where-Object { $_ -notin $localCsv }
$toRemoveRes  = $localCsv | Where-Object { $_ -notin $csvCodes }
```

Also compare file hashes for codes present on both sides; any mismatch must be overwritten from the release.

### 3. Update `MediaInfoLanguage.cs`

- **Add**: new enum members with `[Description("xx")]` and PascalCase English names.
- **Remove**: members whose codes disappeared upstream.
- Keep the XML doc comment code list in sync (alphabetically sorted).

### 4. Update `LanguageExtension.cs`

Set `SupportedCodes` to the full sorted code list:

```csharp
private static readonly string[] SupportedCodes =
    "ar;be;bg;...alphabetically sorted..."
    .Split(';');
```

### 5. Sync resource CSV files

Copy **all** upstream `*.csv` for `$tag` into `src/QuickLook.MediaInfo/Resource/Plugin/Language/`:

- Overwrite existing files from the release
- Add missing files
- Delete local CSVs that are no longer in the release

```powershell
$dst = "src\QuickLook.MediaInfo\Resource\Plugin\Language"
Copy-Item -Path (Join-Path $langDir "*.csv") -Destination $dst -Force
# Remove extras
Get-ChildItem $dst -Filter "*.csv" | Where-Object { $_.BaseName -notin $csvCodes } | Remove-Item -Force
```

When using GitHub API only, download each file’s content (follow `download_url` or decode `content`) for `$tag` and write into `$dst`.

### 6. Tests (optional)

Extend `GetInformTests` `[InlineData(MediaInfoLanguage.…)]` cases when adding major languages.

### 7. Build

```powershell
dotnet build QuickLook.MediaInfo.slnx
```

Require **0 errors / 0 warnings**.

### 8. Report

Summarize:

- MediaInfo tag used (`vYY.MM`)
- Enum adds/removes
- Resource files added / removed / overwritten
- Build result

## Notes

- `gr` is MediaInfo’s Greek code (ISO 639 is usually `el`). Do not rename or drop `gr` mappings in `NormalizeCode` if present.
- `pt-BR`, `zh-CN`, `zh-HK`, `zh-TW` are multi-segment codes; keep special handling in `NormalizeCode`.
- Enum member names use English language names (e.g. `ChineseSimplified` → `zh-CN`); there is no strict code↔name formula.
- After finishing, if a local MediaInfo clone was checked out to `$tag`, leave it on that tag or restore the previous branch — do not silently leave dirty unrelated changes.
