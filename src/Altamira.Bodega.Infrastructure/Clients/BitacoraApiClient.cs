using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class BitacoraApiClient : BaseApiClient
{
    private readonly string _baseUrl;

    public BitacoraApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<BitacoraApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<IReadOnlyList<BitacoraDto>> ConsultarBitacoraAsync(
        int? idUsuario,
        int? idUbicacion,
        string? modulo,
        string? accion,
        DateTimeOffset? fechaDesde,
        DateTimeOffset? fechaHasta,
        CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["idUsuario"] = idUsuario?.ToString(),
            ["idUbicacion"] = idUbicacion?.ToString(),
            ["modulo"] = modulo,
            ["accion"] = accion,
            ["fechaDesde"] = fechaDesde?.ToString("o"),
            ["fechaHasta"] = fechaHasta?.ToString("o"),
            ["pagina"] = "1",
            ["tamano"] = "1000"
        };
        var url = BuildUrl(_baseUrl, "api/v1/bitacora", queryParams);
        var result = await GetAsync<PaginatedApiResponse<BitacoraDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error consultando bitácora");
        return result.Dato?.Dato ?? new List<BitacoraDto>();
    }
}
