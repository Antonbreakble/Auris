using Auris.Core.Models;

namespace Auris.Core.Abstractions;

public interface IPlaybackQueue {
    int Count { get; }

    int Capacity { get; }

    bool TryEnqueue(PlaybackQueueItem item);

    Task<PlaybackQueueItem> DequeueAsync(CancellationToken cancellationToken);

    IReadOnlyList<PlaybackQueueItem> Snapshot();

    void Clear();
}