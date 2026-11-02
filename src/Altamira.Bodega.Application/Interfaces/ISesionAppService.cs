using Altamira.Bodega.Domain.Dtos;

namespace Altamira.Bodega.Application.Interfaces;

public interface ISesionAppService
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
    System.DateTimeOffset? Expiracion { get; }
    void GuardarSesion(LoginResponse response);
    void LimpiarSesion();
    bool PuedeAccederAMinistro(string modulo, string accion);
}
