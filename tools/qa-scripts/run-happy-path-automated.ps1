#####################################################################
# SCRIPT MAESTRO: Automatizar Happy Path Completo
# - Inicia API en background
# - Espera 15 segundos
# - Ejecuta tests
# - Captura reporte
# - Detiene API
#####################################################################

param(
    [int]$WaitSeconds = 15,
    [int]$ApiPort = 7015
)

Write-Host "`n$('='*80)" -ForegroundColor Cyan
Write-Host "SCRIPT MAESTRO: HAPPY PATH AUTOMATIZADO" -ForegroundColor Cyan
Write-Host "$('='*80)`n" -ForegroundColor Cyan

# Paso 1: Iniciar API en background
Write-Host "[1/5] Iniciando API en background..." -ForegroundColor Yellow
$projectPath = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin\src\LAMAMedellin.API\LAMAMedellin.API.csproj"

$apiProcess = Start-Process -FilePath "dotnet" `
    -ArgumentList "run --project `"$projectPath`" --configuration Development" `
    -NoNewWindow `
    -PassThru `
    -WorkingDirectory "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin"

Write-Host "✅ Proceso API iniciado (PID: $($apiProcess.Id))" -ForegroundColor Green

# Paso 2: Esperar a que la API se levante
Write-Host "`n[2/5] Esperando a que API escuche en puerto $ApiPort..." -ForegroundColor Yellow
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$apiReady = $false

for ($i = 1; $i -le $WaitSeconds; $i++) {
    $connected = $false
    try {
        $test = Test-NetConnection -ComputerName localhost -Port $ApiPort -WarningAction SilentlyContinue
        if ($test.TcpTestSucceeded) {
            $apiReady = $true
            Write-Host "✅ API respondiendo después de $i segundos" -ForegroundColor Green
            break
        }
    } catch { }

    Write-Host -NoNewline "."
    Start-Sleep -Seconds 1
}

if (-not $apiReady) {
    Write-Host "`n⚠️ API no respondió después de $WaitSeconds segundos, pero continuando..." -ForegroundColor Yellow
}

# Paso 3: Ejecutar el script de test
Write-Host "`n[3/5] Ejecutando script de Happy Path..." -ForegroundColor Yellow
$testScriptPath = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\test-happy-path-cartera.ps1"

$testOutput = & $testScriptPath -ApiPort $ApiPort 2>&1
Write-Host $testOutput

# Paso 4: Buscar y leer el reporte JSON
Write-Host "`n[4/5] Buscando reporte JSON generado..." -ForegroundColor Yellow
$reportPath = Get-ChildItem "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\test-report-cartera-*.json" -ErrorAction SilentlyContinue | Sort-Object -Property LastWriteTime -Descending | Select-Object -First 1

if ($reportPath) {
    Write-Host "✅ Reporte encontrado: $($reportPath.Name)" -ForegroundColor Green
    $reportContent = Get-Content -Path $reportPath.FullName -Raw
    Write-Host "`n📄 CONTENIDO DEL REPORTE JSON:" -ForegroundColor Cyan
    Write-Host $reportContent
} else {
    Write-Host "⚠️ No se encontró reporte JSON" -ForegroundColor Yellow
}

# Paso 5: Detener la API
Write-Host "`n[5/5] Deteniendo proceso API..." -ForegroundColor Yellow
if ($apiProcess -and -not $apiProcess.HasExited) {
    Stop-Process -InputObject $apiProcess -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    Write-Host "✅ Proceso API detenido (PID: $($apiProcess.Id))" -ForegroundColor Green
}

Write-Host "`n$('='*80)" -ForegroundColor Cyan
Write-Host "✅ AUTOMATIZACIÓN COMPLETADA" -ForegroundColor Cyan
Write-Host "$('='*80)`n" -ForegroundColor Cyan
