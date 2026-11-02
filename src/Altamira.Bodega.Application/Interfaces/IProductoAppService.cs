using Altamira.Bodega.Domain.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Application.Interfaces;

public interface IProductoAppService
{
    Task<IReadOnlyList<ProductoDto>> ListarProductosAsync(string? busqueda, bool soloActivos, CancellationToken ct = default);
    Task<ProductoDto> ObtenerProductoAsync(int id, CancellationToken ct = default);
    Task<ProductoDto> CrearProductoAsync(ProductoCreateDto dto, int idUsuario, CancellationToken ct = default);
    Task<ProductoDto> ActualizarProductoAsync(int id, ProductoUpdateDto dto, int idUsuario, CancellationToken ct = default);
    Task DesactivarProductoAsync(int id, int idUsuario, CancellationToken ct = default);
}
