#!/usr/bin/env pwsh
# Script: Automatizar Happy Path Completo

$apiProjectPath = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin\src\LAMAMedellin.API\LAMAMedellin.API.csproj"
$workingDir = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin"
$testScript = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\test-happy-path-cartera.ps1"
$apiPort = 7015

# Log File
$logFile = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\automation_run.log"

Write-Host "================ HAPPY PATH AUTOMATIZADO ================" -ForegroundColor Cyan
Write-Host "[1/5] Iniciando API en background..." -ForegroundColor Yellow

$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$apiProjectPath`" --configuration Development" -NoNewWindow -PassThru -WorkingDirectory $workingDir -ErrorAction SilentlyContinue

Write-Host "API iniciada (PID: $($apiProcess.Id))" -ForegroundColor Green

Write-Host "[2/5] Esperando 15 segundos..." -ForegroundColor Yellow
for ($i = 1; $i -le 15; $i++) {
    Write-Host -NoNewline "."
    Start-Sleep -Seconds 1
}
Write-Host ""

Write-Host "[3/5] Ejecutando tests..." -ForegroundColor Yellow
& $testScript -ApiPort $apiPort 2>&1 | Out-File -FilePath $logFile -Append

Write-Host "[4/5] Leyendo reporte..." -ForegroundColor Yellow
$reportFiles = Get-ChildItem "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\test-report-cartera-*.json" -ErrorAction SilentlyContinue | Sort-Object -Property LastWriteTime -Descending

if ($reportFiles) {
    $latestReport = $reportFiles[0]
    Write-Host "Reporte: $($latestReport.Name)" -ForegroundColor Green
    $reportContent = Get-Content $latestReport.FullName -Raw
    $reportContent | Out-File -FilePath $logFile -Append
    Write-Host $reportContent
}

Write-Host "[5/5] Deteniendo API..." -ForegroundColor Yellow
if ($apiProcess -and -not $apiProcess.HasExited) {
    Stop-Process -InputObject $apiProcess -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    Write-Host "API detenida" -ForegroundColor Green
}

Write-Host "================ COMPLETADO ================" -ForegroundColor Cyan
