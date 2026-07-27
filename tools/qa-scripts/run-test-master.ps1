Write-Host "========== HAPPY PATH AUTOMATIZADO ==========" -ForegroundColor Cyan
Write-Host "[PASO 1] Iniciando API..." -ForegroundColor Yellow

$projectPath = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin\src\LAMAMedellin.API\LAMAMedellin.API.csproj"
$workDir = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin"

$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`" --configuration Development" -NoNewWindow -PassThru -WorkingDirectory $workDir

Write-Host "API iniciada (PID: $($apiProcess.Id))" -ForegroundColor Green

Write-Host "[PASO 2] Esperando 15 segundos..." -ForegroundColor Yellow
for ($i = 1; $i -le 15; $i++) {
    Write-Host -NoNewline "."
    Start-Sleep -Seconds 1
}
Write-Host " LISTO" -ForegroundColor Green

Write-Host "[PASO 3] Ejecutando tests..." -ForegroundColor Yellow
cd "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin"
& ".\test-cartera-simple.ps1" -ApiPort 7015

Write-Host "[PASO 4] Deteniendo API..." -ForegroundColor Yellow
Stop-Process -InputObject $apiProcess -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "API detenida" -ForegroundColor Green

Write-Host "========== COMPLETADO ==========" -ForegroundColor Cyan
