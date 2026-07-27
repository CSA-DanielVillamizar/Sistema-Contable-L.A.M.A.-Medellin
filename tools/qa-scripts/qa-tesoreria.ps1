param(
    [string]$BaseUrl = 'http://localhost:7099',
    [string]$ApiProject = '.\src\LAMAMedellin.API\LAMAMedellin.API.csproj',
    [string]$TokenFile = '.\.tmp-prod-api-token.txt',
    [string]$ReportPath = '.\qa-tesoreria-report.md'
)

$ErrorActionPreference = 'Stop'

function Write-Step {
    param([string]$Message)
    Write-Host "[QA] $Message" -ForegroundColor Cyan
}

function Convert-FromJwtPayload {
    param([string]$Jwt)

    if ([string]::IsNullOrWhiteSpace($Jwt) -or -not $Jwt.Contains('.')) {
        return $null
    }

    $parts = $Jwt.Split('.')
    if ($parts.Length -lt 2) {
        return $null
    }

    $payload = $parts[1].Replace('-', '+').Replace('_', '/')
    switch ($payload.Length % 4) {
        2 { $payload += '==' }
        3 { $payload += '=' }
    }

    try {
        $bytes = [Convert]::FromBase64String($payload)
        $json = [Text.Encoding]::UTF8.GetString($bytes)
        return $json | ConvertFrom-Json
    }
    catch {
        return $null
    }
}

function Get-AuthToken {
    param(
        [string]$Audience,
        [string]$TokenFilePath
    )

    if (-not [string]::IsNullOrWhiteSpace($env:QA_BEARER_TOKEN)) {
        Write-Step 'Usando token desde variable de entorno QA_BEARER_TOKEN.'
        return $env:QA_BEARER_TOKEN.Trim()
    }

    $azCmd = Get-Command az -ErrorAction SilentlyContinue
    if ($null -ne $azCmd) {
        try {
            Write-Step 'Intentando obtener token con Azure CLI...'
            $tokenFromAz = (& az account get-access-token --resource $Audience --query accessToken -o tsv).Trim()
            if (-not [string]::IsNullOrWhiteSpace($tokenFromAz)) {
                Write-Step 'Token obtenido con Azure CLI.'
                return $tokenFromAz
            }
        }
        catch {
            Write-Step 'Azure CLI no devolvio token para esta API; se intenta token local.'
        }
    }

    if (Test-Path -Path $TokenFilePath) {
        Write-Step "Usando token desde archivo: $TokenFilePath"
        return (Get-Content -Path $TokenFilePath -Raw).Trim()
    }

    throw 'No se pudo obtener token. Defina QA_BEARER_TOKEN o autentiquese con Azure CLI.'
}

function Invoke-ApiJson {
    param(
        [ValidateSet('GET', 'POST')]
        [string]$Method,
        [string]$Uri,
        [string]$Token,
        [object]$Body = $null
    )

    $headers = @{ Authorization = "Bearer $Token" }

    try {
        if ($null -ne $Body) {
            $jsonBody = $Body | ConvertTo-Json -Depth 8
            $response = Invoke-WebRequest -Method $Method -Uri $Uri -Headers $headers -ContentType 'application/json' -Body $jsonBody -UseBasicParsing
        }
        else {
            $response = Invoke-WebRequest -Method $Method -Uri $Uri -Headers $headers -UseBasicParsing
        }

        $parsed = $null
        if (-not [string]::IsNullOrWhiteSpace($response.Content)) {
            try { $parsed = $response.Content | ConvertFrom-Json } catch { }
        }

        return [PSCustomObject]@{
            Success    = $true
            StatusCode = [int]$response.StatusCode
            Body       = $parsed
            Raw        = $response.Content
            Error      = $null
        }
    }
    catch {
        $statusCode = 0
        $raw = ''

        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode
            try {
                $stream = $_.Exception.Response.GetResponseStream()
                if ($stream) {
                    $reader = New-Object System.IO.StreamReader($stream)
                    $raw = $reader.ReadToEnd()
                    $reader.Dispose()
                }
            }
            catch { }
        }

        $parsed = $null
        if (-not [string]::IsNullOrWhiteSpace($raw)) {
            try { $parsed = $raw | ConvertFrom-Json } catch { }
        }

        return [PSCustomObject]@{
            Success    = $false
            StatusCode = $statusCode
            Body       = $parsed
            Raw        = $raw
            Error      = $_.Exception.Message
        }
    }
}

function To-Decimal {
    param([object]$Value)
    if ($null -eq $Value) { return [decimal]0 }
    return [decimal]::Parse([string]$Value, [Globalization.CultureInfo]::InvariantCulture)
}

function Get-PropValue {
    param(
        [object]$Object,
        [string]$Primary,
        [string]$Fallback
    )

    if ($null -eq $Object) {
        return $null
    }

    $primaryProp = $Object.PSObject.Properties[$Primary]
    if ($null -ne $primaryProp -and $null -ne $primaryProp.Value) {
        return $primaryProp.Value
    }

    $fallbackProp = $Object.PSObject.Properties[$Fallback]
    if ($null -ne $fallbackProp) {
        return $fallbackProp.Value
    }

    return $null
}

function Get-NaturalezaCode {
    param([object]$Value)

    if ($null -eq $Value) {
        return 0
    }

    if ($Value -is [int] -or $Value -is [long] -or $Value -is [decimal]) {
        return [int]$Value
    }

    $text = [string]$Value
    $normalized = $text.Trim().ToUpperInvariant()

    if ($normalized -eq 'DEBITO' -or $normalized -eq 'DÉBITO') {
        return 1
    }

    if ($normalized -eq 'CREDITO' -or $normalized -eq 'CRÉDITO') {
        return 2
    }

    $parsed = 0
    if ([int]::TryParse($text, [ref]$parsed)) {
        return $parsed
    }

    return 0
}

$rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $rootPath

$report = New-Object System.Collections.Generic.List[string]
$report.Add('# QA Tesoreria')
$report.Add('')
$report.Add("- Fecha: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
$report.Add("- Base URL: $BaseUrl")
$report.Add('')

$apiOutLog = Join-Path $rootPath 'qa-tesoreria-api.out.log'
$apiErrLog = Join-Path $rootPath 'qa-tesoreria-api.err.log'
if (Test-Path $apiOutLog) { Remove-Item $apiOutLog -Force }
if (Test-Path $apiErrLog) { Remove-Item $apiErrLog -Force }

$apiProcess = $null
$qaPassed = $false

try {
    Write-Step 'Iniciando API en puerto 7099...'
    $apiProcess = Start-Process -FilePath 'dotnet' -ArgumentList @('run', '--project', $ApiProject, '--urls', $BaseUrl) -PassThru -WindowStyle Hidden -RedirectStandardOutput $apiOutLog -RedirectStandardError $apiErrLog

    $ready = $false
    for ($i = 0; $i -lt 60; $i++) {
        Start-Sleep -Seconds 1
        try {
            $ping = Invoke-WebRequest -Method GET -Uri "$BaseUrl/" -UseBasicParsing
            if ([int]$ping.StatusCode -eq 200) {
                $ready = $true
                break
            }
        }
        catch { }
    }

    if (-not $ready) {
        throw 'La API no respondio a tiempo en el puerto 7099.'
    }

    Write-Step 'API disponible. Resolviendo token...'
    $audience = 'api://b81ee2ee-5417-4aa0-8000-e470aec5543e'
    $token = Get-AuthToken -Audience $audience -TokenFilePath (Join-Path $rootPath $TokenFile)
    $jwtPayload = Convert-FromJwtPayload -Jwt $token
    if ($null -ne $jwtPayload -and $jwtPayload.exp) {
        $expDate = [DateTimeOffset]::FromUnixTimeSeconds([int64]$jwtPayload.exp).LocalDateTime
        $report.Add("- Token expira: $($expDate.ToString('yyyy-MM-dd HH:mm:ss'))")
    }

    Write-Step 'Setup: consultando cajas...'
    $cajasResult = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/tesoreria/cajas" -Token $token
    if (-not $cajasResult.Success -or $cajasResult.StatusCode -ne 200) {
        throw "No fue posible consultar cajas. Status=$($cajasResult.StatusCode). Error=$($cajasResult.Error)"
    }

    $cajas = @($cajasResult.Body)
    if ($cajas.Count -eq 0) {
        throw 'No hay cajas disponibles para ejecutar QA.'
    }

    $caja = $cajas[0]
    $cajaId = [string](Get-PropValue -Object $caja -Primary 'Id' -Fallback 'id')
    $saldoInicial = To-Decimal (Get-PropValue -Object $caja -Primary 'SaldoActual' -Fallback 'saldoActual')

    Write-Step 'Setup: consultando centros de costo...'
    $centrosResult = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/transacciones/centros-costo" -Token $token
    if (-not $centrosResult.Success -or $centrosResult.StatusCode -ne 200) {
        throw "No fue posible consultar centros de costo. Status=$($centrosResult.StatusCode)."
    }

    $centroCosto = @($centrosResult.Body) | Select-Object -First 1
    if ($null -eq $centroCosto) {
        throw 'No hay centros de costo disponibles para QA.'
    }
    $centroCostoId = [string](Get-PropValue -Object $centroCosto -Primary 'Id' -Fallback 'id')

    Write-Step 'Setup: consultando cuentas contables...'
    $cuentasResult = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/cuentas-contables" -Token $token
    if (-not $cuentasResult.Success -or $cuentasResult.StatusCode -ne 200) {
        throw "No fue posible consultar cuentas contables. Status=$($cuentasResult.StatusCode)."
    }

    $cuentas = @($cuentasResult.Body)
    $cuentaIngreso = $cuentas | Where-Object {
        (Get-PropValue -Object $_ -Primary 'PermiteMovimiento' -Fallback 'permiteMovimiento') -eq $true -and
        ((Get-NaturalezaCode (Get-PropValue -Object $_ -Primary 'Naturaleza' -Fallback 'naturaleza')) -eq 2)
    } | Select-Object -First 1
    $cuentaEgreso = $cuentas | Where-Object {
        (Get-PropValue -Object $_ -Primary 'PermiteMovimiento' -Fallback 'permiteMovimiento') -eq $true -and
        ((Get-NaturalezaCode (Get-PropValue -Object $_ -Primary 'Naturaleza' -Fallback 'naturaleza')) -eq 1)
    } | Select-Object -First 1

    if ($null -eq $cuentaIngreso) {
        $cuentaIngreso = $cuentas | Where-Object {
            (Get-PropValue -Object $_ -Primary 'PermiteMovimiento' -Fallback 'permiteMovimiento') -eq $true
        } | Select-Object -First 1
    }
    if ($null -eq $cuentaEgreso) {
        $cuentaEgreso = $cuentas | Where-Object {
            (Get-PropValue -Object $_ -Primary 'PermiteMovimiento' -Fallback 'permiteMovimiento') -eq $true
        } | Select-Object -First 1
    }

    if ($null -eq $cuentaIngreso -or $null -eq $cuentaEgreso) {
        throw 'No hay cuentas contables de movimiento disponibles para QA.'
    }

    $cuentaIngresoId = [string](Get-PropValue -Object $cuentaIngreso -Primary 'Id' -Fallback 'id')
    $cuentaEgresoId = [string](Get-PropValue -Object $cuentaEgreso -Primary 'Id' -Fallback 'id')

    $report.Add('## Setup')
    $report.Add("
- Caja seleccionada: $cajaId")
    $report.Add("- Saldo inicial: $saldoInicial")
    $report.Add("- CentroCostoId: $centroCostoId")
    $report.Add("- CuentaIngresoId: $cuentaIngresoId")
    $report.Add("- CuentaEgresoId: $cuentaEgresoId")
    $report.Add('')

    Write-Step 'Prueba 1: Ingreso +50000'
    $ingresoMonto = [decimal]50000
    $ingresoPayload = @{
        Monto            = $ingresoMonto
        Concepto         = 'QA Tesoreria - Ingreso 50000'
        TerceroId        = $null
        CuentaContableId = $cuentaIngresoId
        CajaId           = $cajaId
        CentroCostoId    = $centroCostoId
    }

    $ingresoResult = Invoke-ApiJson -Method POST -Uri "$BaseUrl/api/tesoreria/ingresos" -Token $token -Body $ingresoPayload
    if (-not $ingresoResult.Success -or ($ingresoResult.StatusCode -ne 201 -and $ingresoResult.StatusCode -ne 200)) {
        throw "Prueba 1 fallo al registrar ingreso. Status=$($ingresoResult.StatusCode)."
    }

    $cajasAfterIngreso = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/tesoreria/cajas" -Token $token
    if (-not $cajasAfterIngreso.Success -or $cajasAfterIngreso.StatusCode -ne 200) {
        throw 'Prueba 1 fallo al reconsultar cajas.'
    }

    $cajaAfterIngreso = @($cajasAfterIngreso.Body) | Where-Object {
        [string](Get-PropValue -Object $_ -Primary 'Id' -Fallback 'id') -eq $cajaId
    } | Select-Object -First 1
    $saldoAfterIngreso = To-Decimal (Get-PropValue -Object $cajaAfterIngreso -Primary 'SaldoActual' -Fallback 'saldoActual')
    $esperadoAfterIngreso = $saldoInicial + $ingresoMonto
    $okIngreso = ($saldoAfterIngreso -eq $esperadoAfterIngreso)

    $report.Add('## Prueba 1 - Suma (Ingreso 50,000 COP)')
    $report.Add("- Status POST: $($ingresoResult.StatusCode)")
    $report.Add("- Saldo esperado: $esperadoAfterIngreso")
    $report.Add("- Saldo obtenido: $saldoAfterIngreso")
    $report.Add("- Resultado: $(if ($okIngreso) { 'PASS' } else { 'FAIL' })")
    $report.Add('')

    if (-not $okIngreso) {
        throw 'Prueba 1 fallo: el saldo no incremento exactamente 50,000.'
    }

    Write-Step 'Prueba 2: Egreso -20000'
    $egresoMonto = [decimal]20000
    $egresoPayload = @{
        Monto            = $egresoMonto
        Concepto         = 'QA Tesoreria - Egreso 20000'
        TerceroId        = $null
        CuentaContableId = $cuentaEgresoId
        CajaId           = $cajaId
        CentroCostoId    = $centroCostoId
    }

    $egresoResult = Invoke-ApiJson -Method POST -Uri "$BaseUrl/api/tesoreria/egresos" -Token $token -Body $egresoPayload
    if (-not $egresoResult.Success -or ($egresoResult.StatusCode -ne 201 -and $egresoResult.StatusCode -ne 200)) {
        throw "Prueba 2 fallo al registrar egreso. Status=$($egresoResult.StatusCode)."
    }

    $cajasAfterEgreso = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/tesoreria/cajas" -Token $token
    if (-not $cajasAfterEgreso.Success -or $cajasAfterEgreso.StatusCode -ne 200) {
        throw 'Prueba 2 fallo al reconsultar cajas.'
    }

    $cajaAfterEgreso = @($cajasAfterEgreso.Body) | Where-Object {
        [string](Get-PropValue -Object $_ -Primary 'Id' -Fallback 'id') -eq $cajaId
    } | Select-Object -First 1
    $saldoAfterEgreso = To-Decimal (Get-PropValue -Object $cajaAfterEgreso -Primary 'SaldoActual' -Fallback 'saldoActual')
    $esperadoAfterEgreso = $saldoAfterIngreso - $egresoMonto
    $okEgreso = ($saldoAfterEgreso -eq $esperadoAfterEgreso)

    $report.Add('## Prueba 2 - Resta (Egreso 20,000 COP)')
    $report.Add("- Status POST: $($egresoResult.StatusCode)")
    $report.Add("- Saldo esperado: $esperadoAfterEgreso")
    $report.Add("- Saldo obtenido: $saldoAfterEgreso")
    $report.Add("- Resultado: $(if ($okEgreso) { 'PASS' } else { 'FAIL' })")
    $report.Add('')

    if (-not $okEgreso) {
        throw 'Prueba 2 fallo: el saldo no disminuyo exactamente 20,000.'
    }

    Write-Step 'Prueba 3: Egreso con fondos insuficientes'
    $montoAbsurdamenteAlto = [decimal]999999999
    $egresoInsuficientePayload = @{
        Monto            = $montoAbsurdamenteAlto
        Concepto         = 'QA Tesoreria - Egreso insuficiente'
        TerceroId        = $null
        CuentaContableId = $cuentaEgresoId
        CajaId           = $cajaId
        CentroCostoId    = $centroCostoId
    }

    $egresoInsuficienteResult = Invoke-ApiJson -Method POST -Uri "$BaseUrl/api/tesoreria/egresos" -Token $token -Body $egresoInsuficientePayload
    $statusRechazo = $egresoInsuficienteResult.StatusCode
    $okInsuficiente = (-not $egresoInsuficienteResult.Success) -and ($statusRechazo -eq 400 -or $statusRechazo -eq 500)

    $report.Add('## Prueba 3 - Fondos Insuficientes')
    $report.Add("- Status POST: $statusRechazo")
    $report.Add("- Resultado: $(if ($okInsuficiente) { 'PASS' } else { 'FAIL' })")
    if ($egresoInsuficienteResult.Raw) {
        $detail = $egresoInsuficienteResult.Raw.Replace("`r", ' ').Replace("`n", ' ')
        $report.Add("- Respuesta: $detail")
    }
    $report.Add('')

    if (-not $okInsuficiente) {
        throw 'Prueba 3 fallo: el backend no rechazo el egreso por fondos insuficientes con status esperado.'
    }

    $qaPassed = $true
    $report.Add('## Resultado Final')
    $report.Add('- PASS: Las tres pruebas de Tesoreria pasaron correctamente.')
}
catch {
    $report.Add('## Resultado Final')
    $report.Add("- FAIL: $($_.Exception.Message)")
    Write-Host "[QA][ERROR] $($_.Exception.Message)" -ForegroundColor Red
}
finally {
    if ($apiProcess -and -not $apiProcess.HasExited) {
        Write-Step 'Deteniendo API...'
        Stop-Process -Id $apiProcess.Id -Force
    }

    $reportContent = $report -join "`r`n"
    Set-Content -Path $ReportPath -Value $reportContent -Encoding UTF8

    Write-Host ''
    Write-Host '========== REPORTE QA TESORERIA ==========' -ForegroundColor Yellow
    Write-Host $reportContent
    Write-Host '==========================================' -ForegroundColor Yellow
}

if (-not $qaPassed) {
    exit 1
}

exit 0
