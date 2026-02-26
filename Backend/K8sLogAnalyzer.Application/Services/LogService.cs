using K8sLogAnalyzer.Application.DTOs;
using K8sLogAnalyzer.Application.Interfaces;

namespace K8sLogAnalyzer.Application.Services;

public class LogService : ILogService
{
    private readonly IKubernetesService _kubernetesService;
    private readonly ILogParser _logParser;

    public LogService(IKubernetesService kubernetesService, ILogParser logParser)
    {
        _kubernetesService = kubernetesService;
        _logParser = logParser;
    }

    public async Task<IEnumerable<LogEntryDto>> GetPodLogsAsync(
        string namespaceName, 
        string podName,
        string? containerName = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
            throw new ArgumentException("Namespace cannot be empty", nameof(namespaceName));

        if (string.IsNullOrWhiteSpace(podName))
            throw new ArgumentException("Pod name cannot be empty", nameof(podName));

        var rawLogs = await _kubernetesService.GetPodLogsAsync(namespaceName, podName, containerName, cancellationToken);
        var parsedLogs = _logParser.ParseLogs(rawLogs);

        // Adicionar o nome do pod aos logs
        return parsedLogs.Select(log => 
        {
            log.PodName = podName;
            return log;
        });
    }

    public async Task<IEnumerable<LogEntryDto>> GetMultiplePodLogsAsync(
        string namespaceName, 
        string podNamePrefix,
        string? containerName = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
            throw new ArgumentException("Namespace cannot be empty", nameof(namespaceName));

        if (string.IsNullOrWhiteSpace(podNamePrefix))
            throw new ArgumentException("Pod name prefix cannot be empty", nameof(podNamePrefix));

        // Buscar todos os pods que correspondem ao prefixo
        var podNames = await _kubernetesService.GetPodsAsync(namespaceName, podNamePrefix, cancellationToken);

        if (!podNames.Any())
            return Enumerable.Empty<LogEntryDto>();

        // Buscar logs de todos os pods em paralelo
        var logTasks = podNames.Select(async podName =>
        {
            try
            {
                var rawLogs = await _kubernetesService.GetPodLogsAsync(namespaceName, podName, containerName, cancellationToken);
                var parsedLogs = _logParser.ParseLogs(rawLogs);
                
                // Adicionar o nome do pod aos logs
                return parsedLogs.Select(log => 
                {
                    log.PodName = podName;
                    return log;
                });
            }
            catch (Exception)
            {
                // Se falhar em um pod específico, continuar com os outros
                return Enumerable.Empty<LogEntryDto>();
            }
        });

        var allLogs = await Task.WhenAll(logTasks);
        
        // Agregar todos os logs e ordenar por timestamp
        return allLogs
            .SelectMany(logs => logs)
            .OrderBy(log => log.Timestamp)
            .ToList();
    }
}
