#Requires -Version 5.1
<#
.SYNOPSIS
    QuickLook.MediaInfo.ConsoleTest 功能测试脚本

.DESCRIPTION
    对控制台测试程序进行全面测试，覆盖所有输出格式和多语言功能。
    需要先指定一个媒体文件路径（--MediaFile 参数），或在脚本中修改 $DefaultMediaFile。

.PARAMETER MediaFile
    用于测试的媒体文件路径（支持视频/音频/图片）。

.PARAMETER DllDir
    MediaInfo.dll 所在目录（若与 exe 不在同一目录时使用）。

.PARAMETER Build
    运行测试前先编译项目（Debug 模式）。

.PARAMETER OutputDir
    将每个格式的 Inform 输出保存到此目录（可选）。

.EXAMPLE
    .\run-tests.ps1 -MediaFile "C:\Videos\sample.mkv"

.EXAMPLE
    .\run-tests.ps1 -MediaFile "C:\Videos\sample.mkv" -Build -OutputDir ".\output"
#>
param(
    [string] $MediaFile   = "example.mp4",
    [string] $DllDir      = "",
    [switch] $Build,
    [string] $OutputDir   = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Continue"

# -----------------------------------------------------------------------------
# 常量 & 路径
# -----------------------------------------------------------------------------
$RepoRoot   = Split-Path -Parent $PSScriptRoot
$SrcCsproj  = Join-Path $RepoRoot "src\QuickLook.MediaInfo\QuickLook.MediaInfo.csproj"
$TestCsproj = Join-Path $RepoRoot "tests\QuickLook.MediaInfo.ConsoleTest\QuickLook.MediaInfo.ConsoleTest.csproj"
$ExePath    = Join-Path $RepoRoot "tests\QuickLook.MediaInfo.ConsoleTest\bin\Debug\net8.0-windows\QuickLook.MediaInfo.ConsoleTest.exe"

$Formats   = @("text", "xml", "json", "csv", "html", "maxml")
$Languages = @("en", "zh-CN", "zh-TW", "ja", "ko", "fr", "de", "ru", "es", "it")

$PassCount = 0
$FailCount = 0

# -----------------------------------------------------------------------------
# 辅助函数
# -----------------------------------------------------------------------------
function Write-Header([string]$Text) {
    Write-Host ""
    Write-Host ("-" * 60) -ForegroundColor DarkCyan
    Write-Host "  $Text" -ForegroundColor Cyan
    Write-Host ("-" * 60) -ForegroundColor DarkCyan
}

function Write-Pass([string]$Label) {
    Write-Host "  [PASS] $Label" -ForegroundColor Green
    $script:PassCount++
}

function Write-Fail([string]$Label, [string]$Detail = "") {
    Write-Host "  [FAIL] $Label" -ForegroundColor Red
    if ($Detail) { Write-Host "         $Detail" -ForegroundColor DarkRed }
    $script:FailCount++
}

function Invoke-Exe {
    param(
        [string]   $Label,
        [string[]] $Arguments,
        [string]   $SaveTo = "",
        [switch]   $ExpectNonEmpty,
        [switch]   $ExpectFailure
    )

    $allArgs = $Arguments
    if ($DllDir) { $allArgs = $allArgs + @("--dll-dir", $DllDir) }

    try {
        $stdout = & $ExePath @allArgs 2>$null
        $exitCode = $LASTEXITCODE

        if ($ExpectFailure -and $exitCode -eq 0) {
            Write-Fail $Label "预期失败但退出码为 0"
            return
        }
        if (-not $ExpectFailure -and $exitCode -ne 0) {
            Write-Fail $Label "退出码 $exitCode"
            return
        }
        if ($ExpectNonEmpty -and [string]::IsNullOrWhiteSpace($stdout)) {
            Write-Fail $Label "输出为空"
            return
        }

        if ($SaveTo -and $stdout) {
            $null = New-Item -ItemType Directory -Force -Path (Split-Path $SaveTo)
            [IO.File]::WriteAllText($SaveTo, ($stdout -join "`n"), [System.Text.Encoding]::UTF8)
        }

        Write-Pass $Label
    }
    catch {
        Write-Fail $Label $_.Exception.Message
    }
}

# -----------------------------------------------------------------------------
# Step 0: 编译
# -----------------------------------------------------------------------------
if ($Build) {
    Write-Header "编译项目"
    Write-Host "  编译库 ..." -NoNewline
    dotnet build $SrcCsproj -c Debug -v quiet 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) { Write-Host " 失败" -ForegroundColor Red; exit 1 }
    Write-Host " OK" -ForegroundColor Green

    Write-Host "  编译测试程序 ..." -NoNewline
    dotnet build $TestCsproj -c Debug -v quiet 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) { Write-Host " 失败" -ForegroundColor Red; exit 1 }
    Write-Host " OK" -ForegroundColor Green
}

# 检查 exe 是否存在
if (-not (Test-Path $ExePath)) {
    Write-Warning "未找到可执行文件: $ExePath"
    Write-Warning "请先运行: dotnet build，或使用 -Build 参数"
    exit 1
}

# -----------------------------------------------------------------------------
# Step 1: 基本功能（无需媒体文件）
# -----------------------------------------------------------------------------
Write-Header "基本功能测试（无需媒体文件）"

Invoke-Exe -Label "--help 显示帮助" `
    -Arguments @("--help") `
    -ExpectNonEmpty

Invoke-Exe -Label "-h 显示帮助（短参数）" `
    -Arguments @("-h") `
    -ExpectNonEmpty

Invoke-Exe -Label "--version 显示库版本" `
    -Arguments @("--version") `
    -ExpectNonEmpty

Invoke-Exe -Label "无参数显示帮助" `
    -Arguments @()

Invoke-Exe -Label "--file 指向不存在文件时退出码非零" `
    -Arguments @("--file", "C:\nonexistent_file_xyz.mkv") `
    -ExpectFailure

# -----------------------------------------------------------------------------
# Step 2: 需要媒体文件的测试
# -----------------------------------------------------------------------------
if (-not $MediaFile) {
    Write-Host ""
    Write-Warning "未指定 -MediaFile，跳过格式 / 语言测试。"
    Write-Warning "用法: .\run-tests.ps1 -MediaFile 'C:\path\to\sample.mkv'"
}
else {
    if (-not (Test-Path $MediaFile)) {
        Write-Warning "媒体文件不存在: $MediaFile"
    }
    else {
        # -- 所有输出格式 --------------------------------------------------
        Write-Header "所有 Inform 输出格式"

        foreach ($fmt in $Formats) {
            $savePath = if ($OutputDir) { Join-Path $OutputDir "inform_$fmt.txt" } else { "" }
            Invoke-Exe -Label "格式: $fmt" `
                -Arguments @("--file", $MediaFile, "--format", $fmt) `
                -SaveTo $savePath `
                -ExpectNonEmpty
        }

        # -- 自定义 Inform 模板 --------------------------------------------
        Write-Header "自定义 Inform 模板（--format custom）"

        Invoke-Exe -Label "custom: 视频分辨率模板" `
            -Arguments @("--file", $MediaFile, "--format", "custom",
                         "--template", "Video;%Format%  %Width%x%Height%  %FrameRate% fps") `
            -ExpectNonEmpty

        Invoke-Exe -Label "custom: 音频模板" `
            -Arguments @("--file", $MediaFile, "--format", "custom",
                         "--template", "Audio;%Format%  %Channel(s)%ch  %SamplingRate% Hz") `
            -ExpectNonEmpty

        Invoke-Exe -Label "custom: 通用信息模板" `
            -Arguments @("--file", $MediaFile, "--format", "custom",
                         "--template", "General;%Format%  %Duration/String3%  %FileSize/String4%") `
            -ExpectNonEmpty

        Invoke-Exe -Label "--format custom 缺少 --template 时报错" `
            -Arguments @("--file", $MediaFile, "--format", "custom") `
            -ExpectFailure

        # -- 多语言输出 ----------------------------------------------------
        Write-Header "多语言 Inform 输出（--language）"

        foreach ($lang in $Languages) {
            $savePath = if ($OutputDir) { Join-Path $OutputDir "lang_${lang}.txt" } else { "" }
            Invoke-Exe -Label "语言: $lang  格式: text" `
                -Arguments @("--file", $MediaFile, "--language", $lang) `
                -SaveTo $savePath `
                -ExpectNonEmpty
        }

        # 语言 + XML 组合
        Invoke-Exe -Label "语言 zh-CN + 格式 xml" `
            -Arguments @("--file", $MediaFile, "--format", "xml", "--language", "zh-CN") `
            -ExpectNonEmpty

        # 语言 + JSON 组合
        Invoke-Exe -Label "语言 ja + 格式 json" `
            -Arguments @("--file", $MediaFile, "--format", "json", "--language", "ja") `
            -ExpectNonEmpty

        # -- 结构化输出 ----------------------------------------------------
        Write-Header "结构化输出（--structured）"

        Invoke-Exe -Label "--structured 英文输出" `
            -Arguments @("--file", $MediaFile, "--structured") `
            -ExpectNonEmpty

        Invoke-Exe -Label "--structured 日文语言（语言参数对结构化输出无影响）" `
            -Arguments @("--file", $MediaFile, "--structured", "--language", "ja") `
            -ExpectNonEmpty

        # -- 短参数别名 ----------------------------------------------------
        Write-Header "短参数别名"

        Invoke-Exe -Label "-f 文件参数简写" `
            -Arguments @("-f", $MediaFile) `
            -ExpectNonEmpty

        Invoke-Exe -Label "-l 语言参数简写" `
            -Arguments @("-f", $MediaFile, "-l", "zh-CN") `
            -ExpectNonEmpty

        Invoke-Exe -Label "-s 结构化参数简写" `
            -Arguments @("-f", $MediaFile, "-s") `
            -ExpectNonEmpty

        # -- 裸路径（无 --file 前缀）------------------------------------
        Write-Header "裸路径参数（不带 --file）"

        Invoke-Exe -Label "裸路径作为文件参数" `
            -Arguments @($MediaFile) `
            -ExpectNonEmpty
    }
}

# -----------------------------------------------------------------------------
# 结果汇总
# -----------------------------------------------------------------------------
Write-Host ""
Write-Host ("=" * 60) -ForegroundColor DarkCyan
$total = $PassCount + $FailCount
if ($FailCount -eq 0) { $resultColor = "Green" } else { $resultColor = "Yellow" }
Write-Host ("  测试结果: {0} / {1} 通过" -f $PassCount, $total) -ForegroundColor $resultColor
if ($FailCount -gt 0) {
    Write-Host "  失败数  : $FailCount" -ForegroundColor Red
}
Write-Host ("=" * 60) -ForegroundColor DarkCyan
Write-Host ""

if ($FailCount -gt 0) { exit 1 }
