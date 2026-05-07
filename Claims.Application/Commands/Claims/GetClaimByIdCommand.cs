using Claims.Application.Abstractions;

namespace Claims.Application.Commands.Claims;

public record GetClaimByIdCommand : ICommand
{
    public required string Id { get; init; }
}
