using Auris.Core.Abstractions;
using Auris.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Auris.Host.Endpoints;

public static class AudioLibraryEndpoints {
    public static IEndpointRouteBuilder MapAudioLibraryEndpoints(this IEndpointRouteBuilder endpoints) {
        var api = endpoints.MapGroup("/api/audio/library");
        api.MapGet("/files", GetFiles);
        return endpoints;
    }

    private static IResult GetFiles([FromServices] IAudioLibrary audioLibrary) {
        var files = audioLibrary
            .GetAudioFiles()
            .Select(audioFile => new AudioFileResponse(audioFile.Name))
            .ToArray();
        
        return Results.Ok(files);
    }

    private record AudioFileResponse(string Name);
}