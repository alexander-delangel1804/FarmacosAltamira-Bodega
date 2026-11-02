using Altamira.Bodega.Domain.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Application.Interfaces;

public interface ITrasladoAppService
{
    Task<TrasladoDto> SolicitarTrasladoAsync(TrasladoCreateDto dto, int idUsuario, CancellationToken ct = default);
    Task<IReadOnlyList<TrasladoDto>> ListarTrasladosAsync(string? estado, int? idOrigen, int? idDestino, CancellationToken ct = default);
    Task<TrasladoDto> DespacharTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct = default);
    Task<TrasladoDto> RecibirTrasladoAsync(int idTraslado, TrasladoRecibirDto dto, int idUsuario, CancellationToken ct = default);
    Task<TrasladoDto> CancelarTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct = default);
}
