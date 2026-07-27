$ErrorActionPreference = 'Stop'

$server = 'lamaregionnorte-sql-a90e.database.windows.net'
$db = 'LAMAMedellinContable'
$workspace = 'c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin'
$migracionScript = Join-Path $workspace 'run-migracion-prod.ps1'

function Invoke-ProdSqlJson {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Query
    )

    $sqlToken = az account get-access-token --resource https://database.windows.net/ --query accessToken -o tsv
    if ([string]::IsNullOrWhiteSpace($sqlToken)) {
        throw 'No se pudo obtener token para Azure SQL.'
    }

    $rows = Invoke-Sqlcmd -ServerInstance $server -Database $db -AccessToken $sqlToken -Query $Query
    return $rows
}

# 1) Rollback masivo (soft delete)
$rollbackQuery = @"
SET NOCOUNT ON;

BEGIN TRAN;

DECLARE @AsientosActualizados INT = 0;
DECLARE @ComprobantesActualizados INT = 0;

UPDATE a
SET a.IsDeleted = 1
FROM AsientosContables a
WHERE a.IsDeleted = 0
  AND a.Referencia = 'Migración CSV';

SET @AsientosActualizados = @@ROWCOUNT;

UPDATE c
SET c.IsDeleted = 1
FROM Comprobantes c
WHERE c.IsDeleted = 0
  AND EXISTS (
      SELECT 1
      FROM AsientosContables a
      WHERE a.ComprobanteId = c.Id
        AND a.Referencia = 'Migración CSV'
  );

SET @ComprobantesActualizados = @@ROWCOUNT;

COMMIT;

SELECT @AsientosActualizados AS AsientosSoftDeleted,
       @ComprobantesActualizados AS ComprobantesSoftDeleted;
"@

$rollbackResult = Invoke-ProdSqlJson -Query $rollbackQuery | Select-Object AsientosSoftDeleted, ComprobantesSoftDeleted

# 2) Carga limpia unica
Set-Location $workspace
$migracionOutput = & $migracionScript
if ($LASTEXITCODE -ne 0) {
    throw "La ejecución de run-migracion-prod.ps1 falló con exit code $LASTEXITCODE"
}

$jsonLine = $migracionOutput | Where-Object { $_ -match '^\s*\{' } | Select-Object -Last 1
if ([string]::IsNullOrWhiteSpace($jsonLine)) {
    throw 'No se encontró JSON de respuesta en la salida de run-migracion-prod.ps1.'
}

$migracionJson = $jsonLine | ConvertFrom-Json

# 3) Asentar definitivo solo nuevos comprobantes de migración
$asentarQuery = @"
SET NOCOUNT ON;

UPDATE c
SET c.EstadoComprobante = 2
FROM Comprobantes c
WHERE c.IsDeleted = 0
  AND c.EstadoComprobante = 1
  AND EXISTS (
      SELECT 1
      FROM AsientosContables a
      WHERE a.ComprobanteId = c.Id
        AND a.IsDeleted = 0
        AND a.Referencia = 'Migración CSV'
  );

SELECT @@ROWCOUNT AS ComprobantesAsentados;
"@

$asentarResult = Invoke-ProdSqlJson -Query $asentarQuery | Select-Object ComprobantesAsentados

# 4) Validación contable
$validacionQuery = @"
SET NOCOUNT ON;

WITH Caja AS (
    SELECT Id
    FROM CuentasContables
    WHERE Codigo = '110505' AND IsDeleted = 0
),
MigracionActiva AS (
    SELECT c.Id, c.Descripcion
    FROM Comprobantes c
    WHERE c.IsDeleted = 0
      AND EXISTS (
          SELECT 1
          FROM AsientosContables a
          WHERE a.ComprobanteId = c.Id
            AND a.IsDeleted = 0
            AND a.Referencia = 'Migración CSV'
      )
)
SELECT
    SUM(CASE WHEN a.CuentaContableId IN (SELECT Id FROM Caja) THEN a.Debe ELSE 0 END) AS SumaDebeCajaMigracion,
    SUM(CASE WHEN a.CuentaContableId IN (SELECT Id FROM Caja) THEN a.Haber ELSE 0 END) AS SumaHaberCajaMigracion,
    MAX(CASE WHEN m.Descripcion LIKE 'SALDO EFECTIVO%' AND a.CuentaContableId IN (SELECT Id FROM Caja) THEN a.Debe ELSE 0 END) AS SaldoInicialDebeCaja,
    COUNT(DISTINCT m.Id) AS ComprobantesMigracionActivos
FROM AsientosContables a
INNER JOIN MigracionActiva m ON m.Id = a.ComprobanteId
WHERE a.IsDeleted = 0;
"@

$validacionResult = Invoke-ProdSqlJson -Query $validacionQuery |
    Select-Object SumaDebeCajaMigracion, SumaHaberCajaMigracion, SaldoInicialDebeCaja, ComprobantesMigracionActivos

$reporte = [ordered]@{
    rollback = [ordered]@{
        asientosSoftDeleted = [int]$rollbackResult.AsientosSoftDeleted
        comprobantesSoftDeleted = [int]$rollbackResult.ComprobantesSoftDeleted
    }
    recarga = [ordered]@{
        comprobantesCreados = [int]$migracionJson.comprobantesCreados
        advertencias = $migracionJson.advertencias
    }
    asentado = [ordered]@{
        comprobantesAsentados = [int]$asentarResult.ComprobantesAsentados
    }
    validacion = [ordered]@{
        sumaDebeCajaMigracion = [decimal]$validacionResult.SumaDebeCajaMigracion
        sumaHaberCajaMigracion = [decimal]$validacionResult.SumaHaberCajaMigracion
        saldoInicialDebeCaja = [decimal]$validacionResult.SaldoInicialDebeCaja
        comprobantesMigracionActivos = [int]$validacionResult.ComprobantesMigracionActivos
    }
}

$reporte | ConvertTo-Json -Depth 10
