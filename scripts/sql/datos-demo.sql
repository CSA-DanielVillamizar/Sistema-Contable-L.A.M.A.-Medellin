/*
================================================================================
 Datos de demostracion
================================================================================

 Carga un juego de datos representativo para probar el sistema: tres meses de
 movimiento contable, cartera con estados variados, productos, donantes y
 donaciones.

 PARA QUE SIRVE
 --------------
 Una base recien creada solo trae catalogos (PUC, cajas, bancos, centros de
 costo, miembros). Con catalogos vacios de movimiento todos los tableros salen
 en cero y no hay forma de ver si los libros, el balance o el cierre funcionan.
 Este script llena ese vacio.

 QUE CARGA
 ---------
 - Tres meses de comprobantes asentados (mayo, junio y julio de 2026) con sus
   asientos cuadrados. Tres meses a proposito: asi el libro mayor tiene saldo
   anterior real y el balance de prueba tiene con que comparar.
 - Conceptos de cobro y cuentas por cobrar en los tres estados: pendiente,
   pagada parcial y pagada, para que el tablero de cartera y el reporte de mora
   muestren casos distintos.
 - Productos de merchandising, donantes y donaciones.

 NO carga periodos contables cerrados: se dejan abiertos para poder ejercitar
 el flujo de validar y cerrar desde la aplicacion.

 COMO EJECUTARLO
 ---------------
   docker exec -i lama-sqlserver-dev /opt/mssql-tools18/bin/sqlcmd \
     -S localhost -U sa -P 'LamaDev!2026' -C \
     -d LAMAMedellinContable -i /ruta/datos-demo.sql

 Es idempotente: se puede ejecutar varias veces sin duplicar nada. Todo lo que
 crea lleva el prefijo DEMO- para poder identificarlo y borrarlo despues con
 scripts/sql/datos-demo-limpiar.sql.

 ADVERTENCIA: es para entornos de desarrollo y demostracion. No ejecutarlo
 contra produccion.
================================================================================
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

-- ---------------------------------------------------------------------------
-- Referencias a los catalogos ya sembrados
-- ---------------------------------------------------------------------------
DECLARE @ccCapitulo   UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM CentrosCosto WHERE Tipo = 1);
DECLARE @ccFundacion  UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM CentrosCosto WHERE Tipo = 2);
DECLARE @ccEventos    UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM CentrosCosto WHERE Tipo = 4);

DECLARE @ctaCaja      UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '110505');
DECLARE @ctaBanco     UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '111005');
DECLARE @ctaCuotas    UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '410510');
DECLARE @ctaEventos   UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '411005');
DECLARE @ctaMerch     UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '411010');
DECLARE @ctaDonaLibre UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '411505');
DECLARE @ctaHonor     UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '511005');
DECLARE @ctaTranspo   UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '513015');
DECLARE @ctaDeporte   UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '519520');
DECLARE @ctaOtros     UNIQUEIDENTIFIER = (SELECT Id FROM CuentasContables WHERE Codigo = '519595');

DECLARE @banco        UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Bancos);

IF @ccCapitulo IS NULL OR @ctaCaja IS NULL OR @banco IS NULL
BEGIN
    ROLLBACK TRANSACTION;
    THROW 50010, 'Faltan catalogos base. Arranque la API una vez para que siembre PUC, cajas, bancos y centros de costo.', 1;
END

-- ---------------------------------------------------------------------------
-- 1. Comprobantes contables con sus asientos
--
-- Se registran por pares debe/haber para que cada comprobante cuadre. El
-- procedimiento inserta solo si el consecutivo no existe, de ahi la
-- idempotencia.
-- ---------------------------------------------------------------------------
DECLARE @movimientos TABLE (
    Consecutivo   NVARCHAR(50),
    Fecha         DATETIME2,
    Tipo          INT,
    Descripcion   NVARCHAR(500),
    CuentaDebe    UNIQUEIDENTIFIER,
    CuentaHaber   UNIQUEIDENTIFIER,
    Monto         DECIMAL(18,2),
    CentroCosto   UNIQUEIDENTIFIER
);

INSERT INTO @movimientos VALUES
    -- Mayo 2026
    ('DEMO-ING-00000001', '2026-05-05', 1, 'Recaudo cuotas de sostenimiento mayo', @ctaBanco, @ctaCuotas,    2400000, @ccCapitulo),
    ('DEMO-ING-00000002', '2026-05-12', 1, 'Inscripciones rodada Santa Fe',        @ctaCaja,  @ctaEventos,    850000, @ccEventos),
    ('DEMO-EGR-00000001', '2026-05-20', 2, 'Honorarios contables mayo',            @ctaHonor, @ctaBanco,      600000, @ccFundacion),
    ('DEMO-EGR-00000002', '2026-05-28', 2, 'Transporte logistica rodada',          @ctaTranspo, @ctaCaja,     320000, @ccEventos),
    -- Junio 2026
    ('DEMO-ING-00000003', '2026-06-04', 1, 'Recaudo cuotas de sostenimiento junio', @ctaBanco, @ctaCuotas,   2550000, @ccCapitulo),
    ('DEMO-ING-00000004', '2026-06-15', 1, 'Donacion libre empresa aliada',         @ctaBanco, @ctaDonaLibre, 1500000, @ccFundacion),
    ('DEMO-ING-00000005', '2026-06-22', 1, 'Venta de merchandising junio',          @ctaCaja,  @ctaMerch,      430000, @ccCapitulo),
    ('DEMO-EGR-00000003', '2026-06-18', 2, 'Honorarios contables junio',            @ctaHonor, @ctaBanco,      600000, @ccFundacion),
    ('DEMO-EGR-00000004', '2026-06-25', 2, 'Actividad deportiva jornada social',    @ctaDeporte, @ctaBanco,    780000, @ccFundacion),
    -- Julio 2026
    ('DEMO-ING-00000006', '2026-07-03', 1, 'Recaudo cuotas de sostenimiento julio', @ctaBanco, @ctaCuotas,   2700000, @ccCapitulo),
    ('DEMO-ING-00000007', '2026-07-14', 1, 'Inscripciones rodada Guatape',          @ctaCaja,  @ctaEventos,    920000, @ccEventos),
    ('DEMO-ING-00000008', '2026-07-21', 1, 'Venta de merchandising julio',          @ctaCaja,  @ctaMerch,      615000, @ccCapitulo),
    ('DEMO-EGR-00000005', '2026-07-16', 2, 'Honorarios contables julio',            @ctaHonor, @ctaBanco,      600000, @ccFundacion),
    ('DEMO-EGR-00000006', '2026-07-24', 2, 'Gastos varios administrativos',         @ctaOtros, @ctaBanco,      245000, @ccFundacion);

DECLARE @consecutivo NVARCHAR(50), @fecha DATETIME2, @tipo INT, @descripcion NVARCHAR(500);
DECLARE @cuentaDebe UNIQUEIDENTIFIER, @cuentaHaber UNIQUEIDENTIFIER, @monto DECIMAL(18,2), @cc UNIQUEIDENTIFIER;
DECLARE @comprobanteId UNIQUEIDENTIFIER;

DECLARE cursorMovimientos CURSOR LOCAL FAST_FORWARD FOR
    SELECT Consecutivo, Fecha, Tipo, Descripcion, CuentaDebe, CuentaHaber, Monto, CentroCosto FROM @movimientos;

OPEN cursorMovimientos;
FETCH NEXT FROM cursorMovimientos INTO @consecutivo, @fecha, @tipo, @descripcion, @cuentaDebe, @cuentaHaber, @monto, @cc;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Comprobantes WHERE NumeroConsecutivo = @consecutivo)
    BEGIN
        SET @comprobanteId = NEWID();

        -- EstadoComprobante = 2 (Asentado): los libros solo leen asentados.
        INSERT INTO Comprobantes (Id, NumeroConsecutivo, Fecha, TipoComprobante, Descripcion, EstadoComprobante, IsDeleted, CreatedAt)
        VALUES (@comprobanteId, @consecutivo, @fecha, @tipo, @descripcion, 2, 0, SYSUTCDATETIME());

        INSERT INTO AsientosContables (Id, ComprobanteId, CuentaContableId, TerceroId, CentroCostoId, Debe, Haber, Referencia, IsDeleted, CreatedAt)
        VALUES
            (NEWID(), @comprobanteId, @cuentaDebe,  NULL, @cc, @monto, 0,      @descripcion, 0, SYSUTCDATETIME()),
            (NEWID(), @comprobanteId, @cuentaHaber, NULL, @cc, 0,      @monto, @descripcion, 0, SYSUTCDATETIME());
    END

    FETCH NEXT FROM cursorMovimientos INTO @consecutivo, @fecha, @tipo, @descripcion, @cuentaDebe, @cuentaHaber, @monto, @cc;
END

CLOSE cursorMovimientos;
DEALLOCATE cursorMovimientos;

-- ---------------------------------------------------------------------------
-- 2. Conceptos de cobro
-- ---------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM ConceptosCobro WHERE Nombre = 'DEMO- Cuota de sostenimiento mensual')
    INSERT INTO ConceptosCobro (Id, Nombre, ValorCOP, PeriodicidadMensual, CuentaContableIngresoId, IsDeleted, CreatedAt)
    VALUES (NEWID(), 'DEMO- Cuota de sostenimiento mensual', 60000, 1, @ctaCuotas, 0, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM ConceptosCobro WHERE Nombre = 'DEMO- Cuota extraordinaria de asamblea')
    INSERT INTO ConceptosCobro (Id, Nombre, ValorCOP, PeriodicidadMensual, CuentaContableIngresoId, IsDeleted, CreatedAt)
    VALUES (NEWID(), 'DEMO- Cuota extraordinaria de asamblea', 150000, 12, @ctaCuotas, 0, SYSUTCDATETIME());

DECLARE @conceptoMensual UNIQUEIDENTIFIER = (SELECT Id FROM ConceptosCobro WHERE Nombre = 'DEMO- Cuota de sostenimiento mensual');

-- ---------------------------------------------------------------------------
-- 3. Cartera con estados variados
--
-- Estado: 1 pendiente, 2 pagada parcial, 3 pagada. Se reparten los miembros en
-- los tres estados y con vencimientos distintos, para que el reporte de mora y
-- el tablero muestren casos reales y no una sola situacion repetida.
-- ---------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM CuentasPorCobrar cpc JOIN ConceptosCobro cc ON cc.Id = cpc.ConceptoCobroId WHERE cc.Nombre LIKE 'DEMO-%')
BEGIN
    ;WITH MiembrosNumerados AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY Apellidos, Nombres) AS Fila
        FROM Miembros
        WHERE EsActivo = 1 AND IsDeleted = 0
    )
    INSERT INTO CuentasPorCobrar (Id, MiembroId, ConceptoCobroId, FechaEmision, FechaVencimiento, ValorTotal, SaldoPendiente, Estado, IsDeleted, CreatedAt)
    SELECT
        NEWID(),
        Id,
        @conceptoMensual,
        CASE WHEN Fila % 3 = 0 THEN '2026-05-01' WHEN Fila % 3 = 1 THEN '2026-06-01' ELSE '2026-07-01' END,
        CASE WHEN Fila % 3 = 0 THEN '2026-05-31' WHEN Fila % 3 = 1 THEN '2026-06-30' ELSE '2026-07-31' END,
        60000,
        -- pendiente completo / abono parcial / saldada
        CASE WHEN Fila % 3 = 0 THEN 60000 WHEN Fila % 3 = 1 THEN 25000 ELSE 0 END,
        CASE WHEN Fila % 3 = 0 THEN 1     WHEN Fila % 3 = 1 THEN 2     ELSE 3 END,
        0,
        SYSUTCDATETIME()
    FROM MiembrosNumerados
    WHERE Fila <= 18;
END

-- ---------------------------------------------------------------------------
-- 4. Productos de merchandising
-- ---------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM Productos WHERE CodigoSKU LIKE 'DEMO-%')
    INSERT INTO Productos (Id, Nombre, CodigoSKU, PrecioVenta, CantidadEnStock, CantidadMinima, CuentaContableIngresoId, ImageUrl, IsDeleted, CreatedAt)
    VALUES
        (NEWID(), 'Camiseta oficial L.A.M.A.',  'DEMO-CAM-001',  85000, 42,  10, @ctaMerch, NULL, 0, SYSUTCDATETIME()),
        (NEWID(), 'Parche bordado capitulo',    'DEMO-PAR-001',  35000, 120, 25, @ctaMerch, NULL, 0, SYSUTCDATETIME()),
        (NEWID(), 'Gorra bordada',              'DEMO-GOR-001',  55000,  8,  15, @ctaMerch, NULL, 0, SYSUTCDATETIME()),
        (NEWID(), 'Chaqueta impermeable',       'DEMO-CHA-001', 320000, 14,   5, @ctaMerch, NULL, 0, SYSUTCDATETIME());

-- ---------------------------------------------------------------------------
-- 5. Donantes y donaciones
--
-- TipoPersona: 1 natural, 2 juridica. FormaDonacion: 1 dinero, 2 especie.
-- ---------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM Donantes WHERE NumeroDocumento LIKE 'DEMO-%')
    INSERT INTO Donantes (Id, NombreORazonSocial, TipoDocumento, NumeroDocumento, Email, TipoPersona, IsDeleted, CreatedAt)
    VALUES
        (NEWID(), 'Distribuidora Motopartes S.A.S.', 2, 'DEMO-900123456', 'contacto@motopartes.demo', 2, 0, SYSUTCDATETIME()),
        (NEWID(), 'Talleres del Norte Ltda.',        2, 'DEMO-900987654', 'info@talleresnorte.demo',  2, 0, SYSUTCDATETIME()),
        (NEWID(), 'Carolina Restrepo Alvarez',       1, 'DEMO-43567890',  'carolina@correo.demo',     1, 0, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Donaciones WHERE CodigoVerificacion LIKE 'DEMO-%')
BEGIN
    DECLARE @donante1 UNIQUEIDENTIFIER = (SELECT Id FROM Donantes WHERE NumeroDocumento = 'DEMO-900123456');
    DECLARE @donante2 UNIQUEIDENTIFIER = (SELECT Id FROM Donantes WHERE NumeroDocumento = 'DEMO-900987654');
    DECLARE @donante3 UNIQUEIDENTIFIER = (SELECT Id FROM Donantes WHERE NumeroDocumento = 'DEMO-43567890');

    INSERT INTO Donaciones (Id, DonanteId, MontoCOP, Fecha, BancoId, CentroCostoId, CertificadoEmitido, CodigoVerificacion, FormaDonacion, MedioPagoODescripcion, IsDeleted, CreatedAt)
    VALUES
        (NEWID(), @donante1, 1500000, '2026-06-15', @banco, @ccFundacion, 1, 'DEMO-CERT-0001', 1, 'Transferencia Bancolombia', 0, SYSUTCDATETIME()),
        (NEWID(), @donante2,  750000, '2026-07-08', @banco, @ccFundacion, 1, 'DEMO-CERT-0002', 1, 'Transferencia Bancolombia', 0, SYSUTCDATETIME()),
        (NEWID(), @donante3,  300000, '2026-07-19', @banco, @ccFundacion, 0, 'DEMO-CERT-0003', 1, 'Consignacion en efectivo',   0, SYSUTCDATETIME());
END

COMMIT TRANSACTION;

-- ---------------------------------------------------------------------------
-- Resumen de lo cargado
-- ---------------------------------------------------------------------------
PRINT '';
PRINT '--- Datos de demostracion cargados ---';

SELECT 'Comprobantes DEMO'   AS Concepto, COUNT(*) AS Cantidad FROM Comprobantes WHERE NumeroConsecutivo LIKE 'DEMO-%'
UNION ALL SELECT 'Asientos contables', COUNT(*) FROM AsientosContables a JOIN Comprobantes c ON c.Id = a.ComprobanteId WHERE c.NumeroConsecutivo LIKE 'DEMO-%'
UNION ALL SELECT 'Conceptos de cobro', COUNT(*) FROM ConceptosCobro WHERE Nombre LIKE 'DEMO-%'
UNION ALL SELECT 'Cuentas por cobrar', COUNT(*) FROM CuentasPorCobrar cpc JOIN ConceptosCobro cc ON cc.Id = cpc.ConceptoCobroId WHERE cc.Nombre LIKE 'DEMO-%'
UNION ALL SELECT 'Productos', COUNT(*) FROM Productos WHERE CodigoSKU LIKE 'DEMO-%'
UNION ALL SELECT 'Donantes', COUNT(*) FROM Donantes WHERE NumeroDocumento LIKE 'DEMO-%'
UNION ALL SELECT 'Donaciones', COUNT(*) FROM Donaciones WHERE CodigoVerificacion LIKE 'DEMO-%';

PRINT '';
PRINT '--- Verificacion de partida doble ---';

SELECT
    'Total debe'  AS Concepto, FORMAT(SUM(a.Debe), 'N0')  AS Valor
FROM AsientosContables a JOIN Comprobantes c ON c.Id = a.ComprobanteId
WHERE c.NumeroConsecutivo LIKE 'DEMO-%'
UNION ALL
SELECT 'Total haber', FORMAT(SUM(a.Haber), 'N0')
FROM AsientosContables a JOIN Comprobantes c ON c.Id = a.ComprobanteId
WHERE c.NumeroConsecutivo LIKE 'DEMO-%'
UNION ALL
SELECT 'Cuadra', CASE WHEN SUM(a.Debe) = SUM(a.Haber) THEN 'SI' ELSE 'NO' END
FROM AsientosContables a JOIN Comprobantes c ON c.Id = a.ComprobanteId
WHERE c.NumeroConsecutivo LIKE 'DEMO-%';
