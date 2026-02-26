@echo off
echo ========================================
echo   K8s Log Analyzer - Publishing
echo ========================================
echo.

echo [1/3] Building Backend...
cd Backend
dotnet publish K8sLogAnalyzer.Api\K8sLogAnalyzer.Api.csproj -c Release -o ..\Published\Backend --self-contained false
if %errorlevel% neq 0 (
    echo ERROR: Backend build failed!
    pause
    exit /b 1
)

echo.
echo [2/3] Building Frontend...
cd ..\Frontend
call npm run build
if %errorlevel% neq 0 (
    echo ERROR: Frontend build failed!
    pause
    exit /b 1
)

echo.
echo [3/3] Copying files...
if not exist "..\Published\Frontend" mkdir "..\Published\Frontend"
xcopy /E /I /Y dist\frontend\browser ..\Published\Frontend

echo.
echo ========================================
echo   Publishing completed successfully!
echo ========================================
echo.
echo Published files are in: Published\
echo - Backend: Published\Backend\
echo - Frontend: Published\Frontend\
echo.
echo To run the application:
echo   1. Run: start-app.bat
echo.
pause
