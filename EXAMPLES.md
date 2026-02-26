# API Examples

## Exemplos de Requisições

### 1. Logs de um Pod do CoreDNS (kube-system)

**Request:**
```http
GET http://localhost:5000/api/logs?namespace=kube-system&podName=coredns-abc123
```

**Response (200 OK):**
```json
[
  {
    "timestamp": "2026-02-26T10:15:30.123Z",
    "level": "INFO",
    "message": "[INFO] plugin/reload: Running configuration SHA512 = abc123..."
  },
  {
    "timestamp": "2026-02-26T10:15:31.456Z",
    "level": "INFO",
    "message": "CoreDNS-1.11.1 linux/amd64, go1.21.5, notshipped"
  },
  {
    "timestamp": "2026-02-26T10:15:32.789Z",
    "level": "INFO",
    "message": "53 [INFO] plugin/health: Going into lameduck mode for 30s"
  }
]
```

### 2. Logs de Aplicação com Erros

**Request:**
```http
GET http://localhost:5000/api/logs?namespace=default&podName=my-app-xyz789
```

**Response (200 OK):**
```json
[
  {
    "timestamp": "2026-02-26T14:30:45.001Z",
    "level": "INFO",
    "message": "Starting application on port 8080"
  },
  {
    "timestamp": "2026-02-26T14:30:46.123Z",
    "level": "INFO",
    "message": "Database connection established"
  },
  {
    "timestamp": "2026-02-26T14:30:50.456Z",
    "level": "ERROR",
    "message": "Failed to process request: Connection timeout after 30s"
  },
  {
    "timestamp": "2026-02-26T14:30:51.789Z",
    "level": "WARN",
    "message": "Retry attempt 1 of 3"
  },
  {
    "timestamp": "2026-02-26T14:30:52.012Z",
    "level": "ERROR",
    "message": "Retry failed: java.net.SocketTimeoutException: Read timed out"
  },
  {
    "timestamp": "2026-02-26T14:30:53.345Z",
    "level": "WARN",
    "message": "Falling back to cache"
  },
  {
    "timestamp": "2026-02-26T14:30:54.678Z",
    "level": "INFO",
    "message": "Request completed with cached data (200ms)"
  }
]
```

### 3. Pod Não Encontrado

**Request:**
```http
GET http://localhost:5000/api/logs?namespace=default&podName=non-existent-pod
```

**Response (404 Not Found):**
```json
{
  "error": "Pod 'non-existent-pod' not found in namespace 'default'"
}
```

### 4. Sem Permissão RBAC

**Request:**
```http
GET http://localhost:5000/api/logs?namespace=restricted&podName=secret-pod
```

**Response (403 Forbidden):**
```json
{
  "error": "Access denied. Check RBAC permissions for namespace 'restricted'"
}
```

### 5. Parâmetros Inválidos

**Request:**
```http
GET http://localhost:5000/api/logs?namespace=&podName=test
```

**Response (400 Bad Request):**
```json
{
  "error": "Namespace parameter is required"
}
```

## Exemplos de Logs Reais do Kubernetes

### NGINX Ingress Controller

```
2026-02-26T10:00:01.123Z INFO 192.168.1.100 - - [26/Feb/2026:10:00:01 +0000] "GET /health HTTP/1.1" 200 0 "-" "kube-probe/1.28"
2026-02-26T10:00:02.456Z INFO 192.168.1.101 - - [26/Feb/2026:10:00:02 +0000] "POST /api/users HTTP/1.1" 201 1234 "-" "curl/7.68.0"
2026-02-26T10:00:03.789Z WARN Upstream timed out (110: Connection timed out) while connecting to upstream
2026-02-26T10:00:04.012Z ERROR Failed to establish SSL connection to upstream
```

### PostgreSQL

```
2026-02-26T11:30:00.000Z INFO database system is ready to accept connections
2026-02-26T11:30:15.123Z INFO LOG:  checkpoint starting: time
2026-02-26T11:30:20.456Z WARN WARNING:  could not open statistics file "pg_stat_tmp/global.stat": No such file or directory
2026-02-26T11:31:00.789Z ERROR ERROR:  relation "users" does not exist at character 15
2026-02-26T11:31:01.012Z ERROR STATEMENT:  SELECT * FROM users WHERE id = 123
```

### Redis

```
2026-02-26T15:00:00.000Z INFO Server initialized
2026-02-26T15:00:00.123Z INFO Ready to accept connections
2026-02-26T15:00:15.456Z WARN Client id=12345 addr=192.168.1.50:12345 fd=8 age=300 idle=60 closed by client
2026-02-26T15:00:30.789Z ERROR OOM command not allowed when used memory > 'maxmemory'
```

### MongoDB

```
2026-02-26T16:00:00.000Z INFO NETWORK  [initandlisten] waiting for connections on port 27017
2026-02-26T16:00:05.123Z INFO REPL     [replexec-0] Starting replication coordinator
2026-02-26T16:00:10.456Z WARN STORAGE  [WTCheckpointThread] Taking too long to checkpoint
2026-02-26T16:00:15.789Z ERROR COMMAND [conn123] command mydb.$cmd command: find { find: "users" } failed
```

## Testando com curl

### Comando básico
```bash
curl -X GET "http://localhost:5000/api/logs?namespace=default&podName=nginx-test"
```

### Com jq para formatação
```bash
curl -s "http://localhost:5000/api/logs?namespace=default&podName=nginx-test" | jq '.'
```

### Salvar resposta em arquivo
```bash
curl "http://localhost:5000/api/logs?namespace=kube-system&podName=coredns-abc" -o logs.json
```

### Com headers
```bash
curl -H "Accept: application/json" \
     "http://localhost:5000/api/logs?namespace=default&podName=my-app"
```

## Testando com PowerShell

```powershell
# Requisição simples
Invoke-RestMethod -Uri "http://localhost:5000/api/logs?namespace=default&podName=nginx-test"

# Com tratamento de erro
try {
    $logs = Invoke-RestMethod -Uri "http://localhost:5000/api/logs?namespace=default&podName=test"
    $logs | ConvertTo-Json -Depth 10
} catch {
    Write-Error "Failed: $($_.Exception.Message)"
}

# Filtrar apenas ERRORs
$logs = Invoke-RestMethod -Uri "http://localhost:5000/api/logs?namespace=default&podName=app"
$logs | Where-Object { $_.level -eq "ERROR" } | Format-Table
```

## Swagger UI

Acesse a documentação interativa da API:

```
http://localhost:5000/swagger
```

Features do Swagger:
- 📖 Documentação completa dos endpoints
- 🧪 Executar requisições diretamente no browser
- 📝 Ver schemas de request/response
- 🔍 Testar diferentes cenários de erro
