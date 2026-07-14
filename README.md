# Auris

Auris is a source-available local audio playback and text-to-speech service for Windows and Linux.

The application provides a REST API and a web admin panel. Playback and text-to-speech requests are processed through bounded queues. External CLI applications are used for audio playback and speech synthesis.

## Features

- Local file-based audio library.
- Queued audio playback.
- Text-to-speech generation with WAV caching.
- Configurable external audio player and TTS generator.
- REST API for integration with external systems.
- Web admin panel for playback, queue, library, and text-to-speech operations.

## Documentation

- [REST API](docs/rest-api.md)
- [TTS CLI contract](docs/tts-cli-contract.md)

## Requirements

- .NET 10 SDK or runtime.
- An external audio player, such as `ffplay`.
- A TTS CLI generator implementing the [TTS CLI contract](docs/tts-cli-contract.md), if text-to-speech is used.

## Configuration

Auris is configured through `src/Auris.Host/appsettings.json`.

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
        "ExecutablePath": "/tts-generator",
        "ModelPath": "/voice-model.onnx"
      }
    }
  }
}
```

`AudioLibrary:RootPath` must point to an existing directory. Generated speech files are stored in its `generated` subdirectory.

`{file}` in the player arguments is replaced with the full path of the audio file.

## Running

Ensure the configured external applications are available, then run:

```bash
dotnet run --project src/Auris.Host
```

Open the admin panel at:

```text
/Admin
```

The application address and port depend on the launch configuration.

## License

Auris is source-available software.

Non-commercial use is permitted under the PolyForm Noncommercial License 1.0.0. Commercial use requires separate written permission from the copyright holder.

See [LICENSE](LICENSE), [NOTICE.md](NOTICE.md), and [COMMERCIAL-LICENSE.md](COMMERCIAL-LICENSE.md).