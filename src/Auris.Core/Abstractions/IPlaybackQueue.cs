using Auris.Core.Models;

namespace Auris.Core.Abstractions;

public interface IQueue<T> {
    int Count { get; }

    int Capacity { get; }

    bool TryEnqueue(T item);

    Task<T> DequeueAsync(CancellationToken cancellationToken);

    IReadOnlyList<T> Snapshot();

    void Clear();
}