param([int]$Port = 7099)

$root = "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin"
$proj = "$root\LAMAMedellin\src\LAMAMedellin.API\LAMAMedellin.API.csproj"
$workDir = "$root\LAMAMedellin"
$log = "$root\qa-transcript-final.log"
$mdReport = "$root\qa-report-cartera-$(Get-Date -Format 'yyyyMMdd-HHmmss').md"

Start-Transcript -Path $log -Force

Write-Host "====== QA HAPPY PATH - INICIO: $(Get-Date) ======" -ForegroundColor Cyan

# ── 1. Limpiar estado previo ─────────────────────────────────────────────────
Write-Host "`n[1] Limpiando procesos dotnet y puertos 5006/7015/7099..." -ForegroundColor Yellow
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 3
$freed5006 = -not (Get-NetTCPConnection -LocalPort 5006 -State Listen -ErrorAction SilentlyContinue)
$freed7015 = -not (Get-NetTCPConnection -LocalPort 7015 -State Listen -ErrorAction SilentlyContinue)
$freed7099 = -not (Get-NetTCPConnection -LocalPort 7099 -State Listen -ErrorAction SilentlyContinue)
Write-Host "    5006 libre: $freed5006   7015 libre: $freed7015   7099 libre: $freed7099" -ForegroundColor $(if ($freed5006 -and $freed7015 -and $freed7099) { 'Green' } else { 'Red' })

# ── 2. Iniciar API sin launch profile en puerto 7099 ─────────────────────────
Write-Host "`n[2] Iniciando API con --no-launch-profile en https://localhost:$Port ..." -ForegroundColor Yellow
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"
$apiProcess = Start-Process -FilePath "dotnet" `
    -ArgumentList "run --no-launch-profile --urls https://localhost:$Port --project src/LAMAMedellin.API/LAMAMedellin.API.csproj" `
    -WorkingDirectory $workDir `
    -PassThru

Write-Host "    API PID: $($apiProcess.Id)" -ForegroundColor Green

# ── 3. Poll activo (max 180s) ────────────────────────────────────────────────
Write-Host "`n[3] Esperando que puerto $Port responda..." -ForegroundColor Yellow
$ready = $false
for ($i = 1; $i -le 180; $i++) {
    Start-Sleep -Seconds 1
    $nc = Test-NetConnection -ComputerName localhost -Port $Port -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    if ($nc.TcpTestSucceeded) {
        Write-Host ""
        Write-Host "    Puerto $Port ACTIVO en ${i}s" -ForegroundColor Green
        $ready = $true
        break
    }
    Write-Host -NoNewline "."
}

if (-not $ready) {
    Write-Host ""
    Write-Host "    FALLA: puerto $Port no respondio en 180s" -ForegroundColor Red
    Write-Host "    Verificando si la API crasheo (proceso existe: $(-not $apiProcess.HasExited))" -ForegroundColor Yellow
}

# ── 4. Token Azure AD ─────────────────────────────────────────────────────────
Write-Host "`n[4] Obteniendo token Azure AD..." -ForegroundColor Yellow
$token = az account get-access-token --resource "api://b81ee2ee-5417-4aa0-8000-e470aec5543e" --query "accessToken" -o tsv
if (-not $token -or $token.Length -lt 10) {
    Write-Host "    ERROR: sin token. Abortando." -ForegroundColor Red
    Stop-Transcript; exit 1
}
Write-Host "    Token OK ($($token.Length) chars)" -ForegroundColor Green

$base = "https://localhost:${Port}/api/cartera"
$hdrs = @{ Authorization = "Bearer $token"; "Content-Type" = "application/json" }

# Bypass SSL para localhost dev cert en PS 5.1
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

$results = @()

function Invoke-Step {
    param(
        [int]$Paso,
        [string]$Endpoint,
        [string]$PayloadJson
    )

    $url = "$base/$Endpoint"
    $status = $null
    $responseBody = ""
    $errorText = $null
    $responseObj = $null

    try {
        $resp = Invoke-WebRequest -Uri $url -Method Post -Headers $hdrs -Body $PayloadJson -ErrorAction Stop
        $status = [int]$resp.StatusCode
        $responseBody = $resp.Content
        if ($responseBody) {
            try { $responseObj = $responseBody | ConvertFrom-Json } catch {}
        }
    }
    catch {
        if ($_.Exception.Response) {
            $status = [int]$_.Exception.Response.StatusCode
            try {
                $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
                $responseBody = $reader.ReadToEnd()
                $reader.Close()
            }
            catch {
                $responseBody = ""
            }
        }
        $errorText = $_.Exception.Message
    }

    return [pscustomobject]@{
        Paso         = $Paso
        Endpoint     = "POST /api/cartera/$Endpoint"
        Url          = $url
        Payload      = $PayloadJson
        StatusCode   = $status
        ResponseBody = $responseBody
        Error        = $errorText
        ResponseObj  = $responseObj
    }
}

# ── PASO 1: Crear Miembro ────────────────────────────────────────────────────
Write-Host "`n====== PASO 1: POST $base/miembros ======" -ForegroundColor Cyan
$p1 = @{
    documentoIdentidad = "CC-QA-TEST-001"
    nombres            = "Daniel QA"
    apellidos          = "Villamizar Test"
    apodo              = "DQA"
    fechaIngreso       = "2026-03-29"
    tipoMiembro        = 2
} | ConvertTo-Json
Write-Host "Payload:`n$p1"

try {
    $step1 = Invoke-Step -Paso 1 -Endpoint "miembros" -PayloadJson $p1
    $miembroId = if ($step1.ResponseObj) { $step1.ResponseObj.id } else { $null }
    if (-not $miembroId -and $step1.ResponseObj) { $miembroId = $step1.ResponseObj.Id }
    Write-Host "HTTP STATUS : $($step1.StatusCode)" -ForegroundColor $(if ($step1.StatusCode -eq 201) { 'Green' } else { 'Red' })
    Write-Host "RESPUESTA   : $($step1.ResponseBody)" -ForegroundColor $(if ($step1.StatusCode -eq 201) { 'Green' } else { 'Red' })
    if ($step1.Error) { Write-Host "ERROR       : $($step1.Error)" -ForegroundColor Red }
    $results += $step1
}
catch {}

# ── PASO 1B: Obtener una CuentaContable válida desde endpoint de test ───────
Write-Host "`n[1B] Obteniendo CuentaContable válida desde la API..." -ForegroundColor Cyan
try {
    $cuentaResp = Invoke-WebRequest "https://localhost:$Port/api/cartera/test/valid-cuenta-contable" -ErrorAction Stop
    $cuentaData = $cuentaResp.Content | ConvertFrom-Json
    $validCuentaId = $cuentaData.id
    Write-Host "    Ok: $validCuentaId ($($cuentaData.codigo))" -ForegroundColor Green
}
catch {
    Write-Host "    Advertencia: No se pudo obtener CuentaContable, usando placeholder" -ForegroundColor Yellow
    $validCuentaId = "550e8400-e29b-41d4-a716-446655440001"
}

# ── PASO 2: Crear Concepto de Cobro ─────────────────────────────────────────
Write-Host "`n====== PASO 2: POST $base/conceptos-cobro ======" -ForegroundColor Cyan
$p2 = @{
    nombre                  = "Cuota Mensual Test"
    valorCOP                = 150000
    periodicidadMensual     = 1
    cuentaContableIngresoId = $validCuentaId
} | ConvertTo-Json
Write-Host "Payload:`n$p2"
Write-Host "Nota: Requiere GUID válido de CuentasContables (ejecutar: SELECT TOP 1 Id FROM CuentasContables WHERE Codigo = '110505')" -ForegroundColor Yellow

try {
    $step2 = Invoke-Step -Paso 2 -Endpoint "conceptos-cobro" -PayloadJson $p2
    $conceptoId = if ($step2.ResponseObj) { $step2.ResponseObj.id } else { $null }
    if (-not $conceptoId -and $step2.ResponseObj) { $conceptoId = $step2.ResponseObj.Id }
    Write-Host "HTTP STATUS : $($step2.StatusCode)" -ForegroundColor $(if ($step2.StatusCode -eq 201) { 'Green' } else { 'Red' })
    Write-Host "RESPUESTA   : $($step2.ResponseBody)" -ForegroundColor $(if ($step2.StatusCode -eq 201) { 'Green' } else { 'Red' })
    if ($step2.Error) { Write-Host "ERROR       : $($step2.Error)" -ForegroundColor Red }
    $results += $step2
}
catch {}

# ── PASO 3: Crear Cuenta por Cobrar ─────────────────────────────────────────
Write-Host "`n====== PASO 3: POST $base/cuentas-por-cobrar ======" -ForegroundColor Cyan
if ($miembroId -and $conceptoId) {
    $p3 = @{
        miembroId        = $miembroId
        conceptoCobroId  = $conceptoId
        fechaEmision     = "2026-03-29"
        fechaVencimiento = "2026-04-29"
        valorTotal       = 150000
    } | ConvertTo-Json
    Write-Host "Payload:`n$p3"

    try {
        $step3 = Invoke-Step -Paso 3 -Endpoint "cuentas-por-cobrar" -PayloadJson $p3
        $cxcId = if ($step3.ResponseObj) { $step3.ResponseObj.id } else { $null }
        if (-not $cxcId -and $step3.ResponseObj) { $cxcId = $step3.ResponseObj.Id }
        Write-Host "HTTP STATUS : $($step3.StatusCode)" -ForegroundColor $(if ($step3.StatusCode -eq 201) { 'Green' } else { 'Red' })
        Write-Host "RESPUESTA   : $($step3.ResponseBody)" -ForegroundColor $(if ($step3.StatusCode -eq 201) { 'Green' } else { 'Red' })
        if ($step3.Error) { Write-Host "ERROR       : $($step3.Error)" -ForegroundColor Red }
        $results += $step3
    }
    catch {}
}
else {
    Write-Host "Omitido (sin IDs de pasos anteriores)" -ForegroundColor Yellow
    $results += [pscustomobject]@{
        Paso         = 3
        Endpoint     = "POST /api/cartera/cuentas-por-cobrar"
        Url          = "$base/cuentas-por-cobrar"
        Payload      = ""
        StatusCode   = $null
        ResponseBody = ""
        Error        = "Dependencias previas fallaron"
        ResponseObj  = $null
    }
}

# ── Reporte JSON ─────────────────────────────────────────────────────────────
$rf = "$root\test-report-cartera-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
$results | Select-Object Paso, Endpoint, Url, Payload, StatusCode, ResponseBody, Error | ConvertTo-Json | Out-File $rf -Encoding UTF8
Write-Host "`n====== REPORTE JSON ======" -ForegroundColor Cyan
Get-Content $rf -Raw

# ── Reporte Markdown ─────────────────────────────────────────────────────────
$md = @()
$md += "# QA Report - Happy Path Cartera"
$md += ""
$md += "- Fecha: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
$md += "- Base URL: $base"
$md += ""
foreach ($r in $results) {
    $payloadForMd = "{}"
    if ($r.Payload) { $payloadForMd = $r.Payload }

    $responseForMd = "{}"
    if ($r.ResponseBody) { $responseForMd = $r.ResponseBody }

    $md += "## Paso $($r.Paso) - $($r.Endpoint)"
    $md += ""
    $md += "- HTTP Status: $($r.StatusCode)"
    if ($r.Error) { $md += "- Error: $($r.Error)" }
    $md += ""
    $md += "### Payload"
    $md += '```json'
    $md += $payloadForMd
    $md += '```'
    $md += ""
    $md += "### Response"
    $md += '```json'
    $md += $responseForMd
    $md += '```'
    $md += ""
}
$okCount = @($results | Where-Object { $_.StatusCode -eq 201 }).Count
$md += "## Resumen"
$md += ""
$md += "- Exitosos (201): $okCount/3"
$md | Out-File -FilePath $mdReport -Encoding UTF8
Write-Host "`n====== REPORTE MARKDOWN ======" -ForegroundColor Cyan
Write-Host "$mdReport" -ForegroundColor Green

# ── Cleanup ──────────────────────────────────────────────────────────────────
Write-Host "`n[CLEANUP] Deteniendo API..." -ForegroundColor Yellow
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
if (-not $apiProcess.HasExited) { Stop-Process -Id $apiProcess.Id -Force -ErrorAction SilentlyContinue }
Write-Host "API detenida" -ForegroundColor Green

$exito = @($results | Where-Object { $_.StatusCode -eq 201 }).Count
Write-Host "`n====== RESULTADO FINAL: $exito/3 pasos exitosos ======" -ForegroundColor Cyan
Write-Host "====== QA HAPPY PATH - FIN: $(Get-Date) ======" -ForegroundColor Cyan

Stop-Transcript
