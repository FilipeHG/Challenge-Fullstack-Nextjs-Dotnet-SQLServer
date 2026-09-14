using Bogus;
using Challenge.SupportRequests.Domain.Entities;
using Challenge.SupportRequests.Domain.Enums;
using Challenge.SupportRequests.Domain.Exceptions;

namespace Challenge.SupportRequests.UnitTests.Domain;

public sealed class SupportRequestTests
{
    private static readonly DateTime Now = new(2026, 9, 14, 12, 0, 0, DateTimeKind.Utc);
    private static SupportRequest Create()
    {
        var faker = new Faker { Random = new Randomizer(42) };
        return SupportRequest.Create(faker.Lorem.Sentence(), faker.Lorem.Paragraph(), faker.Name.FullName(), Priority.High, Now);
    }

    [Fact]
    public void Create_ShouldStartOpen()
    {
        var request = Create();
        Assert.Equal(RequestStatus.Open, request.Status);
        Assert.Null(request.CompletedAt);
        Assert.Equal(Now, request.CreatedAt);
    }

    [Fact]
    public void ChangeStatus_ShouldSetAndPreserveCompletedAt_WhenCompleted()
    {
        var request = Create();
        request.ChangeStatus(RequestStatus.Completed, Now);
        request.ChangeStatus(RequestStatus.Completed, Now.AddHours(1));
        Assert.Equal(Now, request.CompletedAt);
    }

    [Fact]
    public void ChangeStatus_ShouldThrow_WhenCompletedRequestReturnsToOpen()
    {
        var request = Create();
        request.ChangeStatus(RequestStatus.Completed, Now);
        Assert.Throws<InvalidStatusTransitionException>(() => request.ChangeStatus(RequestStatus.Open, Now));
        Assert.Equal(RequestStatus.Completed, request.Status);
    }

    [Fact]
    public void ChangeStatus_ShouldClearCompletedAt_WhenCompletedReturnsToInProgress()
    {
        var request = Create();
        request.ChangeStatus(RequestStatus.Completed, Now);
        request.ChangeStatus(RequestStatus.InProgress, Now);
        Assert.Null(request.CompletedAt);
    }

    [Theory]
    [InlineData(RequestStatus.Open)]
    [InlineData(RequestStatus.InProgress)]
    [InlineData(RequestStatus.Completed)]
    public void EnsureCanBeDeleted_ShouldAllowOnlyOpen(RequestStatus status)
    {
        var request = Create();
        request.ChangeStatus(status, Now);
        if (status == RequestStatus.Open) request.EnsureCanBeDeleted();
        else Assert.Throws<SupportRequestDeletionNotAllowedException>(request.EnsureCanBeDeleted);
    }
}
