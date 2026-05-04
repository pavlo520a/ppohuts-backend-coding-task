using Claims.Application.Premium;
using Claims.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddClaimsApplication(this IServiceCollection services)
    {
        services.AddScoped<IPremiumCalculator, PremiumCalculator>();

        services.AddScoped<IGetClaimsUseCase, GetClaimsUseCase>();
        services.AddScoped<IGetClaimByIdUseCase, GetClaimByIdUseCase>();
        services.AddScoped<ICreateClaimUseCase, CreateClaimUseCase>();
        services.AddScoped<IDeleteClaimUseCase, DeleteClaimUseCase>();

        services.AddScoped<IGetCoversUseCase, GetCoversUseCase>();
        services.AddScoped<IGetCoverByIdUseCase, GetCoverByIdUseCase>();
        services.AddScoped<ICreateCoverUseCase, CreateCoverUseCase>();
        services.AddScoped<IDeleteCoverUseCase, DeleteCoverUseCase>();
        services.AddScoped<IComputePremiumUseCase, ComputePremiumUseCase>();

        return services;
    }
}
