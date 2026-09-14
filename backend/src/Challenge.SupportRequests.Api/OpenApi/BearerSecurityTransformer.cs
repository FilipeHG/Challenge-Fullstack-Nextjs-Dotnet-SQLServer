using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Challenge.SupportRequests.Api.OpenApi;

public sealed class BearerSecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info.Title = "Support Requests API";
        document.Info.Description = "Internal support requests. ASP.NET Core JWT Bearer validation uses the configured HS256 secret, issuer, audience and expiration checks.";
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Provide a valid JWT configured for this API."
        };
        foreach (var description in context.DescriptionGroups.SelectMany(x => x.Items))
        {
            if (!description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any()) continue;
            var path = "/" + description.RelativePath;
            if (!document.Paths.TryGetValue(path, out var item) || item.Operations is null) continue;
            foreach (var operation in item.Operations.Values)
                operation.Security = [new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] }];
        }
        return Task.CompletedTask;
    }
}
