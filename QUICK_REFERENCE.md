# Quick Reference - Comandos Úteis

## 🚀 Inicialização Rápida

### Setup Completo (Windows)
```cmd
setup.bat
```

### Setup Completo (Linux/Mac)
```bash
chmod +x setup.sh
./setup.sh
```

### Iniciar Backend
```bash
cd Backend
dotnet run --project K8sLogAnalyzer.Api
```

### Iniciar Frontend
```bash
cd Frontend
npm start
```

### Docker Compose (tudo junto)
```bash
docker-compose up --build
```

---

## 🔍 Kubernetes - Comandos Essenciais

### Listar Pods
```bash
# Todos os namespaces
kubectl get pods --all-namespaces

# Namespace específico
kubectl get pods -n kube-system

# Com mais detalhes
kubectl get pods -o wide

# Watch mode (atualização em tempo real)
kubectl get pods -w
```

### Ver Logs Diretamente
```bash
# Logs completos
kubectl logs pod-name -n namespace

# Últimas 100 linhas
kubectl logs pod-name -n namespace --tail=100

# Seguir logs em tempo real
kubectl logs -f pod-name -n namespace

# Logs de container específico (se pod tem múltiplos)
kubectl logs pod-name -n namespace -c container-name
```

### Criar Pods de Teste
```bash
# NGINX
kubectl run nginx-test --image=nginx:latest
kubectl expose pod nginx-test --port=80

# Busybox com logs contínuos
kubectl run log-generator --image=busybox -- sh -c "i=0; while true; do echo \$(date) INFO Message \$i; i=\$((i+1)); sleep 1; done"

# Apache HTTP Server
kubectl run apache-test --image=httpd:latest

# Redis
kubectl run redis-test --image=redis:latest
```

### Deletar Pods de Teste
```bash
kubectl delete pod nginx-test
kubectl delete pod log-generator
kubectl delete pod apache-test
kubectl delete pod redis-test
```

---

## 🧪 Testes da API

### curl - Requisições Básicas
```bash
# Requisição simples
curl "http://localhost:5000/api/logs?namespace=default&podName=nginx-test"

# Com formatação JSON (usando jq)
curl -s "http://localhost:5000/api/logs?namespace=default&podName=nginx-test" | jq '.'

# Salvar em arquivo
curl "http://localhost:5000/api/logs?namespace=default&podName=nginx-test" -o logs.json

# Ver apenas headers
curl -I "http://localhost:5000/api/logs?namespace=default&podName=nginx-test"

# Com timeout
curl --max-time 10 "http://localhost:5000/api/logs?namespace=default&podName=nginx-test"
```

### PowerShell - Requisições
```powershell
# Requisição simples
Invoke-RestMethod -Uri "http://localhost:5000/api/logs?namespace=default&podName=nginx-test"

# Com tratamento de erro
try {
    $logs = Invoke-RestMethod -Uri "http://localhost:5000/api/logs?namespace=default&podName=test"
    $logs | ConvertTo-Json -Depth 10
} catch {
    Write-Error $_.Exception.Message
}

# Filtrar apenas ERRORs
$logs = Invoke-RestMethod -Uri "http://localhost:5000/api/logs?namespace=default&podName=app"
$logs | Where-Object { $_.level -eq "ERROR" }

# Salvar em arquivo
Invoke-RestMethod -Uri "http://localhost:5000/api/logs?namespace=default&podName=app" | ConvertTo-Json -Depth 10 | Out-File logs.json
```

---

## 🛠️ Desenvolvimento

### Backend - .NET Commands
```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run --project K8sLogAnalyzer.Api

# Watch mode (auto-reload)
dotnet watch run --project K8sLogAnalyzer.Api

# Clean
dotnet clean

# Publish para produção
dotnet publish -c Release -o ./publish

# Ver versão do .NET
dotnet --version

# Listar projetos na solution
dotnet sln list
```

### Frontend - npm Commands
```bash
# Instalar dependências
npm install

# Iniciar dev server
npm start

# Build para produção
npm run build

# Executar testes
npm test

# Limpar cache
npm cache clean --force

# Atualizar Angular CLI
npm install -g @angular/cli@latest

# Ver versão
ng version

# Criar novo componente
ng generate component nome-componente

# Criar novo serviço
ng generate service nome-servico
```

---

## 🐳 Docker

### Build de Imagens
```bash
# Backend
docker build -t k8s-log-analyzer-api:latest -f Backend/Dockerfile Backend/

# Frontend
docker build -t k8s-log-analyzer-frontend:latest -f Frontend/Dockerfile Frontend/
```

### Executar Containers
```bash
# Backend
docker run -d -p 5000:5000 --name api \
  -v ~/.kube/config:/root/.kube/config:ro \
  k8s-log-analyzer-api:latest

# Frontend
docker run -d -p 4200:80 --name frontend \
  k8s-log-analyzer-frontend:latest
```

### Docker Compose
```bash
# Iniciar todos os serviços
docker-compose up

# Em background
docker-compose up -d

# Rebuild e start
docker-compose up --build

# Parar
docker-compose down

# Ver logs
docker-compose logs -f

# Ver status
docker-compose ps
```

### Comandos Úteis
```bash
# Listar imagens
docker images

# Listar containers rodando
docker ps

# Listar todos os containers
docker ps -a

# Ver logs de um container
docker logs -f container-name

# Entrar no container
docker exec -it container-name sh

# Remover container
docker rm -f container-name

# Remover imagem
docker rmi image-name

# Limpar tudo
docker system prune -a
```

---

## 🔧 VS Code

### Atalhos Úteis
```
Ctrl+Shift+P    - Command Palette
Ctrl+`          - Toggle Terminal
Ctrl+B          - Toggle Sidebar
Ctrl+Shift+F    - Search in Files
Ctrl+P          - Quick Open File
F5              - Start Debugging
Ctrl+F5         - Run Without Debugging
```

### Tasks (Ctrl+Shift+P -> Tasks: Run Task)
- **Backend: Run** - Inicia a API
- **Frontend: Serve** - Inicia o Angular
- **Start All** - Inicia Backend e Frontend juntos

---

## 📊 Monitoramento

### Ver Logs da Aplicação
```bash
# Backend logs (se rodando via dotnet run)
# Logs aparecem diretamente no terminal

# Frontend logs (Angular dev server)
# Logs aparecem no navegador (F12 -> Console)

# Docker logs
docker logs -f api
docker logs -f frontend
```

### Health Checks
```bash
# Backend (Swagger)
curl http://localhost:5000/swagger

# Frontend
curl http://localhost:4200/health
```

---

## 🐛 Troubleshooting

### Resetar Tudo
```bash
# Backend
cd Backend
dotnet clean
rm -rf **/bin **/obj
dotnet restore

# Frontend
cd Frontend
rm -rf node_modules package-lock.json .angular
npm install
```

### Verificar Portas em Uso
```bash
# Linux/Mac
lsof -i :5000
lsof -i :4200

# Windows
netstat -ano | findstr :5000
netstat -ano | findstr :4200
```

### Matar Processo na Porta
```bash
# Linux/Mac
kill -9 $(lsof -ti:5000)

# Windows
# Encontre o PID com netstat acima, depois:
taskkill /PID <PID> /F
```

---

## 📦 Deploy em Kubernetes

### Deploy do Backend
```bash
# Criar namespace
kubectl create namespace log-analyzer

# Deploy
kubectl apply -f k8s/backend-deployment.yaml -n log-analyzer

# Verificar
kubectl get pods -n log-analyzer
kubectl logs -f deployment/log-analyzer-api -n log-analyzer

# Expor serviço
kubectl expose deployment log-analyzer-api --port=5000 --type=LoadBalancer -n log-analyzer
```

### Deploy do Frontend
```bash
# Deploy
kubectl apply -f k8s/frontend-deployment.yaml -n log-analyzer

# Verificar
kubectl get pods -n log-analyzer

# Expor serviço
kubectl expose deployment log-analyzer-frontend --port=80 --type=LoadBalancer -n log-analyzer
```

### Obter URLs dos Serviços
```bash
# Minikube
minikube service list

# LoadBalancer externo
kubectl get svc -n log-analyzer
```

---

## 📚 Recursos Úteis

### Documentação
- **Swagger UI**: http://localhost:5000/swagger
- **Angular Material**: https://material.angular.io/
- **Kubernetes Client .NET**: https://github.com/kubernetes-client/csharp

### Logs de Exemplo
```bash
# Ver exemplos de logs
cat EXAMPLES.md

# Ver FAQ
cat FAQ.md

# Ver guia de desenvolvimento
cat DEVELOPMENT.md
```

---

## 💡 Dicas Rápidas

### Find & Replace em Múltiplos Arquivos
```bash
# Trocar porta do backend no frontend
grep -r "localhost:5000" Frontend/src/
sed -i 's/localhost:5000/api.exemplo.com/g' Frontend/src/app/services/log.service.ts
```

### Backup da Configuração
```bash
# Backup do kubeconfig
cp ~/.kube/config ~/.kube/config.backup

# Backup dos appsettings
cp Backend/K8sLogAnalyzer.Api/appsettings.json Backend/K8sLogAnalyzer.Api/appsettings.backup.json
```

### Verificar Performance
```bash
# Tempo de resposta da API
time curl "http://localhost:5000/api/logs?namespace=default&podName=nginx-test"

# Tamanho da resposta
curl -s "http://localhost:5000/api/logs?namespace=default&podName=nginx-test" | wc -c
```

---

**💾 Salve este arquivo como favorito para referência rápida!**
