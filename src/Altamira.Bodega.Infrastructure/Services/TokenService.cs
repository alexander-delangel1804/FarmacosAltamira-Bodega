using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Altamira.Bodega.Infrastructure.Clients;

/// <summary>
/// Servicio de tokens JWT: almacena y gestiona la sesión de autenticación en memoria.
/// </summary>
public sealed class TokenService : ITokenService
{
    private LoginResponse? _session;

    public string? AccessToken => _session?.AccessToken;
    public string? RefreshToken => _session?.RefreshToken;
    public DateTimeOffset? Expiracion => _session?.Expiracion;
    public int? IdUsuario => _session?.IdUsuario;
    public string? Rol => _session?.Rol;
    public int? IdUbicacion => _session?.IdUbicacion;
    public string? NombreUsuario => _session?.NombreUsuario;
    public string? NombreCompleto => _session?.NombreCompleto;
    public string? UbicacionNombre => _session?.UbicacionNombre;
    public int? IdCaja => _session?.IdCaja;
    public string? CajaNumero => _session?.CajaNumero;

    public bool EstaAutenticado => !string.IsNullOrEmpty(AccessToken) && !TokenExpirado;
    public bool TokenExpirado => Expiracion.HasValue && DateTimeOffset.UtcNow >= Expiracion.Value;

    public void GuardarSesion(LoginResponse response)
    {
        _session = response;
    }

    public void LimpiarSesion()
    {
        _session = null;
    }
}
