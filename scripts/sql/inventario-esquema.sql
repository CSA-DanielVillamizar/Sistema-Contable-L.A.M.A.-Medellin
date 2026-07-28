/*
================================================================================
 Inventario de esquema — herramienta de comparacion
================================================================================

 Volca la estructura completa de la base en texto plano, ordenado y estable,
 para poder comparar dos bases con un diff de texto.

 Uso tipico (verificar drift entre produccion y el modelo):

   1. Levantar una base local limpia desde la migracion base:
        docker compose up -d
        cd LAMAMedellin && dotnet run --project src/LAMAMedellin.API --no-launch-profile

   2. Volcar ambas:
        sqlcmd -S <prod>   -d LAMAMedellinContable -h -1 -W -i inventario-esquema.sql > prod.txt
        sqlcmd -S localhost,14330 -U sa -P <pass> -C -d LAMAMedellinContable -h -1 -W -i inventario-esquema.sql > local.txt

   3. Comparar:
        diff prod.txt local.txt

 Salida identica = sin drift. Cualquier diferencia debe resolverse antes de
 registrar la baseline con baseline-registrar-en-produccion.sql.

 Excluye __EFMigrationsHistory a proposito: es metadata de EF, no del modelo.
================================================================================
*/

SET NOCOUNT ON;

PRINT '### TABLAS+COLUMNAS';
SELECT t.name COLLATE DATABASE_DEFAULT + '|' + c.name COLLATE DATABASE_DEFAULT + '|' + ty.name COLLATE DATABASE_DEFAULT
     + '|len=' + CAST(c.max_length AS VARCHAR)
     + '|prec=' + CAST(c.precision AS VARCHAR)
     + '|scale=' + CAST(c.scale AS VARCHAR)
     + '|null=' + CAST(c.is_nullable AS VARCHAR)
     + '|ident=' + CAST(c.is_identity AS VARCHAR)
FROM sys.tables t
JOIN sys.columns c ON c.object_id = t.object_id
JOIN sys.types ty ON ty.user_type_id = c.user_type_id
WHERE t.name <> '__EFMigrationsHistory'
ORDER BY t.name, c.name;

PRINT '### INDICES';
SELECT t.name COLLATE DATABASE_DEFAULT + '|' + ISNULL(i.name COLLATE DATABASE_DEFAULT, '(heap)')
     + '|unique=' + CAST(i.is_unique AS VARCHAR)
     + '|pk=' + CAST(i.is_primary_key AS VARCHAR)
     + '|cols=' + ISNULL(STUFF((
         SELECT ',' + c2.name COLLATE DATABASE_DEFAULT + CASE WHEN ic2.is_descending_key = 1 THEN ' DESC' ELSE '' END
         FROM sys.index_columns ic2
         JOIN sys.columns c2 ON c2.object_id = ic2.object_id AND c2.column_id = ic2.column_id
         WHERE ic2.object_id = i.object_id AND ic2.index_id = i.index_id AND ic2.is_included_column = 0
         ORDER BY ic2.key_ordinal
         FOR XML PATH('')), 1, 1, ''), '-')
     + '|filtro=' + ISNULL(i.filter_definition COLLATE DATABASE_DEFAULT, '-')
FROM sys.indexes i
JOIN sys.tables t ON t.object_id = i.object_id
WHERE t.name <> '__EFMigrationsHistory' AND i.type <> 0
ORDER BY t.name, i.name;

PRINT '### FOREIGN KEYS';
SELECT fk.name COLLATE DATABASE_DEFAULT + '|' + tp.name COLLATE DATABASE_DEFAULT + '->' + tr.name COLLATE DATABASE_DEFAULT
     + '|del=' + fk.delete_referential_action_desc COLLATE DATABASE_DEFAULT
     + '|upd=' + fk.update_referential_action_desc COLLATE DATABASE_DEFAULT
     + '|cols=' + ISNULL(STUFF((
         SELECT ',' + cp.name COLLATE DATABASE_DEFAULT
         FROM sys.foreign_key_columns fkc2
         JOIN sys.columns cp ON cp.object_id = fkc2.parent_object_id AND cp.column_id = fkc2.parent_column_id
         WHERE fkc2.constraint_object_id = fk.object_id
         ORDER BY fkc2.constraint_column_id
         FOR XML PATH('')), 1, 1, ''), '-')
FROM sys.foreign_keys fk
JOIN sys.tables tp ON tp.object_id = fk.parent_object_id
JOIN sys.tables tr ON tr.object_id = fk.referenced_object_id
ORDER BY fk.name;

PRINT '### DEFAULTS';
SELECT t.name COLLATE DATABASE_DEFAULT + '|' + c.name COLLATE DATABASE_DEFAULT + '|'
     + REPLACE(REPLACE(dc.definition COLLATE DATABASE_DEFAULT, ' ', ''), 'N''', '''')
FROM sys.default_constraints dc
JOIN sys.tables t ON t.object_id = dc.parent_object_id
JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
ORDER BY t.name, c.name;

PRINT '### CHECK CONSTRAINTS';
SELECT t.name COLLATE DATABASE_DEFAULT + '|' + cc.name COLLATE DATABASE_DEFAULT + '|'
     + REPLACE(cc.definition COLLATE DATABASE_DEFAULT, ' ', '')
FROM sys.check_constraints cc
JOIN sys.tables t ON t.object_id = cc.parent_object_id
ORDER BY t.name, cc.name;
