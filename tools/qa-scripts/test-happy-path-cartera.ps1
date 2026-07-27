
#####################################################################
# QA Test: Happy Path - Cartera Endpoints
# Script para probar los 3 nuevos endpoints de Cartera
#####################################################################

param(
    [string]$ApiPort = "7015",
    [switch]$SkipAuth = $false
)

# Configuración
$apiBaseUrl = "https://localhost:$ApiPort"
$reportOutput = @()

Write-Host "`n" + "="*70 -ForegroundColor Cyan
Write-Host "QA TEST: HAPPY PATH - ENDPOINTS CARTERA" -ForegroundColor Cyan
Write-Host "="*70 -ForegroundColor Cyan

# Obtener token de autenticación
Write-Host "`n[*] Obteniendo token de Azure AD..." -ForegroundColor Yellow
$token = az account get-access-token --query "accessToken" -o tsv

if (-not $token -or $token.Length -eq 0) {
    Write-Host "❌ No se pudo obtener token. Abortando." -ForegroundColor Red
    exit 1
}

Write-Host "✅ Token obtenido ($('{0:000}' -f $token.Length) caracteres)" -ForegroundColor Green

# Headers para todas las requests
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type"  = "application/json"
}

#####################################################################
# PASO 1: CREAR MIEMBRO
#####################################################################

Write-Host "`n" + "-"*70 -ForegroundColor White
Write-Host "PASO 1: Crear Miembro (POST /api/cartera/miembros)" -ForegroundColor Cyan
Write-Host "-"*70 -ForegroundColor White

$request1 = @{
    documentoIdentidad = "CC-123456789"
    nombres            = "Juan Carlos"
    apellidos          = "García López"
    apodo              = "JC"
    fechaIngreso       = "2026-03-29"
    tipoMiembro        = 2  # Activo
}

$payload1 = $request1 | ConvertTo-Json
Write-Host "`n📤 Payload enviado:" -ForegroundColor Gray
Write-Host $payload1 -ForegroundColor Gray

try {
    $response1 = Invoke-RestMethod -Uri "$apiBaseUrl/api/cartera/miembros" `
        -Method Post `
        -Headers $headers `
        -Body $payload1 `
        -SkipCertificateCheck

    $miembroId = $response1.id

    Write-Host "`n✅ HTTP 201 Created" -ForegroundColor Green
    Write-Host "📥 Respuesta del servidor:" -ForegroundColor Gray
    Write-Host ($response1 | ConvertTo-Json) -ForegroundColor Green

    $reportOutput += @{
        paso            = "1"
        endpoint        = "POST /api/cartera/miembros"
        status          = "201 Created"
        payload         = $payload1
        respuesta       = $response1 | ConvertTo-Json
        exito           = $true
        datosCapturados = "MiembroId = $miembroId"
    }

}
catch {
    Write-Host "`n❌ HTTP $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    $errorBody = $_.Exception.Response.Content.ToString()
    Write-Host "Error: $errorBody" -ForegroundColor Red

    $reportOutput += @{
        paso      = "1"
        endpoint  = "POST /api/cartera/miembros"
        status    = "$($_.Exception.Response.StatusCode)"
        payload   = $payload1
        respuesta = $errorBody
        exito     = $false
    }

    Write-Host "`n⚠️ Test abortado." -ForegroundColor Yellow
    exit 1
}

#####################################################################
# PASO 2: CREAR CONCEPTO COBRO
#####################################################################

Write-Host "`n" + "-"*70 -ForegroundColor White
Write-Host "PASO 2: Crear Concepto de Cobro (POST /api/cartera/conceptos-cobro)" -ForegroundColor Cyan
Write-Host "-"*70 -ForegroundColor White

# Necesitamos una CuentaContableIngresoId válida
# Para este test, usaremos un GUID de ejemplo
$cuentaContableIngresoId = "550e8400-e29b-41d4-a716-446655440000"

$request2 = @{
    nombre                  = "Cuota Mensual Test"
    valorCOP                = 150000
    periodicidadMensual     = 1
    cuentaContableIngresoId = $cuentaContableIngresoId
}

$payload2 = $request2 | ConvertTo-Json
Write-Host "`n📤 Payload enviado:" -ForegroundColor Gray
Write-Host $payload2 -ForegroundColor Gray

try {
    $response2 = Invoke-RestMethod -Uri "$apiBaseUrl/api/cartera/conceptos-cobro" `
        -Method Post `
        -Headers $headers `
        -Body $payload2 `
        -SkipCertificateCheck

    $conceptoId = $response2.id

    Write-Host "`n✅ HTTP 201 Created" -ForegroundColor Green
    Write-Host "📥 Respuesta del servidor:" -ForegroundColor Gray
    Write-Host ($response2 | ConvertTo-Json) -ForegroundColor Green

    $reportOutput += @{
        paso            = "2"
        endpoint        = "POST /api/cartera/conceptos-cobro"
        status          = "201 Created"
        payload         = $payload2
        respuesta       = $response2 | ConvertTo-Json
        exito           = $true
        datosCapturados = "ConceptoCobroId = $conceptoId"
    }

}
catch {
    Write-Host "`n❌ HTTP $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    $errorBody = $_.Exception.Response.Content.ToString()
    Write-Host "Error: $errorBody" -ForegroundColor Red

    $reportOutput += @{
        paso      = "2"
        endpoint  = "POST /api/cartera/conceptos-cobro"
        status    = "$($_.Exception.Response.StatusCode)"
        payload   = $payload2
        respuesta = $errorBody
        exito     = $false
    }

    # Continuamos aunque falle este paso
}

#####################################################################
# PASO 3: CREAR CUENTA POR COBRAR
#####################################################################

Write-Host "`n" + "-"*70 -ForegroundColor White
Write-Host "PASO 3: Crear Cuenta por Cobrar (POST /api/cartera/cuentas-por-cobrar)" -ForegroundColor Cyan
Write-Host "-"*70 -ForegroundColor White

$request3 = @{
    miembroId        = $miembroId
    conceptoCobroId  = $conceptoId
    fechaEmision     = "2026-03-29"
    fechaVencimiento = "2026-04-29"
    valorTotal       = 150000
}

$payload3 = $request3 | ConvertTo-Json
Write-Host "`n📤 Payload enviado:" -ForegroundColor Gray
Write-Host "$payload3 (con IDs obtenidos de pasos anteriores)" -ForegroundColor Gray

try {
    $response3 = Invoke-RestMethod -Uri "$apiBaseUrl/api/cartera/cuentas-por-cobrar" `
        -Method Post `
        -Headers $headers `
        -Body $payload3 `
        -SkipCertificateCheck

    $cxcId = $response3.id

    Write-Host "`n✅ HTTP 201 Created" -ForegroundColor Green
    Write-Host "📥 Respuesta del servidor:" -ForegroundColor Gray
    Write-Host ($response3 | ConvertTo-Json) -ForegroundColor Green

    $reportOutput += @{
        paso            = "3"
        endpoint        = "POST /api/cartera/cuentas-por-cobrar"
        status          = "201 Created"
        payload         = $payload3
        respuesta       = $response3 | ConvertTo-Json
        exito           = $true
        datosCapturados = "CxCId = $cxcId"
    }

}
catch {
    Write-Host "`n❌ HTTP $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    $errorBody = $_.Exception.Response.Content.ToString()
    Write-Host "Error: $errorBody" -ForegroundColor Red

    $reportOutput += @{
        paso      = "3"
        endpoint  = "POST /api/cartera/cuentas-por-cobrar"
        status    = "$($_.Exception.Response.StatusCode)"
        payload   = $payload3
        respuesta = $errorBody
        exito     = $false
    }
}

#####################################################################
# RESUMEN
#####################################################################

Write-Host "`n" + "="*70 -ForegroundColor Cyan
Write-Host "RESUMEN DE PRUEBAS" -ForegroundColor Cyan
Write-Host "="*70 -ForegroundColor Cyan

$exitosos = @($reportOutput | Where-Object { $_.exito -eq $true }).Count
$fallidos = @($reportOutput | Where-Object { $_.exito -ne $true }).Count

Write-Host "`n✅ Exitosos: $exitosos" -ForegroundColor Green
Write-Host "❌ Fallidos: $fallidos" -ForegroundColor Red

foreach ($resultado in $reportOutput) {
    $icon = if ($resultado.exito) { "✅" } else { "❌" }
    Write-Host "`n$icon Paso $($resultado.paso): $($resultado.endpoint)"
    Write-Host "   Status: $($resultado.status)"
    if ($resultado.datosCapturados) {
        Write-Host "   Datos: $($resultado.datosCapturados)"
    }
}

# Exportar report a JSON
$reportPath = "$PSScriptRoot\test-report-cartera-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
$reportOutput | ConvertTo-Json | Out-File -FilePath $reportPath -Encoding UTF8

Write-Host "`n📄 Reporte guardado en: $reportPath" -ForegroundColor Yellow

Write-Host "`n" + "="*70 -ForegroundColor Cyan

# Conclusión
if ($exitosos -eq 3) {
    Write-Host "`n🎉 HAPPY PATH COMPLETADO EXITOSAMENTE" -ForegroundColor Green
    Write-Host "Todos los endpoints respondieron con 201 Created" -ForegroundColor Green
    exit 0
}
else {
    Write-Host "`n⚠️ HAPPY PATH INCOMPLETO" -ForegroundColor Yellow
    Write-Host "$exitosos de 3 pasos completados" -ForegroundColor Yellow
    exit 1
}
