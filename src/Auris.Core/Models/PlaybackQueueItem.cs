namespace Auris.Core.Models;

public class PlaybackQueueItem {
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string FilePath { get; init; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; init; } = "";
}