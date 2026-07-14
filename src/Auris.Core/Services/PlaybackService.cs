using Auris.Core.Abstractions;
using Auris.Core.Models;

namespace Auris.Core.Services;

public class PlaybackService : IPlaybackService {
    private readonly IAudioPlayer _audioPlayer;
    private readonly IPlaybackStateProvider _stateProvider;
    
    private readonly Lock _lock = new();
    private CancellationTokenSource? _currentPlaybackCancellation;

    public PlaybackService(IAudioPlayer audioPlayer, IPlaybackStateProvider stateProvider) {
        _audioPlayer = audioPlayer;
        _stateProvider = stateProvider;
    }

    public async Task PlayAsync(PlaybackQueueItem item, CancellationToken cancellationToken) {
        using var playbackCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        lock (_lock) {
            _currentPlaybackCancellation = playbackCancellation;
        }
        
        _stateProvider.SetPlaying(item);
        
        try {
            await _audioPlayer.PlayAsync(item.FilePath, playbackCancellation.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) {
            
        }
        finally {
            _stateProvider.SetIdle();
            if (ReferenceEquals(_currentPlaybackCancellation, playbackCancellation)) {
                _currentPlaybackCancellation = null;
            }
        }
    }

    public bool StopCurrent() {
        lock (_lock) {
            if (_currentPlaybackCancellation is null || _currentPlaybackCancellation.IsCancellationRequested) {
                return false;
            }
            _currentPlaybackCancellation.Cancel();
            return true;
        }
    }
}