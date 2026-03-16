SET XACT_ABORT ON;
BEGIN TRAN;

DECLARE @Cuentas TABLE
(
    Codigo NVARCHAR(20) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    Naturaleza INT NOT NULL,
    PermiteMovimiento BIT NOT NULL,
    ExigeTercero BIT NOT NULL,
    CodigoPadre NVARCHAR(20) NULL
);

INSERT INTO @Cuentas (Codigo, Descripcion, Naturaleza, PermiteMovimiento, ExigeTercero, CodigoPadre)
VALUES
('3', 'PATRIMONIO INSTITUCIONAL', 2, 0, 0, NULL),
('31', 'Fondo Social', 2, 0, 0, '3'),
('3105', 'Aportes de Fundadores', 2, 0, 0, '31'),
('310505', 'Aportes en Dinero', 2, 1, 1, '3105'),
('310510', 'Aportes en Especie', 2, 1, 1, '3105'),
('3115', 'Fondo de Destinación Específica', 2, 0, 0, '31'),
('311505', 'Reserva para proyectos misionales', 2, 1, 0, '3115'),
('32', 'Resultados del Ejercicio (No Utilidades)', 2, 0, 0, '3'),
('3205', 'Excedente del Ejercicio', 2, 1, 0, '32'),
('3210', 'Déficit del Ejercicio', 1, 1, 0, '32'),
('4', 'INGRESOS', 2, 0, 0, NULL),
('41', 'Ingresos de Actividades Ordinarias', 2, 0, 0, '4'),
('4105', 'Aportes y Cuotas de Sostenimiento', 2, 0, 0, '41'),
('410505', 'Cuotas de Afiliación (Nuevos)', 2, 1, 1, '4105'),
('410510', 'Cuotas de Sostenimiento (Mensualidad)', 2, 1, 1, '4105'),
('4110', 'Ingresos por Eventos y Actividades', 2, 0, 0, '41'),
('411005', 'Inscripciones a Rodadas y Eventos', 2, 1, 1, '4110'),
('411010', 'Venta de Merchandising (Parches, etc.)', 2, 1, 0, '4110'),
('4115', 'Donaciones Recibidas', 2, 0, 0, '41'),
('411505', 'Donaciones No Condicionadas (Libres)', 2, 1, 1, '4115'),
('411510', 'Donaciones Condicionadas (Proyectos)', 2, 1, 1, '4115'),
('5', 'GASTOS ADMINISTRATIVOS', 1, 0, 0, NULL),
('51', 'Operación y Administración', 1, 0, 0, '5'),
('5105', 'Gastos de Representación', 1, 0, 0, '51'),
('510505', 'Reuniones de Junta Directiva', 1, 1, 0, '5105'),
('5110', 'Honorarios y Servicios', 1, 0, 0, '51'),
('511005', 'Honorarios Contables y Legales', 1, 1, 1, '5110'),
('6', 'COSTOS DE PROYECTOS MISIONALES', 1, 0, 0, NULL),
('61', 'Costos de Eventos y Rodadas', 1, 0, 0, '6'),
('6105', 'Logística de Eventos', 1, 0, 0, '61'),
('610505', 'Alquiler de Espacios / Permisos', 1, 1, 1, '6105'),
('610510', 'Alimentación y Refrigerios', 1, 1, 1, '6105'),
('610515', 'Reconocimientos y Trofeos', 1, 1, 1, '6105');

MERGE dbo.CuentasContables AS target
USING (
    SELECT Codigo, Descripcion, Naturaleza, PermiteMovimiento, ExigeTercero
    FROM @Cuentas
) AS source
ON target.Codigo = source.Codigo
WHEN MATCHED THEN
    UPDATE SET
        target.Descripcion = source.Descripcion,
        target.Naturaleza = source.Naturaleza,
        target.PermiteMovimiento = source.PermiteMovimiento,
        target.ExigeTercero = source.ExigeTercero,
        target.IsDeleted = 0,
        target.CuentaPadreId = NULL
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Codigo, Descripcion, PermiteMovimiento, IsDeleted, CuentaPadreId, ExigeTercero, Naturaleza)
    VALUES (NEWID(), source.Codigo, source.Descripcion, source.PermiteMovimiento, 0, NULL, source.ExigeTercero, source.Naturaleza);

UPDATE child
SET child.CuentaPadreId = parent.Id
FROM dbo.CuentasContables child
INNER JOIN @Cuentas s ON s.Codigo = child.Codigo
LEFT JOIN dbo.CuentasContables parent ON parent.Codigo = s.CodigoPadre
WHERE child.IsDeleted = 0;

SELECT COUNT(*) AS TotalCuentasCargadas
FROM dbo.CuentasContables
WHERE IsDeleted = 0;

COMMIT;
