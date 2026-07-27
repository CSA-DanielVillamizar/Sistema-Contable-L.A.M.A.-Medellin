param(
    [string]$BaseUrl = 'http://localhost:7099',
    [string]$ApiProject = '.\src\LAMAMedellin.API\LAMAMedellin.API.csproj',
    [string]$TokenFile = '.\.tmp-prod-api-token.txt',
    [string]$ReportPath = '.\qa-conciliacion-report.md'
)

$ErrorActionPreference = 'Stop'

function Write-Step {
    param([string]$Message)
    Write-Host "[QA-CONCILIACION] $Message" -ForegroundColor Cyan
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
        Write-Step 'Usando token desde QA_BEARER_TOKEN.'
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
            Write-Step 'Azure CLI no devolvio token; se intenta archivo local.'
        }
    }

    if (Test-Path -Path $TokenFilePath) {
        Write-Step "Usando token desde archivo: $TokenFilePath"
        return (Get-Content -Path $TokenFilePath -Raw).Trim()
    }

    throw 'No se pudo obtener token. Configure QA_BEARER_TOKEN o inicie sesion en az CLI.'
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

function To-Decimal {
    param([object]$Value)
    if ($null -eq $Value) { return [decimal]0 }
    return [decimal]::Parse([string]$Value, [Globalization.CultureInfo]::InvariantCulture)
}

function Get-NaturalezaCode {
    param([object]$Value)

    if ($null -eq $Value) { return 0 }

    if ($Value -is [int] -or $Value -is [long] -or $Value -is [decimal]) {
        return [int]$Value
    }

    $text = [string]$Value
    $normalized = $text.Trim().ToUpperInvariant()

    if ($normalized -eq 'DEBITO' -or $normalized -eq 'DÉBITO') { return 1 }
    if ($normalized -eq 'CREDITO' -or $normalized -eq 'CRÉDITO') { return 2 }

    $parsed = 0
    if ([int]::TryParse($text, [ref]$parsed)) {
        return $parsed
    }

    return 0
}

$rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $rootPath

$report = New-Object System.Collections.Generic.List[string]
$report.Add('# QA Conciliacion Automatica')
$report.Add('')
$report.Add("- Fecha: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
$report.Add("- Base URL: $BaseUrl")
$report.Add('')

$apiOutLog = Join-Path $rootPath 'qa-conciliacion-api.out.log'
$apiErrLog = Join-Path $rootPath 'qa-conciliacion-api.err.log'
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

    Write-Step 'Resolviendo token...'
    $audience = 'api://b81ee2ee-5417-4aa0-8000-e470aec5543e'
    $token = Get-AuthToken -Audience $audience -TokenFilePath (Join-Path $rootPath $TokenFile)

    $jwtPayload = Convert-FromJwtPayload -Jwt $token
    if ($null -ne $jwtPayload -and $jwtPayload.exp) {
        $expDate = [DateTimeOffset]::FromUnixTimeSeconds([int64]$jwtPayload.exp).LocalDateTime
        $report.Add("- Token expira: $($expDate.ToString('yyyy-MM-dd HH:mm:ss'))")
    }

    Write-Step 'Obteniendo miembro para crear cuenta por cobrar...'
    $miembrosResult = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/cartera/miembros/lookup" -Token $token
    if (-not $miembrosResult.Success -or $miembrosResult.StatusCode -ne 200) {
        throw "No fue posible consultar miembros lookup. Status=$($miembrosResult.StatusCode)."
    }

    $miembro = @($miembrosResult.Body) | Select-Object -First 1
    if ($null -eq $miembro) {
        throw 'No hay miembros disponibles para crear cuenta por cobrar.'
    }

    $miembroId = [string](Get-PropValue -Object $miembro -Primary 'Id' -Fallback 'id')
    $miembroNombre = [string](Get-PropValue -Object $miembro -Primary 'NombreCompleto' -Fallback 'nombreCompleto')

    Write-Step 'Obteniendo concepto de cobro...'
    $conceptosResult = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/cartera/conceptos-cobro/lookup" -Token $token
    if (-not $conceptosResult.Success -or $conceptosResult.StatusCode -ne 200) {
        throw "No fue posible consultar conceptos lookup. Status=$($conceptosResult.StatusCode)."
    }

    $concepto = @($conceptosResult.Body) | Select-Object -First 1
    if ($null -eq $concepto) {
        Write-Step 'No hay conceptos. Se creara uno automaticamente...'

        $cuentasResult = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/cuentas-contables" -Token $token
        if (-not $cuentasResult.Success -or $cuentasResult.StatusCode -ne 200) {
            throw 'No se pudo consultar cuentas contables para crear concepto.'
        }

        $cuentas = @($cuentasResult.Body)
        $cuentaIngreso = $cuentas | Where-Object {
            (Get-PropValue -Object $_ -Primary 'PermiteMovimiento' -Fallback 'permiteMovimiento') -eq $true -and
            ((Get-NaturalezaCode (Get-PropValue -Object $_ -Primary 'Naturaleza' -Fallback 'naturaleza')) -eq 2)
        } | Select-Object -First 1

        if ($null -eq $cuentaIngreso) {
            throw 'No hay cuenta contable de ingreso para crear concepto de cobro.'
        }

        $cuentaIngresoId = [string](Get-PropValue -Object $cuentaIngreso -Primary 'Id' -Fallback 'id')

        $nuevoConceptoPayload = @{
            Nombre                  = "QA Conciliacion $(Get-Date -Format 'yyyyMMddHHmmss')"
            ValorCOP                = 100000
            PeriodicidadMensual     = 1
            CuentaContableIngresoId = $cuentaIngresoId
        }

        $createConceptResult = Invoke-ApiJson -Method POST -Uri "$BaseUrl/api/cartera/conceptos-cobro" -Token $token -Body $nuevoConceptoPayload
        if (-not $createConceptResult.Success -or ($createConceptResult.StatusCode -ne 201 -and $createConceptResult.StatusCode -ne 200)) {
            throw "No se pudo crear concepto de cobro QA. Status=$($createConceptResult.StatusCode)."
        }

        $nuevoConceptoId = [string](Get-PropValue -Object $createConceptResult.Body -Primary 'id' -Fallback 'Id')
        if ([string]::IsNullOrWhiteSpace($nuevoConceptoId)) {
            throw 'No se obtuvo id del concepto creado para QA.'
        }

        $concepto = [PSCustomObject]@{ Id = $nuevoConceptoId; Nombre = $nuevoConceptoPayload.Nombre }
    }

    $conceptoId = [string](Get-PropValue -Object $concepto -Primary 'Id' -Fallback 'id')
    $conceptoNombre = [string](Get-PropValue -Object $concepto -Primary 'Nombre' -Fallback 'nombre')

    Write-Step 'Creando cuenta por cobrar por 100000 COP...'
    $fechaEmision = (Get-Date).ToString('yyyy-MM-dd')
    $fechaVencimiento = (Get-Date).AddDays(30).ToString('yyyy-MM-dd')

    $crearCuentaPayload = @{
        MiembroId        = $miembroId
        ConceptoCobroId  = $conceptoId
        FechaEmision     = $fechaEmision
        FechaVencimiento = $fechaVencimiento
        ValorTotal       = 100000
    }

    $crearCuentaResult = Invoke-ApiJson -Method POST -Uri "$BaseUrl/api/cartera/cuentas-por-cobrar" -Token $token -Body $crearCuentaPayload
    if (-not $crearCuentaResult.Success -or ($crearCuentaResult.StatusCode -ne 201 -and $crearCuentaResult.StatusCode -ne 200)) {
        throw "No se pudo crear cuenta por cobrar. Status=$($crearCuentaResult.StatusCode)."
    }

    $cuentaId = [string](Get-PropValue -Object $crearCuentaResult.Body -Primary 'id' -Fallback 'Id')
    if ([string]::IsNullOrWhiteSpace($cuentaId)) {
        throw 'No se obtuvo id de la cuenta por cobrar creada.'
    }

    Write-Step 'Consultando caja y saldo inicial...'
    $cajasResult = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/tesoreria/cajas" -Token $token
    if (-not $cajasResult.Success -or $cajasResult.StatusCode -ne 200) {
        throw "No fue posible consultar cajas. Status=$($cajasResult.StatusCode)."
    }

    $caja = @($cajasResult.Body) | Select-Object -First 1
    if ($null -eq $caja) {
        throw 'No hay cajas disponibles para conciliacion.'
    }

    $cajaId = [string](Get-PropValue -Object $caja -Primary 'Id' -Fallback 'id')
    $saldoInicialCaja = To-Decimal (Get-PropValue -Object $caja -Primary 'SaldoActual' -Fallback 'saldoActual')

    Write-Step 'Ejecutando pago de cartera por 40000 COP con caja destino...'
    $montoPago = [decimal]40000
    $pagoResult = Invoke-ApiJson -Method POST -Uri "$BaseUrl/api/cartera/cuentas-por-cobrar/$cuentaId/pagos" -Token $token -Body @{
        Monto  = $montoPago
        CajaId = $cajaId
    }

    if (-not $pagoResult.Success -or ($pagoResult.StatusCode -ne 200 -and $pagoResult.StatusCode -ne 201)) {
        throw "No se pudo registrar pago de cartera. Status=$($pagoResult.StatusCode)."
    }

    Write-Step 'Reconsultando saldo de caja...'
    $cajasAfterResult = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/tesoreria/cajas" -Token $token
    if (-not $cajasAfterResult.Success -or $cajasAfterResult.StatusCode -ne 200) {
        throw "No fue posible reconsultar cajas. Status=$($cajasAfterResult.StatusCode)."
    }

    $cajaAfter = @($cajasAfterResult.Body) | Where-Object {
        [string](Get-PropValue -Object $_ -Primary 'Id' -Fallback 'id') -eq $cajaId
    } | Select-Object -First 1

    if ($null -eq $cajaAfter) {
        throw 'No se encontro la caja luego del pago.'
    }

    $saldoFinalCaja = To-Decimal (Get-PropValue -Object $cajaAfter -Primary 'SaldoActual' -Fallback 'saldoActual')
    $saldoEsperadoCaja = $saldoInicialCaja + $montoPago
    $okCaja = ($saldoFinalCaja -eq $saldoEsperadoCaja)

    Write-Step 'Consultando cuenta por cobrar para validar saldo pendiente...'
    $cuentasPorCobrarResult = Invoke-ApiJson -Method GET -Uri "$BaseUrl/api/cartera/cuentas-por-cobrar" -Token $token
    if (-not $cuentasPorCobrarResult.Success -or $cuentasPorCobrarResult.StatusCode -ne 200) {
        throw "No fue posible consultar cuentas por cobrar. Status=$($cuentasPorCobrarResult.StatusCode)."
    }

    $cuentaAfter = @($cuentasPorCobrarResult.Body) | Where-Object {
        [string](Get-PropValue -Object $_ -Primary 'Id' -Fallback 'id') -eq $cuentaId
    } | Select-Object -First 1

    if ($null -eq $cuentaAfter) {
        throw 'No se encontro la cuenta por cobrar creada en el listado.'
    }

    $saldoPendiente = To-Decimal (Get-PropValue -Object $cuentaAfter -Primary 'SaldoPendiente' -Fallback 'saldoPendiente')
    $saldoPendienteEsperado = [decimal]60000
    $okCartera = ($saldoPendiente -eq $saldoPendienteEsperado)

    $report.Add('## Setup')
    $report.Add("- Miembro: $miembroNombre ($miembroId)")
    $report.Add("- Concepto: $conceptoNombre ($conceptoId)")
    $report.Add("- Cuenta por cobrar creada: $cuentaId")
    $report.Add("- Caja usada: $cajaId")
    $report.Add("- Saldo inicial caja: $saldoInicialCaja")
    $report.Add('')

    $report.Add('## Validacion Tesoreria')
    $report.Add("- Saldo esperado caja: $saldoEsperadoCaja")
    $report.Add("- Saldo obtenido caja: $saldoFinalCaja")
    $report.Add("- Resultado Tesoreria: $(if ($okCaja) { 'PASS' } else { 'FAIL' })")
    $report.Add('')

    $report.Add('## Validacion Cartera')
    $report.Add("- Saldo pendiente esperado: $saldoPendienteEsperado")
    $report.Add("- Saldo pendiente obtenido: $saldoPendiente")
    $report.Add("- Resultado Cartera: $(if ($okCartera) { 'PASS' } else { 'FAIL' })")
    $report.Add('')

    if (-not $okCaja) {
        throw 'Conciliacion fallo: Tesoreria no incremento la caja en 40,000 COP.'
    }

    if (-not $okCartera) {
        throw 'Conciliacion fallo: Cartera no dejo saldo pendiente en 60,000 COP.'
    }

    $qaPassed = $true
    $report.Add('## Resultado Final')
    $report.Add('- PASS: Cartera y Tesoreria cruzaron informacion correctamente (conciliacion automatica OK).')
}
catch {
    $report.Add('## Resultado Final')
    $report.Add("- FAIL: $($_.Exception.Message)")
    Write-Host "[QA-CONCILIACION][ERROR] $($_.Exception.Message)" -ForegroundColor Red
}
finally {
    if ($apiProcess -and -not $apiProcess.HasExited) {
        Write-Step 'Deteniendo API...'
        Stop-Process -Id $apiProcess.Id -Force
    }

    $reportContent = $report -join "`r`n"
    Set-Content -Path $ReportPath -Value $reportContent -Encoding UTF8

    Write-Host ''
    Write-Host '========== REPORTE QA CONCILIACION ==========' -ForegroundColor Yellow
    Write-Host $reportContent
    Write-Host '==============================================' -ForegroundColor Yellow
}

if (-not $qaPassed) {
    exit 1
}

exit 0
