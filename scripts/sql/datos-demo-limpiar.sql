/*
================================================================================
 Retirar los datos de demostracion
================================================================================

 Elimina todo lo que cargo scripts/sql/datos-demo.sql, identificandolo por el
 prefijo DEMO-. No toca los catalogos base (PUC, cajas, bancos, centros de
 costo, miembros), que los siembra la aplicacion al arrancar.

   docker exec -i lama-sqlserver-dev /opt/mssql-tools18/bin/sqlcmd \
     -S localhost -U sa -P 'LamaDev!2026' -C \
     -d LAMAMedellinContable -i /ruta/datos-demo-limpiar.sql

 Borra fisicamente, no con baja logica: son datos de prueba y deben desaparecer
 por completo, no quedar ocultos por el filtro de IsDeleted.

 ADVERTENCIA: es para entornos de desarrollo y demostracion. No ejecutarlo
 contra produccion.
================================================================================
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

-- El orden respeta las claves foraneas: primero los hijos.
DELETE a
FROM AsientosContables a
JOIN Comprobantes c ON c.Id = a.ComprobanteId
WHERE c.NumeroConsecutivo LIKE 'DEMO-%';

DELETE FROM Comprobantes WHERE NumeroConsecutivo LIKE 'DEMO-%';

DELETE cpc
FROM CuentasPorCobrar cpc
JOIN ConceptosCobro cc ON cc.Id = cpc.ConceptoCobroId
WHERE cc.Nombre LIKE 'DEMO-%';

DELETE FROM ConceptosCobro WHERE Nombre LIKE 'DEMO-%';

DELETE FROM Donaciones WHERE CodigoVerificacion LIKE 'DEMO-%';
DELETE FROM Donantes WHERE NumeroDocumento LIKE 'DEMO-%';

DELETE mi
FROM MovimientosInventario mi
JOIN Productos p ON p.Id = mi.ProductoId
WHERE p.CodigoSKU LIKE 'DEMO-%';

DELETE FROM Productos WHERE CodigoSKU LIKE 'DEMO-%';

COMMIT TRANSACTION;

PRINT '';
PRINT '--- Datos de demostracion retirados ---';

SELECT 'Comprobantes DEMO' AS Concepto, COUNT(*) AS Restantes FROM Comprobantes WHERE NumeroConsecutivo LIKE 'DEMO-%'
UNION ALL SELECT 'Conceptos de cobro', COUNT(*) FROM ConceptosCobro WHERE Nombre LIKE 'DEMO-%'
UNION ALL SELECT 'Productos', COUNT(*) FROM Productos WHERE CodigoSKU LIKE 'DEMO-%'
UNION ALL SELECT 'Donantes', COUNT(*) FROM Donantes WHERE NumeroDocumento LIKE 'DEMO-%'
UNION ALL SELECT 'Donaciones', COUNT(*) FROM Donaciones WHERE CodigoVerificacion LIKE 'DEMO-%';
