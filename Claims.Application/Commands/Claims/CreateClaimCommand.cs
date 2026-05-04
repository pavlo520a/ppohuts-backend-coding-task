using Claims.Domain.Enums;

namespace Claims.Application.Commands.Claims;

public record CreateClaimCommand(
    string CoverId,
    DateTime Created,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    string HttpMethod);
