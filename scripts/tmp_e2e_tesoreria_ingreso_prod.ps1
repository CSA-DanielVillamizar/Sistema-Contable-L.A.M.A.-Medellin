$ErrorActionPreference = 'Stop'

Set-Location "c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin"
$apiBase = 'https://app-lamamedellin-backend-prod.azurewebsites.net'
$scope = 'api://b81ee2ee-5417-4aa0-8000-e470aec5543e/.default'
$sqlServer = 'lamaregionnorte-sql-a90e.database.windows.net'
$sqlDb = 'LAMAMedellinContable'

$apiToken = az account get-access-token --scope $scope --query accessToken -o tsv
if ([string]::IsNullOrWhiteSpace($apiToken)) { throw 'No se pudo obtener token API' }
$headers = @{ Authorization = "Bearer $apiToken"; 'Content-Type' = 'application/json' }

$cajasAntes = Invoke-RestMethod -Method GET -Uri "$apiBase/api/tesoreria/cajas" -Headers $headers
$caja = $cajasAntes | Where-Object { $_.nombre -eq 'Caja General L.A.M.A.' } | Select-Object -First 1
if (-not $caja) { throw 'No se encontró Caja General L.A.M.A.' }
$saldoAntes = [decimal]$caja.saldoActual
$cajaId = $caja.id

$sqlToken = az account get-access-token --resource https://database.windows.net/ --query accessToken -o tsv
if ([string]::IsNullOrWhiteSpace($sqlToken)) { throw 'No se pudo obtener token SQL' }

$cuentaIngreso = Invoke-Sqlcmd -ServerInstance $sqlServer -Database $sqlDb -AccessToken $sqlToken -Query @"
SELECT TOP 1
    Id,
    Codigo,
    Descripcion,
    Naturaleza,
    PermiteMovimiento
FROM CuentasContables
WHERE Codigo LIKE '4%'
  AND PermiteMovimiento = 1
  AND IsDeleted = 0
ORDER BY Codigo;
"@
$cuentaIngresoRow = $cuentaIngreso | Select-Object -First 1
if (-not $cuentaIngresoRow) { throw 'No se encontró cuenta contable de ingresos.' }
$cuentaIngresoId = [Guid]$cuentaIngresoRow.Id

$centroCosto = Invoke-Sqlcmd -ServerInstance $sqlServer -Database $sqlDb -AccessToken $sqlToken -Query @"
SELECT TOP 1 Id, Nombre
FROM CentrosCosto
WHERE IsDeleted = 0
ORDER BY Nombre;
"@
$centroCostoRow = $centroCosto | Select-Object -First 1
if (-not $centroCostoRow) { throw 'No se encontró centro de costo.' }
$centroCostoId = [Guid]$centroCostoRow.Id

$payloadObj = @{
    fecha = (Get-Date).ToString('o')
    monto = 50000
    concepto = 'Donacion voluntaria en evento'
    terceroId = $null
    cuentaContableId = $cuentaIngresoId
    cajaId = [Guid]$cajaId
    centroCostoId = $centroCostoId
}
$payloadJson = $payloadObj | ConvertTo-Json

$ingresoResp = Invoke-RestMethod -Method POST -Uri "$apiBase/api/tesoreria/ingresos" -Headers $headers -Body $payloadJson
$comprobanteId = if ($ingresoResp.id) { $ingresoResp.id } elseif ($ingresoResp.Id) { $ingresoResp.Id } else { $null }
if (-not $comprobanteId) { throw 'No se obtuvo id de comprobante del POST ingreso.' }

$cajasDespues = Invoke-RestMethod -Method GET -Uri "$apiBase/api/tesoreria/cajas" -Headers $headers
$cajaDespues = $cajasDespues | Where-Object { $_.id -eq $cajaId } | Select-Object -First 1
$saldoDespues = [decimal]$cajaDespues.saldoActual
$delta = $saldoDespues - $saldoAntes

$comprobanteSql = Invoke-Sqlcmd -ServerInstance $sqlServer -Database $sqlDb -AccessToken $sqlToken -Query @"
SELECT TOP 1
    c.Id,
    c.NumeroComprobante,
    c.Fecha,
    c.TipoComprobante,
    c.Descripcion,
    c.Estado
FROM Comprobantes c
WHERE c.Id = '$comprobanteId';
"@

$asientosSql = Invoke-Sqlcmd -ServerInstance $sqlServer -Database $sqlDb -AccessToken $sqlToken -Query @"
SELECT
    a.Id,
    a.ComprobanteId,
    a.CuentaContableId,
    cc.Codigo,
    cc.Nombre AS CuentaNombre,
    a.Debito,
    a.Credito,
    a.Descripcion
FROM AsientosContables a
INNER JOIN CuentasContables cc ON cc.Id = a.CuentaContableId
WHERE a.ComprobanteId = '$comprobanteId'
ORDER BY a.Debito DESC, a.Credito DESC;
"@

'=== RESULTADO E2E INGRESOS PROD ==='
[PSCustomObject]@{
    ApiBase = $apiBase
    CajaId = $cajaId
    CajaNombre = $caja.nombre
    SaldoAntes = $saldoAntes
    SaldoDespues = $saldoDespues
    Delta = $delta
    EsperadoAntes = 850000
    EsperadoDespues = 900000
    CuentaIngresoId = $cuentaIngresoId
    CuentaIngresoCodigo = $cuentaIngresoRow.Codigo
    CuentaIngresoNombre = $cuentaIngresoRow.Descripcion
    CentroCostoId = $centroCostoId
    ComprobanteId = $comprobanteId
} | Format-List | Out-String | Write-Output

'=== COMPROBANTE SQL ==='
$comprobanteSql | Format-Table -AutoSize | Out-String | Write-Output

'=== ASIENTOS SQL ==='
$asientosSql | Format-Table -AutoSize | Out-String | Write-Output
