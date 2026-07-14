namespace Auris.Core.Models;

public class TextToSpeechQueueItem {
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Text { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; init; } = "";
}