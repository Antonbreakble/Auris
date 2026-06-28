# Auris REST API

This document describes the current Auris REST API.

The API is intended for integration with external systems, operator workstations, SCADA systems, scripts, and engineering tools.

## Current State

The basic REST API is implemented and can be tested locally.

Available operations:

* play an audio file;
* view the playback queue;
* clear the playback queue;
* view available audio files.

## Base URL

The actual address and port depend on the application launch settings.

## Content Type

Requests with a body use JSON:

```http
Content-Type: application/json
```

## Play Audio File

Adds an audio file to the playback queue.

If nothing is currently playing, playback will start when the background playback service processes the queue.

If another file is already playing, the requested file will wait in the queue.

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

| Field         | Required | Description                                                                                             |
| ------------- | -------: | ------------------------------------------------------------------------------------------------------- |
| `fileName`    |      yes | Audio file name from the configured audio library.                                                      |
| `requestedBy` |       no | Source of the playback request. For example, workstation name, system name, SCADA, or operator station. |

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

The audio file was not found.

```http
503 Service Unavailable
```

The playback queue is full.

## View Playback Queue

Returns the current playback queue.

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

## Clear Playback Queue

Clears all waiting items from the playback queue.

```http
DELETE /api/audio/queue/clear
```

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
    "name": "alarm.mp3",
    "extension": ".mp3"
  },
  {
    "name": "warning.wav",
    "extension": ".wav"
  }
]
```

## Notes

The API accepts an audio file name, not an absolute file path.

The same audio file can be added to the playback queue multiple times.

The current API does not provide authentication or authorization.

The current API does not provide a playback status endpoint yet.
