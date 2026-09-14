using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Challenge.SupportRequests.Api.Controllers;

public sealed record HealthResponse(string Status);

[ApiController]
[AllowAnonymous]
[Route("api/health")]
public sealed class HealthController(HealthCheckService healthChecks) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Check API and SQL Server health")]
    [EndpointDescription("Returns 503 when the SQL Server dependency is unavailable. No connection details are exposed.")]
    [ProducesResponseType<HealthResponse>(200)]
    [ProducesResponseType<HealthResponse>(503)]
    public async Task<ActionResult<HealthResponse>> GetAsync(CancellationToken cancellationToken)
    {
        var result = await healthChecks.CheckHealthAsync(cancellationToken);
        return StatusCode(result.Status == HealthStatus.Healthy ? 200 : 503, new HealthResponse(result.Status.ToString()));
    }
}
