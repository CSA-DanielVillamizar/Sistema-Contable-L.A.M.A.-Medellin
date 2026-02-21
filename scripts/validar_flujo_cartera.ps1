param(
    [Parameter(Mandatory = $true)]
    [string]$Token,

    [Parameter(Mandatory = $false)]
    [string]$ApiBaseUrl = "http://localhost:5006",

    [Parameter(Mandatory = $false)]
    [string]$Periodo = "2026-03"
)

$ErrorActionPreference = 'Stop'

function Invoke-Api {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet('GET', 'POST')]
        [string]$Method,

        [Parameter(Mandatory = $true)]
        [string]$Url,

        [Parameter(Mandatory = $false)]
        [object]$Body
    )

    $headers = @{
        Authorization = "Bearer $Token"
        Accept        = 'application/json'
    }

    try {
        if ($Method -eq 'GET') {
            return Invoke-RestMethod -Method Get -Uri $Url -Headers $headers
        }

        $jsonBody = if ($null -ne $Body) { $Body | ConvertTo-Json -Depth 10 } else { $null }
        return Invoke-RestMethod -Method Post -Uri $Url -Headers $headers -ContentType 'application/json' -Body $jsonBody
    }
    catch {
        $status = $null
        $responseText = ''

        if ($_.Exception.Response) {
            try {
                $status = [int]$_.Exception.Response.StatusCode
            } catch { }

            try {
                $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
                $responseText = $reader.ReadToEnd()
                $reader.Close()
            } catch { }
        }

        Write-Host "[ERROR] $Method $Url" -ForegroundColor Red
        if ($status) { Write-Host "Status: $status" -ForegroundColor Red }
        if ($responseText) { Write-Host "Body: $responseText" -ForegroundColor DarkRed }
        throw
    }
}

Write-Host "=== Validación Flujo Cartera ===" -ForegroundColor Cyan
Write-Host "API: $ApiBaseUrl"
Write-Host "Periodo: $Periodo"

# 1) Generar obligaciones para el periodo
Write-Host "[1/4] Generando obligaciones del periodo $Periodo ..." -ForegroundColor Yellow
$generarResponse = Invoke-Api -Method POST -Url "$ApiBaseUrl/api/cartera/generar-obligaciones" -Body @{ Periodo = $Periodo }
Write-Host "Respuesta generar: $($generarResponse | ConvertTo-Json -Depth 5 -Compress)" -ForegroundColor Green

# 2) Listar pendientes antes de pagar
Write-Host "[2/4] Consultando cartera pendiente ..." -ForegroundColor Yellow
$antesRaw = Invoke-Api -Method GET -Url "$ApiBaseUrl/api/cartera/pendiente"
$antes = if ($antesRaw -is [System.Array]) { @($antesRaw) } else { @($antesRaw) }
if (-not $antes -or $antes.Count -eq 0) {
    throw "No hay cartera pendiente para validar el flujo."
}

Write-Host "Pendientes antes: $($antes.Count)"
$primero = $antes[0]
Write-Host "Primer registro: Id=$($primero.id), Miembro=$($primero.nombreMiembro), Saldo=$($primero.saldoPendienteCOP)" -ForegroundColor DarkCyan

# 3) Registrar pago del primer pendiente
Write-Host "[3/4] Registrando pago del primer pendiente ..." -ForegroundColor Yellow
$null = Invoke-Api -Method POST -Url "$ApiBaseUrl/api/cartera/$($primero.id)/pago" -Body @{ MontoCOP = [decimal]$primero.saldoPendienteCOP }
Write-Host "Pago registrado para Id=$($primero.id)" -ForegroundColor Green

# 4) Relistar y verificar desaparición
Write-Host "[4/4] Reconsultando cartera pendiente ..." -ForegroundColor Yellow
$despuesRaw = Invoke-Api -Method GET -Url "$ApiBaseUrl/api/cartera/pendiente"
$despues = if ($despuesRaw -is [System.Array]) { @($despuesRaw) } else { @($despuesRaw) }
$existe = @($despues | Where-Object { $_.id -eq $primero.id }).Count -gt 0

Write-Host "Pendientes después: $($despues.Count)"

if ($existe) {
    throw "Validación fallida: el registro pagado ($($primero.id)) sigue apareciendo en cartera pendiente."
}

Write-Host "Validación exitosa: el miembro pagado desapareció de la lista de pendientes." -ForegroundColor Green
