namespace Auris.Core.Abstractions;

public interface IAudioPlayer {
    Task PlayAsync(string filePath, CancellationToken cancellationToken);
}