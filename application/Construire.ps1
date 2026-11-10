$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
$env:DOTNET_ROOT = Join-Path $PSScriptRoot '.dotnet'
$env:DOTNET_CLI_HOME = Join-Path $PSScriptRoot '.cli'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:NUGET_PACKAGES = Join-Path $PSScriptRoot '.nuget'
$sdk = Join-Path $env:DOTNET_ROOT 'dotnet.exe'
if (!(Test-Path -LiteralPath $sdk)) { throw 'SDK absent. Consulter GUIDE-DEBUTANT.md.' }
& $sdk publish Auto.Server -c Release -o dist/server
if ($LASTEXITCODE -ne 0) { throw 'La compilation du serveur a échoué.' }
& $sdk publish Auto.Desktop -c Release -o dist/desktop
if ($LASTEXITCODE -ne 0) { throw 'La compilation Windows a échoué.' }
& $sdk run --project Auto.Tests -c Release
if ($LASTEXITCODE -ne 0) { throw 'Les tests ont échoué.' }
Write-Host 'AutoClair est prêt. Utilisez Lancer-AutoClair.cmd.'
