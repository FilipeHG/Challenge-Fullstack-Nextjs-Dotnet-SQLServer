using FluentValidation;

namespace Challenge.SupportRequests.Application.SupportRequests.List;

public sealed class ListSupportRequestsRequestValidator : AbstractValidator<ListSupportRequestsRequest>
{
    public ListSupportRequestsRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");
        RuleFor(x => x.Limit).InclusiveBetween(1, ListSupportRequestsRequest.MaximumLimit).WithMessage("O limite deve estar entre 1 e 100.");
        RuleFor(x => x.Priority).Must(x => x is null || EnumInput.IsPriority(x)).WithMessage("A prioridade informada é inválida.");
        RuleFor(x => x.Status).Must(x => x is null || EnumInput.IsStatus(x)).WithMessage("O status informado é inválido.");
    }
}
