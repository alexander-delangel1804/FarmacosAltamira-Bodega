using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class ParametroApiClient : BaseApiClient
{
    private readonly string _baseUrl;

    public ParametroApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ParametroApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<IReadOnlyList<ParametroDto>> ListarParametrosAsync(CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/parametros?pagina=1&tamano=1000";
        var result = await GetAsync<PaginatedApiResponse<ParametroDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error listando parámetros");
        return result.Dato?.Dato ?? new List<ParametroDto>();
    }

    public async Task<ParametroDto> ObtenerParametroAsync(string clave, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/parametros/{clave}";
        var result = await GetAsync<ParametroDto>(url, ct);
        if (!result.Exito)
            throw new NoEncontradoException(result.Mensaje ?? $"Parámetro {clave} no encontrado");
        return result.Dato!;
    }

    public async Task ActualizarParametroAsync(string clave, string valor, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/parametros/{clave}";
        var payload = new { Valor = valor };
        var result = await PutAsync<object>(url, payload, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error actualizando parámetro");
    }
}
