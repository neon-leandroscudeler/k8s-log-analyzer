using K8sLogAnalyzer.Application.DTOs;

namespace K8sLogAnalyzer.Application.Interfaces;

public interface ILogService
{
    Task<IEnumerable<LogEntryDto>> GetPodLogsAsync(string namespaceName, string podName, string? containerName = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<LogEntryDto>> GetMultiplePodLogsAsync(string namespaceName, string podNamePrefix, string? containerName = null, CancellationToken cancellationToken = default);
}
