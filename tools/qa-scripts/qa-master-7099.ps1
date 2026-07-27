param([int]$Port = 7099)

$root = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin"
$apiLog  = "$root\api-7099.log"
$batFile = "$root\start-api-7099.bat"
$testScript = "$root\test-cartera-simple.ps1"

Set-Location $root

#--- 1. Limpiar -----------------------------------------------------------
Write-Host "[1] Limpiando procesos dotnet..." -ForegroundColor Cyan
Get-Process dotnet -ErrorAction SilentlyContinue |
    Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
if (Test-Path $apiLog) { Remove-Item $apiLog -Force }
Write-Host "    OK" -ForegroundColor Green

#--- 2. Iniciar API via .bat (sin escape de argumentos) ──────────────────
Write-Host "[2] Iniciando API en https://localhost:$Port (via bat)..." -ForegroundColor Cyan
$apiProcess = Start-Process -FilePath $batFile -WindowStyle Hidden -PassThru
Write-Host "    PID cmd.exe: $($apiProcess.Id)" -ForegroundColor Green

#--- 3. Poll activo de puerto (max 50 segundos) ───────────────────────────
Write-Host "[3] Esperando que el puerto $Port responda..." -ForegroundColor Cyan
$ready = $false
for ($i = 1; $i -le 50; $i++) {
    Start-Sleep -Seconds 1
    $nc = Test-NetConnection -ComputerName localhost -Port $Port -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    if ($nc.TcpTestSucceeded) {
        Write-Host ""
        Write-Host "    Puerto $Port listo despues de ${i}s" -ForegroundColor Green
        $ready = $true
        break
    }
    Write-Host -NoNewline "."
    if ($i % 10 -eq 0) {
        Write-Host ""
        Write-Host "    Ultimo log:" -ForegroundColor Gray
        if (Test-Path $apiLog) { Get-Content $apiLog | Select-Object -Last 5 }
    }
}

if (-not $ready) {
    Write-Host ""
    Write-Host "    AVISO: API no respondio en 50s. Ultimas lineas del log:" -ForegroundColor Yellow
    if (Test-Path $apiLog) { Get-Content $apiLog | Select-Object -Last 20 }
}

#--- 4. Ejecutar Happy Path ────────────────────────────────────────────────
Write-Host "[4] Ejecutando Happy Path..." -ForegroundColor Cyan
& $testScript -ApiPort $Port

#--- 5. Reporte JSON ──────────────────────────────────────────────────────
Write-Host ""
Write-Host "[5] Buscando reporte JSON..." -ForegroundColor Cyan
$latest = Get-ChildItem "$root\test-report-cartera-*.json" -ErrorAction SilentlyContinue |
          Sort-Object LastWriteTime -Descending |
          Select-Object -First 1
if ($latest) {
    Write-Host "    $($latest.Name)" -ForegroundColor Green
    Write-Host "=== REPORT JSON ===" -ForegroundColor Green
    Get-Content $latest.FullName -Raw
} else {
    Write-Host "    No se encontro reporte" -ForegroundColor Yellow
}

#--- 6. Detener API ────────────────────────────────────────────────────────
Write-Host ""
Write-Host "[6] Deteniendo API..." -ForegroundColor Cyan
Get-Process dotnet -ErrorAction SilentlyContinue |
    Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1
Stop-Process -Id $apiProcess.Id -Force -ErrorAction SilentlyContinue
Write-Host "    Detenida" -ForegroundColor Green

Write-Host ""
Write-Host "=== API LOG (ultimas 25 lineas) ===" -ForegroundColor Gray
if (Test-Path $apiLog) { Get-Content $apiLog | Select-Object -Last 25 }
