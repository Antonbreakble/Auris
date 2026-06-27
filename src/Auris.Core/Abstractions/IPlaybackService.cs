using Auris.Core.Models;

namespace Auris.Core.Abstractions;

public interface IPlaybackService {
    Task PlayAsync(PlaybackQueueItem item, CancellationToken cancellationToken);
}