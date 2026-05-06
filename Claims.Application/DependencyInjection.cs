using Claims.Application.Abstractions;
using Claims.Application.Abstractions.Formulas;
using Claims.Domain.Models.Formulas;
using Claims.Application.Commands.Claims;
using Claims.Application.Commands.Covers;
using Claims.Application.Commands.Formulas;
using Claims.Application.Formulas;
using Claims.Application.HostedServices;
using Claims.Application.Options;
using Claims.Application.Options.Formulas;
using Claims.Application.Options.Outbox;
using Claims.Application.Services;
using Claims.Application.UseCases.Claims;
using Claims.Application.UseCases.Covers;
using Claims.Application.UseCases.Formulas;
using Claims.Application.Validation.Claims;
using Claims.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateClaimCommandValidator>();

        services
            .AddOptions<ValidationRulesOptions>()
            .BindConfiguration(ValidationRulesOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<PremiumPricingOptions>()
            .BindConfiguration(PremiumPricingOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<OutboxProcessorOptions>()
            .BindConfiguration(OutboxProcessorOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<ServiceBusOptions>()
            .BindConfiguration(ServiceBusOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IFormula<CoverPremiumFormulaArgs>, PremiumFormula>();
        services.AddSingleton<IServiceBusService, ServiceBusService>();
        services.AddHostedService<OutboxQueueHostedService>();

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
