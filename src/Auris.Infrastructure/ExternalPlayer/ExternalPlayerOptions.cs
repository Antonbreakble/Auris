namespace Auris.Infrastructure.ExternalPlayer;

public class ExternalPlayerOptions {
    public string ExecutablePath { get; set; } = string.Empty;

    public string[] Arguments { get; set; } = [];

    public string? WorkingDirectory { get; set; }

    public bool KillOnCancellation { get; set; } = true;
}