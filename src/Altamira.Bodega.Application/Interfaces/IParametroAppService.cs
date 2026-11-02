using Altamira.Bodega.Domain.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Application.Interfaces;

public interface IParametroAppService
{
    Task<IReadOnlyList<ParametroDto>> ListarParametrosAsync(CancellationToken ct = default);
    Task<ParametroDto> ObtenerParametroAsync(string clave, CancellationToken ct = default);
    Task ActualizarParametroAsync(string clave, string valor, int idUsuario, CancellationToken ct = default);
}
