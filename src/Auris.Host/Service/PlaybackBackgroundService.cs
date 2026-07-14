using Auris.Core.Abstractions;
using Auris.Core.Models;
using Auris.Core.Services;

namespace Auris.Host.Service;

public class PlaybackBackgroundService : BackgroundService{
    
    private readonly IQueue<PlaybackQueueItem> _playbackQueue;
    private readonly IPlaybackService _playbackService;
    private readonly ILogger<PlaybackBackgroundService> _logger;
    
    public PlaybackBackgroundService(IQueue<PlaybackQueueItem> playbackQueue,
        IPlaybackService playbackService,
        ILogger<PlaybackBackgroundService> logger) 
    {
        _playbackService = playbackService;
        _playbackQueue = playbackQueue;
        _logger = logger; 
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Playback background service started.");
        
        while (!stoppingToken.IsCancellationRequested) {
            PlaybackQueueItem? item = null;

            try {
                item = await _playbackQueue.DequeueAsync(stoppingToken);
                
                await _playbackService.PlayAsync(item, stoppingToken);
                
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                break;
            }
            catch (Exception exception) {
                _logger.LogError(
                    exception,
                    "Playback failed. ItemId: {ItemId}, FilePath: {FilePath}",
                    item?.Id,
                    item?.FilePath);
            }
        }
        
        _logger.LogInformation("Playback background service stopped.");
    }
}