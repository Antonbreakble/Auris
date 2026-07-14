using Auris.Core.Abstractions;
using Auris.Core.Models;
using Auris.Host.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Auris.Host.Endpoints;

public static class PlaybackQueueEndpoints {
    public static IEndpointRouteBuilder MapPlaybackQueueEndpoints(this IEndpointRouteBuilder endpoints) {
        var api = endpoints.MapGroup("/api/audio");

        api.MapPost("/play", Play);
        api.MapGet("/queue", GetQueue);
        api.MapDelete("/queue/clear", ClearQueue);

        return endpoints;
    }

    private static IResult Play(
        [FromBody] PlayAudioRequest request,
        [FromServices] IAudioLibrary audioLibrary,
        [FromServices] IQueue<PlaybackQueueItem> playbackQueue,
        HttpContext httpContext) {

        if (string.IsNullOrWhiteSpace(request.FileName))
            return Results.BadRequest("FileName is required.");
        
        var audioFile = audioLibrary.FindAudioFile(request.FileName);

        if (audioFile is null) 
            return Results.NotFound("Audio file was not found or file name is ambiguous.");

        var queueItem = new PlaybackQueueItem {
            FilePath = audioFile.FullPath,
            CreatedBy = ResolveCreatedBy(request.RequestedBy, httpContext)
        };

        if (!playbackQueue.TryEnqueue(queueItem)) {
            return Results.Problem(
                title: "Playback queue is full.",
                detail: $"Queue capacity is {playbackQueue.Capacity}.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        return Results.Accepted();
    }
    
    private static string ResolveCreatedBy(string? requestedBy, HttpContext httpContext) {

        if (!string.IsNullOrWhiteSpace(requestedBy)) {
            return requestedBy.Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "api";
    }

    private static IResult GetQueue([FromServices] IQueue<PlaybackQueueItem> playbackQueue) {

        var items = playbackQueue
            .Snapshot()
            .Select(ToQueueItemResponse)
            .ToArray();

        return Results.Ok(new PlaybackQueueResponse(
            Count: items.Length,
            Capacity: playbackQueue.Capacity,
            Items: items));
    }

    private static IResult ClearQueue([FromServices] IQueue<PlaybackQueueItem> playbackQueue) {
        playbackQueue.Clear();
        return Results.NoContent();
    }

    private static PlaybackQueueItemResponse ToQueueItemResponse(PlaybackQueueItem item) {
        return new PlaybackQueueItemResponse(
            Id: item.Id,
            FileName: Path.GetFileName(item.FilePath),
            CreatedAt: item.CreatedAt,
            CreatedBy: item.CreatedBy);
    }
    
    private record PlaybackQueueResponse(
        int Count,
        int Capacity,
        IReadOnlyList<PlaybackQueueItemResponse> Items);

    private record PlaybackQueueItemResponse(
        Guid Id,
        string FileName,
        DateTimeOffset CreatedAt,
        string CreatedBy);
    
}