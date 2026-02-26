@echo off
echo 🚀 K8s Log Analyzer - Quick Start Script
echo =========================================
echo.

REM Check prerequisites
echo 📋 Checking prerequisites...

where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ❌ .NET SDK not found. Please install .NET 9 SDK.
    exit /b 1
)

where node >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Node.js not found. Please install Node.js 20+.
    exit /b 1
)

where kubectl >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ⚠️  kubectl not found. Make sure you have access to a Kubernetes cluster.
)

echo ✅ Prerequisites check completed
echo.

REM Backend setup
echo 🔧 Setting up Backend...
cd Backend
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Backend restore failed
    exit /b 1
)
echo ✅ Backend ready
cd ..
echo.

REM Frontend setup
echo 🔧 Setting up Frontend...
cd Frontend
if not exist "node_modules" (
    call npm install
    if %ERRORLEVEL% NEQ 0 (
        echo ❌ Frontend npm install failed
        exit /b 1
    )
)
echo ✅ Frontend ready
cd ..
echo.

echo ✅ Setup completed successfully!
echo.
echo 📝 To start the application:
echo    Backend:  cd Backend ^&^& dotnet run --project K8sLogAnalyzer.Api
echo    Frontend: cd Frontend ^&^& npm start
echo.
echo Or use VS Code tasks: Ctrl+Shift+P -^> Tasks: Run Task -^> Start All

pause
