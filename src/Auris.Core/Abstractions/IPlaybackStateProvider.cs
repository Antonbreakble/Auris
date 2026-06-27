using Auris.Core.Models;

namespace Auris.Core.Abstractions;

public interface IPlaybackStateProvider {
    PlaybackStatus GetStatus();

    void SetIdle();

    void SetPlaying(PlaybackQueueItem item);
}