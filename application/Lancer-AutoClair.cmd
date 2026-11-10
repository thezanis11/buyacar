@echo off
set "DOTNET_ROOT=%~dp0.dotnet"
set "DOTNET_ROOT_X64=%~dp0.dotnet"
set "DOTNET_CLI_TELEMETRY_OPTOUT=1"
if not exist "%~dp0dist\desktop\AutoClair.exe" (
  echo Application non compilee. Consultez GUIDE-DEBUTANT.md.
  pause
  exit /b 1
)
start "" "%~dp0dist\desktop\AutoClair.exe" %*
