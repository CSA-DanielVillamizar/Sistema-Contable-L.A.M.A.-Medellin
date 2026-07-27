param([int]$Port = 7099)

$root = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin"
$proj = "$root\LAMAMedellin\src\LAMAMedellin.API\LAMAMedellin.API.csproj"
$apiLog = "$root\api-7099.log"

Set-Location $root

# ── 1. Limpiar procesos dotnet huerfanos (de ejecuciones anteriores) ────────
Write-Host "[1] Limpiando procesos dotnet..." -ForegroundColor Cyan
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "    OK" -ForegroundColor Green

# ── 2. Iniciar API en puerto limpio 7099 ─────────────────────────────────────
Write-Host "[2] Iniciando API en https://localhost:$Port ..." -ForegroundColor Cyan

if (Test-Path $apiLog) { Remove-Item $apiLog -Force }

# Usamos cmd.exe para capturar stdout+stderr en el log y ASPNETCORE_URLS como env var
$cmdArgs = "/c set ASPNETCORE_URLS=https://localhost:$Port && set ASPNETCORE_ENVIRONMENT=Development && dotnet run --no-launch-profile --project `"$proj`" >> `"$apiLog`" 2>&1"

$apiProcess = Start-Process -FilePath "cmd.exe" -ArgumentList $cmdArgs -WorkingDirectory $root -PassThru
Write-Host "    API PID: $($apiProcess.Id)" -ForegroundColor Green

# ── 3. Poll activo: esperar hasta que el puerto responda (max 60s) ───────────
Write-Host "[3] Esperando que puerto $Port este escuchando..." -ForegroundColor Cyan
$ready = $false
for ($i = 1; $i -le 60; $i++) {
    Start-Sleep -Seconds 1
    $nc = Test-NetConnection -ComputerName localhost -Port $Port -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    if ($nc.TcpTestSucceeded) {
        $ready = $true
        Write-Host "    API lista despues de ${i}s" -ForegroundColor Green
        break
    }
    Write-Host -NoNewline "."
}

if (-not $ready) {
    Write-Host ""
    Write-Host "    La API no respondio en 60s. Ultimas lineas del log:" -ForegroundColor Yellow
    if (Test-Path $apiLog) { Get-Content $apiLog | Select-Object -Last 20 }
}

# ── 4. Ejecutar Happy Path ────────────────────────────────────────────────────
Write-Host "[4] Ejecutando Happy Path contra https://localhost:$Port..." -ForegroundColor Cyan
& "$root\test-cartera-simple.ps1" -ApiPort $Port

# ── 5. Leer reporte JSON mas reciente ─────────────────────────────────────────
Write-Host "[5] Reporte JSON:" -ForegroundColor Cyan
$latest = Get-ChildItem "$root\test-report-cartera-*.json" -ErrorAction SilentlyContinue |
          Sort-Object LastWriteTime -Descending |
          Select-Object -First 1
if ($latest) {
    Write-Host "    Archivo: $($latest.Name)" -ForegroundColor Green
    Get-Content $latest.FullName -Raw
} else {
    Write-Host "    No se encontro reporte JSON" -ForegroundColor Yellow
}

# ── 6. Detener API ───────────────────────────────────────────────────────────
Write-Host "[6] Deteniendo API..." -ForegroundColor Cyan
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
if (-not $apiProcess.HasExited) {
    Stop-Process -Id $apiProcess.Id -Force -ErrorAction SilentlyContinue
}
Write-Host "    API detenida" -ForegroundColor Green

Write-Host ""
Write-Host "--- API LOG (ultimas 20 lineas) ---" -ForegroundColor Gray
if (Test-Path $apiLog) { Get-Content $apiLog | Select-Object -Last 20 }
