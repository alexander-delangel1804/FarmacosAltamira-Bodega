using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class AuthApiClient : BaseApiClient, IAuthService
{
    private readonly string _baseUrl;

    public AuthApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<AuthApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string ipOrigen, CancellationToken ct = default)
    {
        _logger.LogInformation("Intento de login para usuario {Usuario} desde estación {IdEstacion}", request.NombreUsuario, request.IdEstacion);

        var payload = new
        {
            NombreUsuario = request.NombreUsuario,
            Contrasena = request.Contrasena,
            IdEstacion = request.IdEstacion,
            SecretoEstacion = request.SecretoEstacion
        };

        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/auth/login";

        using var content = new StringContent(
            JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.PostAsync(url, content, ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Credenciales inválidas para usuario {Usuario}", request.NombreUsuario);
            throw new SesionExpiradaException("Credenciales inválidas o estación no autorizada.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            _logger.LogWarning("Usuario {Usuario} sin permisos para acceder al sistema", request.NombreUsuario);
            throw new SinPermisosException("No tiene permisos para acceder al sistema.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Error en login: Status={StatusCode}, Body={Body}", response.StatusCode, body);
            throw new ReglaNegocioException($"Error al iniciar sesión: {response.StatusCode}");
        }

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        if (result?.Exito != true || result.Dato == null)
        {
            _logger.LogError("La API no devolvió un token de acceso válido para usuario {Usuario}", request.NombreUsuario);
            throw new ErrorInternoException("La API no devolvió un token de acceso válido.");
        }

        _logger.LogInformation("Login exitoso para usuario {Usuario}, rol {Rol}, ubicación {IdUbicacion}",
            result.Dato.NombreUsuario, result.Dato.Rol, result.Dato.IdUbicacion);

        return result.Dato;
    }

    public Task LogoutAsync(int idUsuario, string? jti, CancellationToken ct = default)
    {
        _logger.LogInformation("Cierre de sesión para usuario {IdUsuario}", idUsuario);
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/auth/logout";
        return _httpClient.PostAsync(url, null, ct);
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        _logger.LogInformation("Renovando token de acceso");
        var payload = new { RefreshToken = refreshToken };
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/auth/refresh";

        using var content = new StringContent(
            JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.PostAsync(url, content, ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Refresh token inválido o expirado");
            throw new SesionExpiradaException("Refresh token inválido o expirado. Inicie sesión de nuevo.");
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error renovando token: Status={StatusCode}", response.StatusCode);
            throw new ErrorInternoException($"Error renovando el token: {response.StatusCode}");
        }

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        if (result?.Exito != true || result.Dato == null)
        {
            throw new ErrorInternoException("La API no devolvió un token válido al renovar.");
        }

        return result.Dato;
    }
}
