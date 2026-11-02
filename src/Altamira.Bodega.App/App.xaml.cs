using Altamira.Bodega.Presentation.Services;
using Altamira.Bodega.Presentation.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;

namespace Altamira.Bodega.App;

public partial class App : System.Windows.Application
{
    public new static App Current => (App)System.Windows.Application.Current!;
    public IServiceProvider Services { get; internal set; } = null!;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);

            if (Services == null)
            {
                Log.Error("Services no fue inicializado correctamente");
                MessageBox.Show("Error: Los servicios de la aplicación no se inicializaron correctamente.", "Error de inicio", MessageBoxButton.OK, MessageBoxImage.Stop);
                Shutdown();
                return;
            }

            var shellView = Services.GetRequiredService<ShellView>();
            shellView.Show();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error al iniciar la aplicación");
            MessageBox.Show($"No se pudo iniciar la aplicación:\n{ex.Message}\n\n{ex.StackTrace}", "Error crítico", MessageBoxButton.OK, MessageBoxImage.Stop);
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
