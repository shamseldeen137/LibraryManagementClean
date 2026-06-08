param(
    [Parameter(Mandatory=$true)] [string]$SourceFolder,
    [Parameter(Mandatory=$true)] [string]$IISSiteName,
    [Parameter(Mandatory=$true)] [string]$AppPoolName,
    [Parameter(Mandatory=$true)] [string]$DestinationPath
)

Write-Host "Deploy script starting"
Write-Host "SourceFolder=$SourceFolder"
Write-Host "IISSiteName=$IISSiteName"
Write-Host "AppPoolName=$AppPoolName"
Write-Host "DestinationPath=$DestinationPath"

try {
    Import-Module WebAdministration -ErrorAction Stop
} catch {
    Write-Error "Failed to import WebAdministration module. Ensure IIS and WebAdministration module are installed and the script is run as Administrator."
    exit 1
}

if (-not (Test-Path -Path $SourceFolder)) {
    Write-Error "Source folder not found: $SourceFolder"
    exit 1
}

if (-not (Test-Path -Path $DestinationPath)) {
    Write-Host "Destination path does not exist; creating: $DestinationPath"
    New-Item -ItemType Directory -Path $DestinationPath -Force | Out-Null
}

try {
    Write-Host "Stopping App Pool: $AppPoolName"
    Stop-WebAppPool -Name $AppPoolName -ErrorAction Stop
} catch {
    Write-Warning "Could not stop app pool '$AppPoolName'. It may not exist or an error occurred: $_"
}

try {
    Write-Host "Removing existing files from destination"
    Get-ChildItem -Path $DestinationPath -Force | Where-Object { $_.Name -ne 'web.config' -or $false } | ForEach-Object {
        Remove-Item -LiteralPath $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
} catch {
    Write-Warning "Failed to remove some files: $_"
}

try {
    Write-Host "Copying files from $SourceFolder to $DestinationPath"
    Copy-Item -Path (Join-Path $SourceFolder '*') -Destination $DestinationPath -Recurse -Force -ErrorAction Stop
} catch {
    Write-Error "Failed to copy files: $_"
    exit 1
}

try {
    Write-Host "Starting App Pool: $AppPoolName"
    Start-WebAppPool -Name $AppPoolName -ErrorAction Stop
} catch {
    Write-Warning "Could not start app pool '$AppPoolName': $_"
}

Write-Host "Deployment complete"
exit 0
