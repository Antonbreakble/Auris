# Auris

Auris is a source-available local audio playback service.

The application provides a REST API for playing audio files from a local library.
If another file is already playing, new playback requests are placed into a queue.
Auris also provides a web admin panel for viewing and clearing the queue.

## Features

- Local audio playback
- REST API
- Playback queue
- Configurable queue depth
- Web admin panel
- Cross-platform runtime target: Windows and Linux
- External player process support, for example ffplay

## Project Status

Auris is in early active development.

### Implemented

* Basic project structure.
* Audio library configuration.
* File system based audio library.
* Audio file discovery and lookup.
* Playback queue core.

### In Progress

* REST API for adding files to the playback queue.
* Integration between audio library, queue, and playback service.

### Planned

* Web admin panel.
* Queue management endpoints.
* Playback status endpoint.
* External player integration.
* Windows and Linux run instructions.


## License

Auris is source-available software.

Non-commercial use is permitted under the PolyForm Noncommercial License 1.0.0.

Commercial use is not permitted under the public license and requires
separate written permission from the copyright holder.

This project is not licensed as open source under the Open Source Definition,
because commercial use is restricted.

See:

- LICENSE
- NOTICE.md
- COMMERCIAL-LICENSE.md

## FFmpeg / ffplay

Auris may use ffplay as an external player process.

FFmpeg / ffplay is not included in this repository.
Users are responsible for installing a compatible FFmpeg build separately.