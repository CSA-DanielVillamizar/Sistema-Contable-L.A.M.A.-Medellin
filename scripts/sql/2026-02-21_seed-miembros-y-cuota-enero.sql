-- =============================================================================
-- Script: 2026-02-21_seed-miembros-y-cuota-enero.sql
-- Purpose: Seed initial members (37) and insert January 2026 cuota (20000 COP)
-- Database: LAMAMedellinContable (Azure SQL)
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 1. INSERT CUOTA ASAMBLEA ENERO 2026 (MesInicioCobro=1, Valor=20000)
-- -----------------------------------------------------------------------------

IF NOT EXISTS (
    SELECT 1
    FROM [dbo].[CuotasAsamblea]
    WHERE [Anio] = 2026
      AND [MesInicioCobro] = 1
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
        20000.00,
        1,
        N'Acta Asamblea Diciembre 2025 (Cuota Histórica)',
        0
    );
    PRINT 'Cuota Asamblea 2026-01 (20000) insertada';
END
ELSE
BEGIN
    PRINT 'Cuota Asamblea 2026-01 ya existe';
END;
GO

-- -----------------------------------------------------------------------------
-- 2. SEED MIEMBROS (37 records)
-- -----------------------------------------------------------------------------

-- Full Color Members (21)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '8336963')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Héctor Mario', N'González Henao', '8336963', 'hecmarg@yahoo.com', '3104363831', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '15432593')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Ramón Antonio', N'González Castaño', '15432593', 'raangoca@gmail.com', '3137672573', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '9528949')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Jhon Harvey', N'Gómez Patiño', '9528949', 'jhongo01@hotmail.com', '3006155416', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '98496540')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'William Humberto', N'Jiménez Perez', '98496540', 'williamhjp@hotmail.com', '3017969572', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '71334468')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Carlos Alberto', N'Araque Betancur', '71334468', 'cocoloquisimo@gmail.com', '3206693638', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '98589814')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Milton Darío', N'Gómez Rivera', '98589814', 'miltondariog@gmail.com', '3183507127', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '75049349')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Carlos Mario', N'Ceballos', '75049349', 'carmace7@gmail.com', '3147244972', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '98699136')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Carlos Andrés', N'Pérez Areiza', '98699136', 'carlosap@gmail.com', '3017560517', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '1095808546')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Juan Esteban', N'Suárez Correa', '1095808546', 'suarezcorreaj@gmail.com', '3156160015', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '72345562')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Jhon Emmanuel', N'Arzuza Páez', '72345562', 'jhonarzuza@gmail.com', '3003876340', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '8335981')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'José Edinson', N'Ospina Cruz', '8335981', 'chattu.1964@hotmail.com', '3008542336', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '1128406344')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Jefferson', N'Montoya Muñoz', '1128406344', 'majayura2011@hotmail.com', '3508319246', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '1036634452')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Anderson Arlex', N'Betancur Rua', '1036634452', 'armigas7@gmail.com', '3194207889', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '71380596')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Robinson Alejandro', N'Galvis Parra', '71380596', 'robin11952@hotmail.com', '3105127314', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '15506596')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Carlos Mario', N'Díaz Díaz', '15506596', 'carlosmario.diazdiaz@gmail.com', '3213167406', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '1128399797')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Juan Esteban', N'Osorio', '1128399797', 'Juan.osorio1429@correo.policia.gov.co', '3112710782', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '8162536')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Carlos Julio', N'Rendón Díaz', '8162536', 'movie.cj@gmail.com', '3507757020', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '8106002')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Daniel Andrey', N'Villamizar Araque', '8106002', 'dvillamizara@gmail.com', '3106328171', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Email] = 'jhonda361@gmail.com' AND [Nombre] = N'Jhon David')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Jhon David', N'Sánchez', '0', 'jhonda361@gmail.com', '3013424220', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '43703788')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Ángela Maria', N'Rodríguez Ochoa', '43703788', 'angelarodriguez40350@gmail.com', '3104490476', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Email] = 'yeferson915@hotmail.com' AND [Nombre] = N'Yeferson Bairon')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Yeferson Bairon', N'Úsuga Agudelo', '0', 'yeferson915@hotmail.com', '3002891509', 1, 1, 0);

-- Rockets Members (3)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '1035424338')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Jennifer Andrea', N'Cardona Benítez', '1035424338', 'tucoach21@gmail.com', '3014005382', 2, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '1094923731')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Gustavo Adolfo', N'Gómez Zuluaga', '1094923731', 'sin-correo-gustavoadolfo.gomezyuluaga@lama.local', '3132672208', 2, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '98472306')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Nelson Augusto', N'Montoya Mataute', '98472306', 'sin-correo-nelsonaugusto.montoyamataute@lama.local', '3137100335', 2, 1, 0);

-- Full Color Member (Additional)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '1090419626')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Laura Viviana', N'Salazar Moreno', '1090419626', 'laura.s.enf@hotmail.com', '3014307375', 1, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '8033065')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'José Julián', N'Villamizar Araque', '8033065', 'julianvilllamizar@outlook.com', '3014873771', 1, 1, 0);

-- Esposa Members (11)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Email] = 'chattu.1964@hotmail.com' AND [Nombre] = N'Janeth Gisela')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Janeth Gisela', N'Ospina Giraldo', '0', 'chattu.1964@hotmail.com', '3008542336', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Email] = 'macrii2009@gmail.com')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'María Cristina', N'Jiménez Ángel', '0', 'macrii2009@gmail.com', '3148103529', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Email] = 'yolaeb@hotmail.com')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Yolanda', N'Echeverry Bohórquez', '0', 'yolaeb@hotmail.com', '3113185201', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Email] = 'linzu28@hotmail.com')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Linda Yulieth', N'Zuleta López', '0', 'linzu28@hotmail.com', '3232611826', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Email] = 'zeaelectricos@hotmail.com')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Karol Natalia', N'Santamaria Gonzalez', '0', 'zeaelectricos@hotmail.com', '3146324621', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Email] = 'natygomez90@gmail.com')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Lizet Natalia', N'Gómez Franco', '0', 'natygomez90@gmail.com', '3102874898', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Telefono] = '3144009341' AND [Nombre] = N'Diana')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Diana', N'Larrota', '0', 'sin-correo-diana.larrota@lama.local', '3144009341', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Nombre] = N'Jazmin Adriana' AND [Apellidos] = 'Rojas')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Jazmin Adriana', N'Rojas', '0', 'sin-correo-jazminadriana.rojas@lama.local', '0000000000', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Telefono] = '3209768510' AND [Nombre] = N'Leidy')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Leidy', N'Hurtado', '0', 'sin-correo-leidy.hurtado@lama.local', '3209768510', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '1094930482')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Natalia', N'Londoño Ocampo', '1094930482', 'sin-correo-natalia.londonoocampo@lama.local', '0000000000', 4, 1, 0);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Miembros] WHERE [Documento] = '21991558')
    INSERT INTO [dbo].[Miembros] ([Id], [Nombre], [Apellidos], [Documento], [Email], [Telefono], [TipoAfiliacion], [Estado], [IsDeleted])
    VALUES (NEWID(), N'Sandra', N'Zapata', '21991558', 'sin-correo-sandra.zapata@lama.local', '6042846744', 4, 1, 0);

GO

PRINT 'Seed completed: Members inserted (idempotent)';
GO
