$ErrorActionPreference = 'Stop'
$appRoot = Split-Path -Parent $PSScriptRoot
$env:DOTNET_ROOT = Join-Path $appRoot '.dotnet'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$ready = $false
try { $r = Invoke-RestMethod 'http://127.0.0.1:5187/api/session' -TimeoutSec 2; $ready = $true } catch {}
if (!$ready) {
    $serverPath = Join-Path $appRoot 'dist/server/Auto.Server.dll'
    $p = Start-Process -FilePath (Join-Path $env:DOTNET_ROOT 'dotnet.exe') -ArgumentList @('"' + $serverPath + '"') -WorkingDirectory (Join-Path $appRoot 'dist/server') -WindowStyle Hidden -PassThru
    for ($i = 0; $i -lt 30; $i++) { Start-Sleep -Milliseconds 300; try { $r = Invoke-RestMethod 'http://127.0.0.1:5187/api/session' -TimeoutSec 1; $ready=$true; break } catch {} }
}
if (!$ready) { throw 'Le service ne répond pas. Consultez GUIDE-DEBUTANT.md.' }
Start-Process 'http://127.0.0.1:5187'
