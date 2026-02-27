namespace K8sLogAnalyzer.Application.DTOs;

public class ClusterInfoDto
{
    public bool Connected { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ClusterName { get; set; }
    public string? ContextName { get; set; }
    public string? ServerUrl { get; set; }
}
