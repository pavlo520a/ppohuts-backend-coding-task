namespace Claims.Application.Commands.Covers;

public record GetCoverByIdCommand
{
    public required string Id { get; init; }
}
