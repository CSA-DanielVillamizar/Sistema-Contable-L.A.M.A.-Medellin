$projectPath = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin\src\LAMAMedellin.API\LAMAMedellin.API.csproj"
$repoRoot = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin"
$apiOut = Join-Path $repoRoot "api-qa-run.out.log"
$apiErr = Join-Path $repoRoot "api-qa-run.err.log"

if (Test-Path $apiOut) { Remove-Item $apiOut -Force }
if (Test-Path $apiErr) { Remove-Item $apiErr -Force }

Write-Host "[1] Iniciando API en background (solo https://localhost:7015)..." -ForegroundColor Cyan
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --no-launch-profile --project `"$projectPath`" --urls https://localhost:7015" -WorkingDirectory $repoRoot -PassThru -RedirectStandardOutput $apiOut -RedirectStandardError $apiErr
Write-Host "API PID: $($apiProcess.Id)"

Write-Host "[2] Esperando 15 segundos..." -ForegroundColor Cyan
Start-Sleep -Seconds 15

$portReady = Test-NetConnection -ComputerName localhost -Port 7015 -WarningAction SilentlyContinue
Write-Host "Puerto 7015 escuchando: $($portReady.TcpTestSucceeded)"

Write-Host "[3] Ejecutando script de prueba..." -ForegroundColor Cyan
Set-Location $repoRoot
& ".\test-cartera-simple.ps1" -ApiPort 7015

Write-Host "[4] Leyendo ultimo reporte JSON..." -ForegroundColor Cyan
$latest = Get-ChildItem "$repoRoot\test-report-cartera-*.json" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($latest) {
  Write-Host "Reporte: $($latest.FullName)"
  Get-Content $latest.FullName -Raw
}

Write-Host "[5] Deteniendo API..." -ForegroundColor Cyan
if ($apiProcess -and -not $apiProcess.HasExited) {
  Stop-Process -Id $apiProcess.Id -Force
  Write-Host "API detenida (PID $($apiProcess.Id))"
} else {
  Write-Host "API ya estaba detenida"
}

Write-Host "--- API stdout (ultimas 30 lineas) ---"
if (Test-Path $apiOut) { Get-Content $apiOut | Select-Object -Last 30 }
Write-Host "--- API stderr (ultimas 30 lineas) ---"
if (Test-Path $apiErr) { Get-Content $apiErr | Select-Object -Last 30 }
