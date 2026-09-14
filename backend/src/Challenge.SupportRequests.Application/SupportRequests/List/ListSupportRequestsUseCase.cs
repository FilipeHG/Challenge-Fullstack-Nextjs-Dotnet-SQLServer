using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Application.Common.Models;
using Challenge.SupportRequests.Domain.Enums;
using FluentValidation;

namespace Challenge.SupportRequests.Application.SupportRequests.List;

public sealed class ListSupportRequestsUseCase(ISupportRequestRepository repository, IValidator<ListSupportRequestsRequest> validator)
{
    public async Task<PagedResult<SupportRequestResponse>> ExecuteAsync(ListSupportRequestsRequest input, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        var filter = new SupportRequestFilter(
            input.Status is null ? null : Enum.Parse<RequestStatus>(input.Status, true),
            input.Priority is null ? null : Enum.Parse<Priority>(input.Priority, true),
            string.IsNullOrWhiteSpace(input.Search) ? null : input.Search.Trim(), input.Page, input.Limit);
        var result = await repository.ListAsync(filter, cancellationToken);
        return new(result.Items.Select(SupportRequestResponse.From).ToArray(), input.Page, input.Limit, result.TotalItems);
    }
}
