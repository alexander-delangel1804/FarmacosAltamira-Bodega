using Altamira.Bodega.Domain.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class LoteInfoViewModel : ObservableObject
{
    private ProductoDto? _producto;
    private LoteDto? _lote;
    private ExistenciaDto? _existencia;

    [ObservableProperty]
    private string? _mensajeError;

    [ObservableProperty]
    private bool _estaCargando;

    public ProductoDto? Producto
    {
        get => _producto;
        set => SetProperty(ref _producto, value);
    }

    public LoteDto? Lote
    {
        get => _lote;
        set
        {
            SetProperty(ref _lote, value);
            CalcularValorInventario();
        }
    }

    public ExistenciaDto? Existencia
    {
        get => _existencia;
        set
        {
            SetProperty(ref _existencia, value);
            CalcularValorInventario();
        }
    }

    [ObservableProperty]
    private decimal _valorInventario;

    [ObservableProperty]
    private string _estadoTexto = string.Empty;

    [ObservableProperty]
    private string _diasHastaCaducidadTexto = string.Empty;

    public LoteInfoViewModel()
    {
    }

    public void CargarDatos(ProductoDto producto, LoteDto lote, ExistenciaDto? existencia = null)
    {
        Producto = producto;
        Lote = lote;
        Existencia = existencia;

        CalcularEstadoTexto();
        CalcularDiasHastaCaducidad();
    }

    private void CalcularValorInventario()
    {
        if (Producto != null && Existencia != null)
        {
            ValorInventario = Producto.PrecioVenta * Existencia.Disponible;
        }
        else
        {
            ValorInventario = 0;
        }
    }

    private void CalcularEstadoTexto()
    {
        if (Lote == null)
        {
            EstadoTexto = string.Empty;
            return;
        }

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var fechaCaducidad = Lote.FechaCaducidad;
        var dias = (fechaCaducidad.ToDateTime(TimeOnly.MinValue) - hoy.ToDateTime(TimeOnly.MinValue)).Days;

        if (dias < 0)
            EstadoTexto = "CADUCADO";
        else if (dias <= 30)
            EstadoTexto = "PROXIMO A CADUCAR";
        else if (Existencia?.Disponible <= 0)
            EstadoTexto = "AGOTADO";
        else if (Existencia?.Disponible <= 10)
            EstadoTexto = "BAJO MINIMO";
        else
            EstadoTexto = "VIGENTE";
    }

    private void CalcularDiasHastaCaducidad()
    {
        if (Lote == null)
        {
            DiasHastaCaducidadTexto = string.Empty;
            return;
        }

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var fechaCaducidad = Lote.FechaCaducidad;
        var dias = (fechaCaducidad.ToDateTime(TimeOnly.MinValue) - hoy.ToDateTime(TimeOnly.MinValue)).Days;

        if (dias < 0)
            DiasHastaCaducidadTexto = "Caducado";
        else if (dias == 0)
            DiasHastaCaducidadTexto = "Caduca hoy";
        else if (dias == 1)
            DiasHastaCaducidadTexto = "Caduca mañana";
        else
            DiasHastaCaducidadTexto = $"Caduca en {dias} días";
    }
}
