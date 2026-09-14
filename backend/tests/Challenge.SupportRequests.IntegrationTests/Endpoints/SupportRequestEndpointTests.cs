using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Domain.Entities;
using Challenge.SupportRequests.Domain.Enums;
using Challenge.SupportRequests.IntegrationTests.Infrastructure;
using NSubstitute;

namespace Challenge.SupportRequests.IntegrationTests.Endpoints;

public sealed class SupportRequestEndpointTests
{
    private const string Route = "/api/support-requests";
    private static object ValidPayload => new { title = "Printer offline", description = "Finance printer needs support.", requester = "Maria Silva", priority = "high" };

    [Fact]
    public async Task Create_ShouldReturnCreatedWithLocationAndServerFields()
    {
        using var factory = new ApiFactory();
        factory.Repository.CreateAsync(Arg.Any<SupportRequest>(), Arg.Any<CancellationToken>()).Returns(42L);
        using var client = factory.AuthenticatedClient();
        var response = await client.PostAsJsonAsync(Route, ValidPayload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.EndsWith("/42", response.Headers.Location!.ToString());
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(42, json.GetProperty("id").GetInt64());
        Assert.Equal("open", json.GetProperty("status").GetString());
        Assert.Equal("high", json.GetProperty("priority").GetString());
        Assert.Equal(JsonValueKind.Null, json.GetProperty("completedAt").ValueKind);
        Assert.EndsWith("Z", json.GetProperty("createdAt").GetString());
    }

    [Fact]
    public async Task Get_ShouldReturnNotFoundProblemWithCorrelation()
    {
        using var factory = new ApiFactory();
        using var client = factory.AuthenticatedClient();
        client.DefaultRequestHeaders.Add("X-Correlation-ID", "test-123");
        var response = await client.GetAsync(Route + "/999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("test-123", json.GetProperty("correlationId").GetString());
        Assert.Equal("test-123", Assert.Single(response.Headers.GetValues("X-Correlation-ID")));
    }

    [Theory]
    [InlineData("", "", "", "urgent")]
    [InlineData(" ", "description", "requester", "high")]
    [InlineData("title", null, "requester", "high")]
    [InlineData("title", "description", null, "high")]
    public async Task Create_ShouldRejectInvalidInputBeforePersistence(string? title, string? description, string? requester, string priority)
    {
        using var factory = new ApiFactory();
        using var client = factory.AuthenticatedClient();
        var response = await client.PostAsJsonAsync(Route, new { title, description, requester, priority });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.GetProperty("errors").EnumerateObject().Any());
        await factory.Repository.DidNotReceive().CreateAsync(Arg.Any<SupportRequest>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("?page=0")]
    [InlineData("?limit=0")]
    [InlineData("?limit=101")]
    [InlineData("?status=closed")]
    [InlineData("?priority=1")]
    [InlineData("?page=abc")]
    public async Task List_ShouldRejectInvalidQuery(string query)
    {
        using var factory = new ApiFactory();
        using var client = factory.AuthenticatedClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync(Route + query)).StatusCode);
    }

    [Fact]
    public async Task List_ShouldApplyDefaultsAndMapFilters()
    {
        using var factory = new ApiFactory();
        factory.Repository.ListAsync(Arg.Any<SupportRequestFilter>(), Arg.Any<CancellationToken>())
            .Returns(new SupportRequestPage([], 0));
        using var client = factory.AuthenticatedClient();
        var response = await client.GetAsync(Route + "?status=open&priority=high&search=printer");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(1, json.GetProperty("page").GetInt32());
        Assert.Equal(100, json.GetProperty("limit").GetInt32());
        Assert.Equal(0, json.GetProperty("totalPages").GetInt64());
        await factory.Repository.Received().ListAsync(Arg.Is<SupportRequestFilter>(x =>
            x.Page == 1 && x.Limit == 100 && x.Status == RequestStatus.Open && x.Priority == Priority.High && x.Search == "printer"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Status_ShouldCompleteRejectOpenAndAllowInProgress()
    {
        using var factory = new ApiFactory();
        var request = SupportRequest.Create("Title", "Description", "Requester", Priority.Low, ApiFactory.Now.UtcDateTime);
        request.AssignId(1);
        factory.Repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(request);
        factory.Repository.UpdateStatusAsync(request, Arg.Any<CancellationToken>()).Returns(true);
        using var client = factory.AuthenticatedClient();
        var completed = await client.PatchAsJsonAsync(Route + "/1/status", new { status = "completed" });
        Assert.Equal(HttpStatusCode.OK, completed.StatusCode);
        Assert.Equal(ApiFactory.Now.UtcDateTime, request.CompletedAt);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PatchAsJsonAsync(Route + "/1/status", new { status = "open" })).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.DeleteAsync(Route + "/1")).StatusCode);
        var reopened = await client.PatchAsJsonAsync(Route + "/1/status", new { status = "inProgress" });
        Assert.Equal(HttpStatusCode.OK, reopened.StatusCode);
        Assert.Null(request.CompletedAt);
    }

    [Fact]
    public async Task PriorityAndDelete_ShouldUpdateThenDeleteOpenRequest()
    {
        using var factory = new ApiFactory();
        var request = SupportRequest.Create("Title", "Description", "Requester", Priority.Low, ApiFactory.Now.UtcDateTime);
        request.AssignId(1);
        factory.Repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(request);
        factory.Repository.UpdatePriorityAsync(request, Arg.Any<CancellationToken>()).Returns(true);
        factory.Repository.DeleteAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        using var client = factory.AuthenticatedClient();
        Assert.Equal(HttpStatusCode.OK, (await client.PatchAsJsonAsync(Route + "/1/priority", new { priority = "high" })).StatusCode);
        Assert.Equal(Priority.High, request.Priority);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync(Route + "/1")).StatusCode);
    }

    [Fact]
    public async Task UnexpectedFailure_ShouldNotExposeDatabaseDetails()
    {
        using var factory = new ApiFactory();
        factory.Repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromException<SupportRequest?>(new InvalidOperationException("private database details")));
        using var client = factory.AuthenticatedClient();
        var response = await client.GetAsync(Route + "/1");
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.DoesNotContain("private database", await response.Content.ReadAsStringAsync());
    }
}
