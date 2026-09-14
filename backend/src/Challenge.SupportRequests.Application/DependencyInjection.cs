using Challenge.SupportRequests.Application.SupportRequests.Create;
using Challenge.SupportRequests.Application.SupportRequests.Delete;
using Challenge.SupportRequests.Application.SupportRequests.GetById;
using Challenge.SupportRequests.Application.SupportRequests.List;
using Challenge.SupportRequests.Application.SupportRequests.UpdatePriority;
using Challenge.SupportRequests.Application.SupportRequests.UpdateStatus;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.SupportRequests.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddValidatorsFromAssemblyContaining<CreateSupportRequestRequestValidator>();
        services.AddScoped<CreateSupportRequestUseCase>();
        services.AddScoped<GetSupportRequestByIdUseCase>();
        services.AddScoped<ListSupportRequestsUseCase>();
        services.AddScoped<UpdateSupportRequestPriorityUseCase>();
        services.AddScoped<UpdateSupportRequestStatusUseCase>();
        services.AddScoped<DeleteSupportRequestUseCase>();
        return services;
    }
}
