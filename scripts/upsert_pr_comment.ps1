[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Repo,

    [Parameter(Mandatory = $true)]
    [int]$PullRequestNumber,

    [string]$Marker = '<!-- release-notes-docs-spec-v1 -->',

    [string]$BodyText,

    [string]$BodyFile,

    [switch]$VerifyOnly
)

$ErrorActionPreference = 'Stop'

function Get-CommentBody {
    param(
        [string]$Text,
        [string]$File,
        [string]$CommentMarker
    )

    if ($Text) {
        return "$CommentMarker`n$Text"
    }

    if ($File) {
        if (-not (Test-Path -LiteralPath $File)) {
            throw "Body file not found: $File"
        }

        $content = Get-Content -LiteralPath $File -Raw -Encoding UTF8
        return "$CommentMarker`n$content"
    }

    throw 'Provide -BodyText or -BodyFile when not using -VerifyOnly.'
}

function Get-Comments {
    param(
        [string]$Repository,
        [int]$IssueNumber
    )

    $json = gh api "repos/$Repository/issues/$IssueNumber/comments?per_page=100"
    if (-not $json) {
        return @()
    }

    return @($json | ConvertFrom-Json)
}

$comments = Get-Comments -Repository $Repo -IssueNumber $PullRequestNumber
$existing = $comments | Where-Object { $_.body -like "*$Marker*" } | Select-Object -First 1

if ($VerifyOnly) {
    if ($existing) {
        Write-Output ("verified=true")
        Write-Output ("comment_id=" + $existing.id)
        Write-Output ("comment_url=" + $existing.html_url)
        exit 0
    }

    Write-Output 'verified=false'
    exit 1
}

$body = Get-CommentBody -Text $BodyText -File $BodyFile -CommentMarker $Marker

if ($existing) {
    $updated = gh api -X PATCH "repos/$Repo/issues/comments/$($existing.id)" -f body="$body" | ConvertFrom-Json
    Write-Output 'action=updated'
    Write-Output ("comment_id=" + $updated.id)
    Write-Output ("comment_url=" + $updated.html_url)
}
else {
    $created = gh api -X POST "repos/$Repo/issues/$PullRequestNumber/comments" -f body="$body" | ConvertFrom-Json
    Write-Output 'action=created'
    Write-Output ("comment_id=" + $created.id)
    Write-Output ("comment_url=" + $created.html_url)
}
