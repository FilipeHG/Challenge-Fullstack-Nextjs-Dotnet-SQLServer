using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Infrastructure.Health;
using Challenge.SupportRequests.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.SupportRequests.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new SqlConnectionFactory(connectionString));
        services.AddScoped<ISupportRequestRepository, SupportRequestRepository>();
        services.AddHealthChecks().AddCheck<SqlServerHealthCheck>("sql-server", timeout: TimeSpan.FromSeconds(5));
        return services;
    }
}
