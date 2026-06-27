namespace Auris.Core.Models;

public record PlaybackStatus {
    public PlaybackState State { get; init; } = PlaybackState.Idle;

    public Guid? CurrentItemId { get; init; }

    public string? FilePath { get; init; }

    public DateTimeOffset? StartedAt { get; init; }

    public DateTimeOffset? FinishedAt { get; init; }
    
}