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
echo "  K8s Log Analyzer - Publishing"
echo "========================================"
echo ""

# Step 1: Build Backend
echo "[1/3] Building Backend..."
cd Backend
dotnet publish K8sLogAnalyzer.Api/K8sLogAnalyzer.Api.csproj -c Release -o ../Published/Backend --self-contained false
if [ $? -ne 0 ]; then
    echo "ERROR: Backend build failed!"
    exit 1
fi

# Step 2: Build Frontend
echo ""
echo "[2/3] Building Frontend..."
cd ../Frontend
if command -v npm &> /dev/null; then
    npm run build
    if [ $? -ne 0 ]; then
        echo "ERROR: Frontend build failed!"
        cd ..
        exit 1
    fi

    # Step 3: Copy files
    echo ""
    echo "[3/3] Copying files..."
    mkdir -p ../Published/Backend/wwwroot
    cp -r dist/k8s-log-analyzer/browser/* ../Published/Backend/wwwroot/
else
    echo "⚠️  npm not found. Skipping frontend build."
    echo "[3/3] Skipped"
fi

echo ""
echo "========================================"
echo "  Publishing completed successfully!"
echo "========================================"
echo ""
echo "Published files are in: Published/Backend/"
echo "- API: Published/Backend/K8sLogAnalyzer.Api"
echo "- Web Files: Published/Backend/wwwroot/"
echo ""
echo "To run the application:"
echo "  1. Run: ./start-app.sh"
echo ""
