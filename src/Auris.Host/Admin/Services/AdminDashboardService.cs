using Auris.Core.Abstractions;
using Auris.Core.Models;
using Auris.Host.Admin.ViewModels;

namespace Auris.Host.Admin.Services;

public class AdminDashboardService
{
    public const string QueueTab = "queue";
    public const string LibraryTab = "library";

    private readonly IQueue<PlaybackQueueItem> _playbackQueue;
    private readonly IAudioLibrary _audioLibrary;

    public AdminDashboardService(
        IQueue<PlaybackQueueItem> playbackQueue,
        IAudioLibrary audioLibrary)
    {
        _playbackQueue = playbackQueue;
        _audioLibrary = audioLibrary;
    }

    public AdminDashboardViewModel GetDashboard(string? tab) {
        var activeTab = NormalizeTab(tab);

        return new AdminDashboardViewModel(
            ActiveTab: activeTab,
            Queue: GetQueue(),
            Library: GetLibrary());
    }

    public void ClearQueue() => _playbackQueue.Clear();

    private QueueTabViewModel GetQueue()
    {
        var items = _playbackQueue
            .Snapshot()
            .Select(item => new QueueItemViewModel(
                Id: item.Id,
                FileName: Path.GetFileName(item.FilePath),
                CreatedAt: item.CreatedAt,
                CreatedBy: item.CreatedBy))
            .ToArray();

        return new QueueTabViewModel(
            Count: _playbackQueue.Count,
            Capacity: _playbackQueue.Capacity,
            Items: items);
    }

    private LibraryTabViewModel GetLibrary() {
        var files = _audioLibrary
            .GetAudioFiles()
            .OrderBy(file => file.Name)
            .Select(file => new AudioFileViewModel(
                Name: file.Name,
                Extension: file.Extension))
            .ToArray();

        return new LibraryTabViewModel(files);
    }

    private static string NormalizeTab(string? tab) {
        return tab?.ToLowerInvariant() switch {
            LibraryTab => LibraryTab,
            _ => QueueTab
        };
    }
}