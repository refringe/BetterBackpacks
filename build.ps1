$ErrorActionPreference = 'Stop'

$ScriptDir = $PSScriptRoot
$ClientDir = Join-Path $ScriptDir 'Client'
$ServerDir = Join-Path $ScriptDir 'Server'
$StagingDir = Join-Path $ScriptDir 'build'

# Parse GUIDs and versions from source files
$ClientSource = Get-Content (Join-Path $ClientDir 'BetterBackpacks.cs') -Raw
$ServerSource = Get-Content (Join-Path $ServerDir 'BetterBackpacks.cs') -Raw

if ($ClientSource -match 'BepInPlugin\("([^"]+)",\s*"[^"]+",\s*"([^"]+)"\)') {
    $ClientGuid = $Matches[1]
    $ClientVersion = $Matches[2]
} else {
    throw 'Failed to parse BepInPlugin attribute from Client/BetterBackpacks.cs'
}

if ($ServerSource -match 'ModGuid\s*\{[^}]*\}\s*=\s*"([^"]+)"') {
    $ServerGuid = $Matches[1]
} else {
    throw 'Failed to parse ModGuid from Server/BetterBackpacks.cs'
}

if ($ServerSource -match 'Version\s*\{[^}]*\}\s*=\s*new\("([^"]+)"\)') {
    $ServerVersion = $Matches[1]
} else {
    throw 'Failed to parse Version from Server/BetterBackpacks.cs'
}

# Validate GUIDs and versions match
$errors = @()
if ($ClientGuid -ne $ServerGuid) {
    $errors += "GUID mismatch: Client='$ClientGuid' Server='$ServerGuid'"
}
if ($ClientVersion -ne $ServerVersion) {
    $errors += "Version mismatch: Client='$ClientVersion' Server='$ServerVersion'"
}
if ($errors.Count -gt 0) {
    throw ($errors -join "`n")
}

$Version = $ClientVersion
$ZipName = "BetterBackpacks-${Version}.zip"
$ZipPath = Join-Path $ScriptDir $ZipName

Write-Host "Building BetterBackpacks v${Version}..."

# Build both projects in Release configuration
Write-Host 'Building Client...'
dotnet build $ClientDir -c Release -v quiet
if ($LASTEXITCODE -ne 0) { throw 'Client build failed' }

Write-Host 'Building Server...'
dotnet build $ServerDir -c Release -v quiet
if ($LASTEXITCODE -ne 0) { throw 'Server build failed' }

# Clean and create staging directory
if (Test-Path $StagingDir) { Remove-Item $StagingDir -Recurse -Force }
New-Item -ItemType Directory -Path (Join-Path $StagingDir 'BepInEx\plugins') -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $StagingDir 'SPT\user\mods\Refringe-BetterBackpacks') -Force | Out-Null

# Copy DLLs to staging
Copy-Item (Join-Path $ClientDir 'bin\Release\netstandard2.1\BetterBackpacks.dll') `
          (Join-Path $StagingDir 'BepInEx\plugins\BetterBackpacks.dll')

Copy-Item (Join-Path $ServerDir 'bin\Release\BetterBackpacks.dll') `
          (Join-Path $StagingDir 'SPT\user\mods\Refringe-BetterBackpacks\BetterBackpacks.dll')

# Create zip
if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
Compress-Archive -Path (Join-Path $StagingDir 'BepInEx'), (Join-Path $StagingDir 'SPT') `
                 -DestinationPath $ZipPath

# Clean up staging
Remove-Item $StagingDir -Recurse -Force

Write-Host "Created $ZipName"
