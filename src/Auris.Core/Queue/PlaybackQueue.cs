using System.Collections.Concurrent;
using Auris.Core.Abstractions;
using Auris.Core.Models;
using Auris.Core.Options;
using Microsoft.Extensions.Options;

namespace Auris.Core.Queue;

public class PlaybackQueue : IPlaybackQueue {
    
    private readonly Queue<PlaybackQueueItem> _queue = new();
    private readonly SemaphoreSlim _itemsAvailable;
    private readonly Lock _lock = new();

    private int _count;

    public PlaybackQueue(IOptions<QueueOptions> options) {
        ArgumentNullException.ThrowIfNull(options);
        var capacity = options.Value.Capacity;
       
        if (capacity <= 0)
            throw new InvalidOperationException("Playback queue capacity must be greater than zero.");

        Capacity = capacity;
        _itemsAvailable = new SemaphoreSlim(0, capacity);
    }

    public int Capacity { get; }

    public int Count
    {
        get {
            lock (_lock)
                return _count;
        }
    }

    public bool TryEnqueue(PlaybackQueueItem item) {
        lock (_lock) {
            if (_count >= Capacity) 
                return false;
            
            _queue.Enqueue(item);
            _count++;

            _itemsAvailable.Release();
            
            return true;
        }
    }

    public async Task<PlaybackQueueItem> DequeueAsync(CancellationToken cancellationToken) {
        while (true) {
            await _itemsAvailable.WaitAsync(cancellationToken);
            lock (_lock) {
                if (!_queue.TryDequeue(out var item)) 
                    continue;
                _count--;
                return item;
            }
        }
    }

    public IReadOnlyList<PlaybackQueueItem> Snapshot() {
        lock (_lock)
            return _queue.ToArray();
    }

    public void Clear() {
        lock (_lock) {
            while (_queue.TryDequeue(out _)) {
            }

            _count = 0;

            while (_itemsAvailable.Wait(0)) {
            }
        }
    }
}