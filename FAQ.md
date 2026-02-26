# Frequently Asked Questions (FAQ)

## Instalação e Configuração

### ❓ Quais são os requisitos mínimos para rodar o projeto?

**Backend:**
- .NET 9 SDK
- Acesso a um cluster Kubernetes (local ou remoto)
- Arquivo ~/.kube/config válido

**Frontend:**
- Node.js 20 ou superior
- npm 10+ ou yarn

### ❓ Como configurar o acesso ao cluster Kubernetes?

1. **Instalando kubectl:**
   ```bash
   # Linux
   curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
   
   # macOS
   brew install kubectl
   
   # Windows
   choco install kubernetes-cli
   ```

2. **Configurando o cluster:**
   - Para clusters gerenciados (EKS, AKS, GKE), use os comandos específicos do provedor
   - Para Minikube: `minikube start`
   - Para Kind: `kind create cluster`

3. **Verificando a configuração:**
   ```bash
   kubectl cluster-info
   kubectl get nodes
   ```

### ❓ O backend não encontra o arquivo kubeconfig

**Erro:** `Unable to load kubeconfig`

**Soluções:**

1. Verificar se o arquivo existe:
   ```bash
   # Linux/Mac
   ls -la ~/.kube/config
   
   # Windows
   dir %USERPROFILE%\.kube\config
   ```

2. Definir a variável de ambiente:
   ```bash
   export KUBECONFIG=~/.kube/config  # Linux/Mac
   set KUBECONFIG=%USERPROFILE%\.kube\config  # Windows
   ```

3. Copiar de outro local:
   ```bash
   mkdir -p ~/.kube
   cp /path/to/your/config ~/.kube/config
   ```

## Problemas com o Backend

### ❓ Erro "Pod not found" mesmo com pod existente

**Causas comuns:**

1. **Nome do pod incorreto**: Pods têm sufixos aleatórios
   ```bash
   # ERRADO
   podName=nginx
   
   # CORRETO (copie o nome completo)
   kubectl get pods
   podName=nginx-deployment-7d9c8f5b4d-x7k2m
   ```

2. **Namespace incorreto**:
   ```bash
   kubectl get pods --all-namespaces | grep nginx
   ```

3. **Pod foi recriado** (novos pods têm nomes diferentes):
   ```bash
   kubectl get pods -w  # watch mode
   ```

### ❓ Erro 403 Forbidden - Access Denied

**Causa:** Sem permissões RBAC para acessar logs do pod.

**Solução:** Criar uma ServiceAccount com permissões adequadas:

```yaml
apiVersion: v1
kind: ServiceAccount
metadata:
  name: log-viewer
  namespace: default
---
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: pod-log-reader
  namespace: default
rules:
- apiGroups: [""]
  resources: ["pods", "pods/log"]
  verbs: ["get", "list"]
---
apiVersion: rbac.authorization.k8s.io/v1
kind: RoleBinding
metadata:
  name: read-pod-logs
  namespace: default
subjects:
- kind: ServiceAccount
  name: log-viewer
  namespace: default
roleRef:
  kind: Role
  name: pod-log-reader
  apiGroup: rbac.authorization.k8s.io
```

Aplicar:
```bash
kubectl apply -f rbac.yaml
```

### ❓ Backend demora muito para responder

**Causas:**

1. **Pod com muitos logs**: Implementar paginação ou limitar linhas
2. **Cluster remoto lento**: Verificar latência de rede
3. **Parser ineficiente**: Otimizar regex

**Soluções:**

Modificar `KubernetesService.cs` para limitar linhas:

```csharp
var logs = await _kubernetesClient.CoreV1.ReadNamespacedPodLogAsync(
    name: podName,
    namespaceParameter: namespaceName,
    tailLines: 1000,  // Últimas 1000 linhas
    cancellationToken: cancellationToken
);
```

## Problemas com o Frontend

### ❓ Erro CORS ao fazer requisição

**Erro:** `Access to XMLHttpRequest blocked by CORS policy`

**Causa:** Backend não está configurado para aceitar requisições do Angular.

**Solução:**

Verificar `Program.cs` no backend:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ...

app.UseCors("AllowAngularApp");
```

### ❓ Frontend não carrega os logs

**Checklist de debug:**

1. **Backend está rodando?**
   ```bash
   curl http://localhost:5000/api/logs?namespace=default&podName=test
   ```

2. **URL da API está correta?**
   Verificar `log.service.ts`:
   ```typescript
   private apiUrl = 'http://localhost:5000/api/logs';
   ```

3. **Inspecionar o navegador:**
   - Abrir DevTools (F12)
   - Aba Network
   - Verificar requisição e resposta

### ❓ Tabela não ordena corretamente

**Problema:** Clicar no header não ordena.

**Solução:** Verificar se `MatSort` está vinculado corretamente:

```typescript
@ViewChild(MatSort) sort!: MatSort;

ngAfterViewInit(): void {
  this.dataSource.sort = this.sort;  // Certifique-se que existe
}
```

### ❓ Paginação não funciona

**Problema:** Todas as linhas aparecem sem paginação.

**Solução:** Verificar se `MatPaginator` está vinculado:

```typescript
@ViewChild(MatPaginator) paginator!: MatPaginator;

ngAfterViewInit(): void {
  this.dataSource.paginator = this.paginator;
}
```

## Kubernetes Local

### ❓ Qual ferramenta usar para K8s local?

| Ferramenta | Prós | Contras | Recomendado para |
|------------|------|---------|------------------|
| **Minikube** | Fácil de usar, completo | Consome recursos | Desenvolvimento geral |
| **Kind** | Rápido, leve | Menos features | CI/CD, testes |
| **k3s** | Muito leve | Setup manual | IoT, edge |
| **Docker Desktop** | Integrado no Docker | Licença comercial | Desenvolvedores Windows/Mac |

### ❓ Como criar pods de teste?

**Pod simples (nginx):**
```bash
kubectl run nginx-test --image=nginx:latest
kubectl logs nginx-test
```

**Deployment com múltiplas réplicas:**
```bash
kubectl create deployment webapp --image=httpd:latest --replicas=3
kubectl get pods
```

**Pod que gera logs continuamente:**
```bash
kubectl run log-generator --image=busybox -- sh -c "while true; do echo $(date) INFO Log entry; sleep 1; done"
kubectl logs -f log-generator
```

## Performance e Otimização

### ❓ Como melhorar a performance com muitos logs?

**Backend:**

1. **Limitar linhas retornadas:**
   ```csharp
   tailLines: 1000  // Últimas 1000 linhas
   ```

2. **Adicionar cache:**
   ```csharp
   services.AddMemoryCache();
   services.AddResponseCaching();
   ```

3. **Comprimir resposta:**
   ```csharp
   builder.Services.AddResponseCompression(options =>
   {
       options.EnableForHttps = true;
   });
   ```

**Frontend:**

1. **Virtual scrolling** para grandes datasets:
   ```typescript
   import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
   ```

2. **Lazy loading** de logs:
   ```typescript
   loadMore() {
     // Carregar próxima página
   }
   ```

### ❓ Como filtrar logs por nível antes de enviar ao frontend?

Adicionar parâmetro ao endpoint:

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<LogEntryDto>>> GetLogs(
    [FromQuery] string @namespace,
    [FromQuery] string podName,
    [FromQuery] string? level = null)  // Novo parâmetro
{
    var logs = await _logService.GetPodLogsAsync(@namespace, podName);
    
    if (!string.IsNullOrWhiteSpace(level))
    {
        logs = logs.Where(l => l.Level.Equals(level, StringComparison.OrdinalIgnoreCase));
    }
    
    return Ok(logs);
}
```

## Deploy em Produção

### ❓ Como fazer deploy para produção?

**Backend (Docker):**

```dockerfile
# Backend/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "K8sLogAnalyzer.Api.dll"]
```

```bash
docker build -t k8s-log-analyzer-api:v1.0 -f Backend/Dockerfile Backend/
docker run -p 5000:5000 k8s-log-analyzer-api:v1.0
```

**Frontend (NGINX):**

```dockerfile
# Frontend/Dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/k8s-log-analyzer /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
```

### ❓ Preciso mudar algo para deploy em cluster K8s?

**Sim!** O backend precisa usar ServiceAccount do pod:

```csharp
// KubernetesService.cs
public KubernetesService()
{
    KubernetesClientConfiguration config;
    
    if (KubernetesClientConfiguration.IsInCluster())
    {
        // Rodando dentro do cluster
        config = KubernetesClientConfiguration.InClusterConfig();
    }
    else
    {
        // Desenvolvimento local
        config = KubernetesClientConfiguration.BuildDefaultConfig();
    }
    
    _kubernetesClient = new k8s.Kubernetes(config);
}
```

**Deployment YAML:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: log-analyzer-api
spec:
  replicas: 2
  selector:
    matchLabels:
      app: log-analyzer-api
  template:
    metadata:
      labels:
        app: log-analyzer-api
    spec:
      serviceAccountName: log-viewer  # ServiceAccount com RBAC
      containers:
      - name: api
        image: k8s-log-analyzer-api:v1.0
        ports:
        - containerPort: 5000
```

## Contribuição e Desenvolvimento

### ❓ Como posso contribuir com o projeto?

1. Fork o repositório
2. Crie uma branch: `git checkout -b feature/minha-feature`
3. Faça suas alterações
4. Execute os testes
5. Commit: `git commit -m "feat: adiciona funcionalidade X"`
6. Push: `git push origin feature/minha-feature`
7. Abra um Pull Request

### ❓ Existe roadmap de features futuras?

**Planejado:**

- [ ] Exportar logs para CSV/JSON
- [ ] Filtro por intervalo de tempo
- [ ] Suporte a múltiplos containers no mesmo pod
- [ ] Websocket para streaming de logs em tempo real
- [ ] Dashboard com métricas agregadas
- [ ] Autenticação e autorização
- [ ] Suporte a logs de múltiplos clusters
- [ ] Alertas baseados em padrões de log

## Contato e Suporte

### ❓ Onde reportar bugs ou pedir ajuda?

- **Issues no GitHub**: Para bugs e feature requests
- **Discussões**: Para perguntas gerais
- **Pull Requests**: Para contribuições de código

### ❓ Qual a licença do projeto?

MIT License - Código aberto e gratuito para uso comercial e pessoal.
