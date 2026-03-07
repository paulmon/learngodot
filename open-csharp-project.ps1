Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$godotCommand = Get-Command godot -ErrorAction Stop
$godotExecutable = $godotCommand.Source
$godotItem = Get-Item $godotExecutable -ErrorAction SilentlyContinue

# Winget often exposes Godot through a symlink in WinGet\Links. Launching the
# real target exe avoids .NET assembly path resolution issues in mono builds.
if ($godotItem -and $godotItem.LinkType -and $godotItem.Target) {
    $godotExecutable = $godotItem.Target
}

$projectPath = Join-Path $PSScriptRoot "grid-shift-csharp"
if (-not (Test-Path $projectPath)) {
    throw "Project folder not found: $projectPath"
}

$godotProjectFile = Join-Path $projectPath "project.godot"
$csproj = Get-ChildItem -Path $projectPath -Filter "*.csproj" -File -ErrorAction SilentlyContinue | Select-Object -First 1

if (-not (Test-Path $godotProjectFile)) {
    Write-Host "No Godot project found in: $projectPath" -ForegroundColor Yellow
    Write-Host "Open Project Manager and create a new Godot 4 .NET project in this folder first." -ForegroundColor Yellow
    & $godotExecutable
    exit 0
}

if (-not $csproj) {
    Write-Host "Godot project exists, but no C# solution was found (*.csproj)." -ForegroundColor Yellow
    Write-Host "In Godot: Project -> Tools -> C# -> Create C# Solution, then build once." -ForegroundColor Yellow
}

& $godotExecutable --editor --path $projectPath
