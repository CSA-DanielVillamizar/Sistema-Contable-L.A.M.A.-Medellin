$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$base = Join-Path $PSScriptRoot "..\src"
$base = (Resolve-Path $base).Path

$folders = @(
    (Join-Path $base "app"),
    (Join-Path $base "components"),
    (Join-Path $base "features"),
    (Join-Path $base "lib"),
    (Join-Path $base "providers"),
    (Join-Path $base "types")
)

foreach ($folder in $folders) {
    New-Item -ItemType Directory -Path $folder -Force | Out-Null
}

Write-Host "Estructura base creada/validada en frontend/src" -ForegroundColor Green
