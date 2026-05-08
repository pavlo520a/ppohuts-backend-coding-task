using Claims.Application.Abstractions;
using Claims.Domain.Enums;

namespace Claims.Application.Commands.Covers;

public record CreateCoverCommand : ICommand
{
    public required DateTime StartDate { get; init; }

    public required DateTime EndDate { get; init; }

    public required CoverType Type { get; init; }

    public required string HttpMethod { get; init; }
}
