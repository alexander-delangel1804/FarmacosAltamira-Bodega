using Altamira.Bodega.Domain.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Application.Interfaces;

public interface IUsuarioAppService
{
    Task<IReadOnlyList<UsuarioDto>> ListarUsuariosAsync(CancellationToken ct = default);
    Task<UsuarioDto> ObtenerUsuarioAsync(int id, CancellationToken ct = default);
    Task<UsuarioDto> CrearUsuarioAsync(UsuarioCreateDto dto, int idUsuario, CancellationToken ct = default);
    Task<UsuarioDto> ActualizarUsuarioAsync(int id, UsuarioUpdateDto dto, int idUsuario, CancellationToken ct = default);
    Task BloquearUsuarioAsync(int id, int idUsuario, CancellationToken ct = default);
    Task DesbloquearUsuarioAsync(int id, int idUsuario, CancellationToken ct = default);
    Task DesactivarUsuarioAsync(int id, int idUsuario, CancellationToken ct = default);
}
