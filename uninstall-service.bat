@echo off
echo ========================================
echo   K8s Log Analyzer - Service Uninstaller
echo ========================================
echo.
echo This will uninstall the backend Windows Service
echo Run this script as Administrator!
echo.
pause

REM Check if running as administrator
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: This script must be run as Administrator!
    pause
    exit /b 1
)

echo Stopping service...
sc stop K8sLogAnalyzer

echo Uninstalling service...
sc delete K8sLogAnalyzer

if %errorlevel% equ 0 (
    echo.
    echo Service uninstalled successfully!
) else (
    echo.
    echo ERROR: Service uninstallation failed!
)

echo.
pause
