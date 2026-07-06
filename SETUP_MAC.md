# Setup para macOS

## Scripts disponíveis

Este projeto inclui três scripts para facilitar o desenvolvimento no macOS:

### 1. **setup.command** - Configuração Inicial
- Verifica pré-requisitos (.NET SDK, Node.js, kubectl)
- Restaura dependências do Backend (.NET)
- Instala dependências do Frontend (npm) se Node.js estiver disponível
- **Uso**: Double-click no Finder ou execute:
  ```bash
  ./setup.command
  ```

### 2. **publish.command** - Publicação/Build
- Compila o Backend em Release
- Compila o Frontend (se npm estiver disponível)
- Cria a pasta `Published/Backend` com arquivos prontos para distribuição
- **Uso**: Double-click no Finder ou execute:
  ```bash
  ./publish.command
  ```

### 3. **start-app.command** - Executar Aplicação
- Verifica se a aplicação já foi publicada
- Inicia o Backend (K8s Log Analyzer API)
- Abre o navegador em `http://localhost:5000`
- **Uso**: Double-click no Finder ou execute:
  ```bash
  ./start-app.command
  ```

## Pré-requisitos

### Obrigatório
- **.NET 9 SDK**: Baixe em https://dotnet.microsoft.com/download/dotnet/9.0
  - ⚠️ **IMPORTANTE**: Este projeto requer .NET 9. Se você tem .NET 10 ou superior, pode haver incompatibilidades. Os scripts avisarão se detectarem uma versão incompatível.

### Opcional
- **Node.js 20+**: Necessário para desenvolvimento do Frontend. Baixe em https://nodejs.org
- **kubectl**: Para conectar a clusters Kubernetes. Instruções em https://kubernetes.io/docs/tasks/tools

## Como usar

### Primeira vez
```bash
./setup.command
```

### Para compilar e publicar
```bash
./publish.command
```

### Para executar localmente
```bash
./start-app.command
```

### Para desenvolvimento (terminal)
Backend:
```bash
cd Backend
dotnet run --project K8sLogAnalyzer.Api
```

Frontend:
```bash
cd Frontend
npm start
```

## Troubleshooting

### Erro: "Permission denied"
Execute no terminal:
```bash
chmod +x setup.command publish.command start-app.command
```

### Scripts não encontram comandos
Os scripts carregam suas configurações de shell automaticamente. Se ainda houver problemas, verifique:
1. Que os programas estão instalados: `which dotnet`, `which node`, `which npm`
2. Que estão no PATH do seu shell

### Frontend não está disponível
Se não tiver Node.js instalado, o setup pula a instalação do frontend. Você pode ainda usar o Backend isoladamente ou instalar Node.js depois e rodar `npm install` manualmente na pasta `Frontend/`.

## Versão do .NET - Importante ⚠️

Este projeto requer **.NET 9**. Se você tiver .NET 10 ou superior instalado via Homebrew ou outro método:

1. Instale o .NET 9 via Homebrew:
   ```bash
   brew install dotnet@9
   ```

2. Os scripts `.command` detectam e usam automaticamente o .NET 9 instalado via Homebrew

Se precisar usar manualmente no terminal:
```bash
# Force .NET 9
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@9/libexec"
export PATH="$DOTNET_ROOT:$PATH"
dotnet --version  # deve mostrar 9.x.x
```
