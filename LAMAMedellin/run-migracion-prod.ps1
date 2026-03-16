$ErrorActionPreference = 'Stop'

$apiAppId = 'b81ee2ee-5417-4aa0-8000-e470aec5543e'
$endpoint = 'https://app-lamamedellin-backend-prod.azurewebsites.net/api/migracion/cargar-historico'
$csvPath = 'c:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\LAMAMedellin\docs\Historico.csv'

$token = az account get-access-token --resource "api://$apiAppId" --query accessToken -o tsv
if ([string]::IsNullOrWhiteSpace($token)) {
    throw 'No se pudo generar token de acceso para la API de produccion.'
}

[string]$contenidoCsv = Get-Content -Path $csvPath -Raw -Encoding UTF8
$contenidoCsv = ($contenidoCsv -replace "`r", "").TrimEnd("`n")
$payload = @{ contenidoCsv = [string]$contenidoCsv } | ConvertTo-Json -Compress
$payloadBytes = [System.Text.Encoding]::UTF8.GetBytes($payload)

$response = Invoke-RestMethod -Method Post -Uri $endpoint -Headers @{ Authorization = "Bearer $token" } -ContentType 'application/json; charset=utf-8' -Body $payloadBytes

$resultado = [ordered]@{
    comprobantesCreados = if ($null -ne $response.comprobantesCreados) { $response.comprobantesCreados } else { $response.ComprobantesCreados }
    advertencias        = if ($null -ne $response.advertencias) { $response.advertencias } else { $response.Advertencias }
}

$resultado | ConvertTo-Json -Depth 20 -Compress
