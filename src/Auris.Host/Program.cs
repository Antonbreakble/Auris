using Auris.Core.Abstractions;
using Auris.Core.Options;
using Auris.Core.Queue;
using Auris.Core.Services;
using Auris.Host.Service;
using Auris.Infrastructure.AudioLibrary;
using Auris.Infrastructure.ExternalPlayer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<QueueOptions>(
    builder.Configuration.GetSection("Auris:Queue"));

builder.Services.Configure<AudioLibraryOptions>(
    builder.Configuration.GetSection("Auris:AudioLibrary"));

builder.Services.Configure<ExternalPlayerOptions>(
    builder.Configuration.GetSection("Auris:Player"));

builder.Services.AddSingleton<IPlaybackQueue, PlaybackQueue>();
builder.Services.AddSingleton<IPlaybackStateProvider, PlaybackStateProvider>();
builder.Services.AddSingleton<IAudioPlayer, ExternalProcessAudioPlayer>();

builder.Services.AddSingleton<IPlaybackService, PlaybackService>();

builder.Services.AddHostedService<PlaybackBackgroundService>();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
