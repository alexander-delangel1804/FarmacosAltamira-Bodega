using Altamira.Bodega.Domain.Interfaces;
using Altamira.Bodega.Infrastructure.DependencyInjection;
using Altamira.Bodega.Presentation.Services;
using Altamira.Bodega.Presentation.ViewModels;
using Altamira.Bodega.Presentation.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Windows;

namespace Altamira.Bodega.App;

public partial class App : System.Windows.Application
{
    public App()
    {
        var host = Program.CreateHostBuilder(Array.Empty<string>()).Build();
        Services = host.Services;
    }

    public new static App Current => (App)System.Windows.Application.Current!;
    public IServiceProvider Services { get; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = Services.GetRequiredService<ShellView>();
        mainWindow.Show();
        base.OnStartup(e);
    }
}
