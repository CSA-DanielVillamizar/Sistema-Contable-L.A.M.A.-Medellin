# Guía de creación de Issues (GitHub) — Fundación / Capítulo L.A.M.A. Medellín

## 1) Objetivo
Estandarizar la creación y gestión del backlog en GitHub Issues usando:
- Prefijos en título (`EPIC:`, `STORY:`, `TASK:`, `BUG:`)
- Labels consistentes (tipo, fase, área, prioridad)
- Milestones por fase (Phase 0..5)
- Criterios de aceptación en cada STORY

---

## 2) Convención de títulos
- **Épica:** `EPIC: Phase X - <nombre>`
- **Historia:** `STORY: <acción del usuario>`
- **Tarea técnica:** `TASK: <actividad técnica concreta>`
- **Bug:** `BUG: <síntoma>`

Ejemplos:
- `EPIC: Phase 1 - Contabilidad general (PUC, comprobantes, libros, cierres)`
- `STORY: Ejecutar cierre mensual con bloqueo (valida tesorero, ejecuta contador)`
- `TASK: Agregar tabla TRM y endpoint para precargar TRM SFC`
- `BUG: Anulación permitida fuera del mes contable`

---

## 3) Milestones (crear primero)
Crear milestones (exactos):
1. `Phase 0 - Foundations`
2. `Phase 1 - MVP Contabilidad + Cuotas`
3. `Phase 2 - Donaciones`
4. `Phase 3 - Proyectos`
5. `Phase 4 - Negocios`
6. `Phase 5 - Tributario avanzado`
7. (Opcional) `Future / Backlog`

---

## 4) Labels (crear primero)

### 4.1 Labels por tipo (obligatorios)
- `epic`
- `story`
- `task`
- `bug`

### 4.2 Labels por fase
- `phase:0`
- `phase:1`
- `phase:2`
- `phase:3`
- `phase:4`
- `phase:5`

### 4.3 Labels por área (recomendados)
- `area:iam`
- `area:infra`
- `area:accounting`
- `area:treasury`
- `area:members`
- `area:donations`
- `area:projects`
- `area:business`
- `area:reports`
- (Opcional) `area:docs`

### 4.4 Labels por prioridad
- `priority:high`
- `priority:medium`
- `priority:low`

### 4.5 Colores sugeridos (opcional)
- epic: `5319e7`
- story: `1d76db`
- task: `0e8a16`
- bug: `d73a4a`
- phase:*: `c5def5` (variar si quieren)
- area:*: `bfdadc`
- priority:high `b60205`, medium `fbca04`, low `0e8a16`

---

## 5) Orden recomendado para crear el backlog
1) Crear milestones  
2) Crear labels  
3) Crear todas las EPIC (una por área/fase) y asignarlas al milestone  
4) Crear las STORY por fase, asignarlas al milestone y label `story`  
5) (Opcional) Crear TASK técnicas que descomponen una STORY  
6) En el cuerpo de cada STORY, enlazar a su EPIC con:
   - `Relates to EPIC: #<issue_number>`

---

## 6) Plantillas de cuerpo (recomendadas)

### 6.1 EPIC
Usar plantilla: `github-issue-template-epic.md` (ver docs o copia estándar)

### 6.2 STORY
Usar plantilla: `github-issue-template-story.md`

**Regla:** ninguna STORY se considera lista sin:
- Criterios de aceptación checklist
- Reglas de negocio relevantes
- Permisos (rol) claro

---

## 7) Reglas de calidad del backlog (Definition of Ready / Done)

### 7.1 Definition of Ready (DoR)
Una STORY está lista para desarrollo si:
- Tiene criterios de aceptación verificables
- Identifica roles y permisos
- Identifica datos mínimos a capturar
- Identifica impactos contables (si aplica)
- Identifica auditoría requerida (si aplica)

### 7.2 Definition of Done (DoD)
Una STORY está “hecha” si:
- Funcionalidad implementada en API + UI (si aplica)
- Pruebas mínimas (unit/integration) para reglas críticas
- Auditoría/logging implementados
- Documentación mínima actualizada
- Revisada por rol responsable (Tesorero/Contador/Gestor según aplique)

---

## 8) Mapeo rápido: qué va en cada fase
- **Phase 0:** IAM Entra, roles, infraestructura, catálogos base, import PUC, mapeos contables
- **Phase 1:** contabilidad, tesorería, cuotas, CxC/CxP, cierres, multimoneda informativa
- **Phase 2:** donaciones + campañas + certificados obligatorios
- **Phase 3:** proyectos sociales + beneficiarios + consentimiento + rendición
- **Phase 4:** negocios (inventario simple, compras/ventas, comprobante interno)
- **Phase 5:** reportes base DIAN (exógena, beneficiarios finales)

---