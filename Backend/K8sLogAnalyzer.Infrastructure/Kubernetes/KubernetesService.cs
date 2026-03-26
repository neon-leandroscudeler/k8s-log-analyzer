using k8s;
using k8s.Autorest;
using K8sLogAnalyzer.Application.Interfaces;
using K8sLogAnalyzer.Application.DTOs;

namespace K8sLogAnalyzer.Infrastructure.Kubernetes;

public class KubernetesService : IKubernetesService
{
    private IKubernetes _kubernetesClient;
    private KubernetesClientConfiguration _config;
    private readonly object _lockObject = new object();

    public KubernetesService()
    {
        _config = KubernetesClientConfiguration.BuildDefaultConfig();
        _kubernetesClient = new k8s.Kubernetes(_config);
    }

    public async Task<string> GetPodLogsAsync(
        string namespaceName, 
        string podName,
        string? containerName = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Se o container não foi especificado, detectar automaticamente
            if (string.IsNullOrWhiteSpace(containerName))
            {
                var pod = await _kubernetesClient.CoreV1.ReadNamespacedPodAsync(
                    name: podName,
                    namespaceParameter: namespaceName,
                    cancellationToken: cancellationToken
                ).ConfigureAwait(false);

                // Pegar o primeiro container que não seja init, sidecar comum (istio-proxy, datadog, etc)
                var mainContainer = pod.Spec.Containers
                    .FirstOrDefault(c => !c.Name.Contains("istio") && 
                                        !c.Name.Contains("datadog") && 
                                        !c.Name.Contains("proxy") &&
                                        !c.Name.EndsWith("-init"));

                containerName = mainContainer?.Name ?? pod.Spec.Containers.FirstOrDefault()?.Name;

                if (string.IsNullOrWhiteSpace(containerName))
                {
                    throw new InvalidOperationException($"No containers found in pod '{podName}'");
                }
            }

            using var logsStream = await _kubernetesClient.CoreV1.ReadNamespacedPodLogAsync(
                name: podName,
                namespaceParameter: namespaceName,
                container: containerName,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            using var reader = new StreamReader(logsStream);
            var logs = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

            return logs;
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException($"Pod '{podName}' not found in namespace '{namespaceName}'", ex);
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException($"Access denied. Check RBAC permissions for namespace '{namespaceName}'", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving logs from pod '{podName}' in namespace '{namespaceName}': {ex.Message}", ex);
        }
    }

    public async Task<List<string>> GetPodsAsync(
        string namespaceName, 
        string podNamePrefix,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var podList = await _kubernetesClient.CoreV1.ListNamespacedPodAsync(
                namespaceParameter: namespaceName,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            // Filtrar pods que começam com o prefixo e estão rodando
            var matchingPods = podList.Items
                .Where(p => p.Metadata.Name.StartsWith(podNamePrefix, StringComparison.OrdinalIgnoreCase) &&
                           p.Status.Phase == "Running")
                .Select(p => p.Metadata.Name)
                .OrderBy(name => name)
                .ToList();

            return matchingPods;
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException($"Namespace '{namespaceName}' not found", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error listing pods in namespace '{namespaceName}': {ex.Message}", ex);
        }
    }

    public async Task<ClusterInfoDto> GetClusterInfoAsync(CancellationToken cancellationToken = default)
    {
        var clusterInfo = new ClusterInfoDto
        {
            ServerUrl = _config.Host,
            ContextName = _config.CurrentContext,
            ClusterName = ExtractClusterName(_config.Host)
        };

        try
        {
            // Tenta fazer uma chamada simples à API do Kubernetes para verificar conectividade
            await _kubernetesClient.CoreV1.ListNamespaceAsync(
                limit: 1,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
            
            clusterInfo.Connected = true;
            clusterInfo.Status = "Connected";
        }
        catch
        {
            clusterInfo.Connected = false;
            clusterInfo.Status = "Disconnected";
        }

        return clusterInfo;
    }

    private string ExtractClusterName(string hostUrl)
    {
        try
        {
            var uri = new Uri(hostUrl);
            var host = uri.Host;
            
            // Tentar extrair nome do cluster de padrões comuns
            // Ex: https://api.cluster-name.k8s.io -> cluster-name
            // Ex: https://127.0.0.1:6443 -> localhost
            if (host.Contains("localhost") || host.StartsWith("127.") || host.StartsWith("192.168."))
            {
                return "local";
            }
            
            var parts = host.Split('.');
            if (parts.Length > 1)
            {
                // Pegar o primeiro segmento relevante
                return parts[0].Replace("api", "").Trim('-');
            }
            
            return host;
        }
        catch
        {
            return "unknown";
        }
    }

    public async Task<List<KubernetesContextDto>> GetAvailableContextsAsync(CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                var k8sConfig = KubernetesClientConfiguration.LoadKubeConfig();
                var contexts = new List<KubernetesContextDto>();

                foreach (var context in k8sConfig.Contexts)
                {
                    contexts.Add(new KubernetesContextDto
                    {
                        Name = context.Name,
                        Cluster = context.ContextDetails?.Cluster ?? "unknown",
                        User = context.ContextDetails?.User ?? "unknown",
                        IsCurrent = context.Name == k8sConfig.CurrentContext
                    });
                }

                return contexts.OrderBy(c => c.Name).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error loading kubeconfig contexts: {ex.Message}", ex);
            }
        }, cancellationToken);
    }

    public async Task<bool> SwitchContextAsync(string contextName, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                lock (_lockObject)
                {
                    // Carregar config com o novo contexto
                    var k8sConfig = KubernetesClientConfiguration.LoadKubeConfig();
                    
                    // Verificar se o contexto existe
                    var context = k8sConfig.Contexts.FirstOrDefault(c => c.Name == contextName);
                    if (context == null)
                    {
                        throw new InvalidOperationException($"Context '{contextName}' not found");
                    }

                    // Criar nova configuração com o contexto especificado
                    _config = KubernetesClientConfiguration.BuildConfigFromConfigObject(k8sConfig, contextName);
                    
                    // Recriar o cliente
                    _kubernetesClient = new k8s.Kubernetes(_config);
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error switching to context '{contextName}': {ex.Message}", ex);
            }
        }, cancellationToken);
    }
}
