namespace K8sLogAnalyzer.Application.DTOs;

public class LogEntryDto
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string PodName { get; set; } = string.Empty;
}
