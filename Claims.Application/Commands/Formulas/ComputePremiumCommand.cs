using Claims.Application.Abstractions;
using Claims.Domain.Enums;

namespace Claims.Application.Commands.Formulas;

public record ComputePremiumCommand : ICommand
{
    public required DateTime StartDate { get; init; }

    public required DateTime EndDate { get; init; }

    public required CoverType CoverType { get; init; }

    public required string HttpMethod { get; init; }
}
