-- ============================================================================
-- Script: Seed de Cuotas de Asamblea y Miembros
-- Fecha: 2026-02-21
-- Descripción: Inserta las cuotas 2026 (Ene=20k, Feb=25k) y 37 miembros base
-- Idempotente: Usa verificación de existencia antes de insertar
-- ============================================================================

USE LAMAMedellinContable;
GO

-- ============================================================================
-- 1. CUOTAS DE ASAMBLEA (2026)
-- ============================================================================
PRINT 'Insertando Cuotas de Asamblea...';

-- Cuota Enero 2026 (Histórica): 20,000 COP
IF NOT EXISTS (SELECT 1 FROM CuotasAsamblea WHERE Anio = 2026 AND MesInicioCobro = 1)
BEGIN
    INSERT INTO CuotasAsamblea (Id, Anio, ValorMensualCOP, MesInicioCobro, ActaSoporte, IsDeleted)
    VALUES (NEWID(), 2026, 20000.00, 1, N'Acta Asamblea Diciembre 2025 (Cuota Histórica)', 0);
    PRINT '  ✓ Insertada cuota Enero 2026: $20,000';
END
ELSE
BEGIN
    PRINT '  ⊘ Cuota Enero 2026 ya existe';
END

-- Cuota Febrero 2026 (Nueva): 25,000 COP
IF NOT EXISTS (SELECT 1 FROM CuotasAsamblea WHERE Anio = 2026 AND MesInicioCobro = 2)
BEGIN
    INSERT INTO CuotasAsamblea (Id, Anio, ValorMensualCOP, MesInicioCobro, ActaSoporte, IsDeleted)
    VALUES (NEWID(), 2026, 25000.00, 2, N'Acta Asamblea Enero 2026', 0);
    PRINT '  ✓ Insertada cuota Febrero 2026: $25,000';
END
ELSE
BEGIN
    PRINT '  ⊘ Cuota Febrero 2026 ya existe';
END

-- ============================================================================
-- 2. MIEMBROS BASE (37 registros)
-- ============================================================================
PRINT '';
PRINT 'Insertando Miembros...';

DECLARE @MiembrosInsertados INT = 0;

-- Helper para insertar miembro si no existe
-- TipoAfiliacion: FullColor=1, Rockets=2, Prospect=3, Esposa=4
-- EstadoMiembro: Activo=1, Inactivo=2, Suspendido=3, Trasladado=4

-- FULL COLOR (1)
IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 100) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 100, N'Carlos Andrés Gómez', 1, 1, 1, '2024-01-15', 'carlos.gomez@email.com', '3001234567', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 101) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 101, N'Diego Fernando Martínez', 1, 1, 1, '2024-01-15', 'diego.martinez@email.com', '3009876543', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 102) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 102, N'Juan Pablo Rodríguez', 1, 1, 1, '2024-01-15', 'juan.rodriguez@email.com', '3012345678', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 103) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 103, N'Luis Eduardo Sánchez', 1, 1, 1, '2024-01-15', 'luis.sanchez@email.com', '3006789012', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 104) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 104, N'Andrés Felipe López', 1, 1, 1, '2024-01-15', 'andres.lopez@email.com', '3015678901', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 105) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 105, N'Miguel Ángel Torres', 1, 1, 1, '2024-02-01', 'miguel.torres@email.com', '3023456789', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 106) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 106, N'Ricardo José Vargas', 1, 1, 0, '2024-03-10', 'ricardo.vargas@email.com', '3034567890', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 107) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 107, N'Fernando Augusto Ramírez', 1, 1, 0, '2024-04-15', 'fernando.ramirez@email.com', '3045678901', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 108) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 108, N'Javier Orlando Pérez', 1, 1, 0, '2024-05-20', 'javier.perez@email.com', '3056789012', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 109) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 109, N'Sebastián David Herrera', 1, 1, 0, '2024-06-01', 'sebastian.herrera@email.com', '3067890123', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 110) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 110, N'Daniel Alejandro Castro', 1, 1, 0, '2024-07-10', 'daniel.castro@email.com', '3078901234', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 111) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 111, N'Mauricio Alberto Jiménez', 1, 1, 0, '2024-08-15', 'mauricio.jimenez@email.com', '3089012345', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 112) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 112, N'Oscar Hernán Morales', 1, 1, 0, '2024-09-01', 'oscar.morales@email.com', '3090123456', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 113) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 113, N'Guillermo Antonio Ruiz', 1, 1, 0, '2024-10-05', 'guillermo.ruiz@email.com', '3001112233', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 114) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 114, N'Hernán Darío Suárez', 1, 1, 0, '2024-11-12', 'hernan.suarez@email.com', '3002223344', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

-- ROCKETS (2)
IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 200) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 200, N'Camilo Andrés Ríos', 2, 1, 0, '2024-02-15', 'camilo.rios@email.com', '3003334455', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 201) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 201, N'Santiago Esteban Mejía', 2, 1, 0, '2024-03-01', 'santiago.mejia@email.com', '3004445566', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 202) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 202, N'Nicolás Felipe Cárdenas', 2, 1, 0, '2024-04-10', 'nicolas.cardenas@email.com', '3005556677', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 203) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 203, N'Mateo Alejandro Gutiérrez', 2, 1, 0, '2024-05-05', 'mateo.gutierrez@email.com', '3006667788', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 204) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 204, N'Felipe Arturo Vega', 2, 1, 0, '2024-06-15', 'felipe.vega@email.com', '3007778899', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 205) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 205, N'Alejandro José Restrepo', 2, 1, 0, '2024-07-20', 'alejandro.restrepo@email.com', '3008889900', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 206) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 206, N'Federico Manuel Osorio', 2, 1, 0, '2024-08-25', 'federico.osorio@email.com', '3009990011', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 207) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 207, N'Julián Andrés Parra', 2, 1, 0, '2024-09-10', 'julian.parra@email.com', '3010001122', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 208) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 208, N'Cristian David Gallego', 2, 1, 0, '2024-10-01', 'cristian.gallego@email.com', '3011112233', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 209) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 209, N'Esteban Alonso Montoya', 2, 1, 0, '2024-11-05', 'esteban.montoya@email.com', '3012223344', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

-- PROSPECTS (3)
IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 300) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 300, N'Andrés Mauricio Silva', 3, 1, 0, '2025-01-15', 'andres.silva@email.com', '3013334455', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 301) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 301, N'Leonardo Fabián Ortiz', 3, 1, 0, '2025-02-10', 'leonardo.ortiz@email.com', '3014445566', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 302) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 302, N'Pablo Emilio Díaz', 3, 1, 0, '2025-03-01', 'pablo.diaz@email.com', '3015556677', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 303) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 303, N'Iván Darío Muñoz', 3, 1, 0, '2025-04-15', 'ivan.munoz@email.com', '3016667788', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 304) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 304, N'Rodrigo Alfonso Benítez', 3, 1, 0, '2025-05-20', 'rodrigo.benitez@email.com', '3017778899', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

-- ESPOSAS (4)
IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 400) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 400, N'María Fernanda González', 4, 1, 0, '2024-02-01', 'maria.gonzalez@email.com', '3018889900', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 401) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 401, N'Claudia Patricia Henao', 4, 1, 0, '2024-03-15', 'claudia.henao@email.com', '3019990011', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 402) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 402, N'Ana María Botero', 4, 1, 0, '2024-04-20', 'ana.botero@email.com', '3020001122', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 403) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 403, N'Laura Catalina Uribe', 4, 1, 0, '2024-05-10', 'laura.uribe@email.com', '3021112233', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 404) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 404, N'Sandra Milena Arango', 4, 1, 0, '2024-06-05', 'sandra.arango@email.com', '3022223344', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 405) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 405, N'Diana Carolina Ramírez', 4, 1, 0, '2024-07-15', 'diana.ramirez@email.com', '3023334455', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

IF NOT EXISTS (SELECT 1 FROM Miembros WHERE NumeroMiembro = 406) BEGIN
    INSERT INTO Miembros (Id, NumeroMiembro, NombreCompleto, TipoAfiliacion, EstadoMiembro, EsSocioFundador, FechaIngreso, Email, Telefono, IsDeleted)
    VALUES (NEWID(), 406, N'Paola Andrea Valencia', 4, 1, 0, '2024-08-20', 'paola.valencia@email.com', '3024445566', 0);
    SET @MiembrosInsertados = @MiembrosInsertados + 1;
END

PRINT '  ✓ Miembros insertados: ' + CAST(@MiembrosInsertados AS VARCHAR(10));

-- ============================================================================
-- 3. VERIFICACIÓN FINAL
-- ============================================================================
PRINT '';
PRINT '=== RESUMEN FINAL ===';

DECLARE @TotalCuotas INT = (SELECT COUNT(*) FROM CuotasAsamblea WHERE Anio = 2026);
DECLARE @TotalMiembros INT = (SELECT COUNT(*) FROM Miembros WHERE IsDeleted = 0);

PRINT 'Cuotas 2026: ' + CAST(@TotalCuotas AS VARCHAR(10)) + ' (esperado: 2)';
PRINT 'Miembros activos: ' + CAST(@TotalMiembros AS VARCHAR(10)) + ' (esperado: 37)';

-- Mostrar detalle de cuotas
PRINT '';
PRINT 'Detalle de cuotas:';
SELECT 
    Anio,
    MesInicioCobro,
    ValorMensualCOP,
    ActaSoporte
FROM CuotasAsamblea
WHERE Anio = 2026
ORDER BY MesInicioCobro;

PRINT '';
PRINT '✓ Script completado exitosamente';
GO
