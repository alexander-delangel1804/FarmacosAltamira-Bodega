using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class TrasladoApiClient : BaseApiClient, ITrasladoService
{
    private readonly string _baseUrl;

    public TrasladoApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<TrasladoApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<TrasladoDto> SolicitarTrasladoAsync(TrasladoCreateDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/traslados";
        var result = await PostAsync<TrasladoDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error solicitando traslado");
        return result.Dato!;
    }

    public async Task<IReadOnlyList<TrasladoDto>> ListarTrasladosAsync(string? estado, int? idOrigen, int? idDestino, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["estado"] = estado,
            ["idOrigen"] = idOrigen?.ToString(),
            ["idDestino"] = idDestino?.ToString(),
            ["pagina"] = "1",
            ["tamano"] = "1000"
        };
        var url = BuildUrl(_baseUrl, "api/v1/traslados", queryParams);
        var result = await GetAsync<PaginatedApiResponse<TrasladoDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error listando traslados");
        return result.Dato?.Dato ?? new List<TrasladoDto>();
    }

    public async Task<TrasladoDto> DespacharTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/traslados/{idTraslado}/despachar";
        var result = await PostAsync<TrasladoDto>(url, null, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error despachando traslado");
        return result.Dato!;
    }

    public async Task<TrasladoDto> RecibirTrasladoAsync(int idTraslado, TrasladoRecibirDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/traslados/{idTraslado}/recibir";
        var result = await PostAsync<TrasladoDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error recibiendo traslado");
        return result.Dato!;
    }

    public async Task<TrasladoDto> CancelarTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/traslados/{idTraslado}/cancelar";
        var result = await PostAsync<TrasladoDto>(url, null, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error cancelando traslado");
        return result.Dato!;
    }
}
