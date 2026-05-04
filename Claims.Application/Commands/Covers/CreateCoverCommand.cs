using Claims.Domain.Enums;

namespace Claims.Application.Commands.Covers;

public record CreateCoverCommand(DateTime StartDate, DateTime EndDate, CoverType Type);
