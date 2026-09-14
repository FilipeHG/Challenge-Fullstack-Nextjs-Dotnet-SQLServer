using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Application.Common.Exceptions;
using Challenge.SupportRequests.Application.SupportRequests.Create;
using Challenge.SupportRequests.Application.SupportRequests.Delete;
using Challenge.SupportRequests.Application.SupportRequests.GetById;
using Challenge.SupportRequests.Application.SupportRequests.List;
using Challenge.SupportRequests.Domain.Entities;
using Challenge.SupportRequests.Domain.Enums;
using Challenge.SupportRequests.Domain.Exceptions;
using FluentValidation;
using NSubstitute;

namespace Challenge.SupportRequests.UnitTests.Application;

public sealed class UseCaseTests
{
    [Fact]
    public async Task GetById_ShouldThrowNotFound_WhenMissing()
    {
        var repository = Substitute.For<ISupportRequestRepository>();
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => new GetSupportRequestByIdUseCase(repository).ExecuteAsync(1, default));
    }

    [Theory]
    [InlineData(RequestStatus.InProgress)]
    [InlineData(RequestStatus.Completed)]
    public async Task Delete_ShouldNotPersist_WhenDomainRejects(RequestStatus status)
    {
        var repository = Substitute.For<ISupportRequestRepository>();
        var request = SupportRequest.Create("Title", "Description", "Requester", Priority.High, DateTime.UnixEpoch);
        request.ChangeStatus(status, DateTime.UnixEpoch);
        repository.GetByIdAsync(1, default).Returns(request);
        await Assert.ThrowsAsync<SupportRequestDeletionNotAllowedException>(() => new DeleteSupportRequestUseCase(repository).ExecuteAsync(1, default));
        await repository.DidNotReceive().DeleteAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ShouldRejectInvalidInputBeforePersistence()
    {
        var repository = Substitute.For<ISupportRequestRepository>();
        var useCase = new CreateSupportRequestUseCase(repository, new CreateSupportRequestRequestValidator(), TimeProvider.System);
        await Assert.ThrowsAsync<ValidationException>(() => useCase.ExecuteAsync(new(null, "", " ", "urgent"), default));
        await repository.DidNotReceive().CreateAsync(Arg.Any<SupportRequest>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0, 100, false)]
    [InlineData(1, 0, false)]
    [InlineData(1, 101, false)]
    [InlineData(1, 100, true)]
    [InlineData(int.MaxValue, 100, true)]
    public void Pagination_ShouldValidateExplicitBounds(int page, int limit, bool valid)
    {
        var result = new ListSupportRequestsRequestValidator().Validate(new ListSupportRequestsRequest { Page = page, Limit = limit });
        Assert.Equal(valid, result.IsValid);
    }
}
