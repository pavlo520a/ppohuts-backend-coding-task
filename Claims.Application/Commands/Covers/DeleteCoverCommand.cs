namespace Claims.Application.Commands.Covers;

public record DeleteCoverCommand
{
    public required string Id { get; init; }

    public required string HttpMethod { get; init; }
}
