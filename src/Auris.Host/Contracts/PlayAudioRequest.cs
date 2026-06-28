namespace Auris.Host.Contracts;

public class PlayAudioRequest {
    public string? FileName { get; init; }
    public string? RequestedBy { get; init; }
}