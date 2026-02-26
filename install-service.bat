@echo off
echo ========================================
echo   K8s Log Analyzer - Service Installer
echo ========================================
echo.
echo This will install the backend as a Windows Service
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

echo Installing backend as Windows Service...
sc create K8sLogAnalyzer binPath= "%~dp0Published\Backend\K8sLogAnalyzer.Api.exe" start= auto DisplayName= "K8s Log Analyzer Backend"

if %errorlevel% equ 0 (
    echo.
    echo Service installed successfully!
    echo Starting service...
    sc start K8sLogAnalyzer
    echo.
    echo Backend will now start automatically with Windows.
) else (
    echo.
    echo ERROR: Service installation failed!
)

echo.
pause
