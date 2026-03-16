$ErrorActionPreference = 'Stop'

$apiBase = 'https://app-lamamedellin-backend-prod.azurewebsites.net'
$scope = 'api://b81ee2ee-5417-4aa0-8000-e470aec5543e/.default'
$sqlServer = 'lamaregionnorte-sql-a90e.database.windows.net'
$sqlDb = 'LAMAMedellinContable'
$monto = 150000
$concepto = 'Pago alquiler sede reunion L.A.M.A.'
$fechaHoy = (Get-Date).ToString('yyyy-MM-ddTHH:mm:ss')

function Exec-Sql([string]$Query) {
    Invoke-Sqlcmd -ServerInstance $sqlServer -Database $sqlDb -AccessToken $script:sqlToken -Query $Query
}

function Get-Prop($obj, [string]$name1, [string]$name2='') {
    if ($null -ne $obj.PSObject.Properties[$name1]) { return $obj.$name1 }
    if ($name2 -and $null -ne $obj.PSObject.Properties[$name2]) { return $obj.$name2 }
    return $null
}

$apiToken = az account get-access-token --scope $scope --query accessToken -o tsv
if ([string]::IsNullOrWhiteSpace($apiToken)) { throw 'No se obtuvo token API.' }
$sqlToken = az account get-access-token --resource https://database.windows.net/ --query accessToken -o tsv
if ([string]::IsNullOrWhiteSpace($sqlToken)) { throw 'No se obtuvo token SQL.' }
$headers = @{ Authorization = "Bearer $apiToken"; 'Content-Type' = 'application/json' }

$repairSql = @"
SET XACT_ABORT ON;
BEGIN TRAN;

IF OBJECT_ID('dbo.TarifasCuota','U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260310013011_AddTarifaCuotaPorTipoAfiliacion')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260310013011_AddTarifaCuotaPorTipoAfiliacion', '8.0.11');
END;

IF NOT EXISTS (SELECT 1 FROM CuentasContables WHERE Codigo = '1110')
BEGIN
    INSERT INTO CuentasContables (Id, Codigo, Descripcion, PermiteMovimiento, IsDeleted, CuentaPadreId, ExigeTercero, Naturaleza)
    SELECT NEWID(), '1110', 'Bancos', 0, 0, p.Id, 0, 1
    FROM CuentasContables p
    WHERE p.Codigo = '11';
END;

IF NOT EXISTS (SELECT 1 FROM CuentasContables WHERE Codigo = '111005')
BEGIN
    INSERT INTO CuentasContables (Id, Codigo, Descripcion, PermiteMovimiento, IsDeleted, CuentaPadreId, ExigeTercero, Naturaleza)
    SELECT NEWID(), '111005', 'Moneda Nacional', 1, 0, p.Id, 0, 1
    FROM CuentasContables p
    WHERE p.Codigo = '1110';
END;

IF OBJECT_ID('dbo.Cajas','U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Cajas](
        [Id] [uniqueidentifier] NOT NULL,
        [Nombre] [nvarchar](150) NOT NULL,
        [TipoCaja] [int] NOT NULL,
        [SaldoActual] [decimal](18,2) NOT NULL,
        [CuentaContableId] [uniqueidentifier] NOT NULL,
        [IsDeleted] [bit] NOT NULL,
        CONSTRAINT [PK_Cajas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Cajas_CuentasContables_CuentaContableId] FOREIGN KEY ([CuentaContableId]) REFERENCES [dbo].[CuentasContables]([Id])
    );
    CREATE UNIQUE INDEX [IX_Cajas_CuentaContableId] ON [dbo].[Cajas] ([CuentaContableId]);
END;

IF OBJECT_ID('dbo.Egresos','U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Egresos](
        [Id] [uniqueidentifier] NOT NULL,
        [Fecha] [datetime2] NOT NULL,
        [Monto] [decimal](18,2) NOT NULL,
        [Concepto] [nvarchar](500) NOT NULL,
        [TerceroId] [uniqueidentifier] NULL,
        [CuentaContableId] [uniqueidentifier] NOT NULL,
        [CajaId] [uniqueidentifier] NOT NULL,
        [CentroCostoId] [uniqueidentifier] NOT NULL,
        [ComprobanteContableId] [uniqueidentifier] NULL,
        [IsDeleted] [bit] NOT NULL,
        CONSTRAINT [PK_Egresos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Egresos_Cajas_CajaId] FOREIGN KEY ([CajaId]) REFERENCES [dbo].[Cajas]([Id]),
        CONSTRAINT [FK_Egresos_CentrosCosto_CentroCostoId] FOREIGN KEY ([CentroCostoId]) REFERENCES [dbo].[CentrosCosto]([Id]),
        CONSTRAINT [FK_Egresos_Comprobantes_ComprobanteContableId] FOREIGN KEY ([ComprobanteContableId]) REFERENCES [dbo].[Comprobantes]([Id]),
        CONSTRAINT [FK_Egresos_CuentasContables_CuentaContableId] FOREIGN KEY ([CuentaContableId]) REFERENCES [dbo].[CuentasContables]([Id])
    );
    CREATE INDEX [IX_Egresos_CajaId] ON [dbo].[Egresos] ([CajaId]);
    CREATE INDEX [IX_Egresos_CentroCostoId] ON [dbo].[Egresos] ([CentroCostoId]);
    CREATE INDEX [IX_Egresos_ComprobanteContableId] ON [dbo].[Egresos] ([ComprobanteContableId]);
    CREATE INDEX [IX_Egresos_CuentaContableId] ON [dbo].[Egresos] ([CuentaContableId]);
END;

IF NOT EXISTS (SELECT 1 FROM Cajas WHERE Nombre = 'Caja General L.A.M.A.')
BEGIN
    INSERT INTO Cajas (Id, Nombre, TipoCaja, SaldoActual, CuentaContableId, IsDeleted)
    SELECT NEWID(), 'Caja General L.A.M.A.', 1, 0, cc.Id, 0
    FROM CuentasContables cc
    WHERE cc.Codigo = '110505'
      AND NOT EXISTS (SELECT 1 FROM Cajas c WHERE c.CuentaContableId = cc.Id);
END;

IF NOT EXISTS (SELECT 1 FROM Cajas WHERE Nombre = 'Cuenta Bancolombia')
BEGIN
    INSERT INTO Cajas (Id, Nombre, TipoCaja, SaldoActual, CuentaContableId, IsDeleted)
    SELECT NEWID(), 'Cuenta Bancolombia', 2, 0, cc.Id, 0
    FROM CuentasContables cc
    WHERE cc.Codigo = '111005'
      AND NOT EXISTS (SELECT 1 FROM Cajas c WHERE c.CuentaContableId = cc.Id);
END;

IF OBJECT_ID('dbo.Cajas','U') IS NOT NULL
   AND OBJECT_ID('dbo.Egresos','U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260313035240_AddTesoreriaModule')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260313035240_AddTesoreriaModule', '8.0.11');
END;

COMMIT;
"@
Exec-Sql $repairSql | Out-Null

$estado = Exec-Sql @"
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory WHERE MigrationId IN ('20260310013011_AddTarifaCuotaPorTipoAfiliacion','20260313035240_AddTesoreriaModule') ORDER BY MigrationId;
SELECT Id, Nombre, TipoCaja, SaldoActual, CuentaContableId FROM Cajas ORDER BY Nombre;
"@
Write-Output '=== REPARACION TESORERIA PROD ==='
$estado | Format-Table -AutoSize | Out-String | Write-Output

$cajasAntes = Invoke-RestMethod -Method Get -Uri "$apiBase/api/tesoreria/cajas" -Headers $headers
if ($cajasAntes -isnot [System.Collections.IEnumerable]) { $cajasAntes = @($cajasAntes) }
$cajaGeneral = $cajasAntes | Where-Object { (Get-Prop $_ 'Nombre' 'nombre') -eq 'Caja General L.A.M.A.' } | Select-Object -First 1
if ($null -eq $cajaGeneral) { throw 'No se encontro la Caja General L.A.M.A. tras la reparacion.' }
$cajaId = Get-Prop $cajaGeneral 'Id' 'id'

$centroCosto = Exec-Sql "SELECT TOP 1 Id, Nombre, Tipo FROM CentrosCosto WHERE IsDeleted = 0 ORDER BY CASE WHEN Nombre LIKE '%Activo%' THEN 0 ELSE 1 END, Nombre;" | Select-Object -First 1
if ($null -eq $centroCosto) { throw 'No se encontro centro de costo disponible.' }
$centroCostoId = $centroCosto.Id

$cuentaGasto = Exec-Sql "SELECT TOP 1 Id, Codigo, Descripcion FROM CuentasContables WHERE Codigo IN ('519595','510505','511005') AND IsDeleted = 0 ORDER BY CASE Codigo WHEN '519595' THEN 0 WHEN '510505' THEN 1 ELSE 2 END;" | Select-Object -First 1
if ($null -eq $cuentaGasto) { throw 'No se encontro cuenta de gasto valida.' }
$cuentaGastoId = $cuentaGasto.Id

$payload = @{
    Fecha = $fechaHoy
    Monto = $monto
    Concepto = $concepto
    CuentaContableId = [string]$cuentaGastoId
    CajaId = [string]$cajaId
    CentroCostoId = [string]$centroCostoId
    TerceroId = $null
} | ConvertTo-Json -Depth 5

$egresoResp = Invoke-RestMethod -Method Post -Uri "$apiBase/api/tesoreria/egresos" -Headers $headers -Body $payload
$cajasDespues = Invoke-RestMethod -Method Get -Uri "$apiBase/api/tesoreria/cajas" -Headers $headers
if ($cajasDespues -isnot [System.Collections.IEnumerable]) { $cajasDespues = @($cajasDespues) }
$cajaGeneralDespues = $cajasDespues | Where-Object { (Get-Prop $_ 'Nombre' 'nombre') -eq 'Caja General L.A.M.A.' } | Select-Object -First 1
$saldoFinal = Get-Prop $cajaGeneralDespues 'SaldoActual' 'saldoActual'

$auditSql = @"
SELECT TOP 1 
    e.Id AS EgresoId,
    e.Fecha,
    e.Monto,
    e.Concepto,
    e.CajaId,
    e.CentroCostoId,
    e.CuentaContableId,
    e.ComprobanteContableId,
    c.Nombre AS Caja,
    cc.Codigo AS CuentaGastoCodigo,
    cc.Descripcion AS CuentaGastoDescripcion,
    comp.NumeroConsecutivo,
    comp.TipoComprobante,
    comp.EstadoComprobante,
    comp.Descripcion AS ComprobanteDescripcion
FROM Egresos e
INNER JOIN Cajas c ON c.Id = e.CajaId
INNER JOIN CuentasContables cc ON cc.Id = e.CuentaContableId
LEFT JOIN Comprobantes comp ON comp.Id = e.ComprobanteContableId
WHERE e.Concepto = '$concepto' AND e.Monto = $monto AND e.IsDeleted = 0
ORDER BY e.Fecha DESC;

SELECT 
    a.Id AS AsientoId,
    cuenta.Codigo AS CuentaCodigo,
    cuenta.Descripcion AS CuentaDescripcion,
    a.Debe,
    a.Haber,
    a.Referencia,
    a.CentroCostoId,
    a.TerceroId
FROM AsientosContables a
INNER JOIN CuentasContables cuenta ON cuenta.Id = a.CuentaContableId
WHERE a.ComprobanteId = (
    SELECT TOP 1 e.ComprobanteContableId
    FROM Egresos e
    WHERE e.Concepto = '$concepto' AND e.Monto = $monto AND e.IsDeleted = 0
    ORDER BY e.Fecha DESC
)
ORDER BY a.Debe DESC, a.Haber DESC, cuenta.Codigo;
"@
$audit = Exec-Sql $auditSql

Write-Output '=== PRUEBA E2E TESORERIA PROD ==='
Write-Output ('CajaId=' + $cajaId)
Write-Output ('CentroCostoId=' + $centroCostoId)
Write-Output ('CentroCostoNombre=' + $centroCosto.Nombre)
Write-Output ('CuentaGastoId=' + $cuentaGastoId)
Write-Output ('CuentaGasto=' + $cuentaGasto.Codigo + ' - ' + $cuentaGasto.Descripcion)
Write-Output ('PostResponse=' + ($egresoResp | ConvertTo-Json -Depth 10 -Compress))
Write-Output ('SaldoCajaGeneralDespues=' + $saldoFinal)
Write-Output '--- Cajas despues ---'
$cajasDespues | ConvertTo-Json -Depth 10 | Write-Output
Write-Output '--- Auditoria SQL ---'
$audit | Format-Table -AutoSize | Out-String | Write-Output
