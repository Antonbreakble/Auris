using System.Diagnostics;
using System.Text;
using Auris.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace Auris.Infrastructure.TextToSpeech;

public sealed class CliTextToSpeechSynthesizer : ITextToSpeechSynthesizer {
   
    private readonly TextToSpeechCliOptions _options;
    
    public CliTextToSpeechSynthesizer(IOptions<TextToSpeechCliOptions> options) {
        _options = options.Value;
    }

    public async Task SynthesizeToFileAsync(string text, string outputFilePath, CancellationToken cancellationToken) {
        
        var directory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var temporaryPath = $"{outputFilePath}.tmp.wav";

        try
        {
            using var process = CreateProcess(temporaryPath);

            process.Start();

            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            try {
                await process.StandardInput.WriteAsync(text.AsMemory(), cancellationToken);
                process.StandardInput.Close();
                await process.WaitForExitAsync(cancellationToken);
            }
            catch {
                if (!process.HasExited) {
                    process.Kill(entireProcessTree: true);
                    await process.WaitForExitAsync(CancellationToken.None);
                }
                throw;
            }

            var stdout = await stdoutTask;
            var stderr = await stderrTask;

            if (process.ExitCode != 0) {
                throw new InvalidOperationException(
                    $"Text-to-speech CLI exited with code " +
                    $"{process.ExitCode}: {stderr.Trim()}");
            }

            if (!File.Exists(temporaryPath)) {
                throw new InvalidOperationException(
                    $"Text-to-speech CLI did not create the output file. " +
                    $"Output: {stdout.Trim()}");
            }

            File.Move(temporaryPath, outputFilePath, overwrite: true);
        }
        finally {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }

    private Process CreateProcess(string outputFilePath)
    {
        var startInfo = new ProcessStartInfo {
            FileName = _options.ExecutablePath,
            UseShellExecute = false,
            CreateNoWindow = true,

            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,

            StandardInputEncoding = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        startInfo.ArgumentList.Add("--model");
        startInfo.ArgumentList.Add(_options.ModelPath);

        startInfo.ArgumentList.Add("--output-file");
        startInfo.ArgumentList.Add(outputFilePath);

        return new Process {
            StartInfo = startInfo
        };
    }
}