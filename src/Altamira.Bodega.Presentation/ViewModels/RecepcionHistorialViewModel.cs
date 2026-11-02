using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class RecepcionHistorialViewModel : ObservableObject
{
    private readonly IInventarioAppService _inventarioService;

    private ObservableCollection<MovimientoDto> _movimientos = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private DateTimeOffset? _fechaDesde;
    private DateTimeOffset? _fechaHasta;
    private string _busqueda = string.Empty;

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

    public bool HayMensajeError => !string.IsNullOrEmpty(_mensajeError);
    public bool HayMovimientos => _movimientos.Count > 0;
    public bool NoHayMovimientos => !HayMovimientos;

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

    public string Busqueda
    {
        get => _busqueda;
        set => SetProperty(ref _busqueda, value);
    }

    public int TotalMovimientos => _movimientos.Count;
    public int TotalCantidad => _movimientos.Sum(m => m.Cantidad);

    public RecepcionHistorialViewModel(IInventarioAppService inventarioService)
    {
        _inventarioService = inventarioService;
        _ = CargarRecepcionesAsync();
    }

    [RelayCommand]
    public async Task CargarRecepcionesAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            var movimientos = await _inventarioService.ListarRecepcionesAsync(_fechaDesde, _fechaHasta, CancellationToken.None);

            var filtrados = movimientos
                .Where(m => m.Tipo == "RECEPCION" || m.DocumentoOrigen == "RECEPCION")
                .Where(m => string.IsNullOrWhiteSpace(_busqueda) ||
                    m.ProductoNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                    m.NumeroLote.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                    m.UsuarioNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                    m.DocumentoOrigen.Contains(_busqueda, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(m => m.FechaHora)
                .ToList();

            _movimientos.Clear();
            foreach (var m in filtrados)
                _movimientos.Add(m);

            OnPropertyChanged(nameof(TotalMovimientos));
            OnPropertyChanged(nameof(TotalCantidad));
            OnPropertyChanged(nameof(HayMovimientos));
            OnPropertyChanged(nameof(NoHayMovimientos));
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar historial de recepciones: {ex.Message}";
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public void LimpiarFiltros()
    {
        _busqueda = string.Empty;
        _fechaDesde = null;
        _fechaHasta = null;
        OnPropertyChanged(nameof(Busqueda));
        OnPropertyChanged(nameof(FechaDesde));
        OnPropertyChanged(nameof(FechaHasta));
        _ = CargarRecepcionesAsync();
    }
}
