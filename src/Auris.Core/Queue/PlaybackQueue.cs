using System.Collections.Concurrent;
using Auris.Core.Abstractions;
using Auris.Core.Models;
using Auris.Core.Options;
using Microsoft.Extensions.Options;

namespace Auris.Core.Queue;

public class BoundedQueue<T> : IQueue<T> {
    
    private readonly Queue<T> _queue = new();
    private readonly SemaphoreSlim _itemsAvailable;
    private readonly Lock _lock = new();

    private int _count;

    public BoundedQueue(IOptions<QueueOptions> options) {
        ArgumentNullException.ThrowIfNull(options);
        var capacity = options.Value.Capacity;
       
        if (capacity <= 0)
            throw new InvalidOperationException("Queue capacity must be greater than zero.");

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

    public bool TryEnqueue(T item) {
        lock (_lock) {
            if (_count >= Capacity) 
                return false;
            
            _queue.Enqueue(item);
            _count++;

            _itemsAvailable.Release();
            
            return true;
        }
    }

    public async Task<T> DequeueAsync(CancellationToken cancellationToken) {
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

    public IReadOnlyList<T> Snapshot() {
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