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

echo Starting K8s Log Analyzer on http://localhost:5000...
start "K8s Log Analyzer" /D "%~dp0Published\Backend" K8sLogAnalyzer.Api.exe

echo Waiting for application to start...
timeout /t 3 /nobreak > nul

echo.
echo ========================================
echo   Application started successfully!
echo ========================================
echo.
echo Application: http://localhost:5000
echo API Swagger: http://localhost:5000/swagger
echo.
echo Opening browser...
timeout /t 2 /nobreak > nul
start http://localhost:5000

echo.
echo Press any key to stop the application...
pause > nul

echo.
echo Stopping application...
taskkill /FI "WINDOWTITLE eq K8s Log Analyzer" /F > nul 2>&1

echo Application stopped.
pause
