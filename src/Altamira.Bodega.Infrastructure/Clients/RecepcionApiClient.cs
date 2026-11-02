using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class RecepcionApiClient : BaseApiClient, IRecepcionService
{
    private readonly string _baseUrl;

    public RecepcionApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<RecepcionApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<IReadOnlyList<RecepcionDto>> ListarRecepcionesAsync(CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/recepciones?pagina=1&tamano=1000";
        var result = await GetAsync<PaginatedApiResponse<RecepcionDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error listando recepciones");
        return result.Dato?.Dato ?? new List<RecepcionDto>();
    }

    public async Task<RecepcionDto> ObtenerRecepcionAsync(int id, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/recepciones/{id}";
        var result = await GetAsync<RecepcionDto>(url, ct);
        if (!result.Exito)
            throw new NoEncontradoException(result.Mensaje ?? $"Recepción {id} no encontrada");
        return result.Dato!;
    }

    public async Task<RecepcionDto> RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/recepciones";
        var result = await PostAsync<RecepcionDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error registrando recepción");
        return result.Dato!;
    }
}
