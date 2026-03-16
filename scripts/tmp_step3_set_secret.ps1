$ErrorActionPreference = 'Continue'
$repoPath = 'c:/Users/DanielVillamizar/Sistema Contable L.A.M.A. Medellin'
Set-Location -Path $repoPath

Write-Output 'STEP3_FETCH_PROFILE'
$perfilXml = az webapp deployment list-publishing-profiles --name app-lamamedellin-backend-prod --resource-group rg-lamaregionnorte-prod --xml 2>&1 | Out-String
$fetchExit = $LASTEXITCODE
Write-Output "PROFILE_FETCH_EXIT=$fetchExit"

if ($fetchExit -ne 0 -or [string]::IsNullOrWhiteSpace($perfilXml)) {
    Write-Output 'STEP3_ABORT=no_profile'
    exit 1
}

Write-Output "PROFILE_LENGTH=$($perfilXml.Length)"
Write-Output 'STEP3_SET_SECRET'
$perfilXml | gh secret set AZURE_WEBAPP_PUBLISH_PROFILE 2>&1
Write-Output "SECRET_SET_EXIT=$LASTEXITCODE"
