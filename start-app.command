#!/bin/bash
[ -f ~/.zprofile ] && source ~/.zprofile
[ -f ~/.zshrc ] && source ~/.zshrc

# Force .NET 9 if available
if [ -d "/opt/homebrew/opt/dotnet@9" ]; then
    export DOTNET_ROOT="/opt/homebrew/opt/dotnet@9/libexec"
    export PATH="$DOTNET_ROOT:$PATH"
fi

# Ir para o diretório do script
cd "$(dirname "$0")" || exit 1

echo "========================================"
echo "  K8s Log Analyzer - Starting"
echo "========================================"
echo ""

# Check if published folder exists
if [ ! -f "Published/Backend/K8sLogAnalyzer.Api" ]; then
    echo "ERROR: Application not published yet!"
    echo "Please run: ./publish.command first"
    exit 1
fi

# Get the directory where this script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
APP_DIR="$SCRIPT_DIR/Published/Backend"

echo "Starting K8s Log Analyzer on http://localhost:5000..."

# Start the application in the background
cd "$APP_DIR"
echo "Launching application from: $APP_DIR"
./K8sLogAnalyzer.Api > app.log 2>&1 &
APP_PID=$!

echo "Application PID: $APP_PID"
echo "Waiting for application to start..."
sleep 5

# Check if process is still running
if ! ps -p $APP_PID > /dev/null; then
    echo ""
    echo "ERROR: Application failed to start!"
    echo "Last output:"
    tail -20 app.log
    exit 1
fi

echo ""
echo "========================================"
echo "  Application started successfully!"
echo "========================================"
echo ""
echo "Application: http://localhost:5000"
echo "API Swagger: http://localhost:5000/swagger"
echo ""
echo "Opening browser in 2 seconds..."
sleep 2

# Open browser (macOS specific)
open http://localhost:5000

echo ""
echo "Application is running (PID: $APP_PID)"
echo "Log file: $APP_DIR/app.log"
echo "Press Ctrl+C to stop the application..."
echo ""

# Wait for the process
wait $APP_PID

echo ""
echo "Application stopped."
