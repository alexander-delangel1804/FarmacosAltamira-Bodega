using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

public sealed class ProductoApiClient : BaseApiClient, IProductoService
{
    private readonly string _baseUrl;

    public ProductoApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ProductoApiClient> logger, string baseUrl)
        : base(httpClient, tokenService, logger)
    {
        _baseUrl = baseUrl;
    }

    public async Task<IReadOnlyList<ProductoDto>> ListarProductosAsync(string? busqueda, bool soloActivos, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["busqueda"] = busqueda,
            ["soloActivos"] = soloActivos.ToString(),
            ["pagina"] = "1",
            ["tamano"] = "1000"
        };
        var url = BuildUrl(_baseUrl, "api/v1/productos", queryParams);
        var result = await GetAsync<PaginatedApiResponse<ProductoDto>>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error listando productos");
        return result.Dato?.Dato ?? new List<ProductoDto>();
    }

    public async Task<ProductoDto> ObtenerProductoAsync(int id, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/productos/{id}";
        var result = await GetAsync<ProductoDto>(url, ct);
        if (!result.Exito)
            throw new NoEncontradoException(result.Mensaje ?? $"Producto {id} no encontrado");
        return result.Dato!;
    }

    public async Task<ProductoDto> CrearProductoAsync(ProductoCreateDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/productos";
        var result = await PostAsync<ProductoDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error creando producto");
        return result.Dato!;
    }

    public async Task<ProductoDto> ActualizarProductoAsync(int id, ProductoUpdateDto dto, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/productos/{id}";
        var result = await PutAsync<ProductoDto>(url, dto, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error actualizando producto");
        return result.Dato!;
    }

    public async Task DesactivarProductoAsync(int id, int idUsuario, CancellationToken ct = default)
    {
        var url = $"{_baseUrl.TrimEnd('/')}/api/v1/productos/{id}";
        var result = await DeleteAsync<ProductoDto>(url, ct);
        if (!result.Exito)
            throw new ReglaNegocioException(result.Mensaje ?? "Error desactivando producto");
    }
}
