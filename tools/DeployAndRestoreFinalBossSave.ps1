[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$dllSource = Join-Path $repoRoot "bin\\Release\\net9.0\\STS2Plus.dll"
$dllDest = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2Plus\\STS2Plus.dll"
$fixtureSave = "C:\\Users\\rebel\\Downloads\\STS2 Plus vibe\\Saves\\current_run.save"
$liveSave = "C:\\Users\\rebel\\AppData\\Roaming\\SlayTheSpire2\\steam\\76561197961957476\\profile1\\saves\\current_run.save"
$liveSaveDir = Split-Path -Parent $liveSave
$backupDir = Join-Path $liveSaveDir "backups"
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"

$runningGame = Get-Process -Name "SlayTheSpire2" -ErrorAction SilentlyContinue
if ($runningGame) {
    throw "SlayTheSpire2.exe is running. Close the game before deploying."
}

Push-Location $repoRoot
try {
    dotnet build -c Release
}
finally {
    Pop-Location
}

if (-not (Test-Path $dllSource)) {
    throw "Built DLL not found: $dllSource"
}
if (-not (Test-Path $fixtureSave)) {
    throw "Fixture save not found: $fixtureSave"
}
if (-not (Test-Path $liveSaveDir)) {
    throw "Live save directory not found: $liveSaveDir"
}

Copy-Item -Path $dllSource -Destination $dllDest -Force

New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
if (Test-Path $liveSave) {
    $backupSave = Join-Path $backupDir ("current_run.{0}.save" -f $timestamp)
    Copy-Item -Path $liveSave -Destination $backupSave -Force
    Write-Host "Backed up live save to $backupSave"
}
else {
    Write-Host "Live save did not exist; no backup created."
}

Copy-Item -Path $fixtureSave -Destination $liveSave -Force

$dllInfo = Get-Item $dllDest
$saveInfo = Get-Item $liveSave

Write-Host ("DLL deployed: {0}" -f $dllInfo.FullName)
Write-Host ("DLL LastWriteTime: {0}" -f $dllInfo.LastWriteTime)
Write-Host ("Save restored: {0}" -f $saveInfo.FullName)
Write-Host ("Save LastWriteTime: {0}" -f $saveInfo.LastWriteTime)
