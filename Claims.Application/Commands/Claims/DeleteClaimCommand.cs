using Claims.Application.Abstractions;

namespace Claims.Application.Commands.Claims;

public record DeleteClaimCommand : ICommand
{
    public required string Id { get; init; }

    public required string HttpMethod { get; init; }
}
