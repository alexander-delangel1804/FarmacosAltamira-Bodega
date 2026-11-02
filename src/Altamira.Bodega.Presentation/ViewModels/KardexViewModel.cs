using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class KardexViewModel : ObservableObject
{
    private readonly IInventarioAppService _inventarioService;

    private ObservableCollection<MovimientoDto> _movimientos = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _busqueda = string.Empty;
    private string _filtroTipo = string.Empty;
    private string _filtroProducto = string.Empty;
    private string _filtroLote = string.Empty;
    private int? _filtroUbicacion;
    private DateTimeOffset? _fechaDesde;
    private DateTimeOffset? _fechaHasta;
    private int _paginaActual = 1;
    private int _tamanoPagina = 50;
    private long _totalRegistros;
    private int _totalPaginas;
    private bool _tieneSiguiente;
    private bool _tieneAnterior;

    public ObservableCollection<MovimientoDto> Movimientos => _movimientos;

    public bool EstaCargando
    {
        get => _estaCargando;
        set => SetProperty(ref _estaCargando, value);
    }

    public string? MensajeError
    {
        get => _mensajeError;
        set
        {
            SetProperty(ref _mensajeError, value);
            OnPropertyChanged(nameof(HayMensajeError));
        }
    }

    public string MensajeExito { get; set; } = string.Empty;

    public bool HayMensajeError => !string.IsNullOrEmpty(_mensajeError);
    public bool HayMensajeExito => !string.IsNullOrEmpty(MensajeExito);

    public string Busqueda
    {
        get => _busqueda;
        set => SetProperty(ref _busqueda, value);
    }

    public string FiltroTipo
    {
        get => _filtroTipo;
        set => SetProperty(ref _filtroTipo, value);
    }

    public string FiltroProducto
    {
        get => _filtroProducto;
        set => SetProperty(ref _filtroProducto, value);
    }

    public string FiltroLote
    {
        get => _filtroLote;
        set => SetProperty(ref _filtroLote, value);
    }

    public int? FiltroUbicacion
    {
        get => _filtroUbicacion;
        set => SetProperty(ref _filtroUbicacion, value);
    }

    public DateTimeOffset? FechaDesde
    {
        get => _fechaDesde;
        set => SetProperty(ref _fechaDesde, value);
    }

    public DateTimeOffset? FechaHasta
    {
        get => _fechaHasta;
        set => SetProperty(ref _fechaHasta, value);
    }

    public int PaginaActual
    {
        get => _paginaActual;
        set => SetProperty(ref _paginaActual, value);
    }

    public int TamanoPagina
    {
        get => _tamanoPagina;
        set => SetProperty(ref _tamanoPagina, value);
    }

    public long TotalRegistros
    {
        get => _totalRegistros;
        set => SetProperty(ref _totalRegistros, value);
    }

    public int TotalPaginas
    {
        get => _totalPaginas;
        set => SetProperty(ref _totalPaginas, value);
    }

    public bool TieneSiguiente
    {
        get => _tieneSiguiente;
        set => SetProperty(ref _tieneSiguiente, value);
    }

    public bool TieneAnterior
    {
        get => _tieneAnterior;
        set => SetProperty(ref _tieneAnterior, value);
    }

    public int TotalMovimientos => _movimientos.Count;
    public bool HayMovimientos => _movimientos.Count > 0;
    public bool NoHayMovimientos => !HayMovimientos;
    public decimal TotalEntradas => _movimientos.Where(m => m.Tipo == "ENTRADA").Sum(m => m.Cantidad);
    public decimal TotalSalidas => _movimientos.Where(m => m.Tipo == "SALIDA").Sum(m => m.Cantidad);

    public KardexViewModel(IInventarioAppService inventarioService)
    {
        _inventarioService = inventarioService;
        _ = CargarMovimientosAsync();
    }

    [RelayCommand]
    public async Task CargarMovimientosAsync()
    {
        EstaCargando = true;
        MensajeError = null;
        MensajeExito = string.Empty;

        try
        {
            var movimientos = await _inventarioService.ConsultarMovimientosAsync(
                null, FiltroUbicacion, FechaDesde, FechaHasta, CancellationToken.None);

            var filtrados = AplicarFiltros(movimientos);

            TotalRegistros = filtrados.Count();
            TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / Math.Max(1, TamanoPagina));
            TieneSiguiente = PaginaActual < TotalPaginas;
            TieneAnterior = PaginaActual > 1;

            var pagina = filtrados
                .Skip((PaginaActual - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();

            _movimientos.Clear();
            foreach (var m in pagina)
                _movimientos.Add(m);

            OnPropertyChanged(nameof(TotalMovimientos));
            OnPropertyChanged(nameof(TotalEntradas));
            OnPropertyChanged(nameof(TotalSalidas));
            OnPropertyChanged(nameof(HayMovimientos));
            OnPropertyChanged(nameof(NoHayMovimientos));
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar movimientos: {ex.Message}";
        }
        finally
        {
            EstaCargando = false;
        }
    }

    private IEnumerable<MovimientoDto> AplicarFiltros(IEnumerable<MovimientoDto> lista)
    {
        var resultado = lista.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(_busqueda))
        {
            var b = _busqueda.ToLowerInvariant();
            resultado = resultado.Where(m =>
                m.ProductoNombre.ToLowerInvariant().Contains(b) ||
                m.NumeroLote.ToLowerInvariant().Contains(b) ||
                m.UsuarioNombre.ToLowerInvariant().Contains(b) ||
                m.DocumentoOrigen.ToLowerInvariant().Contains(b));
        }

        if (!string.IsNullOrWhiteSpace(_filtroTipo))
        {
            var t = _filtroTipo.ToLowerInvariant();
            resultado = resultado.Where(m => m.Tipo.ToLowerInvariant().Contains(t));
        }

        if (!string.IsNullOrWhiteSpace(_filtroProducto))
        {
            var p = _filtroProducto.ToLowerInvariant();
            resultado = resultado.Where(m => m.ProductoNombre.ToLowerInvariant().Contains(p));
        }

        if (!string.IsNullOrWhiteSpace(_filtroLote))
        {
            var l = _filtroLote.ToLowerInvariant();
            resultado = resultado.Where(m => m.NumeroLote.ToLowerInvariant().Contains(l));
        }

        if (_filtroUbicacion.HasValue)
        {
            resultado = resultado.Where(m => m.IdUbicacion == _filtroUbicacion.Value);
        }

        return resultado.OrderByDescending(m => m.FechaHora);
    }

    [RelayCommand]
    public void LimpiarFiltros()
    {
        _busqueda = string.Empty;
        _filtroTipo = string.Empty;
        _filtroProducto = string.Empty;
        _filtroLote = string.Empty;
        _filtroUbicacion = null;
        _fechaDesde = null;
        _fechaHasta = null;
        _paginaActual = 1;

        OnPropertyChanged(nameof(Busqueda));
        OnPropertyChanged(nameof(FiltroTipo));
        OnPropertyChanged(nameof(FiltroProducto));
        OnPropertyChanged(nameof(FiltroLote));
        OnPropertyChanged(nameof(FiltroUbicacion));
        OnPropertyChanged(nameof(FechaDesde));
        OnPropertyChanged(nameof(FechaHasta));
        OnPropertyChanged(nameof(PaginaActual));

        _ = CargarMovimientosAsync();
    }

    [RelayCommand]
    public void CambiarPagina(int delta)
    {
        var nuevaPagina = _paginaActual + delta;
        if (nuevaPagina < 1 || nuevaPagina > TotalPaginas)
            return;

        PaginaActual = nuevaPagina;
        _ = CargarMovimientosAsync();
    }

    [RelayCommand]
    public void Exportar()
    {
        MensajeExito = "Exportación a CSV no implementada en esta versión.";
    }
}
