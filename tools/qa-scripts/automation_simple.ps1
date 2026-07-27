#!/usr/bin/env pwsh
# Script simplificado: Inicia API, espera, prueba, recolecta resultados

$ErrorActionPreference = 'Continue'
$VerbosePreference = 'SilentlyContinue'

$apiProjectPath = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin\src\LAMAMedellin.API\LAMAMedellin.API.csproj"
$workingDir = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin"
$testScript = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\test-happy-path-cartera.ps1"
$apiPort = 7015

# Log
$logFile = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\execution_log.txt"
function Log($message) {
    $timestamp = Get-Date -Format "HH:mm:ss"
    "$timestamp | $message" | Tee-Object -FilePath $logFile -Append | Write-Host
}

Log "================ INICIANDO HAPPY PATH AUTOMATIZADO ================"
Log "[1/5] Iniciando API en background..."

# Iniciar API
$apiProcess = Start-Process -FilePath "dotnet" `
    -ArgumentList "run --project `"$apiProjectPath`" --configuration Development" `
    -NoNewWindow `
    -PassThru `
    -WorkingDirectory $workingDir `
    -ErrorAction SilentlyContinue

Log "API iniciada (PID: $($apiProcess.Id))"

# Esperar
Log "[2/5] Esperando 15 segundos para que API esté lista..."
for ($i = 1; $i -le 15; $i++) {
    Write-Host -NoNewline "."
    Start-Sleep -Seconds 1
}
Log " ✓"

# Verificar puerto
Log "[3/5] Verificando conectividad..."
$connected = Test-NetConnection -ComputerName localhost -Port $apiPort -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
if ($connected.TcpTestSucceeded) {
    Log "✓ API respondiendo en puerto 7015"
} else {
    Log "✗ API no responde aún, continuando..."
}

# Ejecutar test
Log "[4/5] Ejecutando tests..."
Log "========================================================================"
& $testScript -ApiPort $apiPort *>&1 | Tee-Object -FilePath $logFile -Append

# Leer reporte
Log "[5/5] Buscando reporte JSON..."
$reportFile = Get-ChildItem "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\test-report-cartera-*.json" -ErrorAction SilentlyContinue | Sort-Object -Property LastWriteTime -Descending | Select-Object -First 1

if ($reportFile) {
    Log "✓ Reporte encontrado: $($reportFile.Name)"
    Log "========================================================================"
    Log "CONTENIDO DEL REPORTE:"
    Log "========================================================================"
    (Get-Content $reportFile.FullName -Raw) | Tee-Object -FilePath $logFile -Append
} else {
    Log "✗ No se encontró reporte JSON"
}

# Detener API
Log "[6/6] Deteniendo API (PID: $($apiProcess.Id))..."
if ($apiProcess -and -not $apiProcess.HasExited) {
    Stop-Process -InputObject $apiProcess -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    Log "✓ API detenida"
}

Log "================ AUTOMATIZACIÓN COMPLETADA ================"
