using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class UsuarioApiClient : BaseApiClient
{
    private readonly string _baseUrl;

    public UsuarioApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<UsuarioApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<IReadOnlyList<UsuarioDto>> ListarUsuariosAsync(CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/usuarios?pagina=1&tamano=1000";
        var result = await GetAsync<PaginatedApiResponse<UsuarioDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error listando usuarios");
        return result.Dato?.Dato ?? new List<UsuarioDto>();
    }

    public async Task<UsuarioDto> ObtenerUsuarioAsync(int id, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/usuarios/{id}";
        var result = await GetAsync<UsuarioDto>(url, ct);
        if (!result.Exito)
            throw new NoEncontradoException(result.Mensaje ?? $"Usuario {id} no encontrado");
        return result.Dato!;
    }

    public async Task<UsuarioDto> CrearUsuarioAsync(UsuarioCreateDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/usuarios";
        var result = await PostAsync<UsuarioDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error creando usuario");
        return result.Dato!;
    }

    public async Task<UsuarioDto> ActualizarUsuarioAsync(int id, UsuarioUpdateDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/usuarios/{id}";
        var result = await PutAsync<UsuarioDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error actualizando usuario");
        return result.Dato!;
    }

    public async Task BloquearUsuarioAsync(int id, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/usuarios/{id}/bloquear";
        var result = await PostAsync<object>(url, null, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error bloqueando usuario");
    }

    public async Task DesbloquearUsuarioAsync(int id, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/usuarios/{id}/desbloquear";
        var result = await PostAsync<object>(url, null, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error desbloqueando usuario");
    }

    public async Task DesactivarUsuarioAsync(int id, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/usuarios/{id}/desactivar";
        var result = await PostAsync<object>(url, null, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error desactivando usuario");
    }
}
