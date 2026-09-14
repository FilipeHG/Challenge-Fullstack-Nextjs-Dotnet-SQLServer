using FluentValidation;

namespace Challenge.SupportRequests.Application.SupportRequests.UpdateStatus;

public sealed class UpdateSupportRequestStatusRequestValidator : AbstractValidator<UpdateSupportRequestStatusRequest>
{
    public UpdateSupportRequestStatusRequestValidator()
    {
        RuleFor(x => x.Status).Must(EnumInput.IsStatus).WithMessage("O status informado é inválido.");
    }
}
