using Altamira.Bodega.Domain.Dtos;
using System;

namespace Altamira.Bodega.Domain.Interfaces;

public interface IInventarioService
{
    Task<IReadOnlyList<ExistenciaDto>> ConsultarExistenciasAsync(int? idProducto, int? idUbicacion, CancellationToken ct = default);
    Task<IReadOnlyList<LoteDto>> ListarLotesAsync(int? idProducto, CancellationToken ct = default);
    Task<RecepcionDto> RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct = default);
    Task<AjusteDto> RegistrarAjusteAsync(AjusteCreateDto dto, int idUsuario, CancellationToken ct = default);
    Task<IReadOnlyList<MovimientoDto>> ConsultarMovimientosAsync(int? idLote, int? idUbicacion, DateTimeOffset? desde, DateTimeOffset? hasta, CancellationToken ct = default);
    Task<IReadOnlyList<MovimientoDto>> ListarRecepcionesAsync(DateTimeOffset? desde, DateTimeOffset? hasta, CancellationToken ct = default);
}

public interface ITrasladoService
{
    Task<TrasladoDto> SolicitarTrasladoAsync(TrasladoCreateDto dto, int idUsuario, CancellationToken ct = default);
    Task<IReadOnlyList<TrasladoDto>> ListarTrasladosAsync(string? estado, int? idOrigen, int? idDestino, CancellationToken ct = default);
    Task<TrasladoDto> DespacharTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct = default);
    Task<TrasladoDto> RecibirTrasladoAsync(int idTraslado, TrasladoRecibirDto dto, int idUsuario, CancellationToken ct = default);
    Task<TrasladoDto> CancelarTrasladoAsync(int idTraslado, int idUsuario, CancellationToken ct = default);
}

public interface IProductoService
{
    Task<IReadOnlyList<ProductoDto>> ListarProductosAsync(string? busqueda, bool soloActivos, CancellationToken ct = default);
    Task<ProductoDto> ObtenerProductoAsync(int id, CancellationToken ct = default);
    Task<ProductoDto> CrearProductoAsync(ProductoCreateDto dto, int idUsuario, CancellationToken ct = default);
    Task<ProductoDto> ActualizarProductoAsync(int id, ProductoUpdateDto dto, int idUsuario, CancellationToken ct = default);
    Task DesactivarProductoAsync(int id, int idUsuario, CancellationToken ct = default);
}

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, string ipOrigen, CancellationToken ct = default);
    Task LogoutAsync(int idUsuario, string? jti, CancellationToken ct = default);
}

public interface IAlertaService
{
    Task<IReadOnlyList<AlertaDto>> ListarAlertasAsync(int? idUbicacion, bool soloNoAtendidas, CancellationToken ct = default);
    Task MarcarAlertaAtendidaAsync(int idAlerta, int idUsuario, CancellationToken ct = default);
}

public interface ITokenService
{
    string? AccessToken { get; }
    string? RefreshToken { get; }
    DateTimeOffset? Expiracion { get; }
    int? IdUsuario { get; }
    string? Rol { get; }
    int? IdUbicacion { get; }
    string? NombreUsuario { get; }
    string? NombreCompleto { get; }
    string? UbicacionNombre { get; }
    int? IdCaja { get; }
    string? CajaNumero { get; }
    bool EstaAutenticado { get; }
    bool TokenExpirado { get; }
    void GuardarSesion(LoginResponse response);
    void LimpiarSesion();
}

public interface IRecepcionService
{
    Task<IReadOnlyList<RecepcionDto>> ListarRecepcionesAsync(CancellationToken ct = default);
    Task<RecepcionDto> ObtenerRecepcionAsync(int id, CancellationToken ct = default);
    Task<RecepcionDto> RegistrarRecepcionAsync(RecepcionCreateDto dto, int idUsuario, CancellationToken ct = default);
}

public interface IAjusteService
{
    Task<AjusteDto> RegistrarAjusteAsync(AjusteCreateDto dto, int idUsuario, CancellationToken ct = default);
}

public interface INavigationService
{
    event Action<object>? ViewChanged;
    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateTo<TViewModel, TParameter>(TParameter parameter) where TViewModel : class;
    bool CanGoBack { get; }
    void GoBack();
}
