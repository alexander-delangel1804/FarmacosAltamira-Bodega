using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Infrastructure.Clients;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Infrastructure.Services;

public class ProductoAppService : IProductoAppService
{
    private readonly ProductoApiClient _cliente;

    public ProductoAppService(ProductoApiClient cliente)
    {
        _cliente = cliente;
    }

    public Task<IReadOnlyList<ProductoDto>> ListarProductosAsync(string? busqueda, bool soloActivos, CancellationToken ct = default)
        => _cliente.ListarProductosAsync(busqueda, soloActivos, ct);

    public Task<ProductoDto> ObtenerProductoAsync(int id, CancellationToken ct = default)
        => _cliente.ObtenerProductoAsync(id, ct);

    public Task<ProductoDto> CrearProductoAsync(ProductoCreateDto dto, int idUsuario, CancellationToken ct = default)
        => _cliente.CrearProductoAsync(dto, idUsuario, ct);

    public Task<ProductoDto> ActualizarProductoAsync(int id, ProductoUpdateDto dto, int idUsuario, CancellationToken ct = default)
        => _cliente.ActualizarProductoAsync(id, dto, idUsuario, ct);

    public Task DesactivarProductoAsync(int id, int idUsuario, CancellationToken ct = default)
        => _cliente.DesactivarProductoAsync(id, idUsuario, ct);
}

public class InventarioAppService : IInventarioAppService
{
    private readonly InventarioApiClient _cliente;

    public InventarioAppService(InventarioApiClient cliente)
    {
        _cliente = cliente;
    }

    public Task<IReadOnlyList<ExistenciaDto>> ConsultarExistenciasAsync(int? idProducto, int? idUbicacion, CancellationToken ct = default)
        => _cliente.ConsultarExistenciasAsync(idProducto, idUbicacion, ct);

    public Task<IReadOnlyList<LoteDto>> ListarLotesAsync(int? idProducto, CancellationToken ct = default)
        => _cliente.ListarLotesAsync(idProducto, ct);

    public Task<IReadOnlyList<MovimientoDto>> ConsultarMovimientosAsync(int? idLote, int? idUbicacion, System.DateTimeOffset? desde, System.DateTimeOffset? hasta, CancellationToken ct = default)
        => _cliente.ConsultarMovimientosAsync(idLote, idUbicacion, desde, hasta, ct);

    public Task<RecepcionDto> RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct = default)
        => _cliente.RegistrarRecepcionAsync(dto, idUsuario, ct);

    public Task<AjusteDto> RegistrarAjusteAsync(AjusteCreateDto dto, int idUsuario, CancellationToken ct = default)
        => _cliente.RegistrarAjusteAsync(dto, idUsuario, ct);

    public Task<IReadOnlyList<MovimientoDto>> ListarRecepcionesAsync(System.DateTimeOffset? desde, System.DateTimeOffset? hasta, CancellationToken ct = default)
        => _cliente.ListarRecepcionesAsync(desde, hasta, ct);
}

public class TrasladoAppService : ITrasladoAppService
{
    private readonly TrasladoApiClient _cliente;

    public TrasladoAppService(TrasladoApiClient cliente)
    {
        _cliente = cliente;
    }

    public Task<TrasladoDto> SolicitarTrasladoAsync(TrasladoCreateDto dto, int idUsuario, CancellationToken ct = default)
        => _cliente.SolicitarTrasladoAsync(dto, idUsuario, ct);

    public Task<IReadOnlyList<TrasladoDto>> ListarTrasladosAsync(string? estado, int? idOrigen, int? idDestino, CancellationToken ct = default)
        => _cliente.ListarTrasladosAsync(estado, idOrigen, idDestino, ct);

    public Task<TrasladoDto> DespacharTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct = default)
        => _cliente.DespacharTrasladoAsync(idTraslado, idUsuario, ct);

    public Task<TrasladoDto> RecibirTrasladoAsync(int idTraslado, TrasladoRecibirDto dto, int idUsuario, CancellationToken ct = default)
        => _cliente.RecibirTrasladoAsync(idTraslado, dto, idUsuario, ct);

    public Task<TrasladoDto> CancelarTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct = default)
        => _cliente.CancelarTrasladoAsync(idTraslado, idUsuario, ct);
}

public class AlertaAppService : IAlertaAppService
{
    private readonly AlertaApiClient _cliente;

    public AlertaAppService(AlertaApiClient cliente)
    {
        _cliente = cliente;
    }

    public Task<IReadOnlyList<AlertaDto>> ListarAlertasAsync(int? idUbicacion, bool soloNoAtendidas, CancellationToken ct = default)
        => _cliente.ListarAlertasAsync(idUbicacion, soloNoAtendidas, ct);

    public Task MarcarAlertaAtendidaAsync(int idAlerta, int idUsuario, CancellationToken ct = default)
        => _cliente.MarcarAlertaAtendidaAsync(idAlerta, idUsuario, ct);
}

public class UsuarioAppService : IUsuarioAppService
{
    private readonly UsuarioApiClient _cliente;

    public UsuarioAppService(UsuarioApiClient cliente)
    {
        _cliente = cliente;
    }

    public Task<IReadOnlyList<UsuarioDto>> ListarUsuariosAsync(CancellationToken ct = default)
        => _cliente.ListarUsuariosAsync(ct);

    public Task<UsuarioDto> ObtenerUsuarioAsync(int id, CancellationToken ct = default)
        => _cliente.ObtenerUsuarioAsync(id, ct);

    public Task<UsuarioDto> CrearUsuarioAsync(UsuarioCreateDto dto, int idUsuario, CancellationToken ct = default)
        => _cliente.CrearUsuarioAsync(dto, idUsuario, ct);

    public Task<UsuarioDto> ActualizarUsuarioAsync(int id, UsuarioUpdateDto dto, int idUsuario, CancellationToken ct = default)
        => _cliente.ActualizarUsuarioAsync(id, dto, idUsuario, ct);

    public Task BloquearUsuarioAsync(int id, int idUsuario, CancellationToken ct = default)
        => _cliente.BloquearUsuarioAsync(id, idUsuario, ct);

    public Task DesbloquearUsuarioAsync(int id, int idUsuario, CancellationToken ct = default)
        => _cliente.DesbloquearUsuarioAsync(id, idUsuario, ct);

    public Task DesactivarUsuarioAsync(int id, int idUsuario, CancellationToken ct = default)
        => _cliente.DesactivarUsuarioAsync(id, idUsuario, ct);
}

public class RolAppService : IRolAppService
{
    private readonly RolApiClient _cliente;

    public RolAppService(RolApiClient cliente)
    {
        _cliente = cliente;
    }

    public Task<IReadOnlyList<RolDto>> ListarRolesAsync(CancellationToken ct = default)
        => _cliente.ListarRolesAsync(ct);

    public Task<IReadOnlyList<PermisoDto>> ListarPermisosAsync(CancellationToken ct = default)
        => _cliente.ListarPermisosAsync(ct);
}

public class ParametroAppService : IParametroAppService
{
    private readonly ParametroApiClient _cliente;

    public ParametroAppService(ParametroApiClient cliente)
    {
        _cliente = cliente;
    }

    public Task<IReadOnlyList<ParametroDto>> ListarParametrosAsync(CancellationToken ct = default)
        => _cliente.ListarParametrosAsync(ct);

    public Task<ParametroDto> ObtenerParametroAsync(string clave, CancellationToken ct = default)
        => _cliente.ObtenerParametroAsync(clave, ct);

    public Task ActualizarParametroAsync(string clave, string valor, int idUsuario, CancellationToken ct = default)
        => _cliente.ActualizarParametroAsync(clave, valor, idUsuario, ct);
}

public class BitacoraAppService : IBitacoraAppService
{
    private readonly BitacoraApiClient _cliente;

    public BitacoraAppService(BitacoraApiClient cliente)
    {
        _cliente = cliente;
    }

    public Task<IReadOnlyList<BitacoraDto>> ConsultarBitacoraAsync(
        int? idUsuario,
        int? idUbicacion,
        string? modulo,
        string? accion,
        System.DateTimeOffset? fechaDesde,
        System.DateTimeOffset? fechaHasta,
        CancellationToken ct = default)
        => _cliente.ConsultarBitacoraAsync(idUsuario, idUbicacion, modulo, accion, fechaDesde, fechaHasta, ct);
}

public class AuthAppService : IAuthAppService
{
    private readonly AuthApiClient _cliente;
    private readonly ISesionService _sesion;

    public AuthAppService(AuthApiClient cliente, ISesionService sesion)
    {
        _cliente = cliente;
        _sesion = sesion;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string ipOrigen, CancellationToken ct = default)
    {
        var response = await _cliente.LoginAsync(request, ipOrigen, ct);
        _sesion.GuardarSesion(response);
        return response;
    }

    public Task LogoutAsync(int idUsuario, string? jti, CancellationToken ct = default)
    {
        _sesion.LimpiarSesion();
        return _cliente.LogoutAsync(idUsuario, jti, ct);
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var response = await _cliente.RefreshTokenAsync(refreshToken, ct);
        _sesion.GuardarSesion(response);
        return response;
    }
}

public class SesionAppService : ISesionAppService
{
    private readonly ISesionService _sesion;

    public SesionAppService(ISesionService sesion)
    {
        _sesion = sesion;
    }

    public bool EstaAutenticado => _sesion.EstaAutenticado;
    public bool TokenExpirado => _sesion.TokenExpirado;
    public int? IdUsuario => _sesion.IdUsuario;
    public string? Rol => _sesion.Rol;
    public int? IdUbicacion => _sesion.IdUbicacion;
    public string? NombreUsuario => _sesion.NombreUsuario;
    public string? NombreCompleto => _sesion.NombreCompleto;
    public string? UbicacionNombre => _sesion.UbicacionNombre;
    public int? IdCaja => _sesion.IdCaja;
    public string? CajaNumero => _sesion.CajaNumero;
    public System.DateTimeOffset? Expiracion => _sesion.Expiracion;

    public void GuardarSesion(LoginResponse response) => _sesion.GuardarSesion(response);
    public void LimpiarSesion() => _sesion.LimpiarSesion();
    public bool PuedeAccederAMinistro(string modulo, string accion) => _sesion.PuedeAccederAMinistro(modulo, accion);
}
