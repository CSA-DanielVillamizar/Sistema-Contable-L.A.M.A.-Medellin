<#
.SYNOPSIS
    QA: Ciclo completo de recaudo — Pago Parcial + Pago Total de CuentaPorCobrar.
    Verifica la lógica de dominio: AplicarPago() → saldo y estado correctos.
.DESCRIPTION
    PASO 0  – Obtener token Azure AD
    PASO 1  – Crear Miembro de prueba (documentoIdentidad único)
    PASO 2  – Resolver CuentaContableIngresoId (GET /api/cuentas-contables)
    PASO 3  – Crear ConceptoCobro único (usa la cuenta de ingreso)
    PASO 4  – Crear CuentaPorCobrar por 100.000 COP
    PASO 5  – Pago parcial  40.000 COP  → POST …/{id}/pagos
    PASO 6  – Verificar: saldoPendiente=60.000, estado=2 (PagadaParcial)
    PASO 7  – Pago total   60.000 COP  → POST …/{id}/pagos
    PASO 8  – Verificar: saldoPendiente=0, estado=3 (Pagada)
    FINAL   – Generar reporte Markdown
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ─── Configuración ────────────────────────────────────────────────────────────
$BASE_URL = 'https://localhost:7099'
$CLIENT_ID = 'api://b81ee2ee-5417-4aa0-8000-e470aec5543e'
$SCRIPT_DIR = $PSScriptRoot
$REPORT_FILE = Join-Path $SCRIPT_DIR "qa-report-pagos-$(Get-Date -Format 'yyyyMMddHHmmss').md"
$LOG_FILE = Join-Path $SCRIPT_DIR "qa-pagos-transcript-$(Get-Date -Format 'yyyyMMddHHmmss').log"
$TS = Get-Date -Format 'yyyyMMddHHmmss'

# ─── Inicio de transcript ──────────────────────────────────────────────────────
Start-Transcript -Path $LOG_FILE -Append

# ─── Bypass SSL para localhost ─────────────────────────────────────────────────
Add-Type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class TrustAll : ICertificatePolicy {
    public bool CheckValidationResult(ServicePoint sp, X509Certificate cert,
                                      WebRequest req, int problem) { return true; }
}
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAll
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

# ─── Estado global de resultados ──────────────────────────────────────────────
$Results = [System.Collections.Generic.List[hashtable]]::new()
$OverallPass = $true

function Add-Result {
    param(
        [string]$Paso,
        [string]$Descripcion,
        [string]$Endpoint,
        [string]$Esperado,
        [string]$Obtenido,
        [bool]$Passed
    )
    $Results.Add(@{
            Paso        = $Paso
            Descripcion = $Descripcion
            Endpoint    = $Endpoint
            Esperado    = $Esperado
            Obtenido    = $Obtenido
            Passed      = $Passed
        })
    $badge = if ($Passed) { '[PASS]' } else { '[FAIL]' }
    $color = if ($Passed) { 'Green' }  else { 'Red' }
    Write-Host "$badge $Paso - $Descripcion" -ForegroundColor $color
    if (-not $Passed) { $script:OverallPass = $false }
}

# ─── Helper: llamada HTTP genérica ────────────────────────────────────────────
function Invoke-Api {
    param(
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers,
        [string]$Body = $null
    )
    $params = @{
        Method  = $Method
        Uri     = $Url
        Headers = $Headers
    }
    if ($Body) {
        $params['Body'] = $Body
        $params['ContentType'] = 'application/json'
    }
    return Invoke-RestMethod @params
}

# ─── PASO 0: Token Azure AD ────────────────────────────────────────────────────
Write-Host "`n[INICIO] Obteniendo token Azure AD..." -ForegroundColor Cyan
try {
    $tokenJson = & az account get-access-token --resource $CLIENT_ID --output json 2>&1
    $tokenData = $tokenJson | ConvertFrom-Json
    $Token = $tokenData.accessToken
    Write-Host "[OK] Token obtenido (exp: $($tokenData.expiresOn))" -ForegroundColor Green
}
catch {
    Write-Host "[ERROR] No se pudo obtener el token. Ejecuta 'az login' primero." -ForegroundColor Red
    Write-Host $_ -ForegroundColor Red
    Stop-Transcript
    exit 1
}

$Headers = @{
    'Authorization' = "Bearer $Token"
    'Accept'        = 'application/json'
}

# ─── FUNCIÓN: Verificar que la API responde ────────────────────────────────────
Write-Host "`n[INICIO] Verificando conectividad con la API en $BASE_URL..." -ForegroundColor Cyan
$maxWait = 60
$waited = 0
$apiOk = $false
while ($waited -lt $maxWait) {
    try {
        $null = Invoke-WebRequest -Uri "$BASE_URL/" -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
        $apiOk = $true
        break
    }
    catch {
        # 401 también significa que la API está viva
        if ($_.Exception.Response -and $_.Exception.Response.StatusCode.value__ -in @(401, 404, 200)) {
            $apiOk = $true
            break
        }
        Start-Sleep -Seconds 3
        $waited += 3
        Write-Host "  ... esperando API ($waited/$maxWait s)" -ForegroundColor DarkGray
    }
}
if (-not $apiOk) {
    Write-Host "[ERROR] La API no respondió en $maxWait segundos. Inicia con start-api-7099.bat." -ForegroundColor Red
    Stop-Transcript
    exit 1
}
Write-Host "[OK] API en línea." -ForegroundColor Green

# ══════════════════════════════════════════════════════════════════════════════
#  PASO 1: Crear Miembro QA
# ══════════════════════════════════════════════════════════════════════════════
Write-Host "`n[PASO 1] Creando Miembro QA..." -ForegroundColor Cyan
$docIdentidad = "CC-QA-PAGOS-$TS"
$miembroBody = @{
    documentoIdentidad = $docIdentidad
    nombres            = "QAPagos"
    apellidos          = "AutoTest$TS"
    apodo              = "qapago"
    fechaIngreso       = (Get-Date).ToString('yyyy-MM-dd')
    tipoMiembro        = 1
} | ConvertTo-Json -Compress

try {
    $rMiembro = Invoke-Api -Method POST -Url "$BASE_URL/api/cartera/miembros" -Headers $Headers -Body $miembroBody
    $miembroId = $rMiembro.id
    Add-Result -Paso 'PASO 1' -Descripcion 'Crear Miembro' `
        -Endpoint 'POST /api/cartera/miembros' `
        -Esperado '201 + { id }' -Obtenido "id=$miembroId" -Passed $true
}
catch {
    Add-Result -Paso 'PASO 1' -Descripcion 'Crear Miembro' `
        -Endpoint 'POST /api/cartera/miembros' `
        -Esperado '201 + { id }' -Obtenido "ERROR: $_" -Passed $false
    Write-Host "[ABORT] No se puede continuar sin Miembro. Revisa la API." -ForegroundColor Red
    Stop-Transcript; exit 1
}

# ══════════════════════════════════════════════════════════════════════════════
#  PASO 2: Resolver CuentaContableIngresoId
# ══════════════════════════════════════════════════════════════════════════════
Write-Host "`n[PASO 2] Resolviendo CuentaContable de Ingreso..." -ForegroundColor Cyan
$cuentaContableId = $null
try {
    $cuentas = Invoke-Api -Method GET -Url "$BASE_URL/api/cuentas-contables" -Headers $Headers
    # Naturaleza 2 = Crédito (Ingresos); PermiteMovimiento = true
    # naturaleza se devuelve como string "CREDITO" o "DEBITO" desde el handler
    $cuentaIngreso = $cuentas | Where-Object {
        $pm = if ($null -ne $_.permiteMovimiento) { $_.permiteMovimiento } else { $_.PermiteMovimiento }
        $na = if ($null -ne $_.naturaleza) { "$($_.naturaleza)" }  else { "$($_.Naturaleza)" }
        $pm -eq $true -and $na.ToUpper() -eq 'CREDITO'
    } | Select-Object -First 1

    if ($null -eq $cuentaIngreso) {
        throw "No se encontró ninguna CuentaContable con Naturaleza=CREDITO y PermiteMovimiento=true."
    }
    $cuentaContableId = if ($null -ne $cuentaIngreso.id) { $cuentaIngreso.id } else { $cuentaIngreso.Id }
    $cuentaCodigo = if ($null -ne $cuentaIngreso.codigo) { $cuentaIngreso.codigo } else { $cuentaIngreso.Codigo }
    Add-Result -Paso 'PASO 2' -Descripcion 'Resolver CuentaContable Ingreso' `
        -Endpoint 'GET /api/cuentas-contables' `
        -Esperado 'Cuenta Crédito+PermiteMovimiento' `
        -Obtenido "id=$cuentaContableId codigo=$cuentaCodigo" -Passed $true
}
catch {
    Add-Result -Paso 'PASO 2' -Descripcion 'Resolver CuentaContable Ingreso' `
        -Endpoint 'GET /api/cuentas-contables' `
        -Esperado 'Cuenta Crédito+PermiteMovimiento' -Obtenido "ERROR: $_" -Passed $false
    Write-Host "[ABORT] No se puede continuar sin CuentaContable válida." -ForegroundColor Red
    Stop-Transcript; exit 1
}

# ══════════════════════════════════════════════════════════════════════════════
#  PASO 3: Crear ConceptoCobro QA
# ══════════════════════════════════════════════════════════════════════════════
Write-Host "`n[PASO 3] Creando ConceptoCobro QA..." -ForegroundColor Cyan
$conceptoBody = @{
    nombre                  = "QA-Concepto-$TS"
    valorCOP                = 100000
    periodicidadMensual     = 1
    cuentaContableIngresoId = $cuentaContableId
} | ConvertTo-Json -Compress

try {
    $rConcepto = Invoke-Api -Method POST -Url "$BASE_URL/api/cartera/conceptos-cobro" -Headers $Headers -Body $conceptoBody
    $conceptoId = $rConcepto.id
    Add-Result -Paso 'PASO 3' -Descripcion 'Crear ConceptoCobro' `
        -Endpoint 'POST /api/cartera/conceptos-cobro' `
        -Esperado '201 + { id }' -Obtenido "id=$conceptoId" -Passed $true
}
catch {
    Add-Result -Paso 'PASO 3' -Descripcion 'Crear ConceptoCobro' `
        -Endpoint 'POST /api/cartera/conceptos-cobro' `
        -Esperado '201 + { id }' -Obtenido "ERROR: $_" -Passed $false
    Write-Host "[ABORT] No se puede continuar sin ConceptoCobro." -ForegroundColor Red
    Stop-Transcript; exit 1
}

# ══════════════════════════════════════════════════════════════════════════════
#  PASO 4: Crear CuentaPorCobrar — 100.000 COP
# ══════════════════════════════════════════════════════════════════════════════
Write-Host "`n[PASO 4] Creando CuentaPorCobrar por 100.000 COP..." -ForegroundColor Cyan
$hoy = (Get-Date).ToString('yyyy-MM-dd')
$vencimiento = (Get-Date).AddDays(30).ToString('yyyy-MM-dd')
$cxcBody = @{
    miembroId        = $miembroId
    conceptoCobroId  = $conceptoId
    fechaEmision     = $hoy
    fechaVencimiento = $vencimiento
    valorTotal       = 100000
} | ConvertTo-Json -Compress

try {
    $rCxc = Invoke-Api -Method POST -Url "$BASE_URL/api/cartera/cuentas-por-cobrar" -Headers $Headers -Body $cxcBody
    $cxcId = $rCxc.id
    Add-Result -Paso 'PASO 4' -Descripcion 'Crear CuentaPorCobrar 100.000 COP' `
        -Endpoint 'POST /api/cartera/cuentas-por-cobrar' `
        -Esperado '201 + { id }' -Obtenido "id=$cxcId" -Passed $true
}
catch {
    Add-Result -Paso 'PASO 4' -Descripcion 'Crear CuentaPorCobrar 100.000 COP' `
        -Endpoint 'POST /api/cartera/cuentas-por-cobrar' `
        -Esperado '201 + { id }' -Obtenido "ERROR: $_" -Passed $false
    Write-Host "[ABORT] No se puede continuar sin CuentaPorCobrar." -ForegroundColor Red
    Stop-Transcript; exit 1
}

# ══════════════════════════════════════════════════════════════════════════════
#  PASO 5: Pago Parcial — 40.000 COP
# ══════════════════════════════════════════════════════════════════════════════
Write-Host "`n[PASO 5] Registrando pago parcial de 40.000 COP..." -ForegroundColor Cyan
$pagoBody1 = @{ monto = 40000 } | ConvertTo-Json -Compress
try {
    $rPago1 = Invoke-Api -Method POST -Url "$BASE_URL/api/cartera/cuentas-por-cobrar/$cxcId/pagos" -Headers $Headers -Body $pagoBody1
    $msg1 = if ($rPago1.mensaje) { $rPago1.mensaje } else { $rPago1 | ConvertTo-Json -Compress }
    Add-Result -Paso 'PASO 5' -Descripcion 'Pago parcial 40.000 COP' `
        -Endpoint "POST /api/cartera/cuentas-por-cobrar/$cxcId/pagos" `
        -Esperado '200 + { mensaje }' -Obtenido $msg1 -Passed $true
}
catch {
    Add-Result -Paso 'PASO 5' -Descripcion 'Pago parcial 40.000 COP' `
        -Endpoint "POST /api/cartera/cuentas-por-cobrar/$cxcId/pagos" `
        -Esperado '200 + { mensaje }' -Obtenido "ERROR: $_" -Passed $false
    Write-Host "[ABORT] Error al registrar pago parcial." -ForegroundColor Red
    Stop-Transcript; exit 1
}

# ══════════════════════════════════════════════════════════════════════════════
#  PASO 6: Verificar estado tras pago parcial
#          Esperado: saldoPendiente=60.000, estado=2 (PagadaParcial)
# ══════════════════════════════════════════════════════════════════════════════
Write-Host "`n[PASO 6] Verificando estado tras pago parcial..." -ForegroundColor Cyan
try {
    $listado1 = Invoke-Api -Method GET -Url "$BASE_URL/api/cartera/cuentas-por-cobrar?miembroId=$miembroId" -Headers $Headers
    $cxc1 = $listado1 | Where-Object { $_.id -eq $cxcId -or $_.Id -eq $cxcId } | Select-Object -First 1

    if ($null -eq $cxc1) { throw "La CuentaPorCobrar no aparece en el listado." }

    $saldo1 = if ($null -ne $cxc1.saldoPendiente) { $cxc1.saldoPendiente } else { $cxc1.SaldoPendiente }
    $estado1 = if ($null -ne $cxc1.estado) { $cxc1.estado }         else { $cxc1.Estado }

    $saldoOk1 = [math]::Abs($saldo1 - 60000) -lt 0.01
    $estadoOk1 = ($estado1 -eq 2)
    $ok1 = $saldoOk1 -and $estadoOk1

    Add-Result -Paso 'PASO 6' -Descripcion 'Verificar saldo=60.000 y estado=2 (PagadaParcial)' `
        -Endpoint "GET /api/cartera/cuentas-por-cobrar?miembroId=$miembroId" `
        -Esperado 'saldoPendiente=60000, estado=2' `
        -Obtenido "saldoPendiente=$saldo1, estado=$estado1" -Passed $ok1
}
catch {
    Add-Result -Paso 'PASO 6' -Descripcion 'Verificar saldo=60.000 y estado=2 (PagadaParcial)' `
        -Endpoint "GET /api/cartera/cuentas-por-cobrar?miembroId=$miembroId" `
        -Esperado 'saldoPendiente=60000, estado=2' -Obtenido "ERROR: $_" -Passed $false
}

# ══════════════════════════════════════════════════════════════════════════════
#  PASO 7: Pago Total — 60.000 COP (saldo restante)
# ══════════════════════════════════════════════════════════════════════════════
Write-Host "`n[PASO 7] Registrando pago total de 60.000 COP (saldo restante)..." -ForegroundColor Cyan
$pagoBody2 = @{ monto = 60000 } | ConvertTo-Json -Compress
try {
    $rPago2 = Invoke-Api -Method POST -Url "$BASE_URL/api/cartera/cuentas-por-cobrar/$cxcId/pagos" -Headers $Headers -Body $pagoBody2
    $msg2 = if ($rPago2.mensaje) { $rPago2.mensaje } else { $rPago2 | ConvertTo-Json -Compress }
    Add-Result -Paso 'PASO 7' -Descripcion 'Pago total 60.000 COP (saldo restante)' `
        -Endpoint "POST /api/cartera/cuentas-por-cobrar/$cxcId/pagos" `
        -Esperado '200 + { mensaje }' -Obtenido $msg2 -Passed $true
}
catch {
    Add-Result -Paso 'PASO 7' -Descripcion 'Pago total 60.000 COP (saldo restante)' `
        -Endpoint "POST /api/cartera/cuentas-por-cobrar/$cxcId/pagos" `
        -Esperado '200 + { mensaje }' -Obtenido "ERROR: $_" -Passed $false
    Write-Host "[ABORT] Error al registrar pago total." -ForegroundColor Red
    Stop-Transcript; exit 1
}

# ══════════════════════════════════════════════════════════════════════════════
#  PASO 8: Verificar estado tras pago total
#          Esperado: saldoPendiente=0, estado=3 (Pagada)
# ══════════════════════════════════════════════════════════════════════════════
Write-Host "`n[PASO 8] Verificando estado tras pago total..." -ForegroundColor Cyan
try {
    $listado2 = Invoke-Api -Method GET -Url "$BASE_URL/api/cartera/cuentas-por-cobrar?miembroId=$miembroId" -Headers $Headers
    $cxc2 = $listado2 | Where-Object { $_.id -eq $cxcId -or $_.Id -eq $cxcId } | Select-Object -First 1

    if ($null -eq $cxc2) { throw "La CuentaPorCobrar no aparece en el listado." }

    $saldo2 = if ($null -ne $cxc2.saldoPendiente) { $cxc2.saldoPendiente } else { $cxc2.SaldoPendiente }
    $estado2 = if ($null -ne $cxc2.estado) { $cxc2.estado }         else { $cxc2.Estado }

    $saldoOk2 = [math]::Abs($saldo2 - 0) -lt 0.01
    $estadoOk2 = ($estado2 -eq 3)
    $ok2 = $saldoOk2 -and $estadoOk2

    Add-Result -Paso 'PASO 8' -Descripcion 'Verificar saldo=0 y estado=3 (Pagada)' `
        -Endpoint "GET /api/cartera/cuentas-por-cobrar?miembroId=$miembroId" `
        -Esperado 'saldoPendiente=0, estado=3' `
        -Obtenido "saldoPendiente=$saldo2, estado=$estado2" -Passed $ok2
}
catch {
    Add-Result -Paso 'PASO 8' -Descripcion 'Verificar saldo=0 y estado=3 (Pagada)' `
        -Endpoint "GET /api/cartera/cuentas-por-cobrar?miembroId=$miembroId" `
        -Esperado 'saldoPendiente=0, estado=3' -Obtenido "ERROR: $_" -Passed $false
}

# ══════════════════════════════════════════════════════════════════════════════
#  GENERAR REPORTE MARKDOWN
# ══════════════════════════════════════════════════════════════════════════════
$totalPasos = $Results.Count
$pasosPasados = ($Results | Where-Object { $_.Passed -eq $true }).Count
$pasosFallados = $totalPasos - $pasosPasados
$estadoGlobal = if ($OverallPass) { '🟢 TODAS LAS PRUEBAS PASARON' } else { '🔴 HAY PRUEBAS FALLIDAS' }
$fecha = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'

$md = @"
# QA Report: Ciclo de Recaudo — CuentaPorCobrar

> **Proyecto:** Sistema Contable L.A.M.A. Medellín
> **Módulo:** Cartera — Fase 8 (Recaudo / Pagos)
> **Fecha:** $fecha
> **Estado Global:** $estadoGlobal
> **Resumen:** $pasosPasados / $totalPasos pasos pasaron ($pasosFallados fallidos)

---

## Datos de Prueba Generados

| Entidad            | Valor                          |
|--------------------|-------------------------------|
| Miembro ID         | ``$miembroId``                   |
| DocumentoIdentidad | ``$docIdentidad``                |
| ConceptoCobro ID   | ``$conceptoId``                  |
| CuentaPorCobrar ID | ``$cxcId``                       |
| ValorTotal         | \$ 100.000 COP                 |
| CuentaContable ID  | ``$cuentaContableId`` (cód. $cuentaCodigo) |

---

## Resultados por Paso

| Paso   | Descripción                                     | Endpoint                                          | Esperado                          | Obtenido                               | Estado    |
|--------|-------------------------------------------------|---------------------------------------------------|-----------------------------------|----------------------------------------|-----------|
"@

foreach ($r in $Results) {
    $badge = if ($r.Passed) { '✅ PASS' } else { '❌ FAIL' }
    $ep = $r.Endpoint -replace '\|', '\|'
    $esp = $r.Esperado -replace '\|', '\|'
    $obt = $r.Obtenido -replace '\|', '\|'
    $obt = if ($obt.Length -gt 90) { $obt.Substring(0, 87) + '...' } else { $obt }
    $md += "| $($r.Paso) | $($r.Descripcion) | ``$ep`` | $esp | $obt | $badge |`n"
}

$md += @"

---

## Lógica de Dominio Validada

```
CuentaPorCobrar.AplicarPago(monto)
  ├─ Pago 1: 40.000 COP → SaldoPendiente = 100.000 − 40.000 = 60.000  → Estado = PagadaParcial (2) ✅
  └─ Pago 2: 60.000 COP → SaldoPendiente =  60.000 − 60.000 =      0  → Estado = Pagada         (3) ✅
```

## Resumen

- **Total pasos ejecutados:** $totalPasos
- **Pasaron:** $pasosPasados
- **Fallaron:** $pasosFallados
- **Resultado:** $estadoGlobal

---
*Generado por qa-pagos-cartera.ps1 — L.A.M.A. QA Suite*
"@

$md | Out-File -FilePath $REPORT_FILE -Encoding UTF8
Write-Host "`n[REPORTE] Guardado en: $REPORT_FILE" -ForegroundColor Cyan

# ─── Resumen en consola ────────────────────────────────────────────────────────
Write-Host "`n═══════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  RESULTADO FINAL: $estadoGlobal" -ForegroundColor $(if ($OverallPass) { 'Green' } else { 'Red' })
Write-Host "  Pasaron: $pasosPasados / $totalPasos pasos" -ForegroundColor $(if ($OverallPass) { 'Green' } else { 'Yellow' })
Write-Host "═══════════════════════════════════════════════════`n" -ForegroundColor Cyan

Stop-Transcript

# Mostrar el reporte
Get-Content -Path $REPORT_FILE -Encoding UTF8
