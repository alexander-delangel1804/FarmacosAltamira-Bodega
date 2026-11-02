using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class AlertaApiClient : BaseApiClient, IAlertaService
{
    private readonly string _baseUrl;

    public AlertaApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<AlertaApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<IReadOnlyList<AlertaDto>> ListarAlertasAsync(int? idUbicacion, bool soloNoAtendidas, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["idUbicacion"] = idUbicacion?.ToString(),
            ["soloNoAtendidas"] = soloNoAtendidas.ToString(),
            ["pagina"] = "1",
            ["tamano"] = "1000"
        };
        var url = BuildUrl(_baseUrl, "api/v1/alertas", queryParams);
        var result = await GetAsync<PaginatedApiResponse<AlertaDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error listando alertas");
        return result.Dato?.Dato ?? new List<AlertaDto>();
    }

    public async Task MarcarAlertaAtendidaAsync(int idAlerta, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/alertas/{idAlerta}/atender";
        var result = await PostAsync<object>(url, null, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error atendiendo alerta");
    }
}
