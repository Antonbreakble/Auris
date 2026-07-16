using Auris.Core.Abstractions;
using Auris.Core.Options;
using Auris.Core.Queue;
using Auris.Core.Services;
using Auris.Host.Admin.Services;
using Auris.Host.Endpoints;
using Auris.Host.Service;
using Auris.Infrastructure.AudioLibrary;
using Auris.Infrastructure.ExternalPlayer;
using Auris.Infrastructure.TextToSpeech;

namespace Auris.Host;

public static class AurisWebApplication
{
    public static WebApplication Build(string[] args)
    {
        var applicationAssembly = typeof(AurisWebApplication).Assembly;

        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                Args = args,
                
                ApplicationName = applicationAssembly.GetName().Name,
                
                ContentRootPath = AppContext.BaseDirectory,
                WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
            });

        builder.Services.Configure<QueueOptions>(
            builder.Configuration.GetSection("Auris:Queue"));

        builder.Services.AddOptions<AudioLibraryOptions>()
            .Bind(builder.Configuration.GetSection("Auris:AudioLibrary"))
            .Validate(options => Directory.Exists(options.RootPath))
            .ValidateOnStart();

        builder.Services
            .AddOptions<TextToSpeechCliOptions>()
            .Bind(builder.Configuration.GetSection("Auris:TextToSpeech:Cli"));

        builder.Services.Configure<ExternalPlayerOptions>(
            builder.Configuration.GetSection("Auris:Player"));

        builder.Services.AddSingleton(typeof(IQueue<>), typeof(BoundedQueue<>));
        builder.Services.AddSingleton<IPlaybackStateProvider, PlaybackStateProvider>();
        builder.Services.AddSingleton<ExternalProcessRunner>();
        builder.Services.AddSingleton<IAudioPlayer, ExternalProcessAudioPlayer>();
        builder.Services.AddSingleton<IAudioLibrary, FileSystemAudioLibrary>();

        builder.Services.AddSingleton<IPlaybackService, PlaybackService>();
        builder.Services.AddHostedService<PlaybackBackgroundService>();

        builder.Services.AddSingleton<ITextToSpeechSynthesizer,CliTextToSpeechSynthesizer>();
        builder.Services.AddHostedService<TextToSpeechBackgroundService>();
        
        builder.Services.AddRazorPages().AddApplicationPart(applicationAssembly);
        
        builder.Services.AddScoped<AdminDashboardService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.MapRazorPages();

        app.MapAudioLibraryEndpoints();
        app.MapPlaybackQueueEndpoints();
        app.MapPlaybackStatusEndpoints();
        app.MapTextToSpeechEndpoints();

        return app;
    }
}