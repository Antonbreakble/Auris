namespace Auris.Core.Abstractions;

public interface ITextToSpeechSynthesizer {
    Task SynthesizeToFileAsync(
        string text,
        string outputFilePath,
        CancellationToken cancellationToken);
}