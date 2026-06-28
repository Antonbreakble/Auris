using Auris.Core.Abstractions;
using Auris.Core.Models;
using Microsoft.Extensions.Options;

namespace Auris.Infrastructure.AudioLibrary;

public class FileSystemAudioLibrary : IAudioLibrary {
    private readonly AudioLibraryOptions _options;

    public FileSystemAudioLibrary(IOptions<AudioLibraryOptions> options) {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;
    }

    public IReadOnlyList<AudioLibraryFile> GetAudioFiles() {
        return Directory
            .EnumerateFiles(_options.RootPath, "*", SearchOption.AllDirectories)
            .Where(IsAudioFile)
            .Select(fullPath => new AudioLibraryFile(
                Name: Path.GetFileName(fullPath),
                FullPath: fullPath,
                Extension: Path.GetExtension(fullPath))
            )
            .OrderBy(file => file.FullPath, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public AudioLibraryFile? FindAudioFile(string fileName) {
        if (string.IsNullOrWhiteSpace(fileName)) {
            return null;
        }
        
        var matches = GetAudioFiles()
            .Where(file => string.Equals(file.Name, fileName, StringComparison.OrdinalIgnoreCase))
            .Take(2)
            .ToArray();

        return matches.Length == 1
            ? matches[0]
            : null;
    }

    private bool IsAudioFile(string filePath) {
        var extension = Path.GetExtension(filePath);

        return _options.AllowedExtensions.Any(allowedExtension =>
            string.Equals(
                allowedExtension,
                extension,
                StringComparison.OrdinalIgnoreCase));
    }
    
}