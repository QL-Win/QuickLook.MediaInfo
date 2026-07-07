param(
    [string]$OutputDirectory = '.',
    [string]$NuGetExe = (Join-Path $env:TEMP 'QuickLook.MediaInfo.NuGet\nuget.exe')
)

Set-Location $PSScriptRoot

Write-Host @"
███╗   ██╗██╗   ██╗ ██████╗ ███████╗████████╗
████╗  ██║██║   ██║██╔════╝ ██╔════╝╚══██╔══╝
██╔██╗ ██║██║   ██║██║  ███╗█████╗     ██║   
██║╚██╗██║██║   ██║██║   ██║██╔══╝     ██║   
██║ ╚████║╚██████╔╝╚██████╔╝███████╗   ██║   
╚═╝  ╚═══╝ ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝   
"@

Write-Host "`Start to pack QuickLook.MediaInfo ..."
Push-Location "..\src\QuickLook.MediaInfo"
dotnet restore
dotnet build -c Release
dotnet pack -c Release -o ../../build/
Pop-Location

Write-Host "`Start to pack QuickLook.MediaInfo.Native ..."
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$nuspecFiles = @(
    'QuickLook.MediaInfo.Native.win-arm64.nuspec',
    'QuickLook.MediaInfo.Native.win-x64.nuspec',
    'QuickLook.MediaInfo.Native.win-x86.nuspec'
)

function Resolve-NuGetExe {
    param([string]$Path)

    if (Test-Path $Path) {
        return (Resolve-Path $Path).Path
    }

    $nugetDir = Split-Path -Parent $Path
    New-Item -ItemType Directory -Path $nugetDir -Force | Out-Null

    $downloadUri = 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
    Invoke-WebRequest -Uri $downloadUri -OutFile $Path

    if (-not (Test-Path $Path)) {
        throw "Failed to download nuget.exe from $downloadUri"
    }

    return (Resolve-Path $Path).Path
}

$resolvedNuGetExe = Resolve-NuGetExe -Path $NuGetExe
$resolvedOutputDirectory = if ([System.IO.Path]::IsPathRooted($OutputDirectory)) {
    [System.IO.Path]::GetFullPath($OutputDirectory)
} else {
    [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot $OutputDirectory))
}
New-Item -ItemType Directory -Path $resolvedOutputDirectory -Force | Out-Null

Push-Location $PSScriptRoot
try {
    foreach ($nuspec in $nuspecFiles) {
        & $resolvedNuGetExe pack $nuspec -OutputDirectory $resolvedOutputDirectory -NoPackageAnalysis -NonInteractive -ForceEnglishOutput
        if ($LASTEXITCODE -ne 0) {
            throw "nuget pack failed for $nuspec with exit code $LASTEXITCODE"
        }
    }
}
finally {
    Pop-Location
}

Write-Host "Packed 3 native packages into $resolvedOutputDirectory"

Write-Host "`nPress any key to exit..."
[void][System.Console]::ReadKey($true)
