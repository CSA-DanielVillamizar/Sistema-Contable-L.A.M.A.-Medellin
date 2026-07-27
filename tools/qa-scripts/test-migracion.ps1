# Script temporal para ejecutar migracion con SSL bypass
Write-Host "`nDisparando endpoint de migracion..." -ForegroundColor Magenta

# Obtener token
$token = az account get-access-token --resource api://b81ee2ee-5417-4aa0-8000-e470aec5543e --query accessToken -o tsv

# Bypass SSL Certificate Validation
add-type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class TrustAllCertsPolicy : ICertificatePolicy {
    public bool CheckValidationResult(
        ServicePoint svcPoint, X509Certificate certificate,
        WebRequest webRequest, int certificateProblem) {
        return true;
    }
}
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

# Ejecutar request
try {
    $response = Invoke-RestMethod -Method POST -Uri "https://localhost:7015/api/migracion/cargar-historico" -Headers @{ Authorization = "Bearer $token"; Accept = "application/json" } -ContentType "application/json"

    Write-Host "`nMIGRACION EXITOSA`n" -ForegroundColor Green
    Write-Host "RESULTADOS:" -ForegroundColor Cyan
    Write-Host "  Comprobantes Creados: $($response.comprobantesCreados)" -ForegroundColor White
    Write-Host "  Lineas Procesadas: $($response.lineasProcesadas)" -ForegroundColor White
    Write-Host "  Advertencias: $($response.advertencias.Count)" -ForegroundColor Yellow

    if ($response.advertencias.Count -gt 0) {
        Write-Host "`nADVERTENCIAS:" -ForegroundColor Yellow
        $response.advertencias | ForEach-Object {
            Write-Host "  - $_" -ForegroundColor Yellow
        }
    }

    Write-Host "`nJSON Completo:" -ForegroundColor Cyan
    $response | ConvertTo-Json -Depth 5

}
catch {
    Write-Host "`nERROR EN LA MIGRACION" -ForegroundColor Red
    Write-Host "Mensaje: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Red
}
