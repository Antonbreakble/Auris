using Auris.Core.Abstractions;
using Auris.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Auris.Host.Endpoints;

public static class PlaybackStatusEndpoints {
    
    public static IEndpointRouteBuilder MapPlaybackStatusEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api/audio");

        api.MapGet("/status", GetStatus);

        return endpoints;
    }

    private static IResult GetStatus([FromServices] IPlaybackStateProvider stateProvider) {
        var status = stateProvider.GetStatus();

        return Results.Ok(new PlaybackStatusResponse(
            State: status.State.ToString(),
            IsPlaying: status.State == PlaybackState.Playing,
            CurrentItemId: status.CurrentItemId,
            FileName: string.IsNullOrWhiteSpace(status.FilePath)
                ? null
                : Path.GetFileName(status.FilePath),
            StartedAt: status.StartedAt));
    }

    private record PlaybackStatusResponse(
        string State,
        bool IsPlaying,
        Guid? CurrentItemId,
        string? FileName,
        DateTimeOffset? StartedAt);
}