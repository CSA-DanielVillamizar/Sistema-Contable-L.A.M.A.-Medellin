$ErrorActionPreference = 'Continue'
$outputFile = Join-Path $PSScriptRoot 'tmp_step45_output.txt'
if (Test-Path $outputFile) { Remove-Item $outputFile -Force }

function Log {
    param([string]$Message)
    $Message | Tee-Object -FilePath $outputFile -Append
}

$repo = 'CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin'

Log 'STEP4_TRIGGER'
gh workflow run deploy-backend.yml --repo $repo --ref main 2>&1 | Tee-Object -FilePath $outputFile -Append
Log "WORKFLOW_RUN_EXIT=$LASTEXITCODE"

Start-Sleep -Seconds 5

Log 'STEP5_LATEST_RUN'
gh run list --workflow deploy-backend.yml --repo $repo --limit 1 --json databaseId,status,conclusion,createdAt,headBranch,displayTitle,url,event 2>&1 | Tee-Object -FilePath $outputFile -Append
Log "RUN_LIST_EXIT=$LASTEXITCODE"
