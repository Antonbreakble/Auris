using Auris.Core.Abstractions;
using Auris.Core.Models;

namespace Auris.Core.Services;

public class PlaybackService : IPlaybackService {
    private readonly IAudioPlayer _audioPlayer;
    private readonly IPlaybackStateProvider _stateProvider;

    public PlaybackService(IAudioPlayer audioPlayer, IPlaybackStateProvider stateProvider) {
        _audioPlayer = audioPlayer;
        _stateProvider = stateProvider;
    }

    public async Task PlayAsync(PlaybackQueueItem item, CancellationToken cancellationToken) {
        _stateProvider.SetPlaying(item);
        try {
            await _audioPlayer.PlayAsync(item.FilePath, cancellationToken);
        }
        finally {
            _stateProvider.SetIdle();
        }
    }
}