using K8sLogAnalyzer.Application.DTOs;

namespace K8sLogAnalyzer.Application.Interfaces;

public interface ILogParser
{
    IEnumerable<LogEntryDto> ParseLogs(string rawLogs);
}
