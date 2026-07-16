# Auris REST API

This document describes the current Auris REST API.

The API is intended for integration with external systems, operator workstations, SCADA systems, scripts, and engineering tools.

## Current State

The REST API is implemented and can be tested locally.

Available operations:

- play an audio file;
- view the playback queue;
- view playback queue utilization;
- clear the playback queue;
- view available audio files;
- view the current playback status;
- stop the current playback;
- submit text for speech synthesis and playback.

## Base URL

The actual address and port depend on the application launch settings.

Example:

```text
http://localhost:<port>
```

## Content Type

Requests with a body use JSON:

```http
Content-Type: application/json
```

Unless otherwise specified, successful responses use JSON or have an empty body.

The playback queue count endpoint returns plain text:

```http
Content-Type: text/plain
```

## SCADA Compatibility

Some SCADA systems support only `GET` and `POST` requests.

For this reason, operations that are normally exposed through `DELETE` also provide equivalent `POST` command endpoints:

| Operation | REST endpoint | SCADA-compatible endpoint |
| --- | --- | --- |
| Clear playback queue | `DELETE /api/audio/queue/clear` | `POST /api/audio/queue/clear` |
| Stop current playback | `DELETE /api/audio/current` | `POST /api/audio/current/stop` |

Both variants execute the same operation and return the same status codes.

## Request Source

Audio and text-to-speech requests support the optional `requestedBy` field.

When `requestedBy` is not provided or contains only whitespace, Auris uses the remote IP address. If the address is unavailable, the value `api` is used.

## Play Audio File

Adds an audio file to the playback queue.

If nothing is currently playing, playback starts when the background playback service processes the queue.

If another file is already playing, the requested file waits in the queue.

```http
POST /api/audio/play
```

### Request Body

```json
{
  "fileName": "alarm.mp3",
  "requestedBy": "Operator workstation"
}
```

### Fields

| Field | Required | Description |
| --- | ---: | --- |
| `fileName` | yes | Audio file name from the configured audio library. |
| `requestedBy` | no | Source of the request, for example a workstation, SCADA system, script, or operator station. |

### Successful Response

```http
202 Accepted
```

The response body is empty.

### Error Responses

```http
400 Bad Request
```

The request is invalid. For example, `fileName` is empty.

```http
404 Not Found
```

The audio file was not found or the file name is ambiguous.

```http
503 Service Unavailable
```

The playback queue is full.

## Submit Text-to-Speech Request

Adds text to the text-to-speech processing queue.

The request is processed asynchronously. Auris normalizes the text, reuses an existing generated WAV file when available, or invokes the configured command-line synthesizer. The resulting audio file is then submitted to the playback queue.

```http
POST /api/tts/play
```

### Request Body

```json
{
  "text": "Attention. The process has been stopped.",
  "requestedBy": "SCADA"
}
```

### Fields

| Field | Required | Description |
| --- | ---: | --- |
| `text` | yes | Text to synthesize. Empty or whitespace-only values are rejected. |
| `requestedBy` | no | Source of the request, for example a workstation, SCADA system, script, or operator station. |

### Successful Response

```http
202 Accepted
```

The response body is empty.

A successful response means that the request was accepted into the text-to-speech queue. Synthesis and playback happen asynchronously after the response is returned.

### Error Responses

```http
400 Bad Request
```

The `text` field is empty or contains only whitespace.

```http
503 Service Unavailable
```

The text-to-speech queue is full.

### Processing and Cache Behavior

Before synthesis, Auris trims the text and replaces consecutive whitespace characters with a single space.

Generated files are stored in:

```text
<AudioLibrary.RootPath>/generated
```

The file name is calculated from the SHA-256 hash of the normalized text:

```text
tts_<SHA256>.wav
```

If the file already exists, synthesis is skipped and the cached file is used.

The cache key is based only on normalized text. Changing the configured voice or model does not invalidate an existing cached file automatically.

The current API does not provide endpoints for viewing or clearing the text-to-speech queue.

## View Playback Queue

Returns all waiting items currently stored in the playback queue.

The item being played is represented by the playback status endpoint and is not part of the waiting queue response.

```http
GET /api/audio/queue
```

### Successful Response

```http
200 OK
```

Example:

```json
{
  "count": 2,
  "capacity": 100,
  "items": [
    {
      "id": "f3b41c59-3f0e-45b5-8127-33a8be3fd8a1",
      "fileName": "alarm.mp3",
      "createdAt": "2026-06-28T10:15:30.0000000+00:00",
      "createdBy": "Operator workstation"
    },
    {
      "id": "d2f86d81-74aa-4f91-a590-26a2c37b0eb1",
      "fileName": "warning.mp3",
      "createdAt": "2026-06-28T10:16:02.0000000+00:00",
      "createdBy": "SCADA"
    }
  ]
}
```

## View Playback Queue Count

Returns the number of waiting items and the configured playback queue capacity in a compact text format.

The currently playing item is not included in the count.

```http
GET /api/audio/queue/count
```

### Successful Response

```http
200 OK
Content-Type: text/plain; charset=utf-8
```

Example:

```text
0/100
```

The value before `/` is the current number of waiting items. The value after `/` is the maximum queue capacity.

## Clear Playback Queue

Clears all waiting items from the playback queue.

The currently playing item is not stopped.

REST endpoint:

```http
DELETE /api/audio/queue/clear
```

SCADA-compatible endpoint:

```http
POST /api/audio/queue/clear
```

Both endpoints execute the same operation.

### Successful Response

```http
204 No Content
```

The response body is empty.

## View Available Audio Files

Returns audio files available in the configured audio library.

```http
GET /api/audio/library/files
```

### Successful Response

```http
200 OK
```

Example:

```json
[
  {
    "name": "alarm.mp3"
  },
  {
    "name": "warning.wav"
  }
]
```

## View Playback Status

Returns the current playback state.

```http
GET /api/audio/status
```

### Successful Response

```http
200 OK
```

Example while an audio file is playing:

```json
{
  "state": "Playing",
  "isPlaying": true,
  "currentItemId": "f3b41c59-3f0e-45b5-8127-33a8be3fd8a1",
  "fileName": "alarm.mp3",
  "startedAt": "2026-07-14T15:20:00.0000000+00:00"
}
```

Example while idle:

```json
{
  "state": "Idle",
  "isPlaying": false,
  "currentItemId": null,
  "fileName": null,
  "startedAt": null
}
```

### Fields

| Field | Description |
| --- | --- |
| `state` | Playback state. Current values are `Idle` and `Playing`. |
| `isPlaying` | `true` when the current state is `Playing`. |
| `currentItemId` | Identifier of the current playback queue item, or `null`. |
| `fileName` | Name of the currently playing file, or `null`. |
| `startedAt` | UTC timestamp when current playback started, or `null`. |

## Stop Current Playback

Stops the currently playing audio file.

Waiting items remain in the playback queue. If the queue contains another item, its playback starts normally after the current playback is stopped.

REST endpoint:

```http
DELETE /api/audio/current
```

SCADA-compatible endpoint:

```http
POST /api/audio/current/stop
```

Both endpoints execute the same operation.

### Successful Response

```http
204 No Content
```

The current playback was stopped. The response body is empty.

### Error Response

```http
404 Not Found
```

Nothing is currently playing. The response body is empty.

## Notes

The audio playback API accepts a file name, not an absolute file path.

The same audio file can be added to the playback queue multiple times.

The API does not currently provide authentication or authorization.

Queues are stored in memory and are not persisted across application restarts.
