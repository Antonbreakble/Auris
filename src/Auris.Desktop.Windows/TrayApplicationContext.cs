using System.Diagnostics;
using Auris.Branding;
using Auris.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Auris.Desktop.Windows;

internal class TrayApplicationContext : ApplicationContext {
    private readonly NotifyIcon _notifyIcon;
    private readonly ToolStripMenuItem _openItem;

    private WebApplication? _webApplication;
    private string? _adminUrl;
    private bool _isExiting;
    private readonly Icon _applicationIcon;

    public TrayApplicationContext(string[] args) {
        _openItem = new ToolStripMenuItem("Открыть") {
            Enabled = false
        };
        _openItem.Click += (_, _) => OpenAdminPanel();
        
        var exitItem = new ToolStripMenuItem("Закрыть");
        exitItem.Click += async (_, _) => await ExitAsync();

        var menu = new ContextMenuStrip();
        menu.Items.Add(_openItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exitItem);

        _applicationIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;
        
        _notifyIcon = new NotifyIcon {
            Icon = _applicationIcon,
            Text = $"{ProductBrand.Name} — Открыть",
            ContextMenuStrip = menu,
            Visible = true
        };

        _notifyIcon.DoubleClick += (_, _) => OpenAdminPanel();
        Application.Idle += StartHost;

        async void StartHost(object? sender, EventArgs eventArgs) {
            Application.Idle -= StartHost;
            await StartHostAsync(args);
        }
    }

    private async Task StartHostAsync(string[] args)
    {
        try {
            _webApplication = AurisWebApplication.Build(args);

            _adminUrl = BuildAdminUrl(_webApplication.Configuration);

            await _webApplication.StartAsync();

            _notifyIcon.Text = ProductBrand.Name;
            _openItem.Enabled = true;
        }
        catch (Exception exception) {
            _notifyIcon.Visible = false;

            MessageBox.Show(
                $"{ProductBrand.Name} ошибка.\n\n{exception.Message}",
                $"Ошибка во время запуска {ProductBrand.Name}",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            ExitThread();
        }
    }

    private void OpenAdminPanel() {
        if (_adminUrl is null) 
            return;

        Process.Start(
            new ProcessStartInfo
            {
                FileName = _adminUrl,
                UseShellExecute = true
            });
    }

    private async Task ExitAsync()
    {
        if (_isExiting)
            return;
        
        _isExiting = true;
        _openItem.Enabled = false;
        _notifyIcon.Visible = false;

        if (_webApplication is not null)
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            try {
                await _webApplication.StopAsync(cancellationTokenSource.Token);
            }
            finally {
                await _webApplication.DisposeAsync();
            }
        }

        ExitThread();
    }

    protected override void Dispose(bool disposing) {
        if (disposing) 
            _notifyIcon.Dispose();
        
        base.Dispose(disposing);
    }
    
    private static string BuildAdminUrl(IConfiguration configuration)
    {
        var endpointUrl =
            configuration["Kestrel:Endpoints:Http:Url"]
            ?? throw new InvalidOperationException("Kestrel endpoint 'Http' is not configured.");

        endpointUrl = endpointUrl
            .Replace("://0.0.0.0", "://localhost")
            .Replace("://[::]", "://localhost")
            .Replace("://*", "://localhost")
            .Replace("://+", "://localhost");

        var url = new UriBuilder(endpointUrl) {
            Path = "/Admin"
        };

        return url.Uri.AbsoluteUri;
    }
}