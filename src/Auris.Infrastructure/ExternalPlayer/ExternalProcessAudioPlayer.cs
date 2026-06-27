using Auris.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auris.Infrastructure.ExternalPlayer;

public class ExternalProcessAudioPlayer : IAudioPlayer {
    
    private readonly ExternalPlayerOptions _options;
    private readonly ExternalProcessRunner _processRunner;
    private readonly ILogger<ExternalProcessAudioPlayer> _logger;

    public ExternalProcessAudioPlayer(
        IOptions<ExternalPlayerOptions> options,
        ExternalProcessRunner processRunner,
        ILogger<ExternalProcessAudioPlayer> logger)
    {
        _options = options.Value;
        _processRunner = processRunner;
        _logger = logger;
    }

    public async Task PlayAsync(string filePath, CancellationToken cancellationToken)
    {
        //Валидируем ещё раз, т.к. может быть удалено пока оно в очереди
        ValidateFilePath(filePath); //TODO сделать валидацию через DI потому что эта валидация должна быть выше
        
        var status = await _processRunner.RunAsync(_options, filePath, cancellationToken);
        
        if (status == ExternalProcessRunStatus.NotStarted) {
            throw new InvalidOperationException(
                $"External audio player was not started. Executable: {_options.ExecutablePath}");
        }
    }

    private static void ValidateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) {
            throw new ArgumentException(
                "Audio file path is empty.",
                nameof(filePath));
        }

        if (!File.Exists(filePath)) {
            throw new FileNotFoundException(
                "Audio file was not found.",
                filePath);
        }
    }
}