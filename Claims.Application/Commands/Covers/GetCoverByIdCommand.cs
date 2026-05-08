using Claims.Application.Abstractions;

namespace Claims.Application.Commands.Covers;

public record GetCoverByIdCommand : ICommand
{
    public required string Id { get; init; }
}
