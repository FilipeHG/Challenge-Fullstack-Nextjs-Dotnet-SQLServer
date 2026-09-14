using Challenge.SupportRequests.Application.Abstractions.Persistence;
using Challenge.SupportRequests.Application.Common.Exceptions;
using Challenge.SupportRequests.Domain.Enums;
using FluentValidation;

namespace Challenge.SupportRequests.Application.SupportRequests.UpdatePriority;

public sealed class UpdateSupportRequestPriorityUseCase(ISupportRequestRepository repository,
    IValidator<UpdateSupportRequestPriorityRequest> validator)
{
    public async Task<SupportRequestResponse> ExecuteAsync(long id, UpdateSupportRequestPriorityRequest input, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        var request = await repository.GetByIdAsync(id, cancellationToken) ?? throw new ResourceNotFoundException(id);
        request.ChangePriority(Enum.Parse<Priority>(input.Priority!, true));
        if (!await repository.UpdatePriorityAsync(request, cancellationToken)) throw new ResourceNotFoundException(id);
        return SupportRequestResponse.From(request);
    }
}
