using Altamira.Bodega.Domain.Interfaces;
using Altamira.Bodega.Infrastructure.DependencyInjection;
using Altamira.Bodega.Presentation.DependencyInjection;
using Altamira.Bodega.Presentation.Services;
using Altamira.Bodega.Presentation.ViewModels;
using Altamira.Bodega.Presentation.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;

namespace Altamira.Bodega.App;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("logs\\bodega-.txt", rollingInterval: Serilog.RollingInterval.Day, retainedFileCountLimit: 7);
            })
            .ConfigureServices((context, services) =>
            {
                Altamira.Bodega.Infrastructure.DependencyInjection.InfrastructureDependencyInjection.AddInfrastructure(services, context.Configuration);
                Altamira.Bodega.Presentation.DependencyInjection.PresentationDependencyInjection.AddPresentation(services, context.Configuration);
            });
}
