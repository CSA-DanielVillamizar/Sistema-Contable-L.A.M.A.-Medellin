param(
    [string]$BaseUrl = 'http://localhost:7099',
    [string]$ApiProject = '.\\LAMAMedellin\\src\\LAMAMedellin.API\\LAMAMedellin.API.csproj',
    [string]$TokenFile = '.\\.tmp-prod-api-token.txt',
    [string]$Audience = 'api://b81ee2ee-5417-4aa0-8000-e470aec5543e',
    [int]$CantidadVenta = 1
)

$ErrorActionPreference = 'Stop'

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

function Get-AuthToken {
    param(
        [string]$Audience,
        [string]$TokenFilePath
    )

    if (-not [string]::IsNullOrWhiteSpace($env:QA_BEARER_TOKEN)) {
        return $env:QA_BEARER_TOKEN.Trim()
    }

    $azCmd = Get-Command az -ErrorAction SilentlyContinue
    if ($null -ne $azCmd) {
        try {
            $tokenFromAz = (& az account get-access-token --resource $Audience --query accessToken -o tsv).Trim()
            if (-not [string]::IsNullOrWhiteSpace($tokenFromAz)) {
                return $tokenFromAz
            }
        }
        catch {
        }
    }

    if (Test-Path -Path $TokenFilePath) {
        return (Get-Content -Path $TokenFilePath -Raw).Trim()
    }

    throw 'No se pudo obtener token. Defina QA_BEARER_TOKEN o autentiquese con Azure CLI.'
}

function To-Decimal {
    param([object]$Value)

    if ($null -eq $Value) {
        return [decimal]0
    }

    return [decimal]::Parse([string]$Value, [Globalization.CultureInfo]::InvariantCulture)
}

function Get-ProductoPrecio {
    param([object]$Producto)

    return To-Decimal (Get-PropValue -Object $Producto -Primary 'PrecioVenta' -Fallback 'precioVenta')
}

function Get-ProductoStock {
    param([object]$Producto)

    $stock = Get-PropValue -Object $Producto -Primary 'CantidadEnStock' -Fallback 'cantidadEnStock'
    if ($null -eq $stock) {
        $stock = Get-PropValue -Object $Producto -Primary 'CantidadStock' -Fallback 'cantidadStock'
    }

    if ($null -eq $stock) {
        return 0
    }

    return [int]$stock
}

function Wait-ApiReady {
    param([string]$Url)

    for ($i = 0; $i -lt 60; $i++) {
        try {
            $ping = Invoke-WebRequest -Method GET -Uri "$Url/" -UseBasicParsing
            if ([int]$ping.StatusCode -eq 200) {
                return $true
            }
        }
        catch {
        }

        Start-Sleep -Seconds 1
    }

    return $false
}

$rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $rootPath

$apiOutLog = Join-Path $rootPath 'qa-venta-api.out.log'
$apiErrLog = Join-Path $rootPath 'qa-venta-api.err.log'
if (Test-Path $apiOutLog) { Remove-Item $apiOutLog -Force }
if (Test-Path $apiErrLog) { Remove-Item $apiErrLog -Force }

$apiProcess = $null

try {
    Write-Host '[QA-VENTA] Iniciando API local...' -ForegroundColor Cyan
    $apiProcess = Start-Process -FilePath 'dotnet' -ArgumentList @('run', '--project', $ApiProject, '--urls', $BaseUrl) -PassThru -WindowStyle Hidden -RedirectStandardOutput $apiOutLog -RedirectStandardError $apiErrLog

    if (-not (Wait-ApiReady -Url $BaseUrl)) {
        throw 'La API no respondio a tiempo.'
    }

    Write-Host '[QA-VENTA] Resolviendo autenticacion...' -ForegroundColor Cyan
    $token = Get-AuthToken -Audience $Audience -TokenFilePath (Join-Path $rootPath $TokenFile)
    $headers = @{ Authorization = "Bearer $token"; Accept = 'application/json'; 'Content-Type' = 'application/json' }

    Write-Host '[QA-VENTA] Consultando cajas...' -ForegroundColor Cyan
    $cajasAntes = Invoke-RestMethod -Method GET -Uri "$BaseUrl/api/tesoreria/cajas" -Headers $headers
    $caja = @($cajasAntes) | Select-Object -First 1
    if ($null -eq $caja) {
        throw 'No hay cajas disponibles para ejecutar la prueba.'
    }

    $cajaId = [string](Get-PropValue -Object $caja -Primary 'Id' -Fallback 'id')
    $cajaNombre = [string](Get-PropValue -Object $caja -Primary 'Nombre' -Fallback 'nombre')
    $saldoAntes = To-Decimal (Get-PropValue -Object $caja -Primary 'SaldoActual' -Fallback 'saldoActual')

    Write-Host '[QA-VENTA] Consultando productos...' -ForegroundColor Cyan
    $productos = Invoke-RestMethod -Method GET -Uri "$BaseUrl/api/merchandising/productos" -Headers $headers
    $producto = @($productos) |
    Where-Object {
        (Get-ProductoPrecio -Producto $_) -gt 0
    } |
    Select-Object -First 1

    if ($null -eq $producto) {
        throw 'No hay un producto existente con precio de venta valido para la prueba.'
    }

    $productoId = [string](Get-PropValue -Object $producto -Primary 'Id' -Fallback 'id')
    $productoNombre = [string](Get-PropValue -Object $producto -Primary 'Nombre' -Fallback 'nombre')
    $precioVenta = Get-ProductoPrecio -Producto $producto
    $stockActual = Get-ProductoStock -Producto $producto

    if ($stockActual -lt $CantidadVenta) {
        $faltante = $CantidadVenta - $stockActual

        Write-Host '[QA-VENTA] Registrando entrada previa para habilitar stock de prueba...' -ForegroundColor Cyan
        $entradaPayload = @{
            Cantidad = $faltante
            Fecha    = (Get-Date).ToUniversalTime().ToString('o')
            Concepto = 'QA prep stock para venta'
        } | ConvertTo-Json

        Invoke-RestMethod -Method POST -Uri "$BaseUrl/api/merchandising/productos/$productoId/entradas" -Headers $headers -Body $entradaPayload | Out-Null
    }

    $montoEsperado = $precioVenta * [decimal]$CantidadVenta

    $ventaPayload = @{
        Cantidad = $CantidadVenta
        CajaId   = $cajaId
        Concepto = 'QA conciliacion automatica merchandising'
    } | ConvertTo-Json

    Write-Host '[QA-VENTA] Registrando venta...' -ForegroundColor Cyan
    $ventaResponse = Invoke-RestMethod -Method POST -Uri "$BaseUrl/api/merchandising/productos/$productoId/ventas" -Headers $headers -Body $ventaPayload
    $ventaId = [string](Get-PropValue -Object $ventaResponse -Primary 'id' -Fallback 'Id')

    Write-Host '[QA-VENTA] Verificando saldo de caja...' -ForegroundColor Cyan
    $cajasDespues = Invoke-RestMethod -Method GET -Uri "$BaseUrl/api/tesoreria/cajas" -Headers $headers
    $cajaDespues = @($cajasDespues) | Where-Object { [string](Get-PropValue -Object $_ -Primary 'Id' -Fallback 'id') -eq $cajaId } | Select-Object -First 1

    if ($null -eq $cajaDespues) {
        throw 'No fue posible encontrar la caja luego de la venta.'
    }

    $saldoDespues = To-Decimal (Get-PropValue -Object $cajaDespues -Primary 'SaldoActual' -Fallback 'saldoActual')
    $delta = $saldoDespues - $saldoAntes
    $qaOk = $delta -eq $montoEsperado

    Write-Output '=== REPORTE QA VENTA MERCHANDISING -> TESORERIA ==='
    Write-Output ("Caja: {0} ({1})" -f $cajaNombre, $cajaId)
    Write-Output ("Producto: {0} ({1})" -f $productoNombre, $productoId)
    Write-Output ("VentaId: {0}" -f $ventaId)
    Write-Output ("Precio unitario: {0}" -f $precioVenta)
    Write-Output ("Cantidad vendida: {0}" -f $CantidadVenta)
    Write-Output ("Monto esperado: {0}" -f $montoEsperado)
    Write-Output ("Saldo antes: {0}" -f $saldoAntes)
    Write-Output ("Saldo despues: {0}" -f $saldoDespues)
    Write-Output ("Delta saldo: {0}" -f $delta)
    Write-Output ("Resultado QA: {0}" -f $(if ($qaOk) { 'PASS' } else { 'FAIL' }))

    if (-not $qaOk) {
        throw "QA fallo: el saldo de caja no subio por el valor exacto de la venta (esperado: $montoEsperado, real: $delta)."
    }
}
finally {
    if ($null -ne $apiProcess -and -not $apiProcess.HasExited) {
        Stop-Process -Id $apiProcess.Id -Force
    }
}
