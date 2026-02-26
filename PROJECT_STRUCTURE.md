# 📁 K8s Log Analyzer - Estrutura Completa do Projeto

## Visão Geral da Estrutura

```
c:\Git\Logs.View\
│
├── 📄 README.md                      # Documentação principal
├── 📄 LICENSE                        # Licença MIT
├── 📄 .gitignore                     # Arquivos ignorados pelo Git
├── 📄 docker-compose.yml             # Orquestração Docker
├── 📄 setup.bat                      # Script de setup (Windows)
├── 📄 setup.sh                       # Script de setup (Linux/Mac)
├── 📄 DEVELOPMENT.md                 # Guia de desenvolvimento
├── 📄 EXAMPLES.md                    # Exemplos de uso da API
├── 📄 FAQ.md                         # Perguntas frequentes
├── 📄 QUICK_REFERENCE.md             # Referência rápida de comandos
│
├── .vscode/                          # Configurações do VS Code
│   ├── extensions.json               # Extensões recomendadas
│   ├── launch.json                   # Configurações de debug
│   └── tasks.json                    # Tasks automatizadas
│
├── Backend/                          # 🔧 API .NET 9
│   ├── K8sLogAnalyzer.sln           # Solution do Visual Studio
│   ├── Dockerfile                    # Container do backend
│   ├── README.md                     # Documentação do backend
│   │
│   ├── K8sLogAnalyzer.Api/          # Camada de Apresentação
│   │   ├── K8sLogAnalyzer.Api.csproj
│   │   ├── Program.cs                # Configuração da aplicação
│   │   ├── appsettings.json          # Configurações
│   │   ├── appsettings.Development.json
│   │   └── Controllers/
│   │       └── LogsController.cs     # Endpoint /api/logs
│   │
│   ├── K8sLogAnalyzer.Application/  # Camada de Aplicação
│   │   ├── K8sLogAnalyzer.Application.csproj
│   │   ├── DTOs/                     # Data Transfer Objects
│   │   │   ├── LogEntryDto.cs        # Modelo de log estruturado
│   │   │   └── LogQueryRequest.cs    # Request model
│   │   ├── Interfaces/               # Contratos de serviços
│   │   │   ├── ILogService.cs
│   │   │   ├── IKubernetesService.cs
│   │   │   └── ILogParser.cs
│   │   └── Services/                 # Lógica de negócio
│   │       └── LogService.cs         # Orquestração de logs
│   │
│   └── K8sLogAnalyzer.Infrastructure/ # Camada de Infraestrutura
│       ├── K8sLogAnalyzer.Infrastructure.csproj
│       ├── Kubernetes/               # Cliente K8s
│       │   └── KubernetesService.cs  # SDK do Kubernetes
│       └── Parsers/                  # Processamento de logs
│           └── LogParser.cs          # Parser ISO 8601 + níveis
│
└── Frontend/                         # 🎨 Angular 19
    ├── package.json                  # Dependências npm
    ├── angular.json                  # Configuração Angular
    ├── tsconfig.json                 # TypeScript config
    ├── tsconfig.app.json             # TypeScript app config
    ├── Dockerfile                    # Container do frontend
    ├── nginx.conf                    # Configuração NGINX
    ├── README.md                     # Documentação do frontend
    │
    └── src/
        ├── index.html                # HTML principal
        ├── main.ts                   # Bootstrap da aplicação
        ├── styles.scss               # Estilos globais
        │
        └── app/
            ├── app.component.ts      # Componente raiz
            ├── app.config.ts         # Configuração da app
            │
            ├── models/               # Interfaces TypeScript
            │   └── log-entry.model.ts # Modelo de log
            │
            ├── services/             # Serviços HTTP
            │   └── log.service.ts    # Comunicação com API
            │
            └── components/           # Componentes UI
                └── log-viewer/       # Componente principal
                    ├── log-viewer.component.ts       # Lógica
                    ├── log-viewer.component.html     # Template
                    └── log-viewer.component.scss     # Estilos
```

---

## 📊 Estatísticas do Projeto

### Backend (.NET 9)
- **3 Projetos** (Api, Application, Infrastructure)
- **Clean Architecture** com separação de responsabilidades
- **4 Controllers** e endpoints RESTful
- **3 Interfaces** e implementações de serviços
- **2 DTOs** para contratos de dados
- **1 Parser** inteligente de logs com regex
- **SDK Kubernetes** oficial para integração

### Frontend (Angular 19)
- **1 Componente** principal standalone
- **1 Serviço** HTTP para comunicação com API
- **1 Interface** TypeScript para type-safety
- **Angular Material** completo (Table, Sort, Paginator, Forms)
- **Responsive Design** com SCSS modular
- **Filtros em tempo real** client-side

### Documentação
- **7 Arquivos Markdown** de documentação
- **2 Scripts** de setup (Windows + Linux/Mac)
- **4 Arquivos Docker** (2 Dockerfiles + compose + nginx)
- **3 Configurações VS Code** (launch, tasks, extensions)
- **2 READMEs** específicos (Backend + Frontend)

---

## 🎯 Funcionalidades Implementadas

### Backend
✅ Endpoint GET `/api/logs`  
✅ Integração com Kubernetes SDK  
✅ Autenticação via `~/.kube/config`  
✅ Parser de logs com ISO 8601  
✅ Detecção de níveis de log (INFO, ERROR, WARN, DEBUG)  
✅ Tratamento de erros (404, 403, 400, 500)  
✅ CORS configurado para Angular  
✅ Swagger/OpenAPI documentação  
✅ Logging estruturado com ILogger  
✅ Dependency Injection  

### Frontend
✅ Formulário de busca (Namespace + Pod Name)  
✅ MatTable com dados estruturados  
✅ MatSort - Ordenação por coluna  
✅ MatPaginator - Paginação (10, 25, 50, 100)  
✅ Filtro em tempo real (todas as colunas)  
✅ Chips coloridos por nível de log  
✅ Loading spinner  
✅ Notificações com MatSnackBar  
✅ Tratamento de erros amigável  
✅ Design responsivo  

---

## 🔌 Tecnologias e Pacotes

### Backend Stack
| Tecnologia | Versão | Propósito |
|------------|--------|-----------|
| .NET | 9.0 | Framework principal |
| ASP.NET Core | 9.0 | Web API |
| KubernetesClient | 15.0.1 | SDK oficial K8s |
| Swashbuckle | 6.8.0 | Swagger/OpenAPI |

### Frontend Stack
| Tecnologia | Versão | Propósito |
|------------|--------|-----------|
| Angular | 19.0 | Framework SPA |
| Angular Material | 19.0 | Componentes UI |
| RxJS | 7.8 | Programação reativa |
| TypeScript | 5.6 | Linguagem tipada |

---

## 🚀 Comandos de Inicialização Rápida

### Setup Automático
```bash
# Windows
setup.bat

# Linux/Mac
chmod +x setup.sh && ./setup.sh
```

### Execução Manual

**Backend:**
```bash
cd Backend
dotnet restore
dotnet run --project K8sLogAnalyzer.Api
# Disponível em: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

**Frontend:**
```bash
cd Frontend
npm install
npm start
# Disponível em: http://localhost:4200
```

### Docker Compose
```bash
docker-compose up --build
# Backend:  http://localhost:5000
# Frontend: http://localhost:4200
```

---

## 📝 Arquivos de Configuração Importantes

### Backend
- `appsettings.json` - Configurações da API (porta, logging, CORS)
- `Program.cs` - Bootstrap e DI
- `K8sLogAnalyzer.sln` - Solution do Visual Studio

### Frontend
- `package.json` - Dependências npm
- `angular.json` - Build e serve config
- `tsconfig.json` - Configuração TypeScript
- `nginx.conf` - Config NGINX para produção

### DevOps
- `docker-compose.yml` - Orquestração de containers
- `Backend/Dockerfile` - Build da API
- `Frontend/Dockerfile` - Build do Angular + NGINX

### VS Code
- `.vscode/launch.json` - Debug configurations
- `.vscode/tasks.json` - Tasks automatizadas
- `.vscode/extensions.json` - Extensões recomendadas

---

## 🔐 Segurança e Compliance

✅ **Sem shell injection** - Uso do SDK oficial K8s  
✅ **Validação de entrada** - ArgumentException para parâmetros inválidos  
✅ **Tratamento de RBAC** - Erros 403 específicos  
✅ **CORS restrito** - Apenas localhost:4200 em desenvolvimento  
✅ **Type-safety** - TypeScript strict mode  
✅ **Dependency Injection** - Testabilidade e manutenibilidade  
✅ **Logging estruturado** - Auditoria de requisições  

---

## 📚 Documentação Disponível

| Arquivo | Conteúdo |
|---------|----------|
| [README.md](README.md) | Visão geral e quick start |
| [DEVELOPMENT.md](DEVELOPMENT.md) | Guia de desenvolvimento |
| [EXAMPLES.md](EXAMPLES.md) | Exemplos de uso da API |
| [FAQ.md](FAQ.md) | Perguntas frequentes |
| [QUICK_REFERENCE.md](QUICK_REFERENCE.md) | Comandos úteis |
| [Backend/README.md](Backend/README.md) | Documentação da API |
| [Frontend/README.md](Frontend/README.md) | Documentação do Angular |

---

## 🎨 UI/UX Features

### Cores dos Logs
```scss
ERROR   → 🔴 Vermelho (#f44336)
WARN    → 🟠 Laranja  (#ff9800)
INFO    → 🔵 Azul     (#2196f3)
DEBUG   → ⚪ Cinza    (#9e9e9e)
```

### Componentes Material
- **MatTable** - Tabela de dados
- **MatSort** - Ordenação
- **MatPaginator** - Paginação
- **MatFormField** - Campos de input
- **MatButton** - Botões
- **MatCard** - Cards
- **MatChip** - Chips de log level
- **MatSpinner** - Loading
- **MatIcon** - Ícones
- **MatSnackBar** - Notificações

---

## 🧪 Como Testar

### 1. Criar Pod de Teste
```bash
kubectl run nginx-test --image=nginx:latest
```

### 2. Obter Nome Completo
```bash
kubectl get pods
# Output: nginx-test
```

### 3. Usar no Frontend
- Namespace: `default`
- Pod Name: `nginx-test`
- Clicar em **Consultar**

### 4. Verificar Logs Estruturados
Logs aparecem na tabela com:
- ✅ Data/hora formatada
- ✅ Nível de log com chip colorido
- ✅ Mensagem parseada

---

## 🔄 Fluxo de Dados Completo

```
┌─────────────────┐
│  Angular App    │ http://localhost:4200
│  (Frontend)     │
└────────┬────────┘
         │ HTTP GET /api/logs?namespace=X&podName=Y
         ▼
┌─────────────────┐
│  ASP.NET Core   │ http://localhost:5000
│  API (Backend)  │
└────────┬────────┘
         │ LogsController.GetLogs()
         ▼
┌─────────────────┐
│  LogService     │ Application Layer
└────────┬────────┘
         ├─► KubernetesService.GetPodLogsAsync()
         │   └─► KubernetesClient SDK
         │       └─► K8s API Server
         │           └─► Pod Logs (raw string)
         │
         └─► LogParser.ParseLogs()
             └─► Regex matching + ISO 8601
                 └─► Structured JSON
                     └─► List<LogEntryDto>
                         ▼
                   ┌─────────────┐
                   │  Response   │
                   │  JSON Array │
                   └──────┬──────┘
                          │
                          ▼
                   ┌─────────────┐
                   │  MatTable   │ Angular Material
                   │  + Sort     │
                   │  + Paginator│
                   │  + Filter   │
                   └─────────────┘
```

---

## ✅ Checklist de Funcionalidades

### Backend API
- [x] Endpoint GET /api/logs
- [x] Query params: namespace, podName
- [x] Integração com K8s via SDK
- [x] Parser de logs ISO 8601
- [x] Detecção de log levels
- [x] Tratamento de erros HTTP
- [x] CORS configurado
- [x] Swagger UI
- [x] Logging
- [x] DI Container

### Frontend Angular
- [x] Componente standalone
- [x] Formulário de busca
- [x] Validação de campos
- [x] MatTable com dados
- [x] MatSort (ordenação)
- [x] MatPaginator (paginação)
- [x] Filtro client-side
- [x] Chips coloridos
- [x] Loading spinner
- [x] Tratamento de erros
- [x] Notificações

### DevOps
- [x] Dockerfile (Backend)
- [x] Dockerfile (Frontend)
- [x] docker-compose.yml
- [x] Scripts de setup
- [x] .gitignore
- [x] VS Code configs

### Documentação
- [x] README principal
- [x] READMEs específicos
- [x] Guia de desenvolvimento
- [x] Exemplos de API
- [x] FAQ
- [x] Quick reference
- [x] Licença MIT

---

## 🎓 Conceitos Aplicados

### Arquitetura
- **Clean Architecture** - Separação em camadas
- **Dependency Inversion** - Interfaces e abstrações
- **Single Responsibility** - Cada classe com um propósito
- **SOLID Principles** - Design patterns

### Patterns
- **Repository Pattern** - Acesso a dados
- **Service Layer** - Lógica de negócio
- **DTO Pattern** - Contratos de dados
- **Dependency Injection** - Inversão de controle

### Best Practices
- **Async/Await** - Código não bloqueante
- **Type Safety** - TypeScript strict mode
- **Error Handling** - Try-catch e status codes
- **Logging** - Rastreabilidade
- **CORS** - Segurança cross-origin
- **Docker** - Containerização
- **Documentation** - Código documentado

---

## 📈 Possíveis Extensões Futuras

### Backend
- [ ] Filtro por timestamp range
- [ ] Suporte a múltiplos containers
- [ ] WebSocket para logs em tempo real
- [ ] Cache com Redis
- [ ] Rate limiting
- [ ] Autenticação JWT
- [ ] Métricas com Prometheus

### Frontend
- [ ] Exportar logs (CSV, JSON, PDF)
- [ ] Dark mode
- [ ] Múltiplos clusters
- [ ] Dashboard de métricas
- [ ] Alertas configuráveis
- [ ] Histórico de buscas
- [ ] Compartilhar logs via link

---

**🎉 Projeto Completo e Pronto para Uso!**

Para começar, execute:
```bash
setup.bat  # Windows
# OU
./setup.sh # Linux/Mac
```

Depois visite:
- Backend: http://localhost:5000/swagger
- Frontend: http://localhost:4200
