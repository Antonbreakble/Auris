namespace Auris.Host.Admin.ViewModels;

public record AdminDashboardViewModel(
    string ActiveTab,
    QueueTabViewModel Queue,
    LibraryTabViewModel Library);