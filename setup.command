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

echo "🚀 K8s Log Analyzer - Quick Start Script"
echo "========================================="
echo ""

# Check prerequisites
echo "📋 Checking prerequisites..."

# Check for .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 9 SDK."
    exit 1
fi

# Check for Node.js
if ! command -v node &> /dev/null; then
    echo "⚠️  Node.js not found. Please install Node.js 20+ if you plan to develop frontend."
fi

# Check for kubectl (warning, not critical)
if ! command -v kubectl &> /dev/null; then
    echo "⚠️  kubectl not found. Make sure you have access to a Kubernetes cluster."
fi

echo "✅ Prerequisites check completed"
echo ""

# Backend setup
echo "🔧 Setting up Backend..."
cd Backend
dotnet restore
if [ $? -ne 0 ]; then
    echo "❌ Backend restore failed"
    exit 1
fi
echo "✅ Backend ready"
cd ..
echo ""

# Frontend setup
echo "🔧 Setting up Frontend..."
cd Frontend
if command -v npm &> /dev/null; then
    if [ ! -d "node_modules" ]; then
        npm install
        if [ $? -ne 0 ]; then
            echo "❌ Frontend npm install failed"
            cd ..
            exit 1
        fi
    fi
    echo "✅ Frontend ready"
else
    echo "⚠️  npm not found. Skipping frontend setup. Install Node.js 20+ if needed."
fi
cd ..
echo ""

echo "✅ Setup completed successfully!"
echo ""
echo "📝 To start the application:"
echo "   Backend:  cd Backend && dotnet run --project K8sLogAnalyzer.Api"
echo "   Frontend: cd Frontend && npm start"
echo ""
echo "Or use VS Code tasks: Cmd+Shift+P -> Tasks: Run Task -> Start All"
