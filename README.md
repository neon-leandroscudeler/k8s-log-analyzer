# K8s Log Analyzer

Sistema completo de monitoramento e análise de logs do Kubernetes com Backend em .NET 9 e Frontend em Angular 19.

![Architecture](https://img.shields.io/badge/Architecture-Cloud%20Native-blue)
![Backend](https://img.shields.io/badge/Backend-.NET%209-purple)
![Frontend](https://img.shields.io/badge/Frontend-Angular%2019-red)

## 📋 Visão Geral

O **K8sLogAnalyzer** é uma solução moderna e eficiente para visualização e análise de logs de pods do Kubernetes. Utiliza o SDK oficial do Kubernetes Client para integração direta com clusters K8s, proporcionando alta performance e segurança.

### Principais Características

✅ **Backend .NET 9** com Clean Architecture  
✅ **Frontend Angular 19** com Material Design  
✅ **Integração nativa** com Kubernetes usando SDK oficial  
✅ **Parser inteligente** de logs com suporte a ISO 8601  
✅ **UI responsiva** com ordenação, paginação e filtros  
✅ **Diferenciação visual** por nível de log (ERROR, WARN, INFO, DEBUG)  
✅ **Tratamento robusto** de erros (RBAC, Pod não encontrado, etc.)  
✅ **CORS configurado** para desenvolvimento local

## 🏗️ Arquitetura

```
K8sLogAnalyzer/
├── Backend/                          # API .NET 9
│   ├── K8sLogAnalyzer.Api/          # Camada de Apresentação (Controllers)
│   ├── K8sLogAnalyzer.Application/  # Camada de Aplicação (Services, DTOs)
│   └── K8sLogAnalyzer.Infrastructure/ # Camada de Infraestrutura (K8s Client, Parsers)
│
└── Frontend/                         # SPA Angular 19
    └── src/app/
        ├── components/               # Componentes UI
        ├── services/                 # Serviços HTTP
        └── models/                   # Interfaces TypeScript
```

### Fluxo de Dados

```
Angular App (port 4200)
     ↓
  HTTP GET /api/logs?namespace={ns}&podName={pod}
     ↓
ASP.NET Core API (port 5000)
     ↓
KubernetesClient SDK → K8s Cluster
     ↓
Pod Logs (raw strings)
     ↓
LogParser → Structured JSON
     ↓
Response → Angular Material Table
```

## 🚀 Quick Start

### Pré-requisitos

- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 20+** - [Download](https://nodejs.org/)
- **Angular CLI 19** - `npm install -g @angular/cli`
- **Kubectl configurado** - Arquivo `~/.kube/config` válido com acesso ao cluster

### 1️⃣ Backend (.NET 9)

```bash
cd Backend
dotnet restore
dotnet run --project K8sLogAnalyzer.Api
```

Backend disponível em: **http://localhost:5000**  
Swagger UI: **http://localhost:5000/swagger**

### 2️⃣ Frontend (Angular 19)

```bash
cd Frontend
npm install
npm start
```

Frontend disponível em: **http://localhost:4200**

## 📡 API Endpoints

### GET /api/logs

Recupera e analisa logs de um pod específico no Kubernetes.

**Parâmetros Query:**
- `namespace` (obrigatório): Nome do namespace K8s
- `podName` (obrigatório): Nome do pod

**Exemplo de Requisição:**
```http
GET http://localhost:5000/api/logs?namespace=default&podName=my-app-pod-abc123
```

**Resposta de Sucesso (200 OK):**
```json
[
  {
    "timestamp": "2026-02-26T10:15:30.123Z",
    "level": "INFO",
    "message": "Application started successfully"
  },
  {
    "timestamp": "2026-02-26T10:15:31.456Z",
    "level": "ERROR",
    "message": "Database connection timeout after 30s"
  },
  {
    "timestamp": "2026-02-26T10:15:32.789Z",
    "level": "WARN",
    "message": "Retry attempt 1 of 3"
  }
]
```

**Códigos de Erro:**
- `400 Bad Request` - Parâmetros inválidos ou ausentes
- `403 Forbidden` - Sem permissões RBAC no cluster
- `404 Not Found` - Pod não encontrado no namespace
- `500 Internal Server Error` - Erro interno do servidor

## 🎨 Funcionalidades do Frontend

### Filtros de Busca
- Inputs para **Namespace** e **Pod Name**
- Botão **Consultar** com loading spinner
- Validação de campos obrigatórios

### Data Table (Angular Material)
- **Colunas**: Horário, Tipo, Mensagem
- **MatSort**: Ordenação por clique no cabeçalho
- **MatPaginator**: 10, 25, 50 ou 100 itens por página
- **Filtro Client-Side**: Busca em tempo real em todas as colunas

### Diferenciação Visual
| Nível | Cor | Chip |
|-------|-----|------|
| ERROR | Vermelho | 🔴 |
| WARN  | Laranja  | 🟠 |
| INFO  | Azul     | 🔵 |
| DEBUG | Cinza    | ⚪ |

## 🔧 Configuração

### Backend - appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "KubernetesConfig": {
    "ConfigPath": "~/.kube/config"
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  }
}
```

### Frontend - Configuração da API

Edite `src/app/services/log.service.ts`:

```typescript
private apiUrl = 'http://localhost:5000/api/logs';
```

## 🛡️ Segurança e Boas Práticas

### Backend
✅ Uso do **KubernetesClient SDK** oficial (sem shell injection)  
✅ Validação de entrada com `ArgumentException`  
✅ Tratamento específico de exceções HTTP (404, 403)  
✅ Logging estruturado com `ILogger`  
✅ Dependency Injection para testabilidade  
✅ CORS configurado apenas para desenvolvimento

### Frontend
✅ Standalone Components (Angular 19)  
✅ Reactive programming com RxJS  
✅ Type-safety com TypeScript strict mode  
✅ Material Design para consistência UX  
✅ Error handling com MatSnackBar

## 📦 Tecnologias Utilizadas

### Backend Stack
- **.NET 9** - Framework moderno e performático
- **ASP.NET Core Web API** - REST API
- **KubernetesClient 15.0.1** - SDK oficial do Kubernetes
- **Swashbuckle** - Documentação OpenAPI/Swagger

### Frontend Stack
- **Angular 19** - Framework SPA
- **Angular Material** - Biblioteca de componentes UI
- **RxJS** - Programação reativa
- **TypeScript** - Linguagem tipada

## 🧪 Testando a Aplicação

### 1. Verificar conectividade com K8s

```bash
kubectl get pods --all-namespaces
```

### 2. Iniciar Backend

```bash
cd Backend
dotnet run --project K8sLogAnalyzer.Api
```

### 3. Iniciar Frontend

```bash
cd Frontend
npm start
```

### 4. Acessar a aplicação

Abra **http://localhost:4200** no navegador e:
1. Digite um **namespace** válido (ex: `default`, `kube-system`)
2. Digite um **pod name** completo (ex: `coredns-abc123`)
3. Clique em **Consultar**
4. Visualize os logs estruturados na tabela

## 📝 Estrutura de Log Parsing

O parser identifica automaticamente:

**Padrão reconhecido:**
```
2026-02-26T10:30:45.123Z INFO User authentication successful
```

**Resultado estruturado:**
```json
{
  "timestamp": "2026-02-26T10:30:45.123Z",
  "level": "INFO",
  "message": "User authentication successful"
}
```

**Formatos de data suportados:**
- ISO 8601 com milissegundos: `2026-02-26T10:30:45.123Z`
- ISO 8601 sem milissegundos: `2026-02-26T10:30:45Z`
- Com timezone: `2026-02-26T10:30:45.123+03:00`
- Formato alternativo: `2026-02-26 10:30:45.123`

**Níveis de log reconhecidos:**
`INFO`, `ERROR`, `WARN`, `WARNING`, `DEBUG`, `TRACE`, `FATAL`

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto é open source e está disponível sob a [MIT License](LICENSE).

## 👨‍💻 Autor

Desenvolvido como solução Cloud Native para análise de logs Kubernetes.

---

**⭐ Se este projeto foi útil, considere dar uma estrela!**
