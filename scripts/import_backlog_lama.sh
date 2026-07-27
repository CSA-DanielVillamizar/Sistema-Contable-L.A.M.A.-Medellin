#!/usr/bin/env bash
# =============================================================================
# import_backlog_lama.sh
# Crea o actualiza los 34 issues del backlog L.A.M.A. Medellín en GitHub.
# Incluye labels base, criterios Gherkin y trazabilidad completa.
#
# Uso:
#   gh auth login          # autenticarse si no lo está
#   chmod +x import_backlog_lama.sh
#   ./import_backlog_lama.sh
# =============================================================================
set -euo pipefail

REPO="CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin"

# ---------- Pre-check ----------
echo ">> Verificando autenticación GH..."
gh auth status >/dev/null

echo ">> Verificando acceso al repo ${REPO}..."
gh repo view "$REPO" >/dev/null

# ---------- Labels base ----------
echo ">> Asegurando labels base..."
declare -a LABELS=(
  "type:epic:6f42c1"
  "type:story:0e8a16"
  "phase:0:5319e7"
  "phase:1:5319e7"
  "phase:2:5319e7"
  "phase:3:5319e7"
  "phase:4:5319e7"
  "phase:5:5319e7"
  "phase:X:5319e7"
  "area:security:d93f0b"
  "area:infra:d93f0b"
  "area:accounting:d93f0b"
  "area:treasury:d93f0b"
  "area:ar:d93f0b"
  "area:ap:d93f0b"
  "area:fx:d93f0b"
  "area:donations:d93f0b"
  "area:social:d93f0b"
  "area:business:d93f0b"
  "area:tax:d93f0b"
  "area:integration:d93f0b"
  "priority:must:b60205"
  "priority:should:fbca04"
  "priority:could:c5def5"
)
for item in "${LABELS[@]}"; do
  label_name="$(echo "$item" | awk -F: '{print $1 ":" $2}')"
  label_color="$(echo "$item" | awk -F: '{print $3}')"
  gh label create "$label_name" --repo "$REPO" --color "$label_color" 2>/dev/null || \
  gh label edit   "$label_name" --repo "$REPO" --color "$label_color" 2>/dev/null || true
  echo "   label ok: $label_name"
done

# ---------- Helper: upsert issue por título exacto ----------
upsert_issue () {
  local title="$1"
  local body_file="$2"
  local labels="$3"   # comma-separated

  local existing
  existing="$(gh issue list --repo "$REPO" --state all \
    --search "\"$title\" in:title" \
    --json number,title \
    -q ".[] | select(.title==\"$title\") | .number" | head -n1 || true)"

  if [[ -n "${existing}" ]]; then
    echo ">> Updating #${existing}: $title"
    gh issue edit "$existing" --repo "$REPO" \
      --title "$title" --body-file "$body_file" --add-label "$labels" >/dev/null
  else
    echo ">> Creating: $title"
    gh issue create --repo "$REPO" \
      --title "$title" --body-file "$body_file" --label "$labels" >/dev/null
  fi
}

mkdir -p /tmp/lama_issues

# ---------- Catálogo: 34 historias ----------
cat > /tmp/lama_issues/catalog.csv <<'CSV'
US-SEC-01|[US-SEC-01] Crear roles por módulo|#571|0|security|must
US-SEC-02|[US-SEC-02] Asignar múltiples roles por usuario|#571|0|security|must
US-SEC-03|[US-SEC-03] Enrolamiento MFA obligatorio|#571|0|security|must
US-SEC-04|[US-SEC-04] Bitácora de accesos y cambios de permisos|#571|0|security|should
US-OPS-01|[US-OPS-01] Cargar secretos desde Key Vault|#572|0|infra|must
US-OPS-02|[US-OPS-02] Logs estructurados con correlationId|#572|0|infra|must
US-OPS-03|[US-OPS-03] Adjuntar soportes en Blob|#572|0|infra|should
US-OPS-04|[US-OPS-04] Health checks de dependencias críticas|#572|0|infra|must
US-ACC-01|[US-ACC-01] Importar PUC ESAL con validación estructural|#593|1|accounting|must
US-ACC-02|[US-ACC-02] Preview de importación PUC antes de confirmar|#593|1|accounting|must
US-ACC-03|[US-ACC-03] Versionado de importaciones PUC|#593|1|accounting|should
US-ACC-04|[US-ACC-04] Parametrizar mapeo contable por operación|#594|1|accounting|must
US-ACC-05|[US-ACC-05] Bloquear guardado de mapeos incompletos|#594|1|accounting|must
US-ACC-06|[US-ACC-06] Historial de cambios de mapeo contable|#594|1|accounting|should
US-GL-01|[US-GL-01] Validar partida doble en comprobantes|#574|1|accounting|must
US-GL-02|[US-GL-02] Consecutivo único por tipo y período|#574|1|accounting|must
US-GL-03|[US-GL-03] Cierre de período con bloqueo de ediciones|#574|1|accounting|must
US-GL-04|[US-GL-04] Anulación con comprobante reverso enlazado|#574|1|accounting|must
US-TRS-01|[US-TRS-01] Registrar recibos y aplicar recaudos CxC|#575|1|treasury|must
US-TRS-02|[US-TRS-02] Conciliación bancaria movimiento a movimiento|#575|1|treasury|should
US-AR-01|[US-AR-01] Cálculo automático de mora en cartera|#576|1|ar|should
US-AP-01|[US-AP-01] Programación de pagos por vencimiento/prioridad|#577|1|ap|must
US-FX-01|[US-FX-01] Registro de TRM diaria|#578|1|fx|should
US-FX-02|[US-FX-02] Cálculo de diferencia en cambio al cierre|#578|1|fx|should
US-DON-01|[US-DON-01] Registro de donaciones por tipo|#579|2|donations|must
US-DON-02|[US-DON-02] Asociación de donaciones a campañas|#579|2|donations|should
US-DON-03|[US-DON-03] Emisión de certificados de donación|#579|2|donations|must
US-SOC-01|[US-SOC-01] Registro de beneficiarios|#580|3|social|must
US-SOC-02|[US-SOC-02] Consentimiento informado obligatorio|#580|3|social|must
US-BIZ-01|[US-BIZ-01] Alta de ítems y ajustes de inventario|#581|4|business|should
US-BIZ-02|[US-BIZ-02] Venta con salida de inventario|#581|4|business|should
US-BIZ-03|[US-BIZ-03] Comprobante interno por operación comercial|#581|4|business|must
US-TAX-01|[US-TAX-01] Generar reporte tributario base por período|#582|5|tax|must
US-INT-01|[US-INT-01] Adapter FE en modo not_implemented controlado|#583|X|integration|could
CSV

# ---------- Gherkin por historia ----------
gherkin_for () {
  case "$1" in
    US-SEC-01) printf '```gherkin\nScenario: Crear rol con permisos válidos\n  Given admin autenticado\n  When crea un rol por módulo con permisos válidos\n  Then el rol queda activo y auditado\n```' ;;
    US-SEC-02) printf '```gherkin\nScenario: Asignación múltiple de roles\n  Given un usuario existente\n  When admin asigna dos roles\n  Then los permisos efectivos son la unión de ambos\n```' ;;
    US-SEC-03) printf '```gherkin\nScenario: Enrolamiento MFA\n  Given credenciales válidas\n  When el usuario registra segundo factor\n  Then en el próximo inicio se exige MFA\n```' ;;
    US-SEC-04) printf '```gherkin\nScenario: Auditoría de permisos\n  Given cambios de permisos registrados\n  When el auditor consulta bitácora\n  Then visualiza actor, before/after y fecha\n```' ;;
    US-OPS-01) printf '```gherkin\nScenario: Carga de secreto obligatorio\n  Given ambiente productivo\n  When la app solicita DB_CONNECTION_STRING desde Key Vault\n  Then inicia solo si el secreto existe\n```' ;;
    US-OPS-02) printf '```gherkin\nScenario: Log estructurado de error\n  Given una transacción inválida\n  When backend la rechaza\n  Then registra log con correlationId, usuario y código de error\n```' ;;
    US-OPS-03) printf '```gherkin\nScenario: Adjuntar soporte\n  Given un comprobante en edición\n  When el usuario adjunta PDF válido\n  Then se almacena en Blob y se referencia en la operación\n```' ;;
    US-OPS-04) printf '```gherkin\nScenario: Health check degradado\n  Given la base de datos no responde\n  When se consulta /health\n  Then el estado general es degraded o down\n```' ;;
    US-ACC-01) printf '```gherkin\nScenario: Rechazo por estructura inválida\n  Given un archivo PUC sin columnas obligatorias\n  When se ejecuta importación\n  Then el sistema rechaza y reporta errores por fila/columna\n```' ;;
    US-ACC-02) printf '```gherkin\nScenario: Preview de importación\n  Given archivo PUC válido\n  When se carga en modo preview\n  Then se muestran altas, cambios e inválidos\n```' ;;
    US-ACC-03) printf '```gherkin\nScenario: Versionado de importación\n  Given una importación confirmada\n  When finaliza\n  Then crea versión con usuario, fecha y hash del archivo\n```' ;;
    US-ACC-04) printf '```gherkin\nScenario: Mapeo contable por operación\n  Given un tipo de operación definido\n  When se guarda mapeo débito/crédito\n  Then queda activo para contabilización automática\n```' ;;
    US-ACC-05) printf '```gherkin\nScenario: Bloqueo de mapeo incompleto\n  Given un mapeo sin cuenta crédito\n  When se intenta guardar\n  Then el sistema bloquea con validación\n```' ;;
    US-ACC-06) printf '```gherkin\nScenario: Historial de cambios de mapeo\n  Given un mapeo modificado\n  When se consulta historial\n  Then muestra versiones y before/after\n```' ;;
    US-GL-01)  printf '```gherkin\nScenario: Partida doble obligatoria\n  Given un comprobante descuadrado\n  When se contabiliza\n  Then el sistema lo rechaza\n```' ;;
    US-GL-02)  printf '```gherkin\nScenario: Consecutivo único\n  Given período abierto y tipo de comprobante\n  When se contabiliza un comprobante válido\n  Then se asigna consecutivo único correlativo\n```' ;;
    US-GL-03)  printf '```gherkin\nScenario: Cierre de período\n  Given período sin borradores\n  When contador ejecuta cierre\n  Then período queda cerrado y bloquea nuevas contabilizaciones\n```' ;;
    US-GL-04)  printf '```gherkin\nScenario: Anulación con reverso\n  Given comprobante contabilizado\n  When se anula con motivo válido\n  Then crea comprobante reverso enlazado\n```' ;;
    US-TRS-01) printf '```gherkin\nScenario: Recaudo parcial\n  Given una cuenta por cobrar vigente\n  When se registra recaudo parcial\n  Then disminuye el saldo pendiente\n```' ;;
    US-TRS-02) printf '```gherkin\nScenario: Conciliación bancaria\n  Given movimiento de extracto no conciliado\n  When se vincula a movimiento interno\n  Then cambia a estado conciliado\n```' ;;
    US-AR-01)  printf '```gherkin\nScenario: Mora automática\n  Given cuota vencida\n  When se ejecuta cálculo de mora\n  Then aplica recargo según regla vigente\n```' ;;
    US-AP-01)  printf '```gherkin\nScenario: Propuesta de pagos\n  Given facturas con distintos vencimientos\n  When se genera propuesta\n  Then ordena por vencimiento y prioridad\n```' ;;
    US-FX-01)  printf '```gherkin\nScenario: Registro TRM\n  Given fecha sin TRM\n  When se registra TRM válida\n  Then queda disponible para valoración\n```' ;;
    US-FX-02)  printf '```gherkin\nScenario: Diferencia en cambio\n  Given saldos en moneda extranjera y TRM de cierre\n  When se ejecuta valoración\n  Then genera ajuste contable por diferencia en cambio\n```' ;;
    US-DON-01) printf '```gherkin\nScenario: Registro de donación\n  Given donante válido\n  When se registra donación con tipo y valor\n  Then queda confirmada\n```' ;;
    US-DON-02) printf '```gherkin\nScenario: Asociación a campaña\n  Given campaña activa\n  When se vincula donación confirmada\n  Then se actualiza total recaudado\n```' ;;
    US-DON-03) printf '```gherkin\nScenario: Certificado de donación\n  Given donación certificable\n  When se genera certificado\n  Then se emite con consecutivo y trazabilidad\n```' ;;
    US-SOC-01) printf '```gherkin\nScenario: Alta de beneficiario\n  Given datos mínimos completos\n  When se crea beneficiario\n  Then se asigna identificador único\n```' ;;
    US-SOC-02) printf '```gherkin\nScenario: Consentimiento obligatorio\n  Given beneficiario sin consentimiento\n  When se intenta vincular a proyecto\n  Then el sistema bloquea la vinculación\n```' ;;
    US-BIZ-01) printf '```gherkin\nScenario: Ajuste de inventario\n  Given ítem activo\n  When se registra ajuste con motivo\n  Then cambia stock y queda auditado\n```' ;;
    US-BIZ-02) printf '```gherkin\nScenario: Venta con stock suficiente\n  Given stock disponible\n  When se confirma venta\n  Then descuenta inventario\n```' ;;
    US-BIZ-03) printf '```gherkin\nScenario: Comprobante interno\n  Given operación comercial confirmada\n  When se contabiliza\n  Then crea comprobante interno enlazado\n```' ;;
    US-TAX-01) printf '```gherkin\nScenario: Reporte tributario base\n  Given período cerrado\n  When se genera reporte exógena base\n  Then produce archivo en formato definido\n```' ;;
    US-INT-01) printf '```gherkin\nScenario: Adapter FE no implementado controlado\n  Given invocación del adapter FE\n  When la integración full no está implementada\n  Then responde not_implemented y registra evento\n```' ;;
    *)         printf '```gherkin\nScenario: TBD\n  Given ...\n  When ...\n  Then ...\n```' ;;
  esac
}

# ---------- Loop principal ----------
echo ">> Creando/actualizando historias (100%)..."
while IFS='|' read -r ID TITLE EPIC PHASE AREA PRIORITY; do
  [[ -z "${ID}" ]] && continue

  BODY_FILE="/tmp/lama_issues/${ID}.md"
  GHERKIN="$(gherkin_for "$ID")"

  cat > "$BODY_FILE" <<BODY
## Historia
Como miembro del equipo, quiero implementar **${TITLE}** para asegurar el resultado esperado del módulo.

## Contexto / Trazabilidad
- Epic origen: ${EPIC}
- Fase: ${PHASE}
- Story ID interno: ${ID}
- Repositorio: ${REPO}

## Regla(s) de negocio
1. Debe respetar validaciones del dominio contable/financiero asociadas.
2. Debe registrar trazabilidad/auditoría en eventos críticos.
3. Debe cumplir permisos por rol.

## Criterios de aceptación (Gherkin)
${GHERKIN}

## Pruebas sugeridas
- Unitarias de validación de reglas.
- Integración de flujo end-to-end.
- Prueba negativa de permisos/errores.

## Evidencia esperada
- Capturas/logs de ejecución.
- Registro en bitácora/auditoría.
- Resultado persistido en entidad correspondiente.
BODY

  labels="type:story,phase:${PHASE},area:${AREA},priority:${PRIORITY}"
  upsert_issue "$TITLE" "$BODY_FILE" "$labels"

done < /tmp/lama_issues/catalog.csv

echo ""
echo ">> ✅ Proceso finalizado."
echo ">> Revisa issues en: https://github.com/${REPO}/issues"
