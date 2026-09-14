using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Challenge.SupportRequests.IntegrationTests.Infrastructure;

namespace Challenge.SupportRequests.IntegrationTests.Endpoints;

public sealed class AuthenticationAndOpenApiTests
{
    [Fact]
    public async Task ValidHs256Token_ShouldAuthorizeProtectedRoute()
    {
        using var factory = new ApiFactory();
        using var client = factory.AuthenticatedClient();

        var response = await client.GetAsync("/api/support-requests/1");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("wrong-token")]
    public async Task MissingOrMalformedBearerToken_ShouldReturnUnauthorized(string? token)
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        if (token is not null) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/support-requests");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("wrong-secret")]
    [InlineData("wrong-issuer")]
    [InlineData("wrong-audience")]
    [InlineData("expired")]
    [InlineData("wrong-algorithm")]
    public async Task InvalidJwt_ShouldReturnUnauthorized(string failure)
    {
        using var factory = new ApiFactory
        {
            Token = failure switch
            {
                "wrong-secret" => ApiFactory.MakeToken(secret: "wrong-secret-key-0123456789abcdef0123456789abcdef0123456789abcdef"),
                "wrong-issuer" => ApiFactory.MakeToken(issuer: "wrong-issuer"),
                "wrong-audience" => ApiFactory.MakeToken(audience: "wrong-audience"),
                "expired" => ApiFactory.MakeToken(expiration: DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds()),
                "wrong-algorithm" => ApiFactory.MakeToken(algorithm: "HS512"),
                _ => "invalid.jwt.token"
            }
        };
        using var client = factory.AuthenticatedClient();

        var response = await client.GetAsync("/api/support-requests/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(true, HttpStatusCode.OK)]
    [InlineData(false, HttpStatusCode.ServiceUnavailable)]
    public async Task Health_ShouldRemainPublic(bool healthy, HttpStatusCode expected)
    {
        using var factory = new ApiFactory { DatabaseHealthy = healthy };
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/health");

        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task OpenApiAndSwagger_ShouldRemainPublic()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var document = await client.GetFromJsonAsync<JsonElement>("/swagger-json");
        var paths = document.GetProperty("paths");

        foreach (var path in new[] { "/api/support-requests", "/api/support-requests/{id}", "/api/support-requests/{id}/priority", "/api/support-requests/{id}/status", "/api/health" })
            Assert.True(paths.TryGetProperty(path, out _), path);
        Assert.True(document.GetProperty("components").GetProperty("securitySchemes").TryGetProperty("Bearer", out _));
        Assert.True(paths.GetProperty("/api/support-requests").GetProperty("post").TryGetProperty("security", out _));
        Assert.Contains("swagger-ui", await client.GetStringAsync("/swagger"));
        var yaml = await client.GetStringAsync("/swagger-yaml");
        Assert.Contains("openapi:", yaml);
        Assert.Contains("/api/support-requests", yaml);
    }
}
