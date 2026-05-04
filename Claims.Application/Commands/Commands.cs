using Claims.Domain;

namespace Claims.Application.Commands;

public record GetClaimsCommand;

public record GetClaimByIdCommand(string Id);

public record CreateClaimCommand(string CoverId, DateTime Created, string Name, ClaimType Type, decimal DamageCost);

public record DeleteClaimCommand(string Id);

public record GetCoversCommand;

public record GetCoverByIdCommand(string Id);

public record CreateCoverCommand(DateTime StartDate, DateTime EndDate, CoverType Type);

public record DeleteCoverCommand(string Id);

public record ComputePremiumCommand(DateTime StartDate, DateTime EndDate, CoverType CoverType);
