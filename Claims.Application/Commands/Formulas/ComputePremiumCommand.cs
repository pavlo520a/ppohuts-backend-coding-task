using Claims.Domain.Enums;

namespace Claims.Application.Commands.Formulas;

public record ComputePremiumCommand(DateTime StartDate, DateTime EndDate, CoverType CoverType);
