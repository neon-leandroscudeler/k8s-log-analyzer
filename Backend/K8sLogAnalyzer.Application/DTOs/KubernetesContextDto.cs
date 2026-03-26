namespace K8sLogAnalyzer.Application.DTOs;

public class KubernetesContextDto
{
    public string Name { get; set; } = string.Empty;
    public string Cluster { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
}
