namespace Auris.Host.Admin.ViewModels;

public record LibraryTabViewModel(
    IReadOnlyList<AudioFileViewModel> Files);