using Altamira.Bodega.Domain.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Application.Interfaces;

public interface IRolAppService
{
    Task<IReadOnlyList<RolDto>> ListarRolesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PermisoDto>> ListarPermisosAsync(CancellationToken ct = default);
}
