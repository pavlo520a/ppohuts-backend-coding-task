using Claims.Application.Abstractions;
using Claims.Domain.Enums;

namespace Claims.Application.Commands.Claims;

public record CreateClaimCommand : ICommand
{
    public required string CoverId { get; init; }

    public required DateTime Created { get; init; }

    public required string Name { get; init; }

    public required ClaimType Type { get; init; }

    public required decimal DamageCost { get; init; }

    public required string HttpMethod { get; init; }
}
