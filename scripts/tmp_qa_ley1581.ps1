$ErrorActionPreference = "Stop"
$apiBase = "https://app-lamamedellin-backend-prod.azurewebsites.net"
$scope = "api://b81ee2ee-5417-4aa0-8000-e470aec5543e/.default"
$sqlServer = "lamaregionnorte-sql-a90e.database.windows.net"
$sqlDb = "LAMAMedellinContable"
$doCleanup = $true
$ts = Get-Date -Format "yyyyMMddHHmmss"
$doc = "QAL1581-" + $ts
$projName = "Campana Invierno 2026 QA " + $ts

$accessToken = az account get-access-token --scope $scope --query accessToken -o tsv
if ([string]::IsNullOrWhiteSpace($accessToken)) { throw "No se obtuvo token API" }
$headers = @{ Authorization = "Bearer $accessToken"; "Content-Type" = "application/json" }

$centros = Invoke-RestMethod -Method GET -Uri "$apiBase/api/transacciones/centros-costo" -Headers $headers
if (-not $centros -or $centros.Count -eq 0) { throw "No hay centros de costo en catalogo" }
$centroCostoId = if ($centros[0].id) { $centros[0].id } else { $centros[0].Id }

$proyectoBody = @{
  CentroCostoId = $centroCostoId
  Nombre = $projName
  Descripcion = "Proyecto QA legal Ley 1581"
  FechaInicio = (Get-Date).ToString("o")
  FechaFin = (Get-Date).AddMonths(1).ToString("o")
  PresupuestoEstimado = 2500000
  Estado = 1
} | ConvertTo-Json

$proyectoResp = Invoke-RestMethod -Method POST -Uri "$apiBase/api/proyectos" -Headers $headers -Body $proyectoBody
$proyectoId = if ($proyectoResp.id) { $proyectoResp.id } else { $proyectoResp.Id }

$benefBody = @{
  NombreCompleto = "Beneficiario QA Ley1581 $ts"
  TipoDocumento = "CC"
  NumeroDocumento = $doc
  Email = "qa.ley1581.$ts@lama.test"
  Telefono = "3001234567"
  TieneConsentimientoHabeasData = $true
  ProyectoSocialId = $proyectoId
} | ConvertTo-Json

$benefResp = Invoke-RestMethod -Method POST -Uri "$apiBase/api/beneficiarios" -Headers $headers -Body $benefBody
$beneficiarioId = if ($benefResp.id) { $benefResp.id } else { $benefResp.Id }

$sqlToken = az account get-access-token --resource https://database.windows.net/ --query accessToken -o tsv
Add-Type -AssemblyName System.Data
$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString = "Server=tcp:$sqlServer,1433;Initial Catalog=$sqlDb;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
$conn.AccessToken = $sqlToken
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = @"
SELECT TOP 1
    Id,
    NombreCompleto,
    TipoDocumento,
    NumeroDocumento,
    Email,
    Telefono,
    CAST(TieneConsentimientoHabeasData AS INT) AS TieneConsentimientoHabeasData,
    IsDeleted
FROM Beneficiarios
WHERE Id = '$beneficiarioId';
"@
$da = New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
$dt = New-Object System.Data.DataTable
[void]$da.Fill($dt)
$conn.Close()

$deleteBenef = "NO_EJECUTADO"
$deleteProj = "NO_EJECUTADO"
if ($doCleanup) {
  try { Invoke-RestMethod -Method DELETE -Uri "$apiBase/api/beneficiarios/$beneficiarioId" -Headers $headers | Out-Null; $deleteBenef = "OK" } catch { $deleteBenef = "FALLO" }
  try { Invoke-RestMethod -Method DELETE -Uri "$apiBase/api/proyectos/$proyectoId" -Headers $headers | Out-Null; $deleteProj = "OK" } catch { $deleteProj = "FALLO" }
}

Write-Output "=== E2E QA LEY 1581 - RESULTADO ==="
Write-Output ("ProyectoId=" + $proyectoId)
Write-Output ("BeneficiarioId=" + $beneficiarioId)
Write-Output ("CentroCostoId=" + $centroCostoId)
Write-Output ("NumeroDocumentoPrueba=" + $doc)
Write-Output "--- Auditoria SQL Beneficiarios ---"
$dt | Format-Table -AutoSize | Out-String | Write-Output
Write-Output "--- Limpieza ---"
Write-Output ("DeleteBeneficiario=" + $deleteBenef)
Write-Output ("DeleteProyecto=" + $deleteProj)
