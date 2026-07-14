namespace Auris.Host.Contracts;

public sealed class PlayTextRequest {
    public string? Text { get; init; }
    public string? RequestedBy { get; init; }
}