using K8sLogAnalyzer.Application.DTOs;

namespace K8sLogAnalyzer.Application.Interfaces;

public interface IKubernetesService
{
    Task<string> GetPodLogsAsync(string namespaceName, string podName, string? containerName = null, CancellationToken cancellationToken = default);
    Task<List<string>> GetPodsAsync(string namespaceName, string podNamePrefix, CancellationToken cancellationToken = default);
    Task<ClusterInfoDto> GetClusterInfoAsync(CancellationToken cancellationToken = default);
    Task<List<KubernetesContextDto>> GetAvailableContextsAsync(CancellationToken cancellationToken = default);
    Task<bool> SwitchContextAsync(string contextName, CancellationToken cancellationToken = default);
}
