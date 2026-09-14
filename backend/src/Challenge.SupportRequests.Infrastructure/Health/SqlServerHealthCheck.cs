using Dapper;
using Challenge.SupportRequests.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Challenge.SupportRequests.Infrastructure.Health;

public sealed class SqlServerHealthCheck(SqlConnectionFactory factory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await factory.OpenAsync(cancellationToken);
            await connection.ExecuteScalarAsync<int>(new CommandDefinition("SELECT 1;", commandTimeout: 3, cancellationToken: cancellationToken));
            return HealthCheckResult.Healthy();
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy("Database unavailable.");
        }
    }
}
