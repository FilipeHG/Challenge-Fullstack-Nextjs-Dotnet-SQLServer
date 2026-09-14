using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Domain.Entities;
using Challenge.SupportRequests.Domain.Enums;
using FluentValidation;

namespace Challenge.SupportRequests.Application.SupportRequests.Create;

public sealed class CreateSupportRequestUseCase(ISupportRequestRepository repository,
    IValidator<CreateSupportRequestRequest> validator, TimeProvider timeProvider)
{
    public async Task<SupportRequestResponse> ExecuteAsync(CreateSupportRequestRequest input, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        var request = SupportRequest.Create(input.Title!, input.Description!, input.Requester!,
            Enum.Parse<Priority>(input.Priority!, true), timeProvider.GetUtcNow().UtcDateTime);
        request.AssignId(await repository.CreateAsync(request, cancellationToken));
        return SupportRequestResponse.From(request);
    }
}
