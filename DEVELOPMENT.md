# K8s Log Analyzer - Development Guide

## Configuração do Ambiente

### 1. Instalação de Dependências

#### Backend
```bash
cd Backend
dotnet restore
dotnet build
```

#### Frontend
```bash
cd Frontend
npm install
```

### 2. Executando em Modo Desenvolvimento

#### Opção 1: Manualmente

**Terminal 1 - Backend:**
```bash
cd Backend
dotnet watch run --project K8sLogAnalyzer.Api
```

**Terminal 2 - Frontend:**
```bash
cd Frontend
npm start
```

#### Opção 2: VS Code Tasks

1. Pressione `Ctrl+Shift+P`
2. Digite "Tasks: Run Task"
3. Selecione "Start All"

### 3. Testando com Kubernetes Local

#### Usando Minikube

```bash
# Iniciar minikube
minikube start

# Criar um pod de teste
kubectl run nginx-test --image=nginx:latest

# Obter o nome completo do pod
kubectl get pods

# Usar no frontend
# Namespace: default
# Pod Name: nginx-test
```

#### Usando Kind

```bash
# Criar cluster
kind create cluster --name log-analyzer

# Deploy de teste
kubectl create deployment nginx --image=nginx
kubectl get pods
```

## Estrutura de Desenvolvimento

### Backend - Clean Architecture

```
K8sLogAnalyzer.Api/          → Controllers, Program.cs
├── Controllers/
│   └── LogsController.cs    → Endpoint /api/logs

K8sLogAnalyzer.Application/  → Lógica de negócio
├── DTOs/                    → Data Transfer Objects
├── Interfaces/              → Contratos de serviços
└── Services/                → Implementação de lógica

K8sLogAnalyzer.Infrastructure/ → Implementações técnicas
├── Kubernetes/              → Cliente K8s
└── Parsers/                 → Parser de logs
```

### Frontend - Angular Standalone

```
src/app/
├── components/
│   └── log-viewer/         → Componente principal
├── models/                 → Interfaces TypeScript
└── services/               → HTTP services
```

## Debugging

### Backend (.NET)

1. Abra `Backend/K8sLogAnalyzer.sln` no Visual Studio ou VS Code
2. Defina breakpoints nos controllers ou services
3. Pressione `F5` ou use a configuração ".NET Core Launch (web)"

### Frontend (Angular)

1. Instale a extensão "Debugger for Chrome"
2. Pressione `F5` ou use a configuração "Launch Chrome"
3. Use as Chrome DevTools para debugging

## Troubleshooting

### Backend não consegue conectar ao K8s

**Erro:** `Unable to load kubeconfig`

**Solução:**
```bash
# Verificar se kubectl está configurado
kubectl cluster-info

# Verificar arquivo de config
ls ~/.kube/config  # Linux/Mac
dir %USERPROFILE%\.kube\config  # Windows
```

### Frontend não consegue acessar API

**Erro:** `CORS policy blocked`

**Solução:** Verifique se o backend está rodando e o CORS está configurado em `Program.cs`:

```csharp
app.UseCors("AllowAngularApp");
```

### Pod não encontrado (404)

**Erros comuns:**
- Nome do pod incorreto (use tab-completion: `kubectl get pods -n <namespace>`)
- Namespace incorreto
- Pod não existe mais (pods são efêmeros)

## Testing

### Backend - Unit Tests (exemplo)

```bash
cd Backend
dotnet test
```

### Frontend - Unit Tests

```bash
cd Frontend
npm test
```

### Integration Testing

Use Postman ou curl:

```bash
# Listar pods disponíveis
kubectl get pods --all-namespaces

# Testar endpoint
curl "http://localhost:5000/api/logs?namespace=kube-system&podName=coredns-abc123"
```

## Build para Produção

### Backend

```bash
cd Backend
dotnet publish -c Release -o ./publish
```

### Frontend

```bash
cd Frontend
npm run build
```

Arquivos otimizados estarão em `Frontend/dist/`.

## Docker (Opcional)

### Backend Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY publish/ .
ENTRYPOINT ["dotnet", "K8sLogAnalyzer.Api.dll"]
```

### Frontend Dockerfile

```dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/k8s-log-analyzer /usr/share/nginx/html
```

## Extensões VS Code Recomendadas

- C# Dev Kit
- Angular Language Service
- Kubernetes
- REST Client
- Prettier

## Performance Tips

1. **Backend**: Use `ConfigureAwait(false)` em métodos async
2. **Frontend**: Use `ChangeDetectionStrategy.OnPush` em componentes
3. **Logs grandes**: Implemente paginação server-side para +10k linhas
4. **Caching**: Considere cache Redis para logs recentes

## Contribuindo

1. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
2. Commit: `git commit -m "feat: adiciona nova funcionalidade"`
3. Push: `git push origin feature/nova-funcionalidade`
4. Abra um Pull Request

## Referências

- [Kubernetes Client .NET](https://github.com/kubernetes-client/csharp)
- [Angular Material](https://material.angular.io/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
