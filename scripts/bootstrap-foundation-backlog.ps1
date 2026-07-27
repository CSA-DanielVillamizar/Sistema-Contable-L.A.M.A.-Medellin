# scripts/bootstrap-foundation-backlog.ps1
# Uso:
#   gh auth login
#   pwsh ./scripts/bootstrap-foundation-backlog.ps1
#
# Repo y fechas base
$Repo = "CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin"
$StartDate = Get-Date "2026-07-27"

function New-Issue {
  param([string]$Title,[string]$Body,[string[]]$Labels,[string]$Milestone)
  $labelArg = ($Labels | Where-Object { $_ -and $_.Trim() -ne "" }) -join ","
  $args = @("issue","create","--repo",$Repo,"--title",$Title,"--body",$Body)
  if ($labelArg -ne "") { $args += @("--label",$labelArg) }
  if ($Milestone -and $Milestone.Trim() -ne "") { $args += @("--milestone",$Milestone) }
  $out = gh @args
  return $out
}

function Ensure-Label {
  param([string]$Name,[string]$Color,[string]$Description)
  try { gh label create $Name --repo $Repo --color $Color --description $Description 2>$null | Out-Null } catch {}
}

function Ensure-Milestone {
  param([string]$Title,[datetime]$DueDate,[string]$Description)
  $exists = gh api "repos/$Repo/milestones?state=all&per_page=100" | ConvertFrom-Json | Where-Object { $_.title -eq $Title }
  if (-not $exists) {
    gh api "repos/$Repo/milestones" -X POST -f "title=$Title" -f "state=open" -f "description=$Description" -f "due_on=$($DueDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))" | Out-Null
  }
}

Write-Host "==> Creando labels..."
$labels = @(
  @{n="type:epic"; c="5319e7"; d="Epic"},
  @{n="type:story"; c="0e8a16"; d="User story"},
  @{n="priority:P0"; c="b60205"; d="Crítico"},
  @{n="priority:P1"; c="d93f0b"; d="Alto"},
  @{n="priority:P2"; c="fbca04"; d="Medio"},
  @{n="area:seguridad"; c="0052cc"; d="Seguridad"},
  @{n="area:contabilidad"; c="0366d6"; d="Contabilidad"},
  @{n="area:tesoreria"; c="1d76db"; d="Tesorería"},
  @{n="area:cartera"; c="0e8a16"; d="Cartera"},
  @{n="area:compras"; c="5319e7"; d="Compras"},
  @{n="area:activos"; c="bfd4f2"; d="Activos"},
  @{n="area:presupuesto"; c="f9d0c4"; d="Presupuesto"},
  @{n="area:donaciones"; c="c2e0c6"; d="Donaciones"},
  @{n="area:proyectos"; c="006b75"; d="Proyectos"},
  @{n="area:reportes"; c="7057ff"; d="Reportes"},
  @{n="area:devops"; c="24292f"; d="DevOps"},
  @{n="compliance:dian"; c="bfe5bf"; d="Cumplimiento DIAN"},
  @{n="compliance:rte-target"; c="fef2c0"; d="Preparación RTE"},
  @{n="audit:required"; c="c5def5"; d="Evidencia de auditoría"},
  @{n="sprint:S1"; c="ededed"; d="Sprint 1"},
  @{n="sprint:S2"; c="ededed"; d="Sprint 2"},
  @{n="sprint:S3"; c="ededed"; d="Sprint 3"},
  @{n="sprint:S4"; c="ededed"; d="Sprint 4"},
  @{n="sprint:S5"; c="ededed"; d="Sprint 5"},
  @{n="sprint:S6"; c="ededed"; d="Sprint 6"}
)
$labels | ForEach-Object { Ensure-Label -Name $_.n -Color $_.c -Description $_.d }

Write-Host "==> Creando milestones Sprint 1..6 (2 semanas c/u desde 2026-07-27)..."
for ($i=1; $i -le 6; $i++) {
  $due = $StartDate.AddDays(14*$i)
  Ensure-Milestone -Title "Sprint $i" -DueDate $due -Description "Sprint $i - Fundación L.A.M.A. Medellín"
}

Write-Host "==> Creando épicas..."
$epics = @(
  @{k="E1"; t="[EPIC] Seguridad, roles y auditoría inmutable"; l=@("type:epic","priority:P0","area:seguridad","audit:required"); m="Sprint 1";
    b="RBAC + bitácora inmutable para operaciones críticas (contables/financieras)."},
  @{k="E2"; t="[EPIC] Núcleo contable y cierre de períodos"; l=@("type:epic","priority:P0","area:contabilidad"); m="Sprint 1";
    b="Plan de cuentas, comprobantes, asientos balanceados, cierre/reapertura auditada."},
  @{k="E3"; t="[EPIC] Tesorería y conciliación bancaria"; l=@("type:epic","priority:P0","area:tesoreria"); m="Sprint 1";
    b="Ingresos/egresos/traslados y conciliación bancaria con soporte."},
  @{k="E4"; t="[EPIC] Cartera de miembros y recaudo"; l=@("type:epic","priority:P0","area:cartera"); m="Sprint 1";
    b="Obligaciones, pagos parciales/totales, mora, aging."},
  @{k="E5"; t="[EPIC] Cuotas capitulares y renovación internacional"; l=@("type:epic","priority:P0","area:cartera","area:tesoreria","area:contabilidad"); m="Sprint 1";
    b="Cuota mensual COP 25.000 y renovación anual USD 20, ambas parametrizables por vigencia."},
  @{k="E6"; t="[EPIC] Compras y cuentas por pagar"; l=@("type:epic","priority:P0","area:compras","area:tesoreria"); m="Sprint 2"; b="Flujo de compra y CxP."},
  @{k="E7"; t="[EPIC] Reportes financieros y tributarios (DIAN) + preparación RTE"; l=@("type:epic","priority:P0","area:reportes","compliance:dian","compliance:rte-target"); m="Sprint 3"; b="Reportes parametrizables y trazables."},
  @{k="E8"; t="[EPIC] Activos fijos y depreciación"; l=@("type:epic","priority:P1","area:activos"); m="Sprint 4"; b="Ciclo de vida activos + depreciación."},
  @{k="E9"; t="[EPIC] Presupuesto institucional y control de ejecución"; l=@("type:epic","priority:P1","area:presupuesto"); m="Sprint 4"; b="Plan y control presupuestal."},
  @{k="E10"; t="[EPIC] Donaciones, convenios y proyectos"; l=@("type:epic","priority:P1","area:donaciones","area:proyectos"); m="Sprint 5"; b="Fondos y ejecución por proyecto."},
  @{k="E11"; t="[EPIC] Plataforma, calidad y DevOps"; l=@("type:epic","priority:P0","area:devops"); m="Sprint 1"; b="CI/CD, calidad y operación."}
)

$epicUrl = @{}
foreach ($e in $epics) {
  $body = @"
Objetivo:
$($e.b)

Contexto Fundación:
- ESAL Fundación L.A.M.A. Medellín
- Régimen actual ordinario (objetivo RTE)
- Obligaciones DIAN activas
- Auditoría externa/revisor fiscal con bitácora inmutable
"@
  $u = New-Issue -Title $e.t -Body $body -Labels $e.l -Milestone $e.m
  $epicUrl[$e.k] = $u
  Write-Host "Epic $($e.k): $u"
}

Write-Host "==> Creando historias extendidas..."
$stories = @(
  @{e="E1"; t="[HU] Definir matriz RBAC por módulo y acción"; p="P0"; s="S1"; a=@("area:seguridad"); d="Matriz de permisos completa por rol/acción."},
  @{e="E1"; t="[HU] Implementar autorización por políticas en API"; p="P0"; s="S1"; a=@("area:seguridad"); d="Validación de permisos en endpoints críticos."},
  @{e="E1"; t="[HU] Registrar bitácora inmutable de operaciones críticas"; p="P0"; s="S1"; a=@("area:seguridad","audit:required"); d="Evento with before/after, actor, timestamp, origen."},
  @{e="E1"; t="[HU] Auditar cambios de parámetros y catálogos"; p="P1"; s="S2"; a=@("area:seguridad"); d="Registro auditable de cambios de configuración."},
  @{e="E1"; t="[HU] Exportar evidencia de auditoría por rango"; p="P1"; s="S2"; a=@("area:reportes","area:seguridad"); d="Exportación para revisión externa."},
  @{e="E2"; t="[HU] Administrar plan de cuentas jerárquico"; p="P0"; s="S1"; a=@("area:contabilidad"); d="CRUD plan de cuentas con validaciones de naturaleza."},
  @{e="E2"; t="[HU] Registrar comprobantes contables"; p="P0"; s="S1"; a=@("area:contabilidad"); d="Comprobantes manuales con validaciones."},
  @{e="E2"; t="[HU] Validar balance débito/crédito antes de publicar"; p="P0"; s="S1"; a=@("area:contabilidad"); d="Bloqueo de publicación si descuadra."},
  @{e="E2"; t="[HU] Cerrar período contable"; p="P0"; s="S2"; a=@("area:contabilidad"); d="Bloqueo de modificaciones sobre período cerrado."},
  @{e="E2"; t="[HU] Reabrir período con motivo y aprobación"; p="P0"; s="S2"; a=@("area:contabilidad","area:seguridad"); d="Reapertura controlada y auditable."},
  @{e="E3"; t="[HU] Registrar ingresos con soporte"; p="P0"; s="S1"; a=@("area:tesoreria"); d="Ingreso con cuenta, concepto, valor, fecha, soporte."},
  @{e="E3"; t="[HU] Registrar egresos con aprobaciones"; p="P0"; s="S2"; a=@("area:tesoreria"); d="Egreso con control de aprobación."},
  @{e="E3"; t="[HU] Gestionar traslados entre cuentas"; p="P1"; s="S2"; a=@("area:tesoreria"); d="Transferencias internas trazables."},
  @{e="E3"; t="[HU] Conciliación bancaria de movimientos"; p="P1"; s="S3"; a=@("area:tesoreria"); d="Estado conciliado/no conciliado + diferencias."},
  @{e="E3"; t="[HU] Reporte de flujo de caja"; p="P1"; s="S3"; a=@("area:reportes","area:tesoreria"); d="Reporte por período y cuenta."},
  @{e="E4"; t="[HU] Crear obligaciones de cartera por tercero/concepto"; p="P0"; s="S1"; a=@("area:cartera"); d="Cuentas por cobrar con estado inicial."},
  @{e="E4"; t="[HU] Aplicar pagos parciales de cartera"; p="P0"; s="S2"; a=@("area:cartera","area:tesoreria"); d="Recalcula saldo y estado."},
  @{e="E4"; t="[HU] Aplicar pagos totales y cerrar obligación"; p="P0"; s="S2"; a=@("area:cartera","area:tesoreria"); d="Marca obligación como pagada."},
  @{e="E4"; t="[HU] Consultar aging de cartera"; p="P1"; s="S3"; a=@("area:reportes","area:cartera"); d="Antigüedad por rangos de mora."},
  @{e="E4"; t="[HU] Alertas de vencimiento de cartera"; p="P2"; s="S4"; a=@("area:cartera"); d="Notificación de próximas/moras."},
  @{e="E5"; t="[HU] Parametrizar cuota capitular mensual por vigencia (COP)"; p="P0"; s="S1"; a=@("area:cartera"); d="Valor inicial COP 25.000, editable por vigencia sin afectar históricos."},
  @{e="E5"; t="[HU] Parametrizar renovación anual internacional por vigencia (USD)"; p="P0"; s="S1"; a=@("area:cartera","area:tesoreria"); d="Valor inicial USD 20, editable por vigencia sin afectar históricos."},
  @{e="E5"; t="[HU] Generar obligaciones mensuales automáticas de cuota capitular"; p="P0"; s="S2"; a=@("area:cartera"); d="Generación mensual para miembros activos."},
  @{e="E5"; t="[HU] Generar obligaciones anuales de renovación internacional"; p="P0"; s="S2"; a=@("area:cartera"); d="Generación anual para miembros activos."},
  @{e="E5"; t="[HU] Aplicar y almacenar TRM por pago internacional"; p="P0"; s="S2"; a=@("area:tesoreria","area:contabilidad"); d="Guardar tasa aplicada por transacción USD."},
  @{e="E5"; t="[HU] Actualizar estado de membresía según cumplimiento"; p="P1"; s="S3"; a=@("area:cartera","area:reportes"); d="Vigente/suspendido por reglas definidas."},
  @{e="E5"; t="[HU] Reporte consolidado de cuotas y renovaciones"; p="P1"; s="S3"; a=@("area:reportes"); d="Mensual/anual por miembro/vigencia."},
  @{e="E6"; t="[HU] Registrar solicitud de compra"; p="P0"; s="S2"; a=@("area:compras"); d="Inicio formal del flujo de compras."},
  @{e="E6"; t="[HU] Aprobar/rechazar solicitud de compra"; p="P0"; s="S2"; a=@("area:compras"); d="Control por rol y política."},
  @{e="E6"; t="[HU] Emitir orden de compra y recepción"; p="P1"; s="S3"; a=@("area:compras"); d="Formalización de adquisición."},
  @{e="E6"; t="[HU] Causar factura de proveedor"; p="P0"; s="S3"; a=@("area:compras","area:contabilidad"); d="Impacto CxP + asiento contable."},
  @{e="E6"; t="[HU] Programar pago de CxP"; p="P1"; s="S4"; a=@("area:tesoreria","area:compras"); d="Calendario de pagos por vencimiento."},
  @{e="E7"; t="[HU] Generar balance general por período"; p="P0"; s="S3"; a=@("area:reportes","area:contabilidad"); d="Balance exportable y trazable."},
  @{e="E7"; t="[HU] Generar estado de resultados por período"; p="P0"; s="S3"; a=@("area:reportes","area:contabilidad"); d="Reporte de resultados por período."},
  @{e="E7"; t="[HU] Generar auxiliares y mayores"; p="P1"; s="S4"; a=@("area:reportes","area:contabilidad"); d="Detalle por cuenta/tercero."},
  @{e="E7"; t="[HU] Parametrizar salidas tributarias por vigencia DIAN"; p="P0"; s="S4"; a=@("area:reportes","compliance:dian"); d="Plantillas parametrizables."},
  @{e="E7"; t="[HU] Checklist de transición a RTE con evidencias"; p="P1"; s="S5"; a=@("area:reportes","compliance:rte-target"); d="Matriz de cumplimiento RTE objetivo."},
  @{e="E8"; t="[HU] Registrar alta de activo fijo"; p="P1"; s="S4"; a=@("area:activos"); d="Datos mínimos + soporte obligatorio."},
  @{e="E8"; t="[HU] Registrar baja/retiro de activo"; p="P1"; s="S5"; a=@("area:activos"); d="Baja con causal y trazabilidad."},
  @{e="E8"; t="[HU] Ejecutar depreciación periódica"; p="P1"; s="S5"; a=@("area:activos","area:contabilidad"); d="Cálculo según política parametrizada."},
  @{e="E8"; t="[HU] Reporte de activo neto y depreciación acumulada"; p="P1"; s="S5"; a=@("area:activos","area:reportes"); d="Visión patrimonial por período."},
  @{e="E9"; t="[HU] Crear presupuesto anual por rubro"; p="P1"; s="S4"; a=@("area:presupuesto"); d="Definición base del presupuesto."},
  @{e="E9"; t="[HU] Versionar y aprobar presupuesto vigente"; p="P1"; s="S4"; a=@("area:presupuesto"); d="Control de versiones históricas."},
  @{e="E9"; t="[HU] Consultar ejecutado vs presupuesto"; p="P1"; s="S5"; a=@("area:presupuesto","area:reportes"); d="Comparación plan/ejecución."},
  @{e="E9"; t="[HU] Alertas por sobre-ejecución"; p="P2"; s="S6"; a=@("area:presupuesto"); d="Disparo de alertas por umbral."},
  @{e="E10"; t="[HU] Registrar donación en dinero"; p="P1"; s="S4"; a=@("area:donaciones"); d="Origen, monto, destino y soporte."},
  @{e="E10"; t="[HU] Registrar donación en especie"; p="P1"; s="S5"; a=@("area:donaciones"); d="Valoración y soporte de especie."},
  @{e="E10"; t="[HU] Configurar restricciones de uso de fondos"; p="P1"; s="S5"; a=@("area:donaciones","area:proyectos"); d="Reglas de destino de recursos."},
  @{e="E10"; t="[HU] Crear convenio/proyecto con presupuesto"; p="P1"; s="S5"; a=@("area:proyectos"); d="Ficha y meta financiera por proyecto."},
  @{e="E10"; t="[HU] Imputar gastos/ingresos por convenio"; p="P1"; s="S6"; a=@("area:proyectos","area:reportes"); d="Ejecución financiera por proyecto."},
  @{e="E11"; t="[HU] Pipeline CI con build/test/lint"; p="P0"; s="S1"; a=@("area:devops"); d="Validación automática por PR."},
  @{e="E11"; t="[HU] Quality gate obligatorio para merge"; p="P0"; s="S1"; a=@("area:devops"); d="Bloqueo de merge si falla calidad."},
  @{e="E11"; t="[HU] Logging estructurado y correlación"; p="P1"; s="S2"; a=@("area:devops"); d="Observabilidad transversal."},
  @{e="E11"; t="[HU] Monitoreo y alertas de latencia/disponibilidad"; p="P1"; s="S3"; a=@("area:devops"); d="Alertamiento proactivo."},
  @{e="E11"; t="[HU] Política de backup y restauración probada"; p="P0"; s="S3"; a=@("area:devops","audit:required"); d="Evidencia de recuperación exitosa."}
)

foreach ($st in $stories) {
  $milestone = "Sprint " + ($st.s -replace "S","")
  $labels = @("type:story","priority:$($st.p)","sprint:$($st.s)") + $st.a
  $body = @"
Descripción:
$($st.d)

Criterios de aceptación:
- UI/API funcional (si aplica).
- Validaciones de negocio implementadas.
- Trazabilidad en auditoría para operaciones críticas.
- Pruebas mínimas de caso feliz + validaciones.

Epic relacionada:
$($epicUrl[$st.e])
"@
  $u = New-Issue -Title $st.t -Body $body -Labels $labels -Milestone $milestone
  Write-Host "Story: $u"
}

Write-Host "✅ Backlog completo creado (épicas + historias + milestones + labels)."
