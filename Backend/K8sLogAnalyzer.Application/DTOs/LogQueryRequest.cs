namespace K8sLogAnalyzer.Application.DTOs;

public class LogQueryRequest
{
    public string Namespace { get; set; } = string.Empty;
    public string PodName { get; set; } = string.Empty;
}
