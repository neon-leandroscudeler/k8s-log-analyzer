#!/bin/bash

echo "🚀 K8s Log Analyzer - Quick Start Script"
echo "========================================="
echo ""

# Check prerequisites
echo "📋 Checking prerequisites..."

if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 9 SDK."
    exit 1
fi

if ! command -v node &> /dev/null; then
    echo "❌ Node.js not found. Please install Node.js 20+."
    exit 1
fi

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
if [ ! -d "node_modules" ]; then
    npm install
    if [ $? -ne 0 ]; then
        echo "❌ Frontend npm install failed"
        exit 1
    fi
fi
echo "✅ Frontend ready"
cd ..
echo ""

echo "✅ Setup completed successfully!"
echo ""
echo "📝 To start the application:"
echo "   Backend:  cd Backend && dotnet run --project K8sLogAnalyzer.Api"
echo "   Frontend: cd Frontend && npm start"
echo ""
echo "Or use VS Code tasks: Ctrl+Shift+P -> Tasks: Run Task -> Start All"
