namespace Auris.Infrastructure.AudioLibrary;

public class AudioLibraryOptions {
    public string RootPath { get; set; } = string.Empty;
    public string[] AllowedExtensions { get; set; } =
    [
        ".mp3",
        ".wav",
        ".flac",
        ".ogg",
        ".m4a",
        ".aac"
    ];
}