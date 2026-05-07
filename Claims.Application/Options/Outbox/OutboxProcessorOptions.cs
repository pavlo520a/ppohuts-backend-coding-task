using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options.Outbox;

public sealed class OutboxProcessorOptions
{
    public const string SectionName = "OutboxProcessor";

    [Range(1, int.MaxValue)]
    public int BatchSize { get; init; } = 20;

    [Range(1, int.MaxValue)]
    public int PollIntervalMs { get; init; } = 1000;

    [Range(1, int.MaxValue)]
    public int MaxAttempts { get; init; } = 5;
}
