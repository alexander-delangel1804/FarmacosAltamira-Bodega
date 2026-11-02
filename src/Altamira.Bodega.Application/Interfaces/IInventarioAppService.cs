using Altamira.Bodega.Domain.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Application.Interfaces;

public interface IInventarioAppService
{
    Task<IReadOnlyList<ExistenciaDto>> ConsultarExistenciasAsync(int? idProducto, int? idUbicacion, CancellationToken ct = default);
    Task<IReadOnlyList<LoteDto>> ListarLotesAsync(int? idProducto, CancellationToken ct = default);
    Task<IReadOnlyList<MovimientoDto>> ConsultarMovimientosAsync(int? idLote, int? idUbicacion, System.DateTimeOffset? desde, System.DateTimeOffset? hasta, CancellationToken ct = default);
    Task<RecepcionDto> RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct = default);
    Task<AjusteDto> RegistrarAjusteAsync(AjusteCreateDto dto, int idUsuario, CancellationToken ct = default);
    Task<IReadOnlyList<MovimientoDto>> ListarRecepcionesAsync(System.DateTimeOffset? desde, System.DateTimeOffset? hasta, CancellationToken ct = default);
}
