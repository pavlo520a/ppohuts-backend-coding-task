using Claims.Domain.Enums;

namespace Claims.Application.Commands.Covers;

public record ComputePremiumCommand(DateTime StartDate, DateTime EndDate, CoverType CoverType);
