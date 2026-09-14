using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Challenge.SupportRequests.Api.Configuration;
using Challenge.SupportRequests.Api.ErrorHandling;
using Challenge.SupportRequests.Api.Middleware;
using Challenge.SupportRequests.Api.OpenApi;
using Challenge.SupportRequests.Application;
using Challenge.SupportRequests.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") is null or "Development")
    LocalEnvironment.Load();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls($"http://localhost:{builder.Configuration["PORT"] ?? "3000"}");
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
        options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var details = new ValidationProblemDetails(context.ModelState)
        {
            Status = 400,
            Title = "Dados inválidos",
            Detail = "Verifique o formato dos campos informados.",
            Instance = context.HttpContext.Request.Path
        };
        details.Extensions["correlationId"] = context.HttpContext.TraceIdentifier;
        return new BadRequestObjectResult(details) { ContentTypes = { "application/problem+json" } };
    };
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration["DATABASE_URL"] ?? "");
builder.Services.AddOptions<RuntimeConfiguration>().Configure<IConfiguration>((options, configuration) =>
{
    options.DatabaseUrl = configuration["DATABASE_URL"] ?? "";
}).Validate(options => !string.IsNullOrWhiteSpace(options.DatabaseUrl), "DATABASE_URL is required.")
    .ValidateOnStart();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IConfiguration>((options, configuration) =>
    {
        var jwtSecret = configuration["JWT_SECRET"] ?? string.Empty;
        var jwtIssuer = configuration["JWT_ISSUER"] ?? string.Empty;
        var jwtAudience = configuration["JWT_AUDIENCE"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(jwtSecret)
            || string.IsNullOrWhiteSpace(jwtIssuer)
            || string.IsNullOrWhiteSpace(jwtAudience))
            throw new InvalidOperationException("JWT configuration is incomplete. Set JWT_SECRET, JWT_ISSUER and JWT_AUDIENCE to non-empty values.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.Zero,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
            ValidateTokenReplay = false,
            AuthenticationType = JwtBearerDefaults.AuthenticationScheme
        };
        options.RequireHttpsMetadata = false;
        options.IncludeErrorDetails = false;
        options.MapInboundClaims = false;
    });
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
{
    context.ProblemDetails.Extensions["correlationId"] = context.HttpContext.TraceIdentifier;
    context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecurityTransformer>());

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapOpenApi("/openapi/{documentName}.json");
app.MapOpenApi("/openapi/{documentName}.yaml");
app.MapGet("/swagger-json", () => Results.Redirect("/openapi/v1.json")).ExcludeFromDescription();
app.MapGet("/swagger-yaml", () => Results.Redirect("/openapi/v1.yaml")).ExcludeFromDescription();
app.UseSwaggerUI(options => { options.RoutePrefix = "swagger"; options.SwaggerEndpoint("/swagger-json", "Support Requests API"); });
app.Run();

public partial class Program;

internal sealed class RuntimeConfiguration
{
    public string DatabaseUrl { get; set; } = "";
}
