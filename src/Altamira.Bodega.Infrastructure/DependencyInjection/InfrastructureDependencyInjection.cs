using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Interfaces;
using Altamira.Bodega.Infrastructure.Clients;
using Altamira.Bodega.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Altamira.Bodega.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var apiBaseUrl = configuration["Api:BaseUrl"] ?? "http://localhost:5000";
        var timeoutSeconds = int.Parse(configuration["Api:TimeoutSegundos"] ?? "30");

        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<Altamira.Bodega.Infrastructure.Services.ISesionService, Altamira.Bodega.Infrastructure.Services.SesionService>();

services.AddHttpClient("AltamiraApi", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });

        services.AddScoped<AuthApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<AuthApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new AuthApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<InventarioApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<InventarioApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new InventarioApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<ProductoApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<ProductoApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new ProductoApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<TrasladoApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<TrasladoApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new TrasladoApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<RecepcionApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<RecepcionApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new RecepcionApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<AjusteApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<AjusteApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new AjusteApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<AlertaApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<AlertaApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new AlertaApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<UsuarioApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<UsuarioApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new UsuarioApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<RolApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<RolApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new RolApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<ParametroApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<ParametroApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new ParametroApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<BitacoraApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var tokenService = sp.GetRequiredService<ITokenService>();
            var logger = sp.GetRequiredService<ILogger<BitacoraApiClient>>();
            var client = factory.CreateClient("AltamiraApi");
            return new BitacoraApiClient(client, tokenService, logger, apiBaseUrl);
        });

        services.AddScoped<IProductoAppService, ProductoAppService>();
        services.AddScoped<IInventarioAppService, InventarioAppService>();
        services.AddScoped<ITrasladoAppService, TrasladoAppService>();
        services.AddScoped<IAlertaAppService, AlertaAppService>();
        services.AddScoped<IUsuarioAppService, UsuarioAppService>();
        services.AddScoped<IRolAppService, RolAppService>();
        services.AddScoped<IParametroAppService, ParametroAppService>();
        services.AddScoped<IBitacoraAppService, BitacoraAppService>();
        services.AddScoped<IAuthAppService, AuthAppService>();
        services.AddScoped<ISesionAppService, SesionAppService>();

        return services;
    }
}
