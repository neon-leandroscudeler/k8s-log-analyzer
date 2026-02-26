using System.Globalization;
using System.Text.RegularExpressions;
using K8sLogAnalyzer.Application.DTOs;
using K8sLogAnalyzer.Application.Interfaces;

namespace K8sLogAnalyzer.Infrastructure.Parsers;

public class LogParser : ILogParser
{
    // Regex pattern to match ISO 8601 date format and log level
    private static readonly Regex LogPattern = new Regex(
        @"^(?<timestamp>\d{4}-\d{2}-\d{2}[T\s]\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+-]\d{2}:\d{2})?)\s*(?<level>INFO|ERROR|WARN|WARNING|DEBUG|TRACE|FATAL)?\s*(?<message>.*)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline
    );

    private static readonly string[] DateFormats = new[]
    {
        "yyyy-MM-ddTHH:mm:ss.ffffffK",
        "yyyy-MM-ddTHH:mm:ss.fffK",
        "yyyy-MM-ddTHH:mm:ssK",
        "yyyy-MM-dd HH:mm:ss.ffffff",
        "yyyy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fffffffZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ssZ"
    };

    public IEnumerable<LogEntryDto> ParseLogs(string rawLogs)
    {
        if (string.IsNullOrWhiteSpace(rawLogs))
            return Enumerable.Empty<LogEntryDto>();

        var logEntries = new List<LogEntryDto>();
        var lines = rawLogs.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;

            var match = LogPattern.Match(trimmedLine);

            if (match.Success)
            {
                var timestampStr = match.Groups["timestamp"].Value;
                var level = match.Groups["level"].Value;
                var message = match.Groups["message"].Value.Trim();

                DateTime timestamp;
                if (!TryParseTimestamp(timestampStr, out timestamp))
                {
                    timestamp = DateTime.UtcNow;
                }

                logEntries.Add(new LogEntryDto
                {
                    Timestamp = timestamp,
                    Level = NormalizeLogLevel(level),
                    Message = string.IsNullOrWhiteSpace(message) ? trimmedLine : message
                });
            }
            else
            {
                // If no pattern matches, treat entire line as INFO message
                logEntries.Add(new LogEntryDto
                {
                    Timestamp = DateTime.UtcNow,
                    Level = "INFO",
                    Message = trimmedLine
                });
            }
        }

        return logEntries;
    }

    private bool TryParseTimestamp(string timestampStr, out DateTime timestamp)
    {
        foreach (var format in DateFormats)
        {
            if (DateTime.TryParseExact(timestampStr, format, CultureInfo.InvariantCulture, 
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out timestamp))
            {
                return true;
            }
        }

        return DateTime.TryParse(timestampStr, CultureInfo.InvariantCulture, 
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out timestamp);
    }

    private string NormalizeLogLevel(string level)
    {
        if (string.IsNullOrWhiteSpace(level))
            return "INFO";

        level = level.ToUpperInvariant();

        return level switch
        {
            "WARNING" => "WARN",
            "TRACE" => "DEBUG",
            "FATAL" => "ERROR",
            _ => level
        };
    }
}
