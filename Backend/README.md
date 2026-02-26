# K8s Log Analyzer - Backend

Backend API em .NET 9 para análise de logs do Kubernetes.

## Pré-requisitos

- .NET 9 SDK
- Acesso a um cluster Kubernetes configurado (~/.kube/config)

## Executar o Projeto

```bash
cd Backend
dotnet restore
dotnet run --project K8sLogAnalyzer.Api
```

A API estará disponível em: `http://localhost:5000`

## Swagger UI

Acesse: `http://localhost:5000/swagger`

## Endpoints

### GET /api/logs

Recupera os logs de um pod específico no Kubernetes.

**Parâmetros:**
- `namespace` (query, obrigatório): Nome do namespace
- `podName` (query, obrigatório): Nome do pod

**Exemplo:**
```
GET http://localhost:5000/api/logs?namespace=default&podName=my-pod-123
```

**Resposta:**
```json
[
  {
    "timestamp": "2026-02-26T10:30:45.123Z",
    "level": "INFO",
    "message": "Application started successfully"
  },
  {
    "timestamp": "2026-02-26T10:30:46.456Z",
    "level": "ERROR",
    "message": "Connection timeout"
  }
]
```

## Tratamento de Erros

- **400 Bad Request**: Parâmetros inválidos ou ausentes
- **403 Forbidden**: Sem permissões RBAC no cluster
- **404 Not Found**: Pod não encontrado
- **500 Internal Server Error**: Erro interno do servidor

## Configuração

Edite `appsettings.json` para personalizar as configurações.
