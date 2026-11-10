param([string]$RepoPath)
$ErrorActionPreference = 'Stop'
$source = Split-Path -Parent $MyInvocation.MyCommand.Path
if (-not $RepoPath) {
    Add-Type -AssemblyName System.Windows.Forms
    $dlg = New-Object System.Windows.Forms.FolderBrowserDialog
    $dlg.Description = 'Select the EXISTING RoughHouse_Combat_Prototype GitHub repository folder'
    if ($dlg.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) { Write-Host 'Cancelled.'; exit 1 }
    $RepoPath = $dlg.SelectedPath
}
$RepoPath = (Resolve-Path -LiteralPath $RepoPath).Path
if (-not (Test-Path -LiteralPath (Join-Path $RepoPath '.git'))) {
    throw "Not a Git repository: $RepoPath. Select the folder that contains .git, Assets, Packages."
}
if (-not (Test-Path -LiteralPath (Join-Path $RepoPath 'Assets/RoughHouse/Scripts/RoughHousePrototype.cs'))) {
    throw "Wrong repository: RoughHousePrototype.cs not found in $RepoPath"
}
Write-Host "Installing V6.3 to $RepoPath"
foreach ($dir in @('Assets','Packages','ProjectSettings')) {
    Copy-Item -LiteralPath (Join-Path $source $dir) -Destination $RepoPath -Recurse -Force
}
foreach ($file in @('README.md','CLOUD_BUILD_README.md','DELIVERY_STATUS.txt','V6_3_FIX_NOTES.txt')) {
    Copy-Item -LiteralPath (Join-Path $source $file) -Destination (Join-Path $RepoPath $file) -Force
}
$obsolete = @(
 'Assets/RoughHouse/Editor/RoughHouseCloudAssets.cs',
 'Assets/RoughHouse/Editor/RoughHouseCloudAssets.cs.meta',
 'Assets/RoughHouse/Editor/RoughHouseHumanSetup.cs',
 'Assets/RoughHouse/Editor/RoughHouseHumanSetup.cs.meta'
)
foreach ($rel in $obsolete) {
    $full = Join-Path $RepoPath $rel
    if (Test-Path -LiteralPath $full) {
        Remove-Item -LiteralPath $full -Force
        Write-Host "Removed obsolete: $rel"
    }
}
$remaining = @($obsolete | Where-Object { Test-Path -LiteralPath (Join-Path $RepoPath $_) })
if ($remaining.Count -gt 0) { throw ('Old V5 scripts remain: ' + ($remaining -join ', ')) }
$manifestPath = Join-Path $RepoPath 'Packages/manifest.json'
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
if (-not $manifest.dependencies.'com.unity.modules.audio') { throw 'Audio package missing.' }
if ($manifest.dependencies.'com.unity.modules.inputlegacy') { throw 'Invalid legacy input package present.' }
if (-not (Test-Path -LiteralPath (Join-Path $RepoPath 'ProjectSettings/EditorBuildSettings.asset'))) { throw 'Build scene settings missing.' }
Write-Host ''
Write-Host 'SUCCESS: V6.3 files copied, V5 validation scripts removed, manifest checked.' -ForegroundColor Green
Write-Host 'Next: In GitHub Desktop, verify DELETED files are checked, COMMIT to master, then PUSH origin.'
