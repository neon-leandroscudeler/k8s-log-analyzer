# 🚀 Guia de Publicação - K8s Log Analyzer

## Opções de Publicação

### ✅ Opção 1: Executar com Scripts (Mais Simples)

**Passo 1: Publicar a aplicação**
```cmd
publish.bat
```
Isso irá:
- Compilar o backend para `Published\Backend\`
- Compilar o frontend para `Published\Frontend\`

**Passo 2: Iniciar a aplicação**
```cmd
start-app.bat
```
Isso irá:
- Iniciar o backend em http://localhost:5000
- Iniciar o frontend em http://localhost:8080
- Abrir automaticamente o navegador

**Para parar**: Pressione qualquer tecla no console

---

### ✅ Opção 2: Instalar como Serviço do Windows (Produção)

**Passo 1: Publicar**
```cmd
publish.bat
```

**Passo 2: Instalar o backend como serviço** (executar como Administrador)
```cmd
install-service.bat
```

O backend agora inicia automaticamente com o Windows!

**Passo 3: Servir o frontend**

**Opção A - IIS (Recomendado para Produção):**
1. Abra o IIS Manager
2. Crie um novo site:
   - Nome: K8sLogAnalyzer
   - Caminho físico: `C:\Git\Logs.View\Published\Frontend`
   - Binding: http, porta 8080
3. Instale URL Rewrite Module: https://www.iis.net/downloads/microsoft/url-rewrite
4. Adicione `web.config` na pasta Frontend com regra de SPA

**Opção B - Servidor HTTP Simples:**
```cmd
cd Published\Frontend
npx serve -s . -l 8080
```

**Para desinstalar o serviço:**
```cmd
uninstall-service.bat
```

---

### ✅ Opção 3: IIS Completo (Produção Empresarial)

#### Backend no IIS

1. **Publicar backend:**
   ```cmd
   cd Backend
   dotnet publish K8sLogAnalyzer.Api\K8sLogAnalyzer.Api.csproj -c Release -o ..\Published\Backend
   ```

2. **Instalar .NET Hosting Bundle:**
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0
   - Instale: "ASP.NET Core Runtime 9.0.x - Windows Hosting Bundle"

3. **Configurar IIS:**
   - Abra IIS Manager
   - Crie Application Pool:
     - Nome: K8sLogAnalyzerBackend
     - .NET CLR Version: No Managed Code
   - Crie Site:
     - Nome: K8sLogAnalyzer.Api
     - Application Pool: K8sLogAnalyzerBackend
     - Caminho: `C:\Git\Logs.View\Published\Backend`
     - Binding: http, porta 5000

#### Frontend no IIS

1. **Criar web.config para SPA:**
   ```cmd
   echo ^<?xml version="1.0" encoding="UTF-8"?^> > Published\Frontend\web.config
   ```

2. Copie este conteúdo para `Published\Frontend\web.config`:
   ```xml
   <?xml version="1.0" encoding="UTF-8"?>
   <configuration>
     <system.webServer>
       <rewrite>
         <rules>
           <rule name="Angular Routes" stopProcessing="true">
             <match url=".*" />
             <conditions logicalGrouping="MatchAll">
               <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
               <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
             </conditions>
             <action type="Rewrite" url="/" />
           </rule>
         </rules>
       </rewrite>
       <staticContent>
         <mimeMap fileExtension=".json" mimeType="application/json" />
       </staticContent>
     </system.webServer>
   </configuration>
   ```

3. **Configurar IIS:**
   - Crie Application Pool: K8sLogAnalyzerFrontend
   - Crie Site:
     - Nome: K8sLogAnalyzer.Frontend
     - Application Pool: K8sLogAnalyzerFrontend
     - Caminho: `C:\Git\Logs.View\Published\Frontend`
     - Binding: http, porta 8080

---

## 🔧 Configurações Pós-Instalação

### Atualizar URL do Backend

Se o backend não estiver em `http://localhost:5000`, edite:

**arquivo:** `Published\Frontend\main.*.js`
- Procure por: `http://localhost:5000`
- Substitua por: URL do seu backend

Ou antes de publicar, edite:
**arquivo:** `Frontend\src\environments\environment.prod.ts`
```typescript
export const environment = {
  production: true,
  apiUrl: 'http://seu-servidor:5000'
};
```

### Firewall

Se necessário, libere as portas:
```cmd
netsh advfirewall firewall add rule name="K8s Log Analyzer Backend" dir=in action=allow protocol=TCP localport=5000
netsh advfirewall firewall add rule name="K8s Log Analyzer Frontend" dir=in action=allow protocol=TCP localport=8080
```

---

## 📦 Scripts Disponíveis

| Script | Descrição |
|--------|-----------|
| `publish.bat` | Compila backend e frontend |
| `start-app.bat` | Inicia aplicação publicada |
| `install-service.bat` | Instala backend como serviço Windows |
| `uninstall-service.bat` | Remove serviço Windows |

---

## ✅ Verificação

Após publicar, teste:

1. **Backend**: http://localhost:5000/swagger
2. **Frontend**: http://localhost:8080
3. **Health Check**: http://localhost:5000/health (se implementado)

---

## 📝 Notas

- **Requisito**: .NET 9 Runtime instalado na máquina
- **Kubernetes**: Certifique-se de que o kubeconfig está acessível
  - Windows: `%USERPROFILE%\.kube\config`
  - Serviço Windows: Configure KUBECONFIG environment variable
  
- **Produção**: Considere HTTPS e autenticação antes de expor publicamente
