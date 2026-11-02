using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public abstract class BaseApiClient
{
    protected readonly HttpClient _httpClient;
    protected readonly ITokenService _tokenService;
    protected readonly ILogger _logger;
    protected readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    protected BaseApiClient(HttpClient httpClient, ITokenService tokenService, ILogger logger)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _logger = logger;
    }

    protected async Task EnsureAuthenticatedAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_tokenService.AccessToken) || _tokenService.TokenExpirado)
        {
            _logger.LogWarning("Intento de llamada a API sin token válido o expirado");
            throw new SesionExpiradaException("La sesión ha expirado. Inicie sesión de nuevo.");
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenService.AccessToken);

        await Task.CompletedTask;
    }

    protected async Task<ApiResponse<T>> GetAsync<T>(string url, CancellationToken ct = default)
    {
        await EnsureAuthenticatedAsync(ct);
        _logger.LogDebug("GET {Url}", url);
        using var response = await _httpClient.GetAsync(url, ct);
        return await ProcessResponseAsync<T>(response, url, ct);
    }

    protected async Task<ApiResponse<T>> PostAsync<T>(string url, object? payload, CancellationToken ct = default)
    {
        await EnsureAuthenticatedAsync(ct);
        _logger.LogDebug("POST {Url}", url);
        using var content = payload != null
            ? new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8, "application/json")
            : null;
        using var response = await _httpClient.PostAsync(url, content, ct);
        return await ProcessResponseAsync<T>(response, url, ct);
    }

    protected async Task<ApiResponse<T>> PutAsync<T>(string url, object? payload, CancellationToken ct = default)
    {
        await EnsureAuthenticatedAsync(ct);
        _logger.LogDebug("PUT {Url}", url);
        using var content = payload != null
            ? new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8, "application/json")
            : null;
        using var response = await _httpClient.PutAsync(url, content, ct);
        return await ProcessResponseAsync<T>(response, url, ct);
    }

    protected async Task<ApiResponse<T>> DeleteAsync<T>(string url, CancellationToken ct = default)
    {
        await EnsureAuthenticatedAsync(ct);
        _logger.LogDebug("DELETE {Url}", url);
        using var response = await _httpClient.DeleteAsync(url, ct);
        return await ProcessResponseAsync<T>(response, url, ct);
    }

    protected async Task<ApiResponse<T>> ProcessResponseAsync<T>(HttpResponseMessage response, string endpoint, CancellationToken ct)
    {
        var statusCode = (int)response.StatusCode;
        var body = await response.Content.ReadAsStringAsync(ct);
        _logger.LogDebug("Respuesta {StatusCode} de {Endpoint}", statusCode, endpoint);

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var result = JsonSerializer.Deserialize<ApiResponse<T>>(body, _jsonOptions);
                return result ?? new ApiResponse<T>(false, default, "Respuesta vacía de la API", "RESPUESTA_VACIA");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializando respuesta de {Endpoint}", endpoint);
                throw new ErrorInternoException($"Error al procesar la respuesta de la API: {ex.Message}");
            }
        }

        throw MapErrorResponse(statusCode, body, endpoint);
    }

    private Exception MapErrorResponse(int statusCode, string body, string endpoint)
    {
        string? codigo = null;
        string? mensaje = null;
        try
        {
            var errorDoc = JsonSerializer.Deserialize<ApiErrorResponse>(body, _jsonOptions);
            codigo = errorDoc?.Codigo;
            mensaje = errorDoc?.Mensaje;
        }
        catch { }

        _logger.LogWarning("Error {StatusCode} en {Endpoint}: Codigo={Codigo}, Mensaje={Mensaje}",
            statusCode, endpoint, codigo, mensaje);

        return statusCode switch
        {
            400 => new DatosInvalidosException(mensaje ?? $"Datos inválidos en {endpoint}."),
            401 => new SesionExpiradaException(mensaje ?? "Sesión expirada o credenciales inválidas"),
            403 => new SinPermisosException(mensaje ?? "No tiene permisos para realizar esta operación"),
            404 => new NoEncontradoException(mensaje ?? $"Recurso no encontrado en {endpoint}."),
            409 => new ConflictoConcurrenciaException(mensaje ?? $"Conflicto en {endpoint}."),
            422 => new ReglaNegocioException(mensaje ?? $"Regla de negocio violada en {endpoint}."),
            500 or >= 500 => new ErrorInternoException(mensaje ?? $"Error del servidor en {endpoint}."),
            _ => new ErrorInternoException($"Error inesperado ({statusCode}) en {endpoint}.")
        };
    }

    protected string BuildUrl(string baseUrl, string path, Dictionary<string, string?>? queryParams = null)
    {
        var url = $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
        if (queryParams != null && queryParams.Any())
        {
            var query = string.Join("&", queryParams
                .Where(kvp => kvp.Value != null)
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value!)}"));
            if (!string.IsNullOrEmpty(query))
                url += $"?{query}";
        }
        return url;
    }
}

public sealed record ApiResponse<T>(bool Exito, T? Dato, string? Mensaje = null, string? Codigo = null)
{
    public static ApiResponse<T> Ok(T? dato, string? mensaje = null) => new(true, dato, mensaje);
    public static ApiResponse<T> Fail(string mensaje, string? codigo = null) => new(false, default, mensaje, codigo);
}

public sealed record PaginatedApiResponse<T>(bool Exito, IReadOnlyList<T>? Dato, long Total, int Pagina, int TamanoPagina, string? Mensaje = null, string? Codigo = null)
{
    public static PaginatedApiResponse<T> Ok(IReadOnlyList<T> dato, long total, int pagina, int tamanoPagina, string? mensaje = null)
        => new(true, dato, total, pagina, tamanoPagina, mensaje);
}

public sealed record ApiErrorResponse(string Codigo, string Mensaje, object? Detalles = null);
