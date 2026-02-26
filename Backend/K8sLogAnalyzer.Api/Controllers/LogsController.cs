using Microsoft.AspNetCore.Mvc;
using K8sLogAnalyzer.Application.DTOs;
using K8sLogAnalyzer.Application.Interfaces;

namespace K8sLogAnalyzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILogService _logService;
    private readonly ILogger<LogsController> _logger;

    public LogsController(ILogService logService, ILogger<LogsController> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieve logs from a Kubernetes pod
    /// </summary>
    /// <param name="namespace">The Kubernetes namespace</param>
    /// <param name="podName">The name of the pod</param>
    /// <param name="containerName">Optional: specific container name (auto-detected if not provided)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of structured log entries</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LogEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<LogEntryDto>>> GetLogs(
        [FromQuery] string @namespace,
        [FromQuery] string podName,
        [FromQuery] string? containerName = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                return BadRequest(new { error = "Namespace parameter is required" });
            }

            if (string.IsNullOrWhiteSpace(podName))
            {
                return BadRequest(new { error = "PodName parameter is required" });
            }

            _logger.LogInformation("Fetching logs for pod '{PodName}' in namespace '{Namespace}' (container: {Container})", 
                podName, @namespace, containerName ?? "auto-detect");

            var logs = await _logService.GetPodLogsAsync(@namespace, podName, containerName, cancellationToken);

            return Ok(logs);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            _logger.LogWarning(ex, "Pod not found: {PodName} in namespace {Namespace}", podName, @namespace);
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Access denied for pod: {PodName} in namespace {Namespace}", podName, @namespace);
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid arguments");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving logs for pod '{PodName}' in namespace '{Namespace}'", podName, @namespace);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while retrieving logs" });
        }
    }

    /// <summary>
    /// Retrieve logs from multiple Kubernetes pods matching a name prefix (e.g., api-credit-cards will match api-credit-cards and api-credit-cards-primary)
    /// </summary>
    /// <param name="namespace">The Kubernetes namespace</param>
    /// <param name="podNamePrefix">The prefix of pod names to search for</param>
    /// <param name="containerName">Optional: specific container name (auto-detected if not provided)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Aggregated list of structured log entries from all matching pods</returns>
    [HttpGet("multiple")]
    [ProducesResponseType(typeof(IEnumerable<LogEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<LogEntryDto>>> GetMultiplePodLogs(
        [FromQuery] string @namespace,
        [FromQuery] string podNamePrefix,
        [FromQuery] string? containerName = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                return BadRequest(new { error = "Namespace parameter is required" });
            }

            if (string.IsNullOrWhiteSpace(podNamePrefix))
            {
                return BadRequest(new { error = "PodNamePrefix parameter is required" });
            }

            _logger.LogInformation("Fetching logs for pods with prefix '{PodNamePrefix}' in namespace '{Namespace}' (container: {Container})", 
                podNamePrefix, @namespace, containerName ?? "auto-detect");

            var logs = await _logService.GetMultiplePodLogsAsync(@namespace, podNamePrefix, containerName, cancellationToken);

            return Ok(logs);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            _logger.LogWarning(ex, "Namespace not found: {Namespace}", @namespace);
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Access denied for namespace: {Namespace}", @namespace);
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid arguments");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving logs for pods with prefix '{PodNamePrefix}' in namespace '{Namespace}'", podNamePrefix, @namespace);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An error occurred while retrieving logs" });
        }
    }
}
