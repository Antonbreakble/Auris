using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Auris.Core.Abstractions;
using Auris.Core.Models;
using Auris.Infrastructure.AudioLibrary;
using Microsoft.Extensions.Options;

namespace Auris.Host.Service;

public sealed class TextToSpeechBackgroundService : BackgroundService {
    private readonly IQueue<TextToSpeechQueueItem> _textToSpeechQueue;
    private readonly IQueue<PlaybackQueueItem> _playbackQueue;
    private readonly ITextToSpeechSynthesizer _synthesizer;
    private readonly ILogger<TextToSpeechBackgroundService> _logger;

    private readonly string _generatedDirectory;

    public TextToSpeechBackgroundService(
        IQueue<TextToSpeechQueueItem> textToSpeechQueue,
        IQueue<PlaybackQueueItem> playbackQueue,
        ITextToSpeechSynthesizer synthesizer,
        IOptions<AudioLibraryOptions> audioLibraryOptions,
        ILogger<TextToSpeechBackgroundService> logger)
    {
        _textToSpeechQueue = textToSpeechQueue;
        _playbackQueue = playbackQueue;
        _synthesizer = synthesizer;
        _logger = logger;

        _generatedDirectory = Path.Combine(
            audioLibraryOptions.Value.RootPath,
            "generated");

        Directory.CreateDirectory(_generatedDirectory);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Text-to-speech background service started.");

        while (!stoppingToken.IsCancellationRequested) {
            TextToSpeechQueueItem? item = null;

            try {
                item = await _textToSpeechQueue.DequeueAsync(stoppingToken);
                await ProcessAsync(item, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                break;
            }
            catch (Exception exception) {
                _logger.LogError(
                    exception,
                    "Text-to-speech processing failed. " +
                    "ItemId: {ItemId}, Text: {Text}",
                    item?.Id,
                    item?.Text);
            }
        }

        _logger.LogInformation(
            "Text-to-speech background service stopped.");
    }

    private async Task ProcessAsync(TextToSpeechQueueItem item, CancellationToken cancellationToken) {
        var text = NormalizeText(item.Text);
        var filePath = GetAudioFilePath(text);

        if (!File.Exists(filePath))
            await _synthesizer.SynthesizeToFileAsync(text, filePath, cancellationToken);
        
        var playbackItem = new PlaybackQueueItem {
            FilePath = filePath,
            CreatedBy = item.CreatedBy
        };

        _playbackQueue.TryEnqueue(playbackItem);
    }

    private string GetAudioFilePath(string text) {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        var fileName = $"tts_{Convert.ToHexString(hash)}.wav";
        return Path.Combine(_generatedDirectory, fileName);
    }

    private static string NormalizeText(string text) {
        return Regex.Replace(text.Trim(), @"\s+", " ");
    }
}