@echo off
echo ========================================
echo   K8s Log Analyzer - Starting
echo ========================================
echo.

REM Check if published folder exists
if not exist "Published\Backend\K8sLogAnalyzer.Api.exe" (
    echo ERROR: Application not published yet!
    echo Please run: publish.bat first
    pause
    exit /b 1
)

echo Starting Backend API on http://localhost:5000...
start "K8s Log Analyzer - Backend" /D "%~dp0Published\Backend" K8sLogAnalyzer.Api.exe

echo Waiting for backend to start...
timeout /t 3 /nobreak > nul

echo Starting Frontend on http://localhost:8080...
start "K8s Log Analyzer - Frontend" /D "%~dp0Published\Frontend" cmd /k "npx serve -s . -l 8080"

echo.
echo ========================================
echo   Application started successfully!
echo ========================================
echo.
echo Backend API: http://localhost:5000
echo Frontend:    http://localhost:8080
echo.
echo Opening browser...
timeout /t 2 /nobreak > nul
start http://localhost:8080

echo.
echo Press any key to stop the application...
pause > nul

echo.
echo Stopping application...
taskkill /FI "WINDOWTITLE eq K8s Log Analyzer - Backend" /F > nul 2>&1
taskkill /FI "WINDOWTITLE eq K8s Log Analyzer - Frontend" /F > nul 2>&1

echo Application stopped.
pause
