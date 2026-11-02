using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class RolApiClient : BaseApiClient
{
    private readonly string _baseUrl;

    public RolApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<RolApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<IReadOnlyList<RolDto>> ListarRolesAsync(CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/roles?pagina=1&tamano=1000";
        var result = await GetAsync<PaginatedApiResponse<RolDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error listando roles");
        return result.Dato?.Dato ?? new List<RolDto>();
    }

    public async Task<IReadOnlyList<PermisoDto>> ListarPermisosAsync(CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/roles/permisos?pagina=1&tamano=1000";
        var result = await GetAsync<PaginatedApiResponse<PermisoDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error listando permisos");
        return result.Dato?.Dato ?? new List<PermisoDto>();
    }
}
