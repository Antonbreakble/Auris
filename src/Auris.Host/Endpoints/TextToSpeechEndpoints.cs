using Auris.Core.Abstractions;
using Auris.Core.Models;
using Auris.Host.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Auris.Host.Endpoints;

public static class TextToSpeechEndpoints
{
    public static IEndpointRouteBuilder MapTextToSpeechEndpoints(this IEndpointRouteBuilder endpoints) {
        var api = endpoints.MapGroup("/api/tts");
        api.MapPost("/play", Play);
        return endpoints;
    }

    private static IResult Play([FromBody] PlayTextRequest request,
        [FromServices] IQueue<TextToSpeechQueueItem> queue,
        HttpContext httpContext)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return Results.BadRequest("Text is required.");

        var queueItem = new TextToSpeechQueueItem {
            Text = request.Text,
            CreatedBy = ResolveCreatedBy(
                request.RequestedBy,
                httpContext)
        };

        if (!queue.TryEnqueue(queueItem)) {
            return Results.Problem(
                title: "Text-to-speech queue is full.",
                detail: $"Queue capacity is {queue.Capacity}.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        
        return Results.Accepted();
    }

    private static string ResolveCreatedBy(string? requestedBy, HttpContext httpContext) {
        if (!string.IsNullOrWhiteSpace(requestedBy))
            return requestedBy.Trim();
        return httpContext.Connection.RemoteIpAddress?.ToString()
               ?? "api";
    }
}