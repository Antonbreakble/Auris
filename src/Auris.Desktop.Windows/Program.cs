namespace Auris.Desktop.Windows;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        using var mutex = new Mutex(
            initiallyOwned: true,
            name: @"Local\Auris.Desktop",
            createdNew: out var isFirstInstance);

        if (!isFirstInstance)
        {
            MessageBox.Show(
                "Auris уже запущен.",
                "Auris",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        ApplicationConfiguration.Initialize();

        using var context = new TrayApplicationContext(args);
        Application.Run(context);
    } 
}