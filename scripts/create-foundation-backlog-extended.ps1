param(
  [string]$Repo = "CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin"
)

$ErrorActionPreference = "Stop"

function Assert-GhReady {
  gh --version *> $null
  if ($LASTEXITCODE -ne 0) {
    throw "GitHub CLI no está disponible. Instala gh."
  }

  gh auth status *> $null
  if ($LASTEXITCODE -ne 0) {
    throw "No hay sesión activa de GitHub CLI. Ejecuta: gh auth login"
  }
}

function New-Issue {
  param(
    [string]$Title,
    [string]$Body,
    [string[]]$Labels
  )

  $labelArg = ($Labels | Where-Object { $_ -and $_.Trim() -ne "" }) -join ","
  if ($labelArg -ne "") {
    return gh issue create --repo $Repo --title $Title --body $Body --label $labelArg
  }

  return gh issue create --repo $Repo --title $Title --body $Body
}

function Ensure-Label {
  param(
    [string]$Name,
    [string]$Color = "1f6feb",
    [string]$Description = ""
  )

  try {
    gh label create $Name --repo $Repo --color $Color --description $Description 2>$null | Out-Null
  }
  catch {}
}

Assert-GhReady

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
  @{n="area:activos"; c="bfd4f2"; d="Activos fijos"},
  @{n="area:presupuesto"; c="f9d0c4"; d="Presupuesto"},
  @{n="area:donaciones"; c="c2e0c6"; d="Donaciones"},
  @{n="area:proyectos"; c="006b75"; d="Convenios/proyectos"},
  @{n="area:reportes"; c="7057ff"; d="Reportes"},
  @{n="area:devops"; c="24292f"; d="Plataforma/DevOps"},
  @{n="compliance:dian"; c="bfe5bf"; d="Cumplimiento DIAN"},
  @{n="compliance:rte-target"; c="fef2c0"; d="Preparación RTE"},
  @{n="audit:required"; c="c5def5"; d="Requiere evidencia de auditoría"},
  @{n="sprint:S1"; c="ededed"; d="Sprint 1"},
  @{n="sprint:S2"; c="ededed"; d="Sprint 2"},
  @{n="sprint:S3"; c="ededed"; d="Sprint 3"},
  @{n="sprint:S4"; c="ededed"; d="Sprint 4"},
  @{n="sprint:S5"; c="ededed"; d="Sprint 5"},
  @{n="sprint:S6"; c="ededed"; d="Sprint 6"}
)

Write-Host "Creando labels..."
foreach ($l in $labels) {
  Ensure-Label -Name $l.n -Color $l.c -Description $l.d
}

$epics = @(
  @{
    key="E1"; title="[EPIC] Seguridad, roles y auditoría inmutable";
    labels=@("type:epic","priority:P0","area:seguridad","audit:required");
    body=@"
Objetivo:
Implementar control de acceso por roles (RBAC) y bitácora inmutable para toda operación crítica.

Criterios globales:
- Operaciones críticas solo con rol autorizado.
- Trazabilidad before/after, usuario, fecha/hora, origen, motivo.
- Eventos auditables no alterables por perfiles operativos.
"@
  },
  @{
    key="E2"; title="[EPIC] Núcleo contable y cierre de períodos";
    labels=@("type:epic","priority:P0","area:contabilidad");
    body=@"
Objetivo:
Garantizar integridad contable mediante asientos balanceados, comprobantes y control de períodos.

Criterios globales:
- Débito = crédito obligatorio.
- Cierre bloquea cambios.
- Reapertura controlada y auditada.
"@
  },
  @{
    key="E3"; title="[EPIC] Tesorería y conciliación bancaria";
    labels=@("type:epic","priority:P0","area:tesoreria");
    body=@"
Objetivo:
Control de ingresos/egresos/traslados y conciliación bancaria con soporte documental.
"@
  },
  @{
    key="E4"; title="[EPIC] Cartera de miembros y recaudo";
    labels=@("type:epic","priority:P0","area:cartera");
    body=@"
Objetivo:
Gestionar obligaciones, pagos, mora y antigüedad de saldos.
"@
  },
  @{
    key="E5"; title="[EPIC] Cuotas capitulares y renovación internacional";
    labels=@("type:epic","priority:P0","area:cartera","area:tesoreria","area:contabilidad");
    body=@"
Regla de negocio Fundación:
- Cuota capitular mensual: COP 25.000 (parametrizable por vigencia/asamblea).
- Renovación anual L.A.M.A. Internacional: USD 20 (parametrizable por vigencia).

Objetivo:
Automatizar obligaciones mensual/anual, pagos, conversión TRM y estado de membresía.
"@
  },
  @{
    key="E6"; title="[EPIC] Compras y cuentas por pagar";
    labels=@("type:epic","priority:P0","area:compras","area:tesoreria");
    body=@"
Objetivo:
Formalizar flujo solicitud→aprobación→OC→recepción→causación→pago.
"@
  },
  @{
    key="E7"; title="[EPIC] Reportes financieros y tributarios (DIAN) + preparación RTE";
    labels=@("type:epic","priority:P0","area:reportes","compliance:dian","compliance:rte-target");
    body=@"
Objetivo:
Reportería financiera y tributaria trazable, parametrizada por vigencia, con soporte para transición a RTE.
"@
  },
  @{
    key="E8"; title="[EPIC] Activos fijos y depreciación";
    labels=@("type:epic","priority:P1","area:activos");
    body="Objetivo: Gestionar ciclo de vida de activos y depreciación contable."
  },
  @{
    key="E9"; title="[EPIC] Presupuesto institucional y control de ejecución";
    labels=@("type:epic","priority:P1","area:presupuesto");
    body="Objetivo: Planear presupuesto y controlar desviaciones por rubro/proyecto."
  },
  @{
    key="E10"; title="[EPIC] Donaciones, convenios y proyectos";
    labels=@("type:epic","priority:P1","area:donaciones","area:proyectos");
    body="Objetivo: Trazabilidad financiera de donaciones y ejecución por proyecto."
  },
  @{
    key="E11"; title="[EPIC] Plataforma, calidad y DevOps";
    labels=@("type:epic","priority:P0","area:devops");
    body="Objetivo: CI/CD, pruebas automáticas, observabilidad, backup y recuperación."
  }
)

$epicUrl = @{}
Write-Host "Creando épicas..."
foreach ($e in $epics) {
  $u = New-Issue -Title $e.title -Body $e.body -Labels $e.labels
  $epicUrl[$e.key] = $u
  Write-Host "Epic $($e.key): $u"
}

$stories = @(
  @{e="E1"; t="[HU] Definir matriz RBAC por módulo y acción"; p="P0"; s="S1"; a=@("area:seguridad"); b="Como administrador, quiero matriz de permisos por rol/acción para aplicar mínimo privilegio."},
  @{e="E1"; t="[HU] Implementar autorización por política en API"; p="P0"; s="S1"; a=@("area:seguridad"); b="Como sistema, quiero validar permisos por endpoint para bloquear accesos indebidos."},
  @{e="E1"; t="[HU] Registrar bitácora inmutable de operaciones críticas"; p="P0"; s="S1"; a=@("area:seguridad"); b="Como revisor fiscal, quiero eventos inmutables para no repudio."},
  @{e="E1"; t="[HU] Auditar cambios de configuración y catálogos"; p="P1"; s="S2"; a=@("area:seguridad"); b="Como auditor, quiero trazabilidad de cambios de parámetros institucionales."},
  @{e="E1"; t="[HU] Exportar evidencia de auditoría por rango de fechas"; p="P1"; s="S2"; a=@("area:seguridad","area:reportes"); b="Como auditor, quiero exportar logs para revisión externa."},
  @{e="E2"; t="[HU] Administrar plan de cuentas jerárquico"; p="P0"; s="S1"; a=@("area:contabilidad"); b="Como contador, quiero plan de cuentas parametrizable por niveles."},
  @{e="E2"; t="[HU] Registrar comprobantes contables manuales"; p="P0"; s="S1"; a=@("area:contabilidad"); b="Como contador, quiero crear comprobantes con validaciones."},
  @{e="E2"; t="[HU] Validar balance débito/crédito antes de publicar"; p="P0"; s="S1"; a=@("area:contabilidad"); b="Como contador, quiero impedir asientos descuadrados."},
  @{e="E2"; t="[HU] Cerrar período contable"; p="P0"; s="S2"; a=@("area:contabilidad"); b="Como jefe contable, quiero cerrar períodos para bloquear cambios."},
  @{e="E2"; t="[HU] Reabrir período con justificación auditada"; p="P0"; s="S2"; a=@("area:contabilidad","area:seguridad"); b="Como jefe contable, quiero reapertura controlada y trazable."},
  @{e="E3"; t="[HU] Registrar ingresos de tesorería con soporte"; p="P0"; s="S1"; a=@("area:tesoreria"); b="Como tesorero, quiero registrar ingresos y adjuntar soportes."},
  @{e="E3"; t="[HU] Registrar egresos de tesorería con aprobaciones"; p="P0"; s="S2"; a=@("area:tesoreria"); b="Como tesorero, quiero registrar egresos con trazabilidad."},
  @{e="E3"; t="[HU] Gestionar traslados entre caja y bancos"; p="P1"; s="S2"; a=@("area:tesoreria"); b="Como tesorero, quiero trasladar fondos entre cuentas con control."},
  @{e="E3"; t="[HU] Conciliación bancaria de movimientos"; p="P1"; s="S3"; a=@("area:tesoreria"); b="Como revisor fiscal, quiero conciliar extractos y movimientos internos."},
  @{e="E3"; t="[HU] Generar reporte de flujo de caja"; p="P1"; s="S3"; a=@("area:tesoreria","area:reportes"); b="Como dirección, quiero flujo de caja por período."},
  @{e="E4"; t="[HU] Crear obligaciones de cartera por tercero y concepto"; p="P0"; s="S1"; a=@("area:cartera"); b="Como cartera, quiero registrar obligaciones por miembro/concepto."},
  @{e="E4"; t="[HU] Aplicar pagos parciales de cartera"; p="P0"; s="S2"; a=@("area:cartera","area:tesoreria"); b="Como cajero, quiero aplicar pagos parciales y actualizar saldo."},
  @{e="E4"; t="[HU] Aplicar pagos totales y cerrar obligación"; p="P0"; s="S2"; a=@("area:cartera","area:tesoreria"); b="Como cajero, quiero cerrar obligación con pago total."},
  @{e="E4"; t="[HU] Consultar antigüedad de cartera (aging)"; p="P1"; s="S3"; a=@("area:cartera","area:reportes"); b="Como gerencia, quiero ver mora por rangos de antigüedad."},
  @{e="E4"; t="[HU] Alertar vencimientos de cartera"; p="P2"; s="S4"; a=@("area:cartera"); b="Como cartera, quiero alertas preventivas de vencimiento."},
  @{e="E5"; t="[HU] Parametrizar cuota capitular mensual por vigencia (COP)"; p="P0"; s="S1"; a=@("area:cartera"); b="Como tesorería, quiero definir valor mensual por vigencia (inicial COP 25.000)."},
  @{e="E5"; t="[HU] Parametrizar renovación internacional anual por vigencia (USD)"; p="P0"; s="S1"; a=@("area:cartera","area:tesoreria"); b="Como tesorería, quiero definir valor anual internacional (inicial USD 20)."},
  @{e="E5"; t="[HU] Generar obligaciones mensuales automáticas de cuota capitular"; p="P0"; s="S2"; a=@("area:cartera"); b="Como sistema, quiero generar automáticamente cuotas mensuales a miembros activos."},
  @{e="E5"; t="[HU] Generar obligaciones anuales automáticas de renovación internacional"; p="P0"; s="S2"; a=@("area:cartera"); b="Como sistema, quiero generar renovaciones anuales para miembros activos."},
  @{e="E5"; t="[HU] Aplicar TRM y almacenarla por transacción de renovación USD"; p="P0"; s="S2"; a=@("area:contabilidad","area:tesoreria"); b="Como contabilidad, quiero conservar la TRM usada en cada pago USD."},
  @{e="E5"; t="[HU] Determinar estado de membresía por cumplimiento de cuotas"; p="P1"; s="S3"; a=@("area:cartera","area:reportes"); b="Como secretaría, quiero saber estado vigente/suspendido por mora."},
  @{e="E5"; t="[HU] Reportar recaudo mensual y anual de membresías"; p="P1"; s="S3"; a=@("area:reportes"); b="Como dirección, quiero reporte consolidado de cuotas y renovaciones."},
  @{e="E6"; t="[HU] Registrar solicitud de compra"; p="P0"; s="S2"; a=@("area:compras"); b="Como administración, quiero iniciar flujo de compra con requerimiento formal."},
  @{e="E6"; t="[HU] Aprobar/rechazar solicitud de compra"; p="P0"; s="S2"; a=@("area:compras"); b="Como aprobador, quiero autorizar solicitudes según política."},
  @{e="E6"; t="[HU] Emitir orden de compra y recepción"; p="P1"; s="S3"; a=@("area:compras"); b="Como compras, quiero formalizar OC y recepción de bienes/servicios."},
  @{e="E6"; t="[HU] Causar factura de proveedor"; p="P0"; s="S3"; a=@("area:compras","area:contabilidad"); b="Como contabilidad, quiero causar facturas y crear CxP."},
  @{e="E6"; t="[HU] Programar pago de cuentas por pagar"; p="P1"; s="S4"; a=@("area:tesoreria","area:compras"); b="Como tesorería, quiero calendarizar pagos por vencimiento."},
  @{e="E7"; t="[HU] Generar balance general por período"; p="P0"; s="S3"; a=@("area:reportes","area:contabilidad"); b="Como contador, quiero balance general exportable."},
  @{e="E7"; t="[HU] Generar estado de resultados por período"; p="P0"; s="S3"; a=@("area:reportes","area:contabilidad"); b="Como contador, quiero estado de resultados confiable."},
  @{e="E7"; t="[HU] Generar auxiliares contables y mayores"; p="P1"; s="S4"; a=@("area:reportes","area:contabilidad"); b="Como contador, quiero auxiliares por cuenta/tercero."},
  @{e="E7"; t="[HU] Parametrizar reportes tributarios por vigencia"; p="P0"; s="S4"; a=@("area:reportes"); b="Como responsable tributario, quiero ajustar estructura según vigencia DIAN."},
  @{e="E7"; t="[HU] Matriz de transición a RTE con evidencias"; p="P1"; s="S5"; a=@("area:reportes"); b="Como dirección, quiero checklist trazable para transición a RTE."},
  @{e="E8"; t="[HU] Registrar alta de activo fijo"; p="P1"; s="S4"; a=@("area:activos"); b="Como activos, quiero alta con costo, vida útil, cuenta y soporte."},
  @{e="E8"; t="[HU] Registrar baja o retiro de activo"; p="P1"; s="S5"; a=@("area:activos"); b="Como activos, quiero baja con causal y evidencia."},
  @{e="E8"; t="[HU] Calcular depreciación periódica"; p="P1"; s="S5"; a=@("area:activos","area:contabilidad"); b="Como contador, quiero depreciación automática parametrizable."},
  @{e="E8"; t="[HU] Reporte de activos y depreciación acumulada"; p="P1"; s="S5"; a=@("area:activos","area:reportes"); b="Como dirección, quiero visión patrimonial consolidada."},
  @{e="E9"; t="[HU] Crear presupuesto anual por rubros"; p="P1"; s="S4"; a=@("area:presupuesto"); b="Como gerencia, quiero presupuesto por rubros y centros."},
  @{e="E9"; t="[HU] Versionar y aprobar presupuesto vigente"; p="P1"; s="S4"; a=@("area:presupuesto"); b="Como dirección, quiero versiones aprobadas con historial."},
  @{e="E9"; t="[HU] Ejecutado vs presupuesto"; p="P1"; s="S5"; a=@("area:presupuesto","area:reportes"); b="Como finanzas, quiero comparar ejecución vs plan."},
  @{e="E9"; t="[HU] Alertas de sobre-ejecución"; p="P2"; s="S6"; a=@("area:presupuesto"); b="Como gerencia, quiero alertas al superar umbrales."},
  @{e="E10"; t="[HU] Registrar donación en dinero"; p="P1"; s="S4"; a=@("area:donaciones"); b="Como donaciones, quiero registrar aportes monetarios con soporte."},
  @{e="E10"; t="[HU] Registrar donación en especie"; p="P1"; s="S5"; a=@("area:donaciones"); b="Como donaciones, quiero registrar aportes en especie valorizados."},
  @{e="E10"; t="[HU] Configurar restricciones de uso de fondos"; p="P1"; s="S5"; a=@("area:donaciones","area:proyectos"); b="Como dirección, quiero restringir uso de ciertos recursos."},
  @{e="E10"; t="[HU] Crear convenio/proyecto con presupuesto"; p="P1"; s="S5"; a=@("area:proyectos"); b="Como proyectos, quiero crear convenios con objetivos y presupuesto."},
  @{e="E10"; t="[HU] Imputar gastos/ingresos por proyecto"; p="P1"; s="S6"; a=@("area:proyectos","area:reportes"); b="Como proyectos, quiero trazabilidad de ejecución financiera por convenio."},
  @{e="E11"; t="[HU] Pipeline CI con build y pruebas"; p="P0"; s="S1"; a=@("area:devops"); b="Como equipo técnico, quiero pipeline que valide build y tests en cada PR."},
  @{e="E11"; t="[HU] Quality gate para merge"; p="P0"; s="S1"; a=@("area:devops"); b="Como equipo técnico, quiero bloquear merge si falla quality gate."},
  @{e="E11"; t="[HU] Estandarizar logging estructurado"; p="P1"; s="S2"; a=@("area:devops"); b="Como operaciones, quiero logs consultables y consistentes."},
  @{e="E11"; t="[HU] Monitoreo y alertas de disponibilidad/latencia"; p="P1"; s="S3"; a=@("area:devops"); b="Como operaciones, quiero alertas proactivas ante degradación."},
  @{e="E11"; t="[HU] Política de backup y prueba de restauración"; p="P0"; s="S3"; a=@("area:devops","audit:required"); b="Como auditor, quiero evidencia periódica de restauración exitosa."}
)

Write-Host "Creando historias..."
foreach ($st in $stories) {
  $priorityLabel = "priority:$($st.p)"
  $sprintLabel = "sprint:$($st.s)"
  $storyLabels = @("type:story", $priorityLabel, $sprintLabel) + $st.a

  $body = @"
Como usuario del rol indicado, quiero esta capacidad para mejorar control operativo y cumplimiento.

Descripción:
$($st.b)

Criterios de aceptación:
- Implementación funcional en UI + API (cuando aplique).
- Persistencia y validaciones de negocio.
- Registro de auditoría en operaciones críticas.
- Pruebas mínimas (unitarias/integración) para caso feliz y validaciones.

---
Epic relacionada: $($epicUrl[$st.e])
"@

  $u = New-Issue -Title $st.t -Body $body -Labels $storyLabels
  Write-Host "Story ($($st.e)): $u"
}

Write-Host "Backlog extendido creado con éxito."
