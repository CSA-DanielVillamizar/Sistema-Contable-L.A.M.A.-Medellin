/*
================================================================================
 Registro de la migracion Baseline en produccion
================================================================================

 CONTEXTO
 --------
 El historial de migraciones de EF Core estaba roto: 22 de 23 migraciones no
 llevaban el atributo [Migration] (les faltaba su archivo .Designer.cs), asi que
 EF no las reconocia. El esquema productivo nunca se construyo con
 'dotnet ef database update', sino con los scripts SQL de este directorio.

 Las 23 migraciones se colapsaron en una unica migracion base generada desde el
 modelo actual: 20260727233255_Baseline. Se verifico en local que produce un
 esquema identico al del modelo (0 diferencias en columnas, tipos, nullability,
 indices, claves foraneas, defaults y check constraints).

 QUE HACE ESTE SCRIPT
 --------------------
 Marca esa migracion como YA APLICADA en produccion. NO ejecuta DDL: no crea ni
 altera una sola tabla. Su unico efecto es que a partir de ahora
 Database.Migrate() reconozca el esquema existente como la baseline y solo
 aplique las migraciones futuras.

 ANTES DE EJECUTARLO
 -------------------
 1. Sacar respaldo de la base de produccion.
 2. Confirmar que no hay drift entre el esquema de produccion y el del modelo.
    Ejecutar scripts/sql/inventario-esquema.sql contra produccion y contra una
    base local recien creada, y comparar las salidas. Deben ser identicas.
    Si hay diferencias, resolverlas ANTES: registrar la baseline sobre un
    esquema divergente deja el drift oculto y las migraciones futuras fallaran.
 3. Ejecutar este script en una ventana sin despliegues en curso.

 El script aborta solo si detecta que falta alguna tabla esperada.
================================================================================
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @MigrationId  NVARCHAR(150) = N'20260727233255_Baseline';
DECLARE @ProductVersion NVARCHAR(32) = N'8.0.11';

-- ---------------------------------------------------------------------------
-- 1. Estado actual
-- ---------------------------------------------------------------------------
PRINT '--- Base de datos: ' + DB_NAME();
PRINT '--- Tablas de usuario: ' + CAST((
    SELECT COUNT(*) FROM sys.tables WHERE name <> '__EFMigrationsHistory'
) AS VARCHAR);

IF OBJECT_ID(N'__EFMigrationsHistory', N'U') IS NULL
BEGIN
    PRINT '--- __EFMigrationsHistory: no existe (se creara)';
END
ELSE
BEGIN
    PRINT '--- __EFMigrationsHistory: existe, filas actuales:';
    SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
END

-- ---------------------------------------------------------------------------
-- 2. Verificacion: deben existir las 23 tablas del modelo
-- ---------------------------------------------------------------------------
DECLARE @Esperadas TABLE (Nombre SYSNAME PRIMARY KEY);

INSERT INTO @Esperadas (Nombre) VALUES
    ('AsientosContables'), ('AsistenciasEvento'), ('Bancos'), ('Beneficiarios'),
    ('Cajas'), ('CentrosCosto'), ('Comprobantes'), ('ConceptosCobro'),
    ('CuentasContables'), ('CuentasPorCobrar'), ('CuotasAsamblea'), ('Donaciones'),
    ('Donantes'), ('Egresos'), ('Eventos'), ('Ingresos'), ('Miembros'),
    ('MovimientosInventario'), ('Productos'), ('ProyectosSociales'),
    ('TarifasCuota'), ('Transacciones'), ('Usuarios');

DECLARE @Faltantes INT = (
    SELECT COUNT(*)
    FROM @Esperadas e
    WHERE NOT EXISTS (SELECT 1 FROM sys.tables t WHERE t.name = e.Nombre)
);

IF @Faltantes > 0
BEGIN
    PRINT '';
    PRINT '*** ABORTADO: faltan tablas del modelo en esta base. ***';

    SELECT e.Nombre AS TablaFaltante
    FROM @Esperadas e
    WHERE NOT EXISTS (SELECT 1 FROM sys.tables t WHERE t.name = e.Nombre)
    ORDER BY e.Nombre;

    THROW 50001, 'El esquema no corresponde a la baseline. No se registro nada.', 1;
END

PRINT '--- Verificacion de tablas: OK (23 de 23)';

-- ---------------------------------------------------------------------------
-- 3. Registro idempotente de la baseline
-- ---------------------------------------------------------------------------
BEGIN TRANSACTION;

IF OBJECT_ID(N'__EFMigrationsHistory', N'U') IS NULL
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL,
        ProductVersion NVARCHAR(32)  NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );

    PRINT '--- __EFMigrationsHistory creada';
END

IF EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = @MigrationId)
BEGIN
    PRINT '--- La baseline ya estaba registrada. Sin cambios.';
END
ELSE
BEGIN
    -- Las migraciones viejas nunca llegaron a registrarse (les faltaba el
    -- atributo [Migration]); si quedo alguna fila huerfana, se retira para que
    -- el historial refleje unicamente la baseline.
    DELETE FROM __EFMigrationsHistory WHERE MigrationId <> @MigrationId;

    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES (@MigrationId, @ProductVersion);

    PRINT '--- Baseline registrada: ' + @MigrationId;
END

COMMIT TRANSACTION;

-- ---------------------------------------------------------------------------
-- 4. Estado final
-- ---------------------------------------------------------------------------
PRINT '';
PRINT '--- Historial resultante:';
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
