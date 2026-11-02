using Altamira.Bodega.Domain.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Application.Interfaces;

public interface IBitacoraAppService
{
    Task<IReadOnlyList<BitacoraDto>> ConsultarBitacoraAsync(
        int? idUsuario,
        int? idUbicacion,
        string? modulo,
        string? accion,
        System.DateTimeOffset? fechaDesde,
        System.DateTimeOffset? fechaHasta,
        CancellationToken ct = default);
}
