using FluentValidation;

namespace Challenge.SupportRequests.Application.SupportRequests.UpdatePriority;

public sealed class UpdateSupportRequestPriorityRequestValidator : AbstractValidator<UpdateSupportRequestPriorityRequest>
{
    public UpdateSupportRequestPriorityRequestValidator()
    {
        RuleFor(x => x.Priority).Must(EnumInput.IsPriority).WithMessage("A prioridade informada é inválida.");
    }
}
