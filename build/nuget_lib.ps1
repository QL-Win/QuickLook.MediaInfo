param(
    [string]$LibDir = (Join-Path $PSScriptRoot 'lib')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# -------------------------------------------------------------------
# Windows: platform mapping for MediaInfo_DLL_*_Windows_*_WithoutInstaller
# -------------------------------------------------------------------
$script:WinArchMapping = @{
    'x64'   = 'win-x64'
    'i386'  = 'win-x86'
    'ARM64' = 'win-arm64'
}

# -------------------------------------------------------------------
# macOS: platform mapping for MediaInfo_DLL_*_Mac_* archive (universal binary)
# -------------------------------------------------------------------
$script:MacArchMapping = @{
    'Mac_x86_64\+arm64' = 'osx-x64'
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
    foreach ($key in $script:WinArchMapping.Keys) {
        if ($Name -match $key) {
            return $script:WinArchMapping[$key]
        }
    }
    return $null
}

function Get-MacPlatformsFromFilename {
    param([string]$Name)
    foreach ($key in $script:MacArchMapping.Keys) {
        if ($Name -match $key) {
            return $script:MacArchMapping[$key]
        }
    }
    return $null
}

$7zExe = Find-7zExe
Push-Location $PSScriptRoot
try {
    # -------------------------------------------------------------------
    # Windows: extract MediaInfo.dll from .7z / .zip
    # -------------------------------------------------------------------
    $winArchives = Get-ChildItem -Path $PSScriptRoot -Filter 'MediaInfo_DLL_*_Windows_*_WithoutInstaller.*' |
        Where-Object { $_.Extension -match '\.(7z|zip)$' }

    foreach ($archive in $winArchives) {
        $platform = Get-PlatformFromFilename -Name $archive.BaseName
        if (-not $platform) {
            Write-Host "[Win] Warning: Could not determine platform from '$($archive.Name)', skipping."
            continue
        }

        $targetDir = Join-Path $LibDir $platform
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null

        Write-Host "[Win] Extracting MediaInfo.dll from $($archive.Name) -> $targetDir"
        & $7zExe x $archive.FullName -o"$targetDir" MediaInfo.dll -y -bso0 -bsp0
        if ($LASTEXITCODE -ne 0) {
            throw "[Win] 7z extraction failed for $($archive.Name) with exit code $LASTEXITCODE"
        }
    }

    # -------------------------------------------------------------------
    # macOS: extract libmediainfo.0.dylib from .tar.bz2 (universal binary)
    # -------------------------------------------------------------------
    $macArchives = Get-ChildItem -Path $PSScriptRoot -Filter 'MediaInfo_DLL_*_Mac_*.tar.bz2'

    foreach ($archive in $macArchives) {
        $platform = Get-MacPlatformsFromFilename -Name $archive.BaseName
        if (-not $platform) {
            Write-Host "[macOS] Warning: Could not determine platform from '$($archive.Name)', skipping."
            continue
        }

        # Decompress .tar.bz2 to .tar in a temp location
        $tarName = $archive.Name -replace '\.tar\.bz2$', '.tar'
        $tempTar = Join-Path $env:TEMP $tarName
        Write-Host "[macOS] Decompressing $($archive.Name) -> $tempTar"
        & $7zExe x $archive.FullName -o"$env:TEMP" -y -bso0 -bsp0
        if ($LASTEXITCODE -ne 0) {
            throw "[macOS] bz2 decompression failed for $($archive.Name) with exit code $LASTEXITCODE"
        }

        try {
            $targetDir = Join-Path $LibDir $platform
            New-Item -ItemType Directory -Path $targetDir -Force | Out-Null

            Write-Host "[macOS] Extracting libmediainfo*.dylib from tar -> $targetDir"
            & $7zExe x $tempTar -o"$targetDir" -y -bso0 -bsp0 MediaInfoLib/libmediainfo.0.dylib MediaInfoLib/libmediainfo.dylib
            if ($LASTEXITCODE -ne 0) {
                throw "[macOS] tar extraction failed for $($archive.Name) (platform: $platform) with exit code $LASTEXITCODE"
            }

            # Flatten: move dylib files from MediaInfoLib/ to targetDir root, remove empty folder
            $subDir = Join-Path $targetDir 'MediaInfoLib'
            if (Test-Path $subDir) {
                Get-ChildItem -Path $subDir -File | Move-Item -Destination $targetDir -Force
                Remove-Item -Path $subDir -Recurse -Force
            }

            # Resolve symlink: libmediainfo.dylib is a symlink -> libmediainfo.0.dylib.
            # Remove the 0-byte symlink and rename the actual binary.
            $linkFile = Join-Path $targetDir 'libmediainfo.dylib'
            $realFile = Join-Path $targetDir 'libmediainfo.0.dylib'
            if ((Test-Path $linkFile) -and (Get-Item $linkFile).Length -eq 0 -and (Test-Path $realFile)) {
                Remove-Item -Path $linkFile -Force
                Rename-Item -Path $realFile -NewName 'libmediainfo.dylib'
                Write-Host "[macOS] Resolved symlink: libmediainfo.0.dylib -> libmediainfo.dylib"
            }
        }
        finally {
            Remove-Item -Path $tempTar -Force -ErrorAction SilentlyContinue
        }
    }
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
