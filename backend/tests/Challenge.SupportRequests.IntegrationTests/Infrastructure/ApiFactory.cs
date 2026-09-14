using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;

namespace Challenge.SupportRequests.IntegrationTests.Infrastructure;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public const string Issuer = "Challenge-Fullstack-Nextjs-Dotnet-SQLServer";
    public const string TestJwtSecret = "test-secret-key-0123456789abcdef0123456789abcdef0123456789abcdef";
    private const string WrongSignatureSecret = "wrong-secret-key-0123456789abcdef0123456789abcdef0123456789abcdef";
    public static readonly DateTimeOffset Now = DateTimeOffset.UtcNow.AddHours(1);
    public ISupportRequestRepository Repository { get; } = Substitute.For<ISupportRequestRepository>();
    public string Token { get; set; } = MakeToken();
    public bool DatabaseHealthy { get; set; } = true;

    public static string MakeToken(string issuer = Issuer, object? audience = null, long? expiration = null,
        string secret = TestJwtSecret, string algorithm = "HS256")
    {
        var header = WebEncoders.Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new { alg = algorithm, typ = "JWT" }));
        var payload = WebEncoders.Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new
        {
            iss = issuer,
            aud = audience ?? Issuer,
            exp = expiration ?? Now.AddDays(1).ToUnixTimeSeconds(),
            user = "test-user"
        }));

        var signingInput = Encoding.UTF8.GetBytes(header + "." + payload);
        var signatureBytes = algorithm switch
        {
            "HS256" => HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), signingInput),
            "HS512" => HMACSHA512.HashData(Encoding.UTF8.GetBytes(secret), signingInput),
            _ => throw new NotSupportedException($"Unsupported JWT algorithm in tests: {algorithm}")
        };
        return header + "." + payload + "." + WebEncoders.Base64UrlEncode(signatureBytes);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DATABASE_URL"] = "Server=localhost;Database=UnusedTestDatabase;Integrated Security=True",
            ["JWT_SECRET"] = TestJwtSecret,
            ["JWT_ISSUER"] = Issuer,
            ["JWT_AUDIENCE"] = Issuer
        }));
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ISupportRequestRepository>();
            services.AddSingleton(Repository);
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(new FixedTimeProvider());
            services.Configure<HealthCheckServiceOptions>(options =>
            {
                options.Registrations.Clear();
                options.Registrations.Add(new HealthCheckRegistration("test-database",
                    _ => new TestHealthCheck(DatabaseHealthy), null, null));
            });
        });
    }

    public HttpClient AuthenticatedClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        return client;
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class TestHealthCheck(bool healthy) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            => Task.FromResult(healthy ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
    }
}
