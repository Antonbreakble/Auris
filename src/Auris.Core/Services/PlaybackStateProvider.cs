using Auris.Core.Abstractions;
using Auris.Core.Models;

namespace Auris.Core.Services;

public class PlaybackStateProvider : IPlaybackStateProvider {
    private readonly Lock _lock = new();

    private PlaybackStatus _status = CreateIdleStatus();

    public PlaybackStatus GetStatus() {
        lock (_lock) {
            return _status;
        }
    }

    public void SetIdle() {
        lock (_lock) {
            _status = CreateIdleStatus();
        }
    }

    public void SetPlaying(PlaybackQueueItem item) {
        lock (_lock) {
            _status = CreatePlayingStatus(item);
        }
    }
    
    private static PlaybackStatus CreateIdleStatus()
    {
        return new PlaybackStatus
        {
            State = PlaybackState.Idle,
            FinishedAt = DateTimeOffset.UtcNow
        };
    }

    private static PlaybackStatus CreatePlayingStatus(PlaybackQueueItem item)
    {
        return new PlaybackStatus
        {
            State = PlaybackState.Playing,
            CurrentItemId = item.Id,
            FilePath = item.FilePath,
            StartedAt = DateTimeOffset.UtcNow,
            FinishedAt = null
        };
    }
    
}