namespace Altamira.Bodega.Domain.Enums;

public enum TipoMovimientoInventario
{
    Recepcion,
    Venta,
    Devolucion,
    TrasladoSalida,
    TrasladoEntrada,
    Ajuste,
    Merma
}

public enum EstadoLote
{
    Vigente,
    ProximoACaducar,
    Caducado,
    Agotado
}

public enum EstadoTraslado
{
    Solicitado,
    EnTransito,
    Recibido,
    Cancelado
}

public enum TipoAlertaInventario
{
    Caducidad,
    Caducado,
    StockMinimo,
    Discrepancia
}

public enum TipoUbicacion
{
    Sucursal,
    Bodega
}

public enum FormaPago
{
    Efectivo,
    TarjetaDebito,
    TarjetaCredito,
    Transferencia
}
