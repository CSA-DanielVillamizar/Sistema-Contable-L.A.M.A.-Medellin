param(
    [string]$SolutionPath = ".\",
    [string]$SolutionName = "LAMAMedellin",
    [switch]$Force = $false,
    [switch]$WhatIf = $false
)

<#
.SYNOPSIS
    Initialize LAMAMedellin backend solution with Clean Architecture structure

.DESCRIPTION
    Creates a complete .NET 8 backend solution following Clean Architecture pattern.
    Generates 4 projects with correct dependencies and layer structure.

.PARAMETER SolutionPath
    Directory where the solution will be created (default: current directory)

.PARAMETER SolutionName
    Solution name (default: LAMAMedellin)

.PARAMETER Force
    Overwrite existing structure if it exists

.PARAMETER WhatIf
    Show what would be executed without actually executing

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File init-backend.ps1

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File init-backend.ps1 -SolutionPath "C:\Projects" -Force

.NOTES
    Requires: .NET 8 SDK or higher
    Architecture: Clean Architecture (4-layer)
    Author: LAMA Medellín Development Team
    Date: February 2026
#>

$ErrorActionPreference = 'Stop'

function Write-Status {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "HH:mm:ss"
    $prefix = switch ($Level) {
        "ERROR" { "[ERROR]" }
        "WARN" { "[WARN]" }
        "SUCCESS" { "[OK]" }
        default { "[INFO]" }
    }
    Write-Host "$timestamp $prefix $Message" -ForegroundColor $(
        if ($Level -eq "ERROR") { "Red" }
        elseif ($Level -eq "WARN") { "Yellow" }
        elseif ($Level -eq "SUCCESS") { "Green" }
        else { "Cyan" }
    )
}

function Test-Prerequisites {
    Write-Status "Checking prerequisites..."

    $dotnetVersion = & dotnet --version 2>$null
    if (-not $dotnetVersion) {
        Write-Status "dotnet SDK not found" "ERROR"
        exit 1
    }

    $majorVersion = [int]($dotnetVersion -split '\.' | Select-Object -First 1)
    if ($majorVersion -lt 8) {
        Write-Status ".NET 8 SDK required (found: $dotnetVersion)" "WARN"
        exit 1
    }

    Write-Status ".NET $dotnetVersion found" "SUCCESS"
}

function Initialize-Solution {
    param([string]$Path, [string]$Name)

    Write-Status "Creating solution directory structure..."

    if (-not (Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
        Write-Status "Created directory: $Path" "SUCCESS"
    }

    Push-Location $Path

    if ((Test-Path "$Name.slnx") -or (Test-Path "$Name.sln")) {
        if (-not $Force) {
            Write-Status "Solution already exists at $Path\$Name" "WARN"
            Pop-Location
            return $false
        }
        Write-Status "Removing existing solution structure..." "WARN"
        Remove-Item -Recurse -Force "src" -ErrorAction SilentlyContinue
        Remove-Item "$Name.slnx", "$Name.sln" -Force -ErrorAction SilentlyContinue
    }

    Pop-Location
    return $true
}

function Create-SolutionFile {
    param([string]$Path, [string]$Name)

    Write-Status "Creating solution file: $Name.slnx"

    Push-Location $Path

    if ($WhatIf) {
        Write-Host "  [WhatIf] dotnet new sln -n $Name"
    }
    else {
        & dotnet new sln -n $Name | Out-Null
        Write-Status "Solution created: $Name.slnx" "SUCCESS"
    }

    Pop-Location
}

function Create-Projects {
    param([string]$Path, [string]$SolutionName)

    Write-Status "Creating projects..."

    $projects = @(
        @{
            Name        = "$SolutionName.Domain"
            Type        = "classlib"
            Path        = "src/$SolutionName.Domain"
            Description = "Domain entities and business logic"
        },
        @{
            Name        = "$SolutionName.Application"
            Type        = "classlib"
            Path        = "src/$SolutionName.Application"
            Description = "Application services and use cases (CQRS)"
        },
        @{
            Name        = "$SolutionName.Infrastructure"
            Type        = "classlib"
            Path        = "src/$SolutionName.Infrastructure"
            Description = "Data access and external service integration"
        },
        @{
            Name        = "$SolutionName.API"
            Type        = "webapi"
            Path        = "src/$SolutionName.API"
            Description = "HTTP API endpoints with controllers"
        }
    )

    Push-Location $Path

    foreach ($project in $projects) {
        Write-Status "Creating project: $($project.Name) ($($project.Description))"

        if ($WhatIf) {
            if ($project.Type -eq "webapi") {
                Write-Host "  [WhatIf] dotnet new webapi -n $($project.Name) -f net8.0 --use-controllers --no-openapi -o $($project.Path)"
            }
            else {
                Write-Host "  [WhatIf] dotnet new classlib -n $($project.Name) -f net8.0 -o $($project.Path)"
            }
        }
        else {
            if ($project.Type -eq "webapi") {
                & dotnet new webapi -n $project.Name -f net8.0 --use-controllers --no-openapi -o $project.Path | Out-Null
            }
            else {
                & dotnet new classlib -n $project.Name -f net8.0 -o $project.Path | Out-Null
            }
            Write-Status "Project created: $($project.Name)" "SUCCESS"
        }
    }

    Pop-Location
}

function Add-ProjectReferences {
    param([string]$Path, [string]$SolutionName)

    Write-Status "Adding project references (Clean Architecture dependencies)..."

    Push-Location $Path

    $references = @(
        @{
            Project     = "src/$SolutionName.Application/$SolutionName.Application.csproj"
            References  = @("src/$SolutionName.Domain/$SolutionName.Domain.csproj")
            Description = "Application references Domain"
        },
        @{
            Project     = "src/$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj"
            References  = @("src/$SolutionName.Domain/$SolutionName.Domain.csproj")
            Description = "Infrastructure references Domain"
        },
        @{
            Project     = "src/$SolutionName.API/$SolutionName.API.csproj"
            References  = @(
                "src/$SolutionName.Application/$SolutionName.Application.csproj",
                "src/$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj"
            )
            Description = "API references Application and Infrastructure"
        }
    )

    foreach ($ref in $references) {
        Write-Status $ref.Description
        foreach ($referencedProject in $ref.References) {
            if ($WhatIf) {
                Write-Host "  [WhatIf] dotnet add `"$($ref.Project)`" reference `"$referencedProject`""
            }
            else {
                & dotnet add $ref.Project reference $referencedProject | Out-Null
                Write-Status "Added reference: $($ref.Project) -> $referencedProject" "SUCCESS"
            }
        }
    }

    Pop-Location
}

function Add-ProjectsToSolution {
    param([string]$Path, [string]$SolutionName)

    Write-Status "Adding projects to solution..."

    Push-Location $Path

    $projectPaths = @(
        "src/$SolutionName.Domain/$SolutionName.Domain.csproj",
        "src/$SolutionName.Application/$SolutionName.Application.csproj",
        "src/$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj",
        "src/$SolutionName.API/$SolutionName.API.csproj"
    )

    foreach ($projectPath in $projectPaths) {
        if ($WhatIf) {
            Write-Host "  [WhatIf] dotnet sln $SolutionName.slnx add `"$projectPath`""
        }
        else {
            & dotnet sln "$SolutionName.slnx" add $projectPath | Out-Null
            Write-Status "Added to solution: $projectPath" "SUCCESS"
        }
    }

    Pop-Location
}

function Create-FolderStructure {
    param([string]$Path, [string]$SolutionName)

    Write-Status "Creating folder structure within layers..."

    $folderStructure = @{
        "src/$SolutionName.Domain"         = @(
            "Entities",
            "ValueObjects",
            "Interfaces",
            "Constants"
        )
        "src/$SolutionName.Application"    = @(
            "Commands",
            "Queries",
            "DTOs",
            "Interfaces",
            "Validators",
            "Mappings"
        )
        "src/$SolutionName.Infrastructure" = @(
            "Persistence",
            "Services",
            "Configuration",
            "Seeders"
        )
        "src/$SolutionName.API"            = @(
            "Controllers",
            "Middleware",
            "Extensions"
        )
    }

    Push-Location $Path

    foreach ($folderPath in $folderStructure.Keys) {
        foreach ($folder in $folderStructure[$folderPath]) {
            $fullPath = "$folderPath/$folder"
            if ($WhatIf) {
                Write-Host "  [WhatIf] Create folder: $fullPath"
            }
            else {
                New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
            }
        }
    }

    if (-not $WhatIf) {
        Write-Status "Folder structure created" "SUCCESS"
    }

    Pop-Location
}

function Test-Build {
    param([string]$Path, [string]$SolutionName)

    Write-Status "Testing build..."

    Push-Location $Path

    if ($WhatIf) {
        Write-Host "  [WhatIf] dotnet build $SolutionName.slnx"
    }
    else {
        $buildResult = & dotnet build "$SolutionName.slnx" 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Status "Build successful" "SUCCESS"
        }
        else {
            Write-Status "Build failed" "ERROR"
            Write-Host $buildResult
            Pop-Location
            exit 1
        }
    }

    Pop-Location
}

function Show-Summary {
    param([string]$Path, [string]$SolutionName)

    Write-Host "`n" + ("=" * 70)
    Write-Status "Backend initialization $(if($WhatIf) {'preview'} else {'completed'})" "SUCCESS"
    Write-Host ("=" * 70)

    Write-Host "`nSolution Structure:"
    Write-Host "  Location: $(Convert-Path $Path)"
    Write-Host "  Solution: $SolutionName.slnx"
    Write-Host "`nProjects (Clean Architecture):"
    Write-Host "  - $SolutionName.Domain (Entities, Business Logic)"
    Write-Host "  - $SolutionName.Application (Use Cases, CQRS)"
    Write-Host "  - $SolutionName.Infrastructure (Data Access, Services)"
    Write-Host "  - $SolutionName.API (HTTP Endpoints, Controllers)"

    Write-Host "`nDependencies:"
    Write-Host "  API           -> Application, Infrastructure"
    Write-Host "  Application   -> Domain"
    Write-Host "  Infrastructure-> Domain"
    Write-Host "  Domain        -> (no dependencies)"

    Write-Host "`nNext Steps:"
    Write-Host "  1. cd $(Convert-Path $Path)"
    if (-not $WhatIf) {
        Write-Host "  2. dotnet build"
        Write-Host "  3. dotnet run --project src/$SolutionName.API"
        Write-Host "`nDocumentation:"
        Write-Host "  See: docs/BACKEND-SETUP.md"
        Write-Host "  See: docs/guides/LOCAL-SETUP.md"
    }

    Write-Host "`n"
}

# Main Execution
Write-Host "`n"
Write-Status "LAMAMedellin Backend Initialization Script" "INFO"
Write-Host "Clean Architecture (.NET 8) - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"

if ($WhatIf) {
    Write-Status "Running in WhatIf mode (preview only)" "WARN"
}

Write-Host ""

Test-Prerequisites

if (-not (Initialize-Solution -Path $SolutionPath -Name $SolutionName) -and -not $Force) {
    Write-Status "Initialization cancelled" "WARN"
    exit 0
}

Create-SolutionFile -Path $SolutionPath -Name $SolutionName
Create-Projects -Path $SolutionPath -SolutionName $SolutionName
Add-ProjectReferences -Path $SolutionPath -SolutionName $SolutionName
Add-ProjectsToSolution -Path $SolutionPath -SolutionName $SolutionName
Create-FolderStructure -Path $SolutionPath -SolutionName $SolutionName

if (-not $WhatIf) {
    Test-Build -Path $SolutionPath -SolutionName $SolutionName
}

Show-Summary -Path $SolutionPath -SolutionName $SolutionName
