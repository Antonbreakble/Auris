using System.Diagnostics;

namespace Auris.Infrastructure.ExternalPlayer;

public class ExternalProcessRunner {
     private const string FilePlaceholder = "{file}";

    public async Task<ExternalProcessRunStatus> RunAsync(
        ExternalPlayerOptions options,
        string filePath,
        CancellationToken cancellationToken)
    {
        using var process = CreateProcess(options, filePath);

        try {
            if (!process.Start())
                return ExternalProcessRunStatus.NotStarted;
        }
        catch {
            return ExternalProcessRunStatus.NotStarted;
        }

        try {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            await StopProcessAsync(process, options.KillOnCancellation);
        }

        return ExternalProcessRunStatus.Completed;
    }

    private static Process CreateProcess(ExternalPlayerOptions options, string filePath) {
        var startInfo = new ProcessStartInfo {
            FileName = options.ExecutablePath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = false,
            RedirectStandardError = false
        };

        if (!string.IsNullOrWhiteSpace(options.WorkingDirectory)) {
            startInfo.WorkingDirectory = options.WorkingDirectory;
        }

        foreach (var argument in BuildArguments(options, filePath)) {
            startInfo.ArgumentList.Add(argument);
        }

        return new Process {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };
    }

    private static IEnumerable<string> BuildArguments(ExternalPlayerOptions options, string filePath) {
        return options.Arguments.Select(argument => argument.Replace(FilePlaceholder, filePath));
    }

    private static async Task StopProcessAsync(Process process, bool killOnCancellation)
    {
        if (process.HasExited)
            return;

        if (!killOnCancellation)
            return;

        process.Kill(entireProcessTree: true);

        await process.WaitForExitAsync(CancellationToken.None);
    }
}