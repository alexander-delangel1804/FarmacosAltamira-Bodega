namespace Altamira.Bodega.Domain.Dtos;

public sealed record ExistenciaDto(
    int IdProducto,
    string ProductoNombre,
    string CodigoBarras,
    int IdLote,
    string NumeroLote,
    DateOnly FechaCaducidad,
    int IdUbicacion,
    string UbicacionNombre,
    int Cantidad,
    int CantidadReservada,
    int Disponible);

public sealed record LoteDto(
    int IdLote,
    int IdProducto,
    string ProductoNombre,
    string CodigoBarras,
    string NumeroLote,
    DateOnly FechaCaducidad,
    DateOnly FechaIngreso,
    string Estado);

public sealed record MovimientoDto(
    long IdMovimiento,
    int IdLote,
    string ProductoNombre,
    string NumeroLote,
    int IdUbicacion,
    string UbicacionNombre,
    string UsuarioNombre,
    DateTimeOffset FechaHora,
    string Tipo,
    int Cantidad,
    string? Motivo,
    string? DocumentoOrigen,
    long? IdDocumento);

public sealed record ProductoDto(
    int IdProducto,
    int IdCategoria,
    string CategoriaNombre,
    string CodigoBarras,
    string NombreComercial,
    string? SustanciaActiva,
    string? Presentacion,
    decimal PrecioVenta,
    decimal TasaIva,
    int StockMinimo,
    bool EsControlado,
    bool Activo);

public sealed record TrasladoDto(
    int IdTraslado,
    string Folio,
    int IdOrigen,
    string OrigenNombre,
    int IdDestino,
    string DestinoNombre,
    int IdUsuarioSolicita,
    string UsuarioSolicitaNombre,
    DateTimeOffset FechaSolicitud,
    DateTimeOffset? FechaEnvio,
    DateTimeOffset? FechaRecepcion,
    string Estado,
    string? Observaciones);

public sealed record AlertaDto(
    int IdAlerta,
    int IdProducto,
    string ProductoNombre,
    int IdUbicacion,
    string UbicacionNombre,
    int? IdLote,
    string? NumeroLote,
    string Tipo,
    DateTimeOffset FechaGeneracion,
    string? Mensaje,
    bool Atendida);

public sealed record RecepcionDto(
    int IdRecepcion,
    int IdUbicacion,
    string UbicacionNombre,
    int IdUsuario,
    string UsuarioNombre,
    DateTimeOffset FechaHora,
    string? DocumentoReferencia,
    string? Observaciones);

public sealed record AjusteDto(
    int IdLote,
    int IdUbicacion,
    int Cantidad,
    string Motivo);

public sealed record UsuarioDto(
    int IdUsuario,
    string NombreUsuario,
    string NombreCompleto,
    string Rol,
    int? IdUbicacion,
    string? UbicacionNombre,
    bool Activo,
    bool Bloqueado,
    int IntentosFallidos,
    DateTimeOffset? UltimoAcceso,
    DateOnly FechaAlta);

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int IdUsuario,
    string NombreUsuario,
    string NombreCompleto,
    string Rol,
    int? IdUbicacion,
    string? UbicacionNombre,
    int? IdCaja,
    string? CajaNumero,
    DateTimeOffset Expiracion);

// DTOs de entrada (request)

public sealed record LoginRequest(
    string NombreUsuario,
    string Contrasena,
    int IdEstacion,
    string? SecretoEstacion);

public sealed record RecepcionCreateDto(
    int IdUbicacion,
    string? DocumentoReferencia,
    string? Observaciones,
    IReadOnlyList<DetalleRecepcionCreateDto> Detalles);

public sealed record DetalleRecepcionCreateDto(
    int IdProducto,
    string NumeroLote,
    DateOnly FechaCaducidad,
    int Cantidad);

public sealed record AjusteCreateDto(
    int IdLote,
    int IdUbicacion,
    int Cantidad,
    string Motivo);

public sealed record TrasladoCreateDto(
    int IdOrigen,
    int IdDestino,
    string? Observaciones,
    IReadOnlyList<DetalleTrasladoCreateDto> Detalles);

public sealed record DetalleTrasladoCreateDto(
    int IdLote,
    int CantidadEnviada);

public sealed record TrasladoRecibirDto(
    IReadOnlyList<DetalleRecibirDto> Detalles);

public sealed record DetalleRecibirDto(
    int IdLote,
    int CantidadRecibida);

public sealed record ProductoCreateDto(
    int IdCategoria,
    string CodigoBarras,
    string NombreComercial,
    string? SustanciaActiva,
    string? Presentacion,
    decimal PrecioVenta,
    decimal TasaIva,
    int StockMinimo,
    bool EsControlado);

public sealed record ProductoUpdateDto(
    int IdCategoria,
    string NombreComercial,
    string? SustanciaActiva,
    string? Presentacion,
    decimal PrecioVenta,
    decimal TasaIva,
    int StockMinimo,
    bool EsControlado);

// DTOs de seguridad y administración

public sealed record RolDto(
    int IdRol,
    string Nombre,
    string? Descripcion);

public sealed record PermisoDto(
    int IdPermiso,
    string Modulo,
    string Accion);

public sealed record BitacoraDto(
    long IdEvento,
    int IdUsuario,
    string NombreUsuario,
    int? IdUbicacion,
    string? UbicacionNombre,
    DateTimeOffset FechaHora,
    string Modulo,
    string Accion,
    string? Entidad,
    string? IdEntidad,
    string? Detalle,
    string? IpOrigen);

public sealed record ParametroDto(
    string Clave,
    string Valor,
    string? Descripcion,
    DateTimeOffset FechaModificacion);

public sealed record UsuarioCreateDto(
    string NombreUsuario,
    string NombreCompleto,
    string Contrasena,
    int IdRol,
    int? IdUbicacion);

public sealed record UsuarioUpdateDto(
    string NombreCompleto,
    int IdRol,
    int? IdUbicacion,
    bool Activo);

