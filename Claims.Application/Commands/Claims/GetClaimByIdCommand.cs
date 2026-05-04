namespace Claims.Application.Commands.Claims;

public record GetClaimByIdCommand
{
    public required string Id { get; init; }
}
