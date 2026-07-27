param([string]$ApiPort = "7015")

$apiBaseUrl = "https://localhost:$ApiPort"
$results = @()

# PowerShell 5.1 compatibility for local self-signed certificate
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

Write-Host "QA TEST: HAPPY PATH - ENDPOINTS CARTERA" -ForegroundColor Cyan
Write-Host "Obteniendo token..." -ForegroundColor Yellow
$token = az account get-access-token --query "accessToken" -o tsv

if (-not $token) {
    Write-Host "ERROR: No se pudo obtener token" -ForegroundColor Red
    exit 1
}

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type"  = "application/json"
}

# PASO 1: Crear Miembro
Write-Host "`nPASO 1: Crear Miembro..." -ForegroundColor Cyan
$body1 = @{
    documentoIdentidad = "CC-TEST-001"
    nombres            = "Juan Carlos"
    apellidos          = "Garcia Lopez"
    apodo              = "JC"
    fechaIngreso       = "2026-03-29"
    tipoMiembro        = 2
} | ConvertTo-Json

Write-Host "Request: $body1" -ForegroundColor Gray

try {
    $resp1 = Invoke-RestMethod "$apiBaseUrl/api/cartera/miembros" -Method Post -Headers $headers -Body $body1
    $miembroId = $resp1.id
    Write-Host "EXITO: MiembroId = $miembroId" -ForegroundColor Green
    $results += @{paso=1; status=201; id=$miembroId}
} catch {
    Write-Host "FALLO: $($_.Exception.Message)" -ForegroundColor Red
    $results += @{paso=1; status=$($_.Exception.Response.StatusCode); error=$($_.Exception.Message)}
}

# PASO 2: Crear Concepto
Write-Host "`nPASO 2: Crear Concepto de Cobro..." -ForegroundColor Cyan
$cuentaId = "550e8400-e29b-41d4-a716-446655440000"
$body2 = @{
    nombre                  = "Cuota Mensual Test"
    valorCOP                = 150000
    periodicidadMensual     = 1
    cuentaContableIngresoId = $cuentaId
} | ConvertTo-Json

Write-Host "Request: $body2" -ForegroundColor Gray

try {
    $resp2 = Invoke-RestMethod "$apiBaseUrl/api/cartera/conceptos-cobro" -Method Post -Headers $headers -Body $body2
    $conceptoId = $resp2.id
    Write-Host "EXITO: ConceptoId = $conceptoId" -ForegroundColor Green
    $results += @{paso=2; status=201; id=$conceptoId}
} catch {
    Write-Host "FALLO: $($_.Exception.Message)" -ForegroundColor Red
    $results += @{paso=2; status=$($_.Exception.Response.StatusCode); error=$($_.Exception.Message)}
}

# PASO 3: Crear CxC
if ($miembroId -and $conceptoId) {
    Write-Host "`nPASO 3: Crear Cuenta por Cobrar..." -ForegroundColor Cyan
    $body3 = @{
        miembroId        = $miembroId
        conceptoCobroId  = $conceptoId
        fechaEmision     = "2026-03-29"
        fechaVencimiento = "2026-04-29"
        valorTotal       = 150000
    } | ConvertTo-Json

    Write-Host "Request: $body3" -ForegroundColor Gray

    try {
        $resp3 = Invoke-RestMethod "$apiBaseUrl/api/cartera/cuentas-por-cobrar" -Method Post -Headers $headers -Body $body3
        $cxcId = $resp3.id
        Write-Host "EXITO: CxCId = $cxcId" -ForegroundColor Green
        $results += @{paso=3; status=201; id=$cxcId}
    } catch {
        Write-Host "FALLO: $($_.Exception.Message)" -ForegroundColor Red
        $results += @{paso=3; status=$($_.Exception.Response.StatusCode); error=$($_.Exception.Message)}
    }
}

# Guardar reporte
$reportFile = "test-report-cartera-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
$results | ConvertTo-Json | Out-File -FilePath $reportFile -Encoding UTF8
Write-Host "`nReporte guardado en: $reportFile" -ForegroundColor Yellow
Write-Host "Contenido:"
Get-Content $reportFile

# Resumen
$success = @($results | Where-Object {$_.status -eq 201}).Count
Write-Host "`nRESUMEN: $success de 3 pasos exitosos" -ForegroundColor Green
