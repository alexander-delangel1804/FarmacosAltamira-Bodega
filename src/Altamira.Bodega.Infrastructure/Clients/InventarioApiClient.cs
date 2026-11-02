using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class InventarioApiClient : BaseApiClient, IInventarioService
{
    private readonly string _baseUrl;

    public InventarioApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<InventarioApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<IReadOnlyList<ExistenciaDto>> ConsultarExistenciasAsync(int? idProducto, int? idUbicacion, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["idProducto"] = idProducto?.ToString(),
            ["idUbicacion"] = idUbicacion?.ToString(),
            ["pagina"] = "1",
            ["tamano"] = "1000"
        };
        var url = BuildUrl(_baseUrl, "api/v1/inventario", queryParams);
        var result = await GetAsync<PaginatedApiResponse<ExistenciaDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error consultando existencias");
        return result.Dato?.Dato ?? new List<ExistenciaDto>();
    }

    public async Task<IReadOnlyList<LoteDto>> ListarLotesAsync(int? idProducto, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["idProducto"] = idProducto?.ToString(),
            ["pagina"] = "1",
            ["tamano"] = "1000"
        };
        var url = BuildUrl(_baseUrl, "api/v1/lotes", queryParams);
        var result = await GetAsync<PaginatedApiResponse<LoteDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error consultando lotes");
        return result.Dato?.Dato ?? new List<LoteDto>();
    }

    public async Task<IReadOnlyList<MovimientoDto>> ConsultarMovimientosAsync(int? idLote, int? idUbicacion, DateTimeOffset? desde, DateTimeOffset? hasta, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["idLote"] = idLote?.ToString(),
            ["idUbicacion"] = idUbicacion?.ToString(),
            ["desde"] = desde?.ToString("o"),
            ["hasta"] = hasta?.ToString("o"),
            ["pagina"] = "1",
            ["tamano"] = "1000"
        };
        var url = BuildUrl(_baseUrl, "api/v1/movimientos", queryParams);
        var result = await GetAsync<PaginatedApiResponse<MovimientoDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error consultando movimientos");
        return result.Dato?.Dato ?? new List<MovimientoDto>();
    }

    public async Task<RecepcionDto> RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/recepciones";
        var result = await PostAsync<RecepcionDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error registrando recepción");
        return result.Dato!;
    }

    public async Task<AjusteDto> RegistrarAjusteAsync(AjusteCreateDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/ajustes";
        var result = await PostAsync<AjusteDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error registrando ajuste");
        return result.Dato!;
    }

    public async Task<IReadOnlyList<MovimientoDto>> ListarRecepcionesAsync(DateTimeOffset? desde, DateTimeOffset? hasta, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["desde"] = desde?.ToString("o"),
            ["hasta"] = hasta?.ToString("o"),
            ["pagina"] = "1",
            ["tamano"] = "1000"
        };
        var url = BuildUrl(_baseUrl, "api/v1/recepciones", queryParams);
        var result = await GetAsync<PaginatedApiResponse<MovimientoDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error consultando recepciones");
        return result.Dato?.Dato ?? new List<MovimientoDto>();
    }
}
