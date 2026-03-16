IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221051812_InitialCreate'
)
BEGIN
    CREATE TABLE [Bancos] (
        [Id] uniqueidentifier NOT NULL,
        [NumeroCuenta] nvarchar(50) NOT NULL,
        [SaldoActual] decimal(18,2) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Bancos] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221051812_InitialCreate'
)
BEGIN
    CREATE TABLE [CentrosCosto] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(200) NOT NULL,
        [Tipo] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CentrosCosto] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221051812_InitialCreate'
)
BEGIN
    CREATE TABLE [Transacciones] (
        [Id] uniqueidentifier NOT NULL,
        [MontoCOP] decimal(18,2) NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Tipo] int NOT NULL,
        [MedioPago] int NOT NULL,
        [CentroCostoId] uniqueidentifier NOT NULL,
        [BancoId] uniqueidentifier NOT NULL,
        [MonedaOrigen] nvarchar(10) NULL,
        [MontoMonedaOrigen] decimal(18,2) NULL,
        [TasaCambioUsada] decimal(18,2) NULL,
        [FechaTasaCambio] datetime2 NULL,
        [FuenteTasaCambio] int NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Transacciones] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Transacciones_Bancos_BancoId] FOREIGN KEY ([BancoId]) REFERENCES [Bancos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Transacciones_CentrosCosto_CentroCostoId] FOREIGN KEY ([CentroCostoId]) REFERENCES [CentrosCosto] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221051812_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Bancos_NumeroCuenta] ON [Bancos] ([NumeroCuenta]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221051812_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Transacciones_BancoId] ON [Transacciones] ([BancoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221051812_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Transacciones_CentroCostoId] ON [Transacciones] ([CentroCostoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221051812_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260221051812_InitialCreate', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221232252_AddCarteraCoreEntities'
)
BEGIN
    CREATE TABLE [CuotasAsamblea] (
        [Id] uniqueidentifier NOT NULL,
        [Anio] int NOT NULL,
        [ValorMensualCOP] decimal(18,2) NOT NULL,
        [MesInicioCobro] int NOT NULL,
        [ActaSoporte] nvarchar(500) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CuotasAsamblea] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221232252_AddCarteraCoreEntities'
)
BEGIN
    CREATE TABLE [Miembros] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Apellidos] nvarchar(120) NOT NULL,
        [Documento] nvarchar(30) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [Telefono] nvarchar(30) NOT NULL,
        [TipoAfiliacion] int NOT NULL,
        [Estado] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Miembros] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221232252_AddCarteraCoreEntities'
)
BEGIN
    CREATE TABLE [CuentasPorCobrar] (
        [Id] uniqueidentifier NOT NULL,
        [MiembroId] uniqueidentifier NOT NULL,
        [Periodo] nvarchar(7) NOT NULL,
        [ValorEsperadoCOP] decimal(18,2) NOT NULL,
        [SaldoPendienteCOP] decimal(18,2) NOT NULL,
        [Estado] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CuentasPorCobrar] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CuentasPorCobrar_Miembros_MiembroId] FOREIGN KEY ([MiembroId]) REFERENCES [Miembros] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221232252_AddCarteraCoreEntities'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CuentasPorCobrar_MiembroId_Periodo] ON [CuentasPorCobrar] ([MiembroId], [Periodo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221232252_AddCarteraCoreEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260221232252_AddCarteraCoreEntities', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260223215438_AddDescripcionToTransaccion'
)
BEGIN
    ALTER TABLE [Transacciones] ADD [Descripcion] nvarchar(500) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260223215438_AddDescripcionToTransaccion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260223215438_AddDescripcionToTransaccion', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225132237_AddDonacionesModule'
)
BEGIN
    CREATE TABLE [Donantes] (
        [Id] uniqueidentifier NOT NULL,
        [NombreORazonSocial] nvarchar(200) NOT NULL,
        [TipoDocumento] int NOT NULL,
        [NumeroDocumento] nvarchar(30) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [TipoPersona] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Donantes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225132237_AddDonacionesModule'
)
BEGIN
    CREATE TABLE [Donaciones] (
        [Id] uniqueidentifier NOT NULL,
        [DonanteId] uniqueidentifier NOT NULL,
        [MontoCOP] decimal(18,2) NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [BancoId] uniqueidentifier NOT NULL,
        [CentroCostoId] uniqueidentifier NOT NULL,
        [CertificadoEmitido] bit NOT NULL,
        [CodigoVerificacion] nvarchar(50) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Donaciones] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Donaciones_Bancos_BancoId] FOREIGN KEY ([BancoId]) REFERENCES [Bancos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Donaciones_CentrosCosto_CentroCostoId] FOREIGN KEY ([CentroCostoId]) REFERENCES [CentrosCosto] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Donaciones_Donantes_DonanteId] FOREIGN KEY ([DonanteId]) REFERENCES [Donantes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225132237_AddDonacionesModule'
)
BEGIN
    CREATE INDEX [IX_Donaciones_BancoId] ON [Donaciones] ([BancoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225132237_AddDonacionesModule'
)
BEGIN
    CREATE INDEX [IX_Donaciones_CentroCostoId] ON [Donaciones] ([CentroCostoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225132237_AddDonacionesModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Donaciones_CodigoVerificacion] ON [Donaciones] ([CodigoVerificacion]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225132237_AddDonacionesModule'
)
BEGIN
    CREATE INDEX [IX_Donaciones_DonanteId] ON [Donaciones] ([DonanteId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225132237_AddDonacionesModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Donantes_TipoDocumento_NumeroDocumento] ON [Donantes] ([TipoDocumento], [NumeroDocumento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225132237_AddDonacionesModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260225132237_AddDonacionesModule', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225153516_AddProyectosModule'
)
BEGIN
    CREATE TABLE [Beneficiarios] (
        [Id] uniqueidentifier NOT NULL,
        [NombreCompleto] nvarchar(200) NOT NULL,
        [TipoDocumento] nvarchar(30) NOT NULL,
        [NumeroDocumento] nvarchar(30) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [Telefono] nvarchar(30) NOT NULL,
        [TieneConsentimientoHabeasData] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Beneficiarios] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225153516_AddProyectosModule'
)
BEGIN
    CREATE TABLE [ProyectosSociales] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(200) NOT NULL,
        [Descripcion] nvarchar(1000) NOT NULL,
        [FechaInicio] datetime2 NOT NULL,
        [FechaFin] datetime2 NULL,
        [PresupuestoEstimado] decimal(18,2) NOT NULL,
        [Estado] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ProyectosSociales] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225153516_AddProyectosModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Beneficiarios_TipoDocumento_NumeroDocumento] ON [Beneficiarios] ([TipoDocumento], [NumeroDocumento]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225153516_AddProyectosModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260225153516_AddProyectosModule', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225160331_AddLegalDonacionFormaYDetalle'
)
BEGIN
    ALTER TABLE [Donaciones] ADD [FormaDonacion] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225160331_AddLegalDonacionFormaYDetalle'
)
BEGIN
    ALTER TABLE [Donaciones] ADD [MedioPagoODescripcion] nvarchar(500) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225160331_AddLegalDonacionFormaYDetalle'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260225160331_AddLegalDonacionFormaYDetalle', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225213507_AddMotorContablePartidaDoble'
)
BEGIN
    CREATE TABLE [Comprobantes] (
        [Id] uniqueidentifier NOT NULL,
        [NumeroConsecutivo] nvarchar(50) NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [TipoComprobante] int NOT NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        [EstadoComprobante] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Comprobantes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225213507_AddMotorContablePartidaDoble'
)
BEGIN
    CREATE TABLE [CuentasContables] (
        [Id] uniqueidentifier NOT NULL,
        [Codigo] nvarchar(20) NOT NULL,
        [Descripcion] nvarchar(300) NOT NULL,
        [PermiteMovimiento] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CuentasContables] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225213507_AddMotorContablePartidaDoble'
)
BEGIN
    CREATE TABLE [AsientosContables] (
        [Id] uniqueidentifier NOT NULL,
        [ComprobanteId] uniqueidentifier NOT NULL,
        [CuentaContableId] uniqueidentifier NOT NULL,
        [TerceroId] uniqueidentifier NULL,
        [CentroCostoId] uniqueidentifier NOT NULL,
        [Debe] decimal(18,2) NOT NULL,
        [Haber] decimal(18,2) NOT NULL,
        [Referencia] nvarchar(500) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_AsientosContables] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_AsientoContable_DebeHaber_Exclusivo] CHECK ((([Debe] > 0 AND [Haber] = 0) OR ([Debe] = 0 AND [Haber] > 0))),
        CONSTRAINT [FK_AsientosContables_CentrosCosto_CentroCostoId] FOREIGN KEY ([CentroCostoId]) REFERENCES [CentrosCosto] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AsientosContables_Comprobantes_ComprobanteId] FOREIGN KEY ([ComprobanteId]) REFERENCES [Comprobantes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AsientosContables_CuentasContables_CuentaContableId] FOREIGN KEY ([CuentaContableId]) REFERENCES [CuentasContables] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225213507_AddMotorContablePartidaDoble'
)
BEGIN
    CREATE INDEX [IX_AsientosContables_CentroCostoId] ON [AsientosContables] ([CentroCostoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225213507_AddMotorContablePartidaDoble'
)
BEGIN
    CREATE INDEX [IX_AsientosContables_ComprobanteId] ON [AsientosContables] ([ComprobanteId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225213507_AddMotorContablePartidaDoble'
)
BEGIN
    CREATE INDEX [IX_AsientosContables_CuentaContableId] ON [AsientosContables] ([CuentaContableId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225213507_AddMotorContablePartidaDoble'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Comprobantes_NumeroConsecutivo] ON [Comprobantes] ([NumeroConsecutivo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225213507_AddMotorContablePartidaDoble'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CuentasContables_Codigo] ON [CuentasContables] ([Codigo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225213507_AddMotorContablePartidaDoble'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260225213507_AddMotorContablePartidaDoble', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225231354_AddCuentaContableJerarquiaYNaturaleza'
)
BEGIN
    ALTER TABLE [CuentasContables] ADD [CuentaPadreId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225231354_AddCuentaContableJerarquiaYNaturaleza'
)
BEGIN
    ALTER TABLE [CuentasContables] ADD [ExigeTercero] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225231354_AddCuentaContableJerarquiaYNaturaleza'
)
BEGIN
    ALTER TABLE [CuentasContables] ADD [Naturaleza] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225231354_AddCuentaContableJerarquiaYNaturaleza'
)
BEGIN
    CREATE INDEX [IX_CuentasContables_CuentaPadreId] ON [CuentasContables] ([CuentaPadreId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225231354_AddCuentaContableJerarquiaYNaturaleza'
)
BEGIN
    ALTER TABLE [CuentasContables] ADD CONSTRAINT [FK_CuentasContables_CuentasContables_CuentaPadreId] FOREIGN KEY ([CuentaPadreId]) REFERENCES [CuentasContables] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225231354_AddCuentaContableJerarquiaYNaturaleza'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260225231354_AddCuentaContableJerarquiaYNaturaleza', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260226003510_AddMerchandisingModule'
)
BEGIN
    CREATE TABLE [Articulos] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(200) NOT NULL,
        [SKU] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        [Categoria] int NOT NULL,
        [PrecioVenta] decimal(18,2) NOT NULL,
        [CostoPromedio] decimal(18,2) NOT NULL,
        [StockActual] int NOT NULL,
        [CuentaContableIngresoId] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Articulos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Articulos_CuentasContables_CuentaContableIngresoId] FOREIGN KEY ([CuentaContableIngresoId]) REFERENCES [CuentasContables] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260226003510_AddMerchandisingModule'
)
BEGIN
    CREATE TABLE [Ventas] (
        [Id] uniqueidentifier NOT NULL,
        [NumeroFacturaInterna] nvarchar(100) NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [CompradorId] uniqueidentifier NULL,
        [Total] decimal(18,2) NOT NULL,
        [MetodoPago] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Ventas] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260226003510_AddMerchandisingModule'
)
BEGIN
    CREATE TABLE [DetallesVenta] (
        [Id] uniqueidentifier NOT NULL,
        [VentaId] uniqueidentifier NOT NULL,
        [ArticuloId] uniqueidentifier NOT NULL,
        [Cantidad] int NOT NULL,
        [PrecioUnitario] decimal(18,2) NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DetallesVenta] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DetallesVenta_Articulos_ArticuloId] FOREIGN KEY ([ArticuloId]) REFERENCES [Articulos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DetallesVenta_Ventas_VentaId] FOREIGN KEY ([VentaId]) REFERENCES [Ventas] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260226003510_AddMerchandisingModule'
)
BEGIN
    CREATE INDEX [IX_Articulos_CuentaContableIngresoId] ON [Articulos] ([CuentaContableIngresoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260226003510_AddMerchandisingModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Articulos_SKU] ON [Articulos] ([SKU]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260226003510_AddMerchandisingModule'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_ArticuloId] ON [DetallesVenta] ([ArticuloId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260226003510_AddMerchandisingModule'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_VentaId] ON [DetallesVenta] ([VentaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260226003510_AddMerchandisingModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Ventas_NumeroFacturaInterna] ON [Ventas] ([NumeroFacturaInterna]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260226003510_AddMerchandisingModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260226003510_AddMerchandisingModule', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304175125_AddCentroCostoToProyectoSocial'
)
BEGIN
    ALTER TABLE [ProyectosSociales] ADD [CentroCostoId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304175125_AddCentroCostoToProyectoSocial'
)
BEGIN
    UPDATE p
                                        SET p.CentroCostoId = c.Id
                                        FROM ProyectosSociales p
                                        CROSS JOIN (
                                            SELECT TOP 1 Id
                                            FROM CentrosCosto
                                            ORDER BY Nombre
                                        ) c
                                        WHERE p.CentroCostoId IS NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304175125_AddCentroCostoToProyectoSocial'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProyectosSociales]') AND [c].[name] = N'CentroCostoId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ProyectosSociales] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [ProyectosSociales] ALTER COLUMN [CentroCostoId] uniqueidentifier NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304175125_AddCentroCostoToProyectoSocial'
)
BEGIN
    CREATE INDEX [IX_ProyectosSociales_CentroCostoId] ON [ProyectosSociales] ([CentroCostoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304175125_AddCentroCostoToProyectoSocial'
)
BEGIN
    ALTER TABLE [ProyectosSociales] ADD CONSTRAINT [FK_ProyectosSociales_CentrosCosto_CentroCostoId] FOREIGN KEY ([CentroCostoId]) REFERENCES [CentrosCosto] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304175125_AddCentroCostoToProyectoSocial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260304175125_AddCentroCostoToProyectoSocial', N'8.0.11');
END;
GO

COMMIT;
GO

