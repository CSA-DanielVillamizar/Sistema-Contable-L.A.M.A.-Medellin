# QA Scripts — Uso Local

Scripts de pruebas end-to-end y validacion manual. Solo para uso en entornos de desarrollo local, **nunca en produccion**.

## Scripts disponibles

| Script | Proposito |
|--------|-----------|
| `automation_run.ps1` | Automatizacion general de flujos |
| `automation_simple.ps1` | Automatizacion simplificada |
| `qa-final.ps1` | QA regresion final |
| `qa-master-7099.ps1` | Suite completa QA sobre puerto 7099 |
| `qa-pagos-cartera.ps1` | QA flujo pagos y cartera |
| `qa-run-7099.ps1` | Ejecutor rapido QA sobre puerto 7099 |
| `qa-venta.ps1` | QA flujo ventas/merchandising |
| `qa-conciliacion.ps1` | QA conciliacion bancaria |
| `qa-tesoreria.ps1` | QA modulo tesoreria |
| `run-e2e-happy-path.ps1` | Happy path E2E completo |
| `run-happy-path-automated.ps1` | Happy path automatizado |
| `run-test-master.ps1` | Ejecutor maestro de tests |
| `test-cartera-simple.ps1` | Test rapido de cartera |
| `test-happy-path-cartera.ps1` | Happy path cartera |
| `test-migracion.ps1` | Validacion de migraciones |
| `run-migracion-prod.ps1` | Ejecucion de migracion en produccion |
| `run-contingencia-migracion-prod.ps1` | Contingencia de migracion |
| `start-api-7099.bat` | Inicio rapido de la API en puerto 7099 |

## Prerequisitos

- PowerShell 7+ (Windows/Linux/macOS)
- API backend corriendo localmente (`dotnet run --project src/LAMAMedellin.API`)
- Variables de entorno configuradas (ver `docs/BACKEND-SETUP.md`)

## Uso

```powershell
# Ejemplo: ejecutar QA completo
powershell -ExecutionPolicy Bypass -File .\tools\qa-scripts\qa-master-7099.ps1

# Ejemplo: happy path E2E
powershell -ExecutionPolicy Bypass -File .\tools\qa-scripts\run-e2e-happy-path.ps1
```
