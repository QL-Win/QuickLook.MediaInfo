param(
    [string]$LibDir = (Join-Path $PSScriptRoot 'lib')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$script:ArchMapping = @{
    'x64'   = 'win-x64'
    'i386'  = 'win-x86'
    'ARM64' = 'win-arm64'
}

function Find-7zExe {
    $paths = @(
        (Get-Command '7z.exe' -ErrorAction SilentlyContinue).Source,
        'C:\Program Files\7-Zip\7z.exe',
        'C:\Program Files (x86)\7-Zip\7z.exe'
    )
    foreach ($path in $paths) {
        if ($path -and (Test-Path $path)) {
            return (Resolve-Path $path).Path
        }
    }
    throw '7-Zip (7z.exe) not found. Please install 7-Zip from https://www.7-zip.org/'
}

function Get-PlatformFromFilename {
    param([string]$Name)
    foreach ($key in $script:ArchMapping.Keys) {
        if ($Name -match $key) {
            return $script:ArchMapping[$key]
        }
    }
    return $null
}

$7zExe = Find-7zExe
Push-Location $PSScriptRoot
try {
    $archives = Get-ChildItem -Path $PSScriptRoot -Filter 'MediaInfo_DLL_*_Windows_*_WithoutInstaller.*' |
        Where-Object { $_.Extension -match '\.(7z|zip)$' }

    if (-not $archives) {
        Write-Host 'No MediaInfo DLL archives found matching the pattern.'
        return
    }

    foreach ($archive in $archives) {
        $platform = Get-PlatformFromFilename -Name $archive.BaseName
        if (-not $platform) {
            Write-Host "Warning: Could not determine platform from '$($archive.Name)', skipping."
            continue
        }

        $targetDir = Join-Path $LibDir $platform
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null

        Write-Host "Extracting MediaInfo.dll from $($archive.Name) -> $targetDir"
        & $7zExe x $archive.FullName -o"$targetDir" MediaInfo.dll -y -bso0 -bsp0
        if ($LASTEXITCODE -ne 0) {
            throw "7z extraction failed for $($archive.Name) with exit code $LASTEXITCODE"
        }
    }

    Write-Host "All native DLLs extracted to $LibDir"
}
finally {
    Pop-Location
}

if ([Environment]::UserInteractive -and
    $Host.Name -eq 'ConsoleHost' -and
    -not [Console]::IsInputRedirected) {
    Write-Host "`nPress any key to exit..."
    [void][System.Console]::ReadKey($true)
}
