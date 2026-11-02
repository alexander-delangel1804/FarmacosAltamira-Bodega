using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Application.Interfaces;

public interface IAuthAppService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, string ipOrigen, CancellationToken ct = default);
    Task LogoutAsync(int idUsuario, string? jti, CancellationToken ct = default);
    Task<LoginResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}
