using Auris.Core.Models;

namespace Auris.Core.Abstractions;

public interface IAudioLibrary {
    IReadOnlyList<AudioLibraryFile> GetAudioFiles();

    /// <summary>
    /// Finds an audio file by file name.
    /// Returns null when file is not found or when file name is ambiguous.
    /// </summary>
    AudioLibraryFile? FindAudioFile(string fileName);
}