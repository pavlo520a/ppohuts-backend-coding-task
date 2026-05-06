using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Models;
using FluentValidation;
using System.Diagnostics.Tracing;
using System.Net.Http;
using System.Reflection.Metadata;

namespace Claims.Application.UseCases.Claims;

public sealed class CreateClaimUseCase(
    IClaimRepository claimRepository,
    IValidator<CreateClaimCommand> validator) : IUseCase<CreateClaimCommand, Claim>
{
    public async Task<Claim> ExecuteAsync(CreateClaimCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var claim = new Claim
        {
            Id = Guid.NewGuid().ToString(),
            CoverId = command.CoverId,
            Created = command.Created,
            Name = command.Name,
            Type = command.Type,
            DamageCost = command.DamageCost
        };

        await claimRepository.AddAsync(claim, command.HttpMethod, cancellationToken);
        return claim;
    }
}
