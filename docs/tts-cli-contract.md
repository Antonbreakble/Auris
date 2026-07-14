# TTS CLI Contract

This document defines the interface between Auris and an external command-line
text-to-speech generator.

## Process invocation

Auris starts the generator as an external process using the following arguments:

```text
<ExecutablePath> --model <ModelPath> --output-file <OutputFilePath>
```

| Argument | Description |
|---|---|
| `ExecutablePath` | Path to the TTS generator executable |
| `ModelPath` | Path to the voice model |
| `OutputFilePath` | Path where the generated WAV file must be created |

Arguments are passed directly to the process without using a command shell.

## Text input

Auris sends the text to synthesize through the process standard input:

- encoding: UTF-8;
- the complete text is written to `stdin`;
- `stdin` is closed after the text is written;
- the generator must process the input after receiving EOF.

The generator must not expect the text as a command-line argument.

## Audio output

The generator must:

1. Create a WAV file at the exact path provided in `--output-file`.
2. Fully write and close the file before the process exits.
3. Replace the file if it already exists.

Audio data must not be written to `stdout`.

## Exit codes

The generator must return:

- `0` when synthesis completes successfully;
- a non-zero code when synthesis fails.

Auris considers the operation successful only when both conditions are met:

- the process exits with code `0`;
- the output file exists.

## Diagnostic output

- Error details should be written to `stderr`.
- `stdout` may be used for informational or diagnostic messages.
- Diagnostic output must not contain binary audio data.

## Cancellation

When synthesis is cancelled or Auris is stopped, Auris terminates the generator
process and its child processes.

The generator should not start detached background processes.

## Minimal compatibility requirements

A compatible TTS generator must:

- support `--model`;
- support `--output-file`;
- read UTF-8 text from `stdin`;
- start synthesis after EOF;
- create a valid WAV file at the requested path;
- return `0` only after successful synthesis;
- return a non-zero exit code and write an error message to `stderr` on failure.