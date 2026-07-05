# Auris

Auris is a source-available local audio playback service.

The application provides a REST API and a simple web admin panel for working with audio files from a local library. If another file is already playing, new playback requests are placed into a queue.

Auris is intended to run as a small local service on Windows or Linux and use an external audio player process, for example `ffplay`.

## Features

* Local audio playback from a configured audio library.
* File system based audio library.
* Audio file discovery and lookup.
* Playback queue.
* Configurable queue depth.
* REST API for integration with external systems.
* Web admin panel.
* Queue viewing and clearing from the admin panel.
* Available audio files view from the admin panel.
* External player process support, for example `ffplay`.
* Cross-platform runtime target: Windows and Linux.

## Web Admin Panel

Auris includes a Razor Pages based web admin panel.

Default page:

```text
/Admin
```

Current admin panel tabs:

* **Current queue**

  * View current playback queue.
  * Refresh queue state.
  * Clear waiting queue items.

* **Audio files**

  * View audio files available in the configured audio library.
  * Refresh file list.

The admin panel uses a shared layout with common header and footer.

The UI currently uses Bootstrap 5. Bootstrap assets are expected to be available locally in the host project:

```text
src/Auris.Host/wwwroot/lib/bootstrap/css/bootstrap.min.css
src/Auris.Host/wwwroot/lib/bootstrap/js/bootstrap.bundle.min.js
```

## REST API

API documentation:

```text
docs/rest-api.md
```

Available operations:

```http
POST /api/audio/play
```

Adds an audio file to the playback queue.

```http
GET /api/audio/queue
```

Returns the current playback queue.

```http
DELETE /api/audio/queue/clear
```

Clears all waiting items from the playback queue.

```http
GET /api/audio/library/files
```

Returns audio files available in the configured audio library.

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
      "RootPath": "D:\\AudioLibrary",
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
    }
  }
}
```

### Audio library

`RootPath` must point to the directory with audio files.

Only files with extensions listed in `AllowedExtensions` are discovered by the audio library.

### Player

`ExecutablePath` defines the external player executable.

For example:

```text
ffplay
```

or an absolute path to `ffplay`.

`{file}` in `Arguments` is replaced with the full path of the audio file being played.

## Running locally

Configure the audio library path in `appsettings.json`.

Make sure the external player is available. For example, install FFmpeg and make `ffplay` available in `PATH`, or configure an absolute path to the executable.

Run the host project:

```bash
dotnet run --project src/Auris.Host
```

Then open the admin panel:

```text
/Admin
```

## Current State

Auris is in early active development, but the basic playback flow can already be tested locally.

Implemented:

* Basic project structure.
* Audio library configuration.
* File system based audio library.
* Audio file discovery and lookup.
* Playback queue core.
* External player integration.
* Background playback service.
* REST API for playback requests.
* REST API for viewing and clearing the playback queue.
* REST API for viewing available audio files.
* Web admin panel.
* Shared Razor Pages layout.
* Admin panel tab for current queue.
* Admin panel tab for available audio files.

## Planned

* Playback status endpoint.
* Playback status view in the admin panel.
* Add-to-queue action from the admin panel.
* Windows and Linux run instructions.
* Packaging and deployment improvements.
* Authentication and authorization if required.

## FFmpeg / ffplay

Auris may use `ffplay` as an external player process.

FFmpeg / ffplay is not included in this repository. Users are responsible for installing a compatible FFmpeg build separately.

## License

Auris is source-available software.

Non-commercial use is permitted under the PolyForm Noncommercial License 1.0.0.

Commercial use is not permitted under the public license and requires separate written permission from the copyright holder.

This project is not licensed as open source under the Open Source Definition, because commercial use is restricted.

See:

* `LICENSE`
* `NOTICE.md`
* `COMMERCIAL-LICENSE.md`
