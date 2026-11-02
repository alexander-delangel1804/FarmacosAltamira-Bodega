namespace Altamira.Bodega.Application.Dtos;

public sealed record LoginRequestDto(
    string NombreUsuario,
    string Contrasena,
    int IdEstacion,
    string? SecretoEstacion);

public sealed record ProductoCreateRequestDto(
    int IdCategoria,
    string CodigoBarras,
    string NombreComercial,
    string? SustanciaActiva,
    string? Presentacion,
    decimal PrecioVenta,
    decimal TasaIva,
    int StockMinimo,
    bool EsControlado);

public sealed record TrasladoCreateRequestDto(
    int IdOrigen,
    int IdDestino,
    string? Observaciones,
    IReadOnlyList<DetalleTrasladoCreateRequestDto> Detalles);

public sealed record DetalleTrasladoCreateRequestDto(
    int IdLote,
    int CantidadEnviada);

public sealed record AjusteCreateRequestDto(
    int IdLote,
    int IdUbicacion,
    int Cantidad,
    string Motivo);
