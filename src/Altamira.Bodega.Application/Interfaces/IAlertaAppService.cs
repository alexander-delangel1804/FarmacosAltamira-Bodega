using Altamira.Bodega.Domain.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Application.Interfaces;

public interface IAlertaAppService
{
    Task<IReadOnlyList<AlertaDto>> ListarAlertasAsync(int? idUbicacion, bool soloNoAtendidas, CancellationToken ct = default);
    Task MarcarAlertaAtendidaAsync(int idAlerta, int idUsuario, CancellationToken ct = default);
}
