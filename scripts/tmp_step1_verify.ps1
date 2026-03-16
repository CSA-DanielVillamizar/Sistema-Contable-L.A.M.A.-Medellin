$ErrorActionPreference = 'Continue'
$outputFile = Join-Path $PSScriptRoot 'tmp_step1_output.txt'
if (Test-Path $outputFile) { Remove-Item $outputFile -Force }

function Log {
    param([string]$Message)
    $Message | Tee-Object -FilePath $outputFile -Append
}

Log 'STEP1_PLAN_CHECK'
az appservice plan show --name plan-lamamedellin-prod --resource-group rg-lamaregionnorte-prod --query "{name:name,location:location,kind:kind,reserved:reserved,sku:sku.name}" -o json 2>&1 | Tee-Object -FilePath $outputFile -Append
Log "PLAN_EXIT=$LASTEXITCODE"

Log 'STEP1_WEBAPP_CHECK'
az webapp show --name app-lamamedellin-backend-prod --resource-group rg-lamaregionnorte-prod --query "{name:name,location:location,state:state,kind:kind,linuxFxVersion:siteConfig.linuxFxVersion,reserved:reserved}" -o json 2>&1 | Tee-Object -FilePath $outputFile -Append
Log "WEBAPP_EXIT=$LASTEXITCODE"
