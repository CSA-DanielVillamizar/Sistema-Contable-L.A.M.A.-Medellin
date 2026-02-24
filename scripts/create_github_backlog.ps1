param(
    [string]$Repo = "",
    [string]$BacklogPath = "C:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\backlog",
    [switch]$ResetCatalog,
    [switch]$WhatIf
)

$ErrorActionPreference = "Continue"

function Write-Info([string]$Message) {
    Write-Host "[INFO] $Message" -ForegroundColor Cyan
}

function Resolve-RepoFromGit {
    $origin = (& git remote get-url origin 2>$null)
    if (-not $origin) {
        throw "No se pudo detectar el repo desde git. Pasa -Repo owner/name."
    }

    if ($origin -match "github.com[:/](?<owner>[^/]+)/(?<name>[^/.]+)(\.git)?$") {
        return "$($Matches.owner)/$($Matches.name)"
    }

    throw "No se pudo parsear origin '$origin' como repo GitHub owner/name."
}

function Ensure-GhReady {
    & gh --version *> $null
    if ($LASTEXITCODE -ne 0) {
        throw "GitHub CLI (gh) no está instalado o no está en PATH."
    }

    & gh auth status *> $null
    if ($LASTEXITCODE -ne 0) {
        throw "No hay sesión activa de gh. Ejecuta: gh auth login"
    }
}

function Get-MilestoneMap {
    return [ordered]@{
        "0" = "Phase 0 - Foundations"
        "1" = "Phase 1 - MVP Contabilidad + Cuotas"
        "2" = "Phase 2 - Donaciones"
        "3" = "Phase 3 - Proyectos"
        "4" = "Phase 4 - Negocios"
        "5" = "Phase 5 - Tributario avanzado"
        "X" = "Future / Backlog"
    }
}

function Ensure-Milestones([string]$RepoName, [hashtable]$MilestoneMap) {
    $existing = & gh api "repos/$RepoName/milestones?state=all&per_page=100" | ConvertFrom-Json
    $existingTitles = @{}
    foreach ($m in $existing) { $existingTitles[$m.title] = $true }

    foreach ($title in $MilestoneMap.Values) {
        if ($existingTitles.ContainsKey($title)) {
            continue
        }

        if ($WhatIf) {
            Write-Info "[WhatIf] Crearía milestone: $title"
            continue
        }

        Write-Info "Creando milestone: $title"
        & gh api "repos/$RepoName/milestones" --method POST --field title="$title" *> $null
    }
}

function Get-LabelDefinitions {
    return @(
        @{ name = "epic"; color = "5319e7"; description = "Epic issue" },
        @{ name = "story"; color = "1d76db"; description = "Story issue" },
        @{ name = "task"; color = "0e8a16"; description = "Task issue" },
        @{ name = "bug"; color = "d73a4a"; description = "Bug issue" },

        @{ name = "phase:0"; color = "c5def5"; description = "Phase 0" },
        @{ name = "phase:1"; color = "c5def5"; description = "Phase 1" },
        @{ name = "phase:2"; color = "c5def5"; description = "Phase 2" },
        @{ name = "phase:3"; color = "c5def5"; description = "Phase 3" },
        @{ name = "phase:4"; color = "c5def5"; description = "Phase 4" },
        @{ name = "phase:5"; color = "c5def5"; description = "Phase 5" },

        @{ name = "area:iam"; color = "bfdadc"; description = "Identity and access" },
        @{ name = "area:infra"; color = "bfdadc"; description = "Infrastructure" },
        @{ name = "area:accounting"; color = "bfdadc"; description = "Accounting" },
        @{ name = "area:treasury"; color = "bfdadc"; description = "Treasury" },
        @{ name = "area:members"; color = "bfdadc"; description = "Members" },
        @{ name = "area:donations"; color = "bfdadc"; description = "Donations" },
        @{ name = "area:projects"; color = "bfdadc"; description = "Projects" },
        @{ name = "area:business"; color = "bfdadc"; description = "Business" },
        @{ name = "area:reports"; color = "bfdadc"; description = "Reports" },

        @{ name = "priority:high"; color = "b60205"; description = "High priority" },
        @{ name = "priority:medium"; color = "fbca04"; description = "Medium priority" },
        @{ name = "priority:low"; color = "0e8a16"; description = "Low priority" }
    )
}

function Ensure-Labels([string]$RepoName) {
    $existing = & gh label list --repo $RepoName --limit 200 --json name | ConvertFrom-Json
    $existingNames = @{}
    foreach ($label in $existing) { $existingNames[$label.name] = $true }

    foreach ($def in (Get-LabelDefinitions)) {
        if ($existingNames.ContainsKey($def.name)) {
            continue
        }

        if ($WhatIf) {
            Write-Info "[WhatIf] Crearía label: $($def.name)"
            continue
        }

        Write-Info "Creando label: $($def.name)"
        & gh label create $def.name --repo $RepoName --color $def.color --description $def.description *> $null
    }
}

function Get-OpenIssues([string]$Repository) {
    $all = @()
    $page = 1
    while ($true) {
        $batch = gh api "repos/$Repository/issues?state=open&per_page=100&page=$page" | ConvertFrom-Json
        $items = @($batch | Where-Object { -not $_.pull_request })
        if ($items.Count -eq 0) { break }
        $all += $items
        if ($items.Count -lt 100) { break }
        $page++
    }
    return $all
}

function Get-CanonicalTargets {
    return @(
        @{title = 'EPIC: Phase 0 - IAM Entra External ID + Roles internos + MFA'; labels = @('epic', 'phase:0', 'area:iam', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-epic-01.md' },
        @{title = 'EPIC: Phase 0 - Infra mínima Azure + Observabilidad + Key Vault + Blob'; labels = @('epic', 'phase:0', 'area:infra', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-epic-02.md' },
        @{title = 'EPIC: Phase 0 - Modelo base: Centros de costo + Medios de pago + Terceros + Mapeo contable'; labels = @('epic', 'phase:0', 'area:accounting', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-epic-03.md' },
        @{title = 'EPIC: Phase 1 - Contabilidad general (PUC, comprobantes, libros, cierres)'; labels = @('epic', 'phase:1', 'area:accounting', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-epic-04.md' },
        @{title = 'EPIC: Phase 1 - Tesorería bancarizada + recibos + anulaciones'; labels = @('epic', 'phase:1', 'area:treasury', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-epic-05.md' },
        @{title = 'EPIC: Phase 1 - Cuotas miembros + CxC + mora + recaudo'; labels = @('epic', 'phase:1', 'area:members', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-epic-06.md' },
        @{title = 'EPIC: Phase 1 - CxP proveedores (facturas, vencimientos, pagos)'; labels = @('epic', 'phase:1', 'area:accounting', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-epic-07.md' },
        @{title = 'EPIC: Phase 1 - Multimoneda informativa USD + Diferencia en cambio'; labels = @('epic', 'phase:1', 'area:accounting', 'area:treasury', 'priority:medium'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-epic-08.md' },
        @{title = 'EPIC: Phase 2 - Donaciones + campañas + certificados obligatorios'; labels = @('epic', 'phase:2', 'area:donations', 'priority:high'); milestone = 'Phase 2 - Donaciones'; file = 'issue-epic-09.md' },
        @{title = 'EPIC: Phase 3 - Proyectos sociales + beneficiarios + consentimiento + rendición'; labels = @('epic', 'phase:3', 'area:projects', 'priority:high'); milestone = 'Phase 3 - Proyectos'; file = 'issue-epic-10.md' },
        @{title = 'EPIC: Phase 4 - Negocios (inventario simple, compras/ventas, comprobante interno)'; labels = @('epic', 'phase:4', 'area:business', 'priority:medium'); milestone = 'Phase 4 - Negocios'; file = 'issue-epic-11.md' },
        @{title = 'EPIC: Phase 5 - Reportes tributarios base (exógena, beneficiarios finales)'; labels = @('epic', 'phase:5', 'area:reports', 'priority:medium'); milestone = 'Phase 5 - Tributario avanzado'; file = 'issue-epic-12.md' },
        @{title = 'EPIC: Phase X - Preparación para facturación electrónica (estructura/adapter) — NO implementar'; labels = @('epic', 'area:business', 'priority:low'); milestone = 'Future / Backlog'; file = 'issue-epic-13.md' },

        @{title = 'STORY: Integrar login OIDC con Entra External ID (Next.js + API)'; labels = @('story', 'phase:0', 'area:iam', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-story-iam-01.md' },
        @{title = 'STORY: Registrar transacciones con moneda origen USD (informativa) y COP contable'; labels = @('story', 'phase:1', 'area:accounting', 'priority:medium'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-fx-01.md' },

        @{title = 'STORY: Crear modelo de usuario interno vinculado a Entra (sin password)'; labels = @('story', 'phase:0', 'area:iam', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-story-0-2.md' },
        @{title = 'STORY: CRUD de roles internos (Admin/Operador/Tesorero/Contador/Junta) con auditoría'; labels = @('story', 'phase:0', 'area:iam', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-story-0-3.md' },
        @{title = 'STORY: Implementar matriz de permisos (RBAC) en API y UI'; labels = @('story', 'phase:0', 'area:iam', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-story-0-4.md' },
        @{title = 'STORY: Parametrizar centros de costo (CAPITULO/FUNDACION/PROYECTO/EVENTO)'; labels = @('story', 'phase:0', 'area:accounting', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-story-0-5.md' },
        @{title = 'STORY: Catálogo de medios de pago obligatorio en transacciones monetarias'; labels = @('story', 'phase:0', 'area:treasury', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-story-0-6.md' },
        @{title = 'STORY: Catálogo de tipos de afiliación y CC por defecto (CAPITULO/FUNDACION)'; labels = @('story', 'phase:0', 'area:members', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-story-0-7.md' },
        @{title = 'STORY: Parametrizar cuenta bancaria Bancolombia (única activa) y cuenta contable asociada'; labels = @('story', 'phase:0', 'area:treasury', 'priority:high'); milestone = 'Phase 0 - Foundations'; file = 'issue-story-0-8.md' },

        @{title = 'STORY: Importar PUC ESAL desde archivo entregado por contador'; labels = @('story', 'phase:1', 'area:accounting', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-1.md' },
        @{title = 'STORY: Parametrizar mapeo contable por tipo de operación'; labels = @('story', 'phase:1', 'area:accounting', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-2.md' },
        @{title = 'STORY: Registrar comprobante contable con líneas debe/haber balanceadas'; labels = @('story', 'phase:1', 'area:accounting', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-3.md' },
        @{title = 'STORY: Generar libros contables (diario, mayor, balance de prueba) en COP'; labels = @('story', 'phase:1', 'area:accounting', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-4.md' },
        @{title = 'STORY: Cierre contable mensual con bloqueo (valida Tesorero, ejecuta Contador)'; labels = @('story', 'phase:1', 'area:accounting', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-5.md' },
        @{title = 'STORY: Registrar movimiento bancario (ingreso/egreso) con soporte, CC y medio de pago'; labels = @('story', 'phase:1', 'area:treasury', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-6.md' },
        @{title = 'STORY: Emitir recibo PDF con QR y verificación pública'; labels = @('story', 'phase:1', 'area:treasury', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-7.md' },
        @{title = 'STORY: Anulación intra-mes con aprobación del Tesorero y motivo obligatorio'; labels = @('story', 'phase:1', 'area:treasury', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-8.md' },
        @{title = 'STORY: Configurar cuota anual con mes de inicio y acta soporte'; labels = @('story', 'phase:1', 'area:members', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-9.md' },
        @{title = 'STORY: Generar obligaciones mensuales de cuotas (CxC) por miembro activo'; labels = @('story', 'phase:1', 'area:members', 'area:accounting', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-10.md' },
        @{title = 'STORY: Registrar pago de cuota y aplicar a obligaciones (anticipos permitidos)'; labels = @('story', 'phase:1', 'area:members', 'area:treasury', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-11.md' },
        @{title = 'STORY: Reporte de mora y aging de cartera por miembro'; labels = @('story', 'phase:1', 'area:members', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-12.md' },
        @{title = 'STORY: Registrar factura de proveedor como CxP (pendiente) con soporte'; labels = @('story', 'phase:1', 'area:accounting', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-13.md' },
        @{title = 'STORY: Registrar pago de CxP y cruce contra obligación (impacta Banco)'; labels = @('story', 'phase:1', 'area:accounting', 'area:treasury', 'priority:high'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-14.md' },
        @{title = 'STORY: Precargar TRM SFC del día hábil aplicable para operaciones en USD'; labels = @('story', 'phase:1', 'area:accounting', 'priority:medium'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-16.md' },
        @{title = 'STORY: Generar diferencia en cambio al liquidar CxP/CxC en USD si COP difiere'; labels = @('story', 'phase:1', 'area:accounting', 'priority:medium'); milestone = 'Phase 1 - MVP Contabilidad + Cuotas'; file = 'issue-story-1-17.md' },

        @{title = 'STORY: Crear campaña de donación con meta, vigencia y CC/proyecto asociado'; labels = @('story', 'phase:2', 'area:donations', 'priority:high'); milestone = 'Phase 2 - Donaciones'; file = 'issue-story-2-1.md' },
        @{title = 'STORY: Registrar donación (dinero o especie) con soporte y donante'; labels = @('story', 'phase:2', 'area:donations', 'priority:high'); milestone = 'Phase 2 - Donaciones'; file = 'issue-story-2-2.md' },
        @{title = 'STORY: Emitir certificado obligatorio de donación (PDF + QR + verificación)'; labels = @('story', 'phase:2', 'area:donations', 'priority:high'); milestone = 'Phase 2 - Donaciones'; file = 'issue-story-2-3.md' },
        @{title = 'STORY: Reporte de donaciones por campaña/donante/proyecto (exportable)'; labels = @('story', 'phase:2', 'area:donations', 'priority:medium'); milestone = 'Phase 2 - Donaciones'; file = 'issue-story-2-4.md' },

        @{title = 'STORY: Crear proyecto social con presupuesto, cronograma, actividades y evidencias'; labels = @('story', 'phase:3', 'area:projects', 'priority:high'); milestone = 'Phase 3 - Proyectos'; file = 'issue-story-3-1.md' },
        @{title = 'STORY: Registrar beneficiario con consentimiento obligatorio para almacenar PII'; labels = @('story', 'phase:3', 'area:projects', 'priority:high'); milestone = 'Phase 3 - Proyectos'; file = 'issue-story-3-2.md' },
        @{title = 'STORY: Control de acceso a PII de beneficiarios (Junta solo agregado/anónimo)'; labels = @('story', 'phase:3', 'area:projects', 'priority:high'); milestone = 'Phase 3 - Proyectos'; file = 'issue-story-3-3.md' },
        @{title = 'STORY: Registrar indicadores agregados por proyecto y generar informe de rendición (PDF/Excel)'; labels = @('story', 'phase:3', 'area:projects', 'priority:high'); milestone = 'Phase 3 - Proyectos'; file = 'issue-story-3-4.md' },

        @{title = 'STORY: Registrar compra de inventario simple (cantidades + valor unitario) con CxP opcional'; labels = @('story', 'phase:4', 'area:business', 'priority:medium'); milestone = 'Phase 4 - Negocios'; file = 'issue-story-4-1.md' },
        @{title = 'STORY: Registrar venta y emitir comprobante interno PDF + QR (sin facturación electrónica)'; labels = @('story', 'phase:4', 'area:business', 'priority:medium'); milestone = 'Phase 4 - Negocios'; file = 'issue-story-4-2.md' },
        @{title = 'STORY: Reportes de inventario, ventas y utilidad simple (exportable)'; labels = @('story', 'phase:4', 'area:business', 'priority:low'); milestone = 'Phase 4 - Negocios'; file = 'issue-story-4-3.md' },

        @{title = 'STORY: Generar reporte estructurado exportable para exógena (base para diligenciamiento)'; labels = @('story', 'phase:5', 'area:reports', 'priority:medium'); milestone = 'Phase 5 - Tributario avanzado'; file = 'issue-story-5-1.md' },
        @{title = 'STORY: Generar reporte base beneficiarios finales (estructura para diligenciar)'; labels = @('story', 'phase:5', 'area:reports', 'priority:medium'); milestone = 'Phase 5 - Tributario avanzado'; file = 'issue-story-5-2.md' }
    )
}

if (-not $Repo) {
    $Repo = Resolve-RepoFromGit
}

if (-not (Test-Path $BacklogPath)) {
    throw "No existe BacklogPath: $BacklogPath"
}

Ensure-GhReady
Write-Info "Repo objetivo: $Repo"
Write-Info "Backlog folder: $BacklogPath"
Write-Info "Modo reset canónico: $([bool]$ResetCatalog)"

$milestoneMap = Get-MilestoneMap
Ensure-Milestones -RepoName $Repo -MilestoneMap $milestoneMap
Ensure-Labels -RepoName $Repo

$targets = Get-CanonicalTargets
if ($targets.Count -eq 0) {
    throw "No hay catálogo canónico configurado."
}

$open = Get-OpenIssues -Repository $Repo
$openEpicStory = @($open | Where-Object { (@($_.labels.name) -contains 'epic') -or (@($_.labels.name) -contains 'story') })

if ($ResetCatalog) {
    $closed = 0
    foreach ($i in $openEpicStory) {
        if ($WhatIf) {
            Write-Info "[WhatIf] Cerraría issue #$($i.number): $($i.title)"
        }
        else {
            gh issue close $i.number --repo $Repo --reason "not planned" 2>&1 | Out-Null
            if ($LASTEXITCODE -eq 0) { $closed++ }
            Start-Sleep -Milliseconds 150
        }
    }
    Write-Info "Issues epic/story cerrados: $closed"

    $open = @()
}

$existingOpenTitles = @{}
foreach ($i in $open) {
    $existingOpenTitles[[string]$i.title] = $true
}

$created = 0
$skipped = 0
foreach ($t in $targets) {
    if ($existingOpenTitles.ContainsKey([string]$t.title)) {
        $skipped++
        continue
    }

    $bodyPath = Join-Path $BacklogPath $t.file
    if (-not (Test-Path $bodyPath)) {
        throw "No existe body-file: $bodyPath"
    }

    $labelsCsv = (($t.labels | Select-Object -Unique) -join ',')

    if ($WhatIf) {
        Write-Info "[WhatIf] Crearía issue: $($t.title)"
        $created++
        continue
    }

    $createdOk = $false
    $lastErrorText = ""
    for ($attempt = 1; $attempt -le 4; $attempt++) {
        $createOut = gh issue create --repo $Repo --title $t.title --body-file $bodyPath --label $labelsCsv --milestone $t.milestone 2>&1
        if ($LASTEXITCODE -eq 0) {
            $createdOk = $true
            break
        }

        $lastErrorText = ($createOut | Out-String).Trim()
        Start-Sleep -Seconds ([Math]::Min(2 * $attempt, 8))
    }

    if (-not $createdOk) {
        throw "Error creando issue: $($t.title). Detalle: $lastErrorText"
    }

    Start-Sleep -Milliseconds 250
    $created++
}

$after = Get-OpenIssues -Repository $Repo
$afterEpic = @($after | Where-Object { @($_.labels.name) -contains 'epic' }).Count
$afterStory = @($after | Where-Object { @($_.labels.name) -contains 'story' }).Count

Write-Output "created_catalog=$created"
Write-Output "skipped_existing_open=$skipped"
Write-Output "open_epic=$afterEpic"
Write-Output "open_story=$afterStory"
Write-Output ("open_epic_story_total=" + ($afterEpic + $afterStory))
