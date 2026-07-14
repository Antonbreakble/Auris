# Auris

Auris is a source-available local audio playback and text-to-speech service.

The application provides a REST API and a simple web admin panel for playing audio files from a local library and generating speech from text. Requests are processed through bounded in-memory queues and played sequentially.

Auris is intended to run as a small local service on Windows or Linux. Audio playback is performed by an external player process, for example `ffplay`. Text-to-speech synthesis is performed by an external command-line application.

## Features

* Local audio playback from a configured audio library.
* File system based audio library.
* Audio file discovery and lookup.
* Sequential playback through a bounded queue.
* Configurable queue capacity.
* CLI-based text-to-speech synthesis.
* Text-to-speech request queue.
* Generated speech caching based on normalized text.
* REST API for integration with external systems.
* Web admin panel.
* Current playback status view.
* Playback queue viewing and clearing.
* Available audio files view.
* Add audio files from the library to the playback queue.
* Submit text-to-speech requests from the admin panel.
* External player process support, for example `ffplay`.
* Cross-platform runtime target: Windows and Linux.

## Text-to-Speech Flow

Text-to-speech requests are processed separately from normal playback requests:

1. A text request is submitted through the REST API or the admin panel.
2. The request is added to the text-to-speech queue.
3. The background service normalizes the text and calculates its SHA-256 hash.
4. If a generated WAV file for the normalized text already exists, it is reused.
5. Otherwise, the configured command-line synthesizer generates the WAV file.
6. The generated audio file is added to the playback queue.

Generated files are stored in:

```text
<AudioLibrary.RootPath>/generated
```

File names use the following format:

```text
tts_<SHA256>.wav
```

The cache key is based only on normalized text. Changing the configured model does not invalidate previously generated files automatically.

## Web Admin Panel

Auris includes a Razor Pages based web admin panel.

Default page:

```text
/Admin
```

Current admin panel features:

* View current playback status.
* **Current queue**

  * View the current playback queue.
  * Refresh queue state.
  * Clear waiting queue items.
* **Audio files**

  * View audio files available in the configured audio library.
  * Refresh the file list.
  * Add audio files to the playback queue.
* **Speech synthesis**

  * Enter text to be synthesized.
  * Submit the text to the text-to-speech queue.
  * View the result of the submission.

The speech synthesis tab is currently an MVP implementation and requires further refactoring.

The admin panel uses a shared layout with common header and footer. The UI currently uses Bootstrap 5.

Bootstrap assets are expected to be available locally in the host project:

```text
src/Auris.Host/wwwroot/lib/bootstrap/css/bootstrap.min.css
src/Auris.Host/wwwroot/lib/bootstrap/js/bootstrap.bundle.min.js
```

## REST API

Full API documentation:

```text
docs/rest-api.md
```

Available operations:

| Method   | Path                       | Description                                      |
| -------- | -------------------------- | ------------------------------------------------ |
| `POST`   | `/api/audio/play`          | Add an audio file to the playback queue.         |
| `GET`    | `/api/audio/queue`         | Return the current playback queue.               |
| `DELETE` | `/api/audio/queue/clear`   | Clear all waiting playback items.                |
| `GET`    | `/api/audio/library/files` | Return available audio files.                    |
| `GET`    | `/api/audio/status`        | Return the current playback status.              |
| `POST`   | `/api/tts/play`            | Add text to the text-to-speech processing queue. |

## Configuration

Auris is configured through the host application settings.

Example configuration:

```json
{
  "Auris": {
    "Queue": {
      "Capacity": 100
    },
    "AudioLibrary": {
      "RootPath": "/AudioLibrary",
      "AllowedExtensions": [
        ".mp3",
        ".wav",
        ".flac",
        ".ogg",
        ".m4a",
        ".aac"
      ]
    },
    "Player": {
      "ExecutablePath": "ffplay",
      "Arguments": [
        "-nodisp",
        "-autoexit",
        "{file}"
      ],
      "WorkingDirectory": "",
      "KillOnCancellation": true
    },
    "TextToSpeech": {
      "Cli": {
        "ExecutablePath": "/tts_engine",
        "ModelPath": "/model.onnx"
      }
    }
  }
}
```

### Queue

`Capacity` defines the maximum number of waiting items.

Playback and text-to-speech requests use separate queue instances. The configured capacity is applied independently to each queue.

### Audio library

`RootPath` must point to an existing directory containing audio files.

Only files with extensions listed in `AllowedExtensions` are discovered by the audio library.

The `generated` subdirectory used for synthesized speech is created automatically.

### Player

`ExecutablePath` defines the external player executable.

For example:

```text
ffplay
```

or an absolute path to the executable.

`{file}` in `Arguments` is replaced with the full path of the audio file being played.

### Text-to-speech CLI

`ExecutablePath` defines the command-line text-to-speech executable.

`ModelPath` defines the model passed to the executable.

Paths may be absolute or relative to the application working directory.

The current CLI adapter:

* writes UTF-8 text to the process standard input;
* passes the model using `--model`;
* passes the output WAV path using `--output-file`;
* expects the process to exit with code `0`;
* expects the output file to be created successfully.

The text-to-speech executable and model are not included in this repository.

## Running Locally

Requirements:

* .NET 10 SDK.
* An external audio player.
* A compatible command-line text-to-speech executable and model when speech synthesis is used.

Configure the audio library, player, and text-to-speech paths in `appsettings.json`.

Make sure the configured external processes are available.

Run the host project:

```bash
dotnet run --project src/Auris.Host
```

Then open the admin panel:

```text
/Admin
```

## Current State

Auris is in early active development, but the main playback and text-to-speech flows can already be tested locally.

Implemented:

* Basic project structure.
* Audio library configuration.
* File system based audio library.
* Audio file discovery and lookup.
* Generic bounded queue implementation.
* Playback queue.
* Text-to-speech queue.
* External player integration.
* Background playback service.
* CLI-based text-to-speech synthesis.
* Generated speech caching.
* Background text-to-speech processing service.
* REST API for audio playback requests.
* REST API for text-to-speech requests.
* REST API for viewing and clearing the playback queue.
* REST API for viewing available audio files.
* REST API for current playback status.
* Web admin panel.
* Shared Razor Pages layout.
* Admin panel tab for the current queue.
* Admin panel tab for available audio files.
* Admin panel tab for speech synthesis.
* Current playback status view in the admin panel.
* Add-to-queue action from the admin panel audio library.

## Planned

* Refactoring of the MVP text-to-speech admin interface.
* Windows and Linux run instructions.
* Packaging and deployment improvements.
* Authentication and authorization if required.

## External Components

### FFmpeg / ffplay

Auris may use `ffplay` as an external player process.

FFmpeg / ffplay is not included in this repository. Users are responsible for installing a compatible FFmpeg build separately.

### Text-to-speech engine

The command-line text-to-speech engine and its models are not included in this repository. Users are responsible for installing and configuring compatible components separately.

## License

Auris is source-available software.

Non-commercial use is permitted under the PolyForm Noncommercial License 1.0.0.

Commercial use is not permitted under the public license and requires separate written permission from the copyright holder.

This project is not licensed as open source under the Open Source Definition, because commercial use is restricted.

See:

* `LICENSE`
* `NOTICE.md`
* `COMMERCIAL-LICENSE.md`
