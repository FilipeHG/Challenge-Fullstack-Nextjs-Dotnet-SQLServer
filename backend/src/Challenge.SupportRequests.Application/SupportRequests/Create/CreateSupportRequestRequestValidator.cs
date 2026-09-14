using FluentValidation;

namespace Challenge.SupportRequests.Application.SupportRequests.Create;

public sealed class CreateSupportRequestRequestValidator : AbstractValidator<CreateSupportRequestRequest>
{
    public CreateSupportRequestRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(4000).WithMessage("A descrição deve ter no máximo 4000 caracteres.");
        RuleFor(x => x.Requester).NotEmpty().WithMessage("O solicitante é obrigatório.")
            .MaximumLength(200).WithMessage("O solicitante deve ter no máximo 200 caracteres.");
        RuleFor(x => x.Priority).Must(EnumInput.IsPriority).WithMessage("A prioridade informada é inválida.");
    }
}
