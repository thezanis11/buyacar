$ErrorActionPreference='Stop'
$appRoot=Split-Path -Parent $PSScriptRoot
$shell=New-Object -ComObject WScript.Shell
$shortcut=$shell.CreateShortcut((Join-Path ([Environment]::GetFolderPath('Desktop')) 'AutoClair.lnk'))
$shortcut.TargetPath=Join-Path $appRoot 'Lancer-AutoClair.cmd'
$shortcut.WorkingDirectory=$appRoot
$shortcut.WindowStyle=7
$shortcut.Description='Votre espace automobile privé'
$shortcut.Save()
$startup=Join-Path ([Environment]::GetFolderPath('Startup')) 'AutoClair.cmd'
$launcher=Join-Path $appRoot 'Lancer-AutoClair.cmd'
Set-Content -LiteralPath $startup -Encoding ascii -Value ('@echo off' + "`r`n" + 'start "" /min "' + $launcher + '" --background')
Write-Host 'Raccourci AutoClair créé sur le Bureau. Démarrage à l’ouverture de session activé.'
Write-Host 'Pour le désactiver : clic droit sur AutoClair dans la zone de notification.'
Read-Host 'Appuyez sur Entrée pour fermer'
