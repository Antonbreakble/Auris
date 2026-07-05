namespace Auris.Host.Admin.ViewModels;


public record QueueItemViewModel(
    Guid Id,
    string FileName,
    DateTimeOffset CreatedAt,
    string CreatedBy);