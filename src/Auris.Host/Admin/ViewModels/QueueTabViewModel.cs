namespace Auris.Host.Admin.ViewModels;

public record QueueTabViewModel(
    int Count,
    int Capacity,
    IReadOnlyList<QueueItemViewModel> Items);