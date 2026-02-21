# Backend Automation Scripts

Scripts para inicializar, validar y mantener la infraestructura de desarrollo del backend LAMAMedellin.

## init-backend.ps1

Script de inicialización completa del backend con arquitectura limpia (.NET 8).

### Descripción

Crea automáticamente:

- Solución `LAMAMedellin.slnx`
- 4 proyectos siguiendo Clean Architecture
- Referencias correctas entre capas
- Estructura de carpetas organizadas
- Validación de build

### Requisitos Previos

- .NET 8 SDK o superior (`dotnet --version`)
- PowerShell 5.1 o superior
- Acceso a internet (para descargar templates de dotnet)

### Uso

**Ejecución básica:**

```powershell
cd c:\ruta\a\"Sistema Contable L.A.M.A. Medellin"
powershell -ExecutionPolicy Bypass -File .\src\scripts\init-backend.ps1
```

**Con opciones personalizadas:**

```powershell
# Especificar ubicación y nombre
powershell -ExecutionPolicy Bypass -File .\src\scripts\init-backend.ps1 `
  -SolutionPath "C:\Projects" `
  -SolutionName "LAMAMedellin"

# Forzar recreación si existe
powershell -ExecutionPolicy Bypass -File .\src\scripts\init-backend.ps1 -Force

# Vista previa sin crear nada
powershell -ExecutionPolicy Bypass -File .\src\scripts\init-backend.ps1 -WhatIf
```

### Parámetros

| Parámetro | Tipo | Default | Descripción |
|-----------|------|---------|-------------|
| `-SolutionPath` | String | `.\` | Directorio donde se creará la solución |
| `-SolutionName` | String | `LAMAMedellin` | Nombre de la solución |
| `-Force` | Switch | $false | Sobrescribir estructura existente |
| `-WhatIf` | Switch | $false | Mostrar acciones sin ejecutarlas (preview) |

### Estructura Generada

```
src/
├── LAMAMedellin.Domain/              (Capa de Dominio)
│   ├── Entities/                     (Entidades del negocio)
│   ├── ValueObjects/                 (Objetos de valor)
│   ├── Interfaces/                   (Contratos de dominio)
│   └── Constants/                    (Constantes de negocio)
│
├── LAMAMedellin.Application/         (Capa de Aplicación)
│   ├── Commands/                     (Comandos CQRS)
│   ├── Queries/                      (Consultas CQRS)
│   ├── DTOs/                         (Data Transfer Objects)
│   ├── Interfaces/                   (Contratos de aplicación)
│   ├── Validators/                   (Validadores con FluentValidation)
│   └── Mappings/                     (AutoMapper profiles)
│
├── LAMAMedellin.Infrastructure/      (Capa de Infraestructura)
│   ├── Persistence/                  (EF Core, DbContext)
│   ├── Services/                     (Servicios externos)
│   ├── Configuration/                (Configuraciones)
│   └── Seeders/                      (Semillas de datos)
│
└── LAMAMedellin.API/                 (Capa de API)
    ├── Controllers/                  (Endpoints HTTP)
    ├── Middleware/                   (Pipeline de request)
    └── Extensions/                   (Extensiones de DI)

LAMAMedellin.slnx                      (Archivo de solución)
```

### Dependencias de Capas (Clean Architecture)

```
API
├── Application  ────────────────┐
│                               │
└──────────────────────────────► Infrastructure
                                │
                                Domain
```

**Reglas de Dependencias:**

- API puede referenciar: Application, Infrastructure
- Application puede referenciar: Domain
- Infrastructure puede referenciar: Domain
- Domain no referencia nada (solo interfaces propias)

### Ejemplo de Ejecución

```powershell
PS C:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin> powershell -ExecutionPolicy Bypass -File .\src\scripts\init-backend.ps1

21:45:30 [INFO] LAMAMedellin Backend Initialization Script
Clean Architecture (.NET 8) - 2026-02-20 21:45:30

21:45:30 [INFO] Checking prerequisites...
21:45:31 [SUCCESS] .NET 8.0.500 found
21:45:31 [INFO] Creating solution directory structure...
21:45:31 [SUCCESS] Solution created: LAMAMedellin.slnx
21:45:31 [INFO] Creating projects...
21:45:32 [INFO] Creating project: LAMAMedellin.Domain (Domain entities and business logic)
21:45:33 [SUCCESS] Project created: LAMAMedellin.Domain
21:45:33 [INFO] Creating project: LAMAMedellin.Application (Application services and use cases (CQRS))
21:45:35 [SUCCESS] Project created: LAMAMedellin.Application
21:45:35 [INFO] Creating project: LAMAMedellin.Infrastructure (Data access and external service integration)
21:45:37 [SUCCESS] Project created: LAMAMedellin.Infrastructure
21:45:37 [INFO] Creating project: LAMAMedellin.API (HTTP API endpoints with controllers)
21:45:39 [SUCCESS] Project created: LAMAMedellin.API
21:45:40 [INFO] Adding project references...
...
21:46:15 [SUCCESS] Build successful

======================================================================
[OK] Backend initialization completed
======================================================================

Solution Structure:
  Location: C:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin
  Solution: LAMAMedellin.slnx
...
```

### Solución de Problemas

**Error: "dotnet SDK not found"**

- Instalar .NET 8 SDK desde: <https://dotnet.microsoft.com/download>

**Error: "Build failed"**

- Verificar que .NET 8 está instalado correctamente
- Ejecutar: `dotnet --version`
- Revisar el output de error en consola

**Error: "Solution already exists"**

- Usar `-Force` para sobrescribir
- O eliminar manualmente el directorio `src/` y archivo `.slnx`

### Siguientes Pasos

1. Inicializar base de datos (Entity Framework)
2. Configurar Dependency Injection
3. Implementar primera entidad de dominio
4. Crear primer use case (Command/Handler)
5. Generar repositorios por Aggregate Root

Consultar: [docs/guides/LOCAL-SETUP.md](../docs/guides/LOCAL-SETUP.md)

### Convenciones Aplicadas

Ver [governance/process-docs/CODE-STANDARDS.md](../governance/process-docs/CODE-STANDARDS.md) para:

- Convenciones de nombres (Spanish/English)
- Patrones de arquitectura
- Estándares de código
- Reglas de seguridad

---

**Última actualización:** Febrero 2026
**Autor:** LAMA Medellín Development Team
**Estado:** Production Ready
