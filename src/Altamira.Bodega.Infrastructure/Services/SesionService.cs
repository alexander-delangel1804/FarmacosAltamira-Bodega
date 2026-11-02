using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Altamira.Bodega.Infrastructure.Services;

/// <summary>
/// Gestión de sesión de usuario: almacena datos de autenticación, verifica expiración
/// y proporciona información del usuario autenticado para la capa de presentación.
/// </summary>
public sealed class SesionService : ISesionService
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<SesionService> _logger;

    public SesionService(ITokenService tokenService, ILogger<SesionService> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public bool EstaAutenticado => _tokenService.EstaAutenticado;
    public bool TokenExpirado => _tokenService.TokenExpirado;
    public int? IdUsuario => _tokenService.IdUsuario;
    public string? Rol => _tokenService.Rol;
    public int? IdUbicacion => _tokenService.IdUbicacion;
    public string? NombreUsuario => _tokenService.NombreUsuario;
    public string? NombreCompleto => _tokenService.NombreCompleto;
    public string? UbicacionNombre => _tokenService.UbicacionNombre;
    public int? IdCaja => _tokenService.IdCaja;
    public string? CajaNumero => _tokenService.CajaNumero;
    public DateTimeOffset? Expiracion => _tokenService.Expiracion;

    public void GuardarSesion(LoginResponse response)
    {
        _tokenService.GuardarSesion(response);
        _logger.LogInformation("Sesión guardada para usuario {NombreUsuario} (rol: {Rol})", response.NombreUsuario, response.Rol);
    }

    public void LimpiarSesion()
    {
        _tokenService.LimpiarSesion();
        _logger.LogInformation("Sesión limpiada");
    }

    public bool PuedeAccederAMinistro(string modulo, string accion)
    {
        // La validación real de permisos se hace en el servidor.
        // Aquí solo verificamos que el usuario esté autenticado.
        // Los permisos específicos se validarán contra la API en cada operación.
        return EstaAutenticado;
    }
}

/// <summary>
/// Interfaz para el servicio de sesión.
/// </summary>
public interface ISesionService
{
    bool EstaAutenticado { get; }
    bool TokenExpirado { get; }
    int? IdUsuario { get; }
    string? Rol { get; }
    int? IdUbicacion { get; }
    string? NombreUsuario { get; }
    string? NombreCompleto { get; }
    string? UbicacionNombre { get; }
    int? IdCaja { get; }
    string? CajaNumero { get; }
    DateTimeOffset? Expiracion { get; }
    void GuardarSesion(LoginResponse response);
    void LimpiarSesion();
    bool PuedeAccederAMinistro(string modulo, string accion);
}
