<#
.SYNOPSIS
    Builds the STS2Plus mod DLL.

.DESCRIPTION
    Compiles STS2Plus.dll against the game's assemblies and optionally
    installs it to the game's Mods directory.

.PARAMETER GameDir
    Path to the Slay the Spire 2 installation directory.
    Falls back to the STS2_GAME_DIR environment variable if not specified.

.PARAMETER Configuration
    Build configuration (default: Release).

.PARAMETER Install
    If specified, copies the built DLL and manifest to the game's Mods directory.

.EXAMPLE
    .\build.ps1
    .\build.ps1 -Install
    .\build.ps1 -GameDir "D:\SteamLibrary\steamapps\common\Slay the Spire 2" -Install
#>
param(
    [string]$GameDir,
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    [switch]$Install
)

$ErrorActionPreference = "Stop"

# --- Resolve game directory ---
if (-not $GameDir) {
    $GameDir = $env:STS2_GAME_DIR
}
if (-not $GameDir) {
    # Try common Steam path
    $defaultPath = "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
    if (Test-Path $defaultPath) {
        $GameDir = $defaultPath
    }
}
if (-not $GameDir) {
    Write-Host @"
ERROR: Game directory not specified.

Provide it via parameter or environment variable:
  .\build.ps1 -GameDir "D:\SteamLibrary\steamapps\common\Slay the Spire 2"

Or set it once in your PowerShell profile:
  `$env:STS2_GAME_DIR = "D:\SteamLibrary\steamapps\common\Slay the Spire 2"
"@ -ForegroundColor Red
    exit 1
}

$dllDir = Join-Path $GameDir "data_sts2_windows_x86_64"
if (-not (Test-Path (Join-Path $dllDir "sts2.dll"))) {
    Write-Host "ERROR: Could not find sts2.dll in '$dllDir'." -ForegroundColor Red
    Write-Host "Make sure -GameDir points to the Slay the Spire 2 installation root." -ForegroundColor Red
    exit 1
}

# --- Check prerequisites ---
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host @"
ERROR: 'dotnet' not found.

Install the .NET 9 SDK from:
  https://dotnet.microsoft.com/download/dotnet/9.0
"@ -ForegroundColor Red
    exit 1
}

# --- Build ---
$scriptDir = $PSScriptRoot
$project = Join-Path $scriptDir "STS2Plus.csproj"
$outDir = Join-Path (Join-Path $scriptDir "out") "STS2Plus"

Write-Host "=== Building STS2Plus ($Configuration) ===" -ForegroundColor Cyan
Write-Host "Game directory : $GameDir"
Write-Host "Output         : $outDir"
Write-Host ""

dotnet build $project -c $Configuration -o $outDir -p:STS2GameDir="$GameDir"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "=== Build succeeded ===" -ForegroundColor Green

# --- Install ---
if ($Install) {
    $modDir = Join-Path $GameDir "Mods\STS2Plus"
    Write-Host ""
    Write-Host "Installing to: $modDir" -ForegroundColor Cyan

    if (-not (Test-Path $modDir)) {
        New-Item -ItemType Directory -Path $modDir -Force | Out-Null
    }

    Copy-Item (Join-Path $outDir "STS2Plus.dll") $modDir -Force
    Copy-Item (Join-Path $scriptDir "mod_manifest.json") (Join-Path $modDir "STS2Plus.json") -Force

    Write-Host "=== Installed ===" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "To install, run:  .\build.ps1 -Install"
    Write-Host "Or manually copy:"
    Write-Host "  $outDir\STS2Plus.dll  ->  <game>\Mods\STS2Plus\STS2Plus.dll"
    Write-Host "  $scriptDir\mod_manifest.json  ->  <game>\Mods\STS2Plus\STS2Plus.json"
}
