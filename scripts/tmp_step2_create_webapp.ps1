$ErrorActionPreference = 'Continue'
$outputFile = Join-Path $PSScriptRoot 'tmp_step2_output.txt'
if (Test-Path $outputFile) { Remove-Item $outputFile -Force }

function Log {
    param([string]$Message)
    $Message | Tee-Object -FilePath $outputFile -Append
}

Log "STEP2_PLACEHOLDER_CHECK"
$scriptPath = Join-Path $PSScriptRoot 'provisionar-backend.ps1'
$content = Get-Content $scriptPath -Raw
$sqlUser = [regex]::Match($content, '\$SqlUser\s*=\s*"([^"]+)"').Groups[1].Value
$sqlPassword = [regex]::Match($content, '\$SqlPassword\s*=\s*"([^"]+)"').Groups[1].Value
Log "SqlUser=$sqlUser"
Log "SqlPassword=$sqlPassword"

Log "STEP2_CREATE_WEBAPP"
az webapp create --name app-lamamedellin-backend-prod --resource-group rg-lamaregionnorte-prod --plan plan-lamamedellin-prod --runtime "DOTNETCORE`|8.0" --https-only true -o json 2>&1 | Tee-Object -FilePath $outputFile -Append
Log "CREATE_EXIT=$LASTEXITCODE"

Log "STEP2_VERIFY_WEBAPP"
az webapp show --name app-lamamedellin-backend-prod --resource-group rg-lamaregionnorte-prod --query "{name:name,location:location,state:state,kind:kind,linuxFxVersion:siteConfig.linuxFxVersion,reserved:reserved}" -o json 2>&1 | Tee-Object -FilePath $outputFile -Append
Log "VERIFY_EXIT=$LASTEXITCODE"

if ($sqlUser -eq '<SQL_USER>' -or $sqlPassword -eq '<SQL_PASSWORD>') {
    Log 'STEP2_CONNSTRING_BLOCKED=placeholders_not_replaced'
    exit 2
}

$connectionString = "Server=tcp:lamaregionnorte-sql-a90e.database.windows.net,1433;Initial Catalog=LAMAMedellinContable;Persist Security Info=False;User ID=$sqlUser;Password=$sqlPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
Log "STEP2_SET_CONNSTRING"
az webapp config connection-string set --name app-lamamedellin-backend-prod --resource-group rg-lamaregionnorte-prod --connection-string-type SQLAzure --settings DefaultConnection="$connectionString" -o json 2>&1 | Tee-Object -FilePath $outputFile -Append
Log "CONNSTRING_EXIT=$LASTEXITCODE"
