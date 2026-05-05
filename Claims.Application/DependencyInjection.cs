using Claims.Application.Abstractions;
using Claims.Application.Abstractions.Formulas;
using Claims.Domain.Models.Formulas;
using Claims.Application.Commands.Claims;
using Claims.Application.Commands.Covers;
using Claims.Application.Commands.Formulas;
using Claims.Application.Formulas;
using Claims.Application.UseCases.Claims;
using Claims.Application.UseCases.Covers;
using Claims.Application.UseCases.Formulas;
using Claims.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IFormula<CoverPremiumFormulaArgs>, PremiumFormula>();

        services.AddScoped<IUseCase<GetClaimsCommand, IReadOnlyList<Claim>>, GetClaimsUseCase>();
        services.AddScoped<IUseCase<GetClaimByIdCommand, Claim>, GetClaimByIdUseCase>();
        services.AddScoped<IUseCase<CreateClaimCommand, Claim>, CreateClaimUseCase>();
        services.AddScoped<IUseCase<DeleteClaimCommand>, DeleteClaimUseCase>();

        services.AddScoped<IUseCase<GetCoversCommand, IReadOnlyList<Cover>>, GetCoversUseCase>();
        services.AddScoped<IUseCase<GetCoverByIdCommand, Cover>, GetCoverByIdUseCase>();
        services.AddScoped<IUseCase<CreateCoverCommand, Cover>, CreateCoverUseCase>();
        services.AddScoped<IUseCase<DeleteCoverCommand>, DeleteCoverUseCase>();
        services.AddScoped<IUseCase<ComputePremiumCommand, decimal>, ComputePremiumUseCase>();

        return services;
    }
}
