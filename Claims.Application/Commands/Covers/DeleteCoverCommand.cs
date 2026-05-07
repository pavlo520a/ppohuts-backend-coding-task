using Claims.Application.Abstractions;

namespace Claims.Application.Commands.Covers;

public record DeleteCoverCommand : ICommand
{
    public required string Id { get; init; }

    public required string HttpMethod { get; init; }
}
