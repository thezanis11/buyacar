$ErrorActionPreference='Stop'
$appRoot=Split-Path -Parent $PSScriptRoot
$addresses=Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.IPAddress -match '^(192\.168\.|10\.|172\.(1[6-9]|2[0-9]|3[01])\.)' -and $_.PrefixOrigin -ne 'WellKnown' }
Write-Host 'Adresses locales de ce PC :'
$addresses | Select-Object IPAddress,InterfaceAlias | Format-Table
$address=Read-Host 'Copiez l’adresse correspondant à votre Wi-Fi ou Ethernet domestique'
$env:DOTNET_ROOT=Join-Path $appRoot '.dotnet'
& (Join-Path $env:DOTNET_ROOT 'dotnet.exe') (Join-Path $appRoot 'dist/server/Auto.Server.dll') --configure-lan $address
if ($LASTEXITCODE -ne 0) { throw 'Configuration non effectuée.' }
Write-Host 'Le certificat public est à installer sur votre téléphone. Consultez GUIDE-DEBUTANT.md.'
Write-Host 'Le pare-feu doit autoriser TCP 5188 uniquement sur réseau Privé et sous-réseau local. Aucun port du routeur ne doit être ouvert.'
Read-Host 'Appuyez sur Entrée pour fermer'
