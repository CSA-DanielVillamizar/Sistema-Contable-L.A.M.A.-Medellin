-- Configuración aplicada en Azure SQL (lamaregionnorte-sql-server / LAMAMedellinContable)
-- Fecha: 2026-02-21
-- Objetivo:
-- 1) Crear tablas de cartera faltantes (si no existen): Miembros, CuotasAsamblea, CuentasPorCobrar
-- 2) Insertar cuota de asamblea 2026-02 (idempotente)

IF OBJECT_ID(N'[dbo].[Miembros]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Miembros]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Nombre] NVARCHAR(100) NOT NULL,
        [Apellidos] NVARCHAR(120) NOT NULL,
        [Documento] NVARCHAR(30) NOT NULL,
        [Email] NVARCHAR(200) NOT NULL,
        [Telefono] NVARCHAR(30) NOT NULL,
        [TipoAfiliacion] INT NOT NULL,
        [Estado] INT NOT NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_Miembros_IsDeleted] DEFAULT(0),
        CONSTRAINT [PK_Miembros] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[dbo].[CuotasAsamblea]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CuotasAsamblea]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Anio] INT NOT NULL,
        [ValorMensualCOP] DECIMAL(18,2) NOT NULL,
        [MesInicioCobro] INT NOT NULL,
        [ActaSoporte] NVARCHAR(500) NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_CuotasAsamblea_IsDeleted] DEFAULT(0),
        CONSTRAINT [PK_CuotasAsamblea] PRIMARY KEY ([Id])
    );
END;

IF OBJECT_ID(N'[dbo].[CuentasPorCobrar]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CuentasPorCobrar]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [MiembroId] UNIQUEIDENTIFIER NOT NULL,
        [Periodo] NVARCHAR(7) NOT NULL,
        [ValorEsperadoCOP] DECIMAL(18,2) NOT NULL,
        [SaldoPendienteCOP] DECIMAL(18,2) NOT NULL,
        [Estado] INT NOT NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_CuentasPorCobrar_IsDeleted] DEFAULT(0),
        CONSTRAINT [PK_CuentasPorCobrar] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CuentasPorCobrar_Miembros_MiembroId] FOREIGN KEY ([MiembroId]) REFERENCES [dbo].[Miembros]([Id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_CuentasPorCobrar_MiembroId_Periodo] ON [dbo].[CuentasPorCobrar]([MiembroId], [Periodo]);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM [dbo].[CuotasAsamblea]
    WHERE [Anio] = 2026
      AND [MesInicioCobro] = 2
      AND [ValorMensualCOP] = 25000.00
      AND [ActaSoporte] = N'Acta Asamblea Enero 2026'
      AND [IsDeleted] = 0
)
BEGIN
    INSERT INTO [dbo].[CuotasAsamblea]
    (
        [Id],
        [Anio],
        [ValorMensualCOP],
        [MesInicioCobro],
        [ActaSoporte],
        [IsDeleted]
    )
    VALUES
    (
        NEWID(),
        2026,
        25000.00,
        2,
        N'Acta Asamblea Enero 2026',
        0
    );
END;

-- Verificación rápida
SELECT COUNT(1) AS TotalCuotas2026
FROM [dbo].[CuotasAsamblea]
WHERE [Anio] = 2026;

SELECT TOP (5)
    [Id],
    [Anio],
    [ValorMensualCOP],
    [MesInicioCobro],
    [ActaSoporte],
    [IsDeleted]
FROM [dbo].[CuotasAsamblea]
WHERE [Anio] = 2026
ORDER BY [MesInicioCobro] DESC, [Id] DESC;
