@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0tools\Installer-Raccourcis.ps1"
if errorlevel 1 pause
