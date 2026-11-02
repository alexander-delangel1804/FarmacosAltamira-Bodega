using Altamira.Bodega.Domain.Enums;

namespace Altamira.Bodega.Domain.Entities;

public sealed class Ubicacion
{
    public int IdUbicacion { get; set; }
    public TipoUbicacion Tipo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Municipio { get; set; }
    public string? Telefono { get; set; }
    public bool Activa { get; set; } = true;
}

public sealed class Rol
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public sealed class Permiso
{
    public int IdPermiso { get; set; }
    public string Modulo { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
}

public sealed class Usuario
{
    public int IdUsuario { get; set; }
    public int IdRol { get; set; }
    public int? IdUbicacion { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public bool Bloqueado { get; set; } = false;
    public int IntentosFallidos { get; set; } = 0;
    public DateTimeOffset? UltimoAcceso { get; set; }
    public DateOnly FechaAlta { get; set; }
}

public sealed class Producto
{
    public int IdProducto { get; set; }
    public int IdCategoria { get; set; }
    public string CodigoBarras { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string? SustanciaActiva { get; set; }
    public string? Presentacion { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal TasaIva { get; set; }
    public int StockMinimo { get; set; }
    public bool EsControlado { get; set; }
    public bool Activo { get; set; } = true;
}

public sealed class Lote
{
    public int IdLote { get; set; }
    public int IdProducto { get; set; }
    public string NumeroLote { get; set; } = string.Empty;
    public DateOnly FechaCaducidad { get; set; }
    public DateOnly FechaIngreso { get; set; }
    public EstadoLote Estado { get; set; } = EstadoLote.Vigente;
}

public sealed class MovimientoInventario
{
    public long IdMovimiento { get; set; }
    public int IdLote { get; set; }
    public int IdUbicacion { get; set; }
    public int IdUsuario { get; set; }
    public DateTimeOffset FechaHora { get; set; }
    public TipoMovimientoInventario Tipo { get; set; }
    public int Cantidad { get; set; }
    public string? Motivo { get; set; }
    public string? DocumentoOrigen { get; set; }
    public long? IdDocumento { get; set; }
}

public sealed class Existencia
{
    public int IdLote { get; set; }
    public int IdUbicacion { get; set; }
    public int Cantidad { get; set; }
    public int CantidadReservada { get; set; }
    public int Disponible => Cantidad - CantidadReservada;
}

public sealed class Traslado
{
    public int IdTraslado { get; set; }
    public string Folio { get; set; } = string.Empty;
    public int IdOrigen { get; set; }
    public int IdDestino { get; set; }
    public int IdUsuarioSolicita { get; set; }
    public DateTimeOffset FechaSolicitud { get; set; }
    public DateTimeOffset? FechaEnvio { get; set; }
    public DateTimeOffset? FechaRecepcion { get; set; }
    public EstadoTraslado Estado { get; set; } = EstadoTraslado.Solicitado;
    public string? Observaciones { get; set; }
}

public sealed class RecepcionMercancia
{
    public int IdRecepcion { get; set; }
    public int IdUbicacion { get; set; }
    public int IdUsuario { get; set; }
    public DateTimeOffset FechaHora { get; set; }
    public string? DocumentoReferencia { get; set; }
    public string? Observaciones { get; set; }
}


