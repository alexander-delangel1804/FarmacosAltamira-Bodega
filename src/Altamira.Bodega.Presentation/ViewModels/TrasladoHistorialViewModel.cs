using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class TrasladoHistorialViewModel : ObservableObject
{
    private readonly ITrasladoAppService _trasladoAppService;

    private ObservableCollection<TrasladoDto> _traslados = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _busqueda = string.Empty;
    private string _filtroEstado = string.Empty;
    private int? _filtroOrigen;
    private int? _filtroDestino;
    private DateTimeOffset? _fechaDesde;
    private DateTimeOffset? _fechaHasta;

    public ObservableCollection<TrasladoDto> Traslados => _traslados;

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

    public string FiltroEstado
    {
        get => _filtroEstado;
        set => SetProperty(ref _filtroEstado, value);
    }

    public int? FiltroOrigen
    {
        get => _filtroOrigen;
        set => SetProperty(ref _filtroOrigen, value);
    }

    public int? FiltroDestino
    {
        get => _filtroDestino;
        set => SetProperty(ref _filtroDestino, value);
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

    public int TotalTraslados => _traslados.Count;
    public bool HayTraslados => _traslados.Count > 0;

    public TrasladoHistorialViewModel(ITrasladoAppService trasladoAppService)
    {
        _trasladoAppService = trasladoAppService;
        _ = CargarTrasladosAsync();
    }

    [RelayCommand]
    public async Task CargarTrasladosAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            var traslados = await _trasladoAppService.ListarTrasladosAsync(
                string.IsNullOrWhiteSpace(_filtroEstado) ? null : _filtroEstado,
                _filtroOrigen,
                _filtroDestino,
                CancellationToken.None);

            var filtrados = traslados.Where(t =>
                (string.IsNullOrWhiteSpace(_busqueda) ||
                 t.Folio.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 t.OrigenNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 t.DestinoNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 t.UsuarioSolicitaNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase)) &&
                (_fechaDesde == null || t.FechaSolicitud >= _fechaDesde.Value) &&
                (_fechaHasta == null || t.FechaSolicitud <= _fechaHasta.Value));

            _traslados.Clear();
            foreach (var t in filtrados.OrderByDescending(t => t.FechaSolicitud))
                _traslados.Add(t);
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar historial de traslados: {ex.Message}";
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
        _filtroEstado = string.Empty;
        _filtroOrigen = null;
        _filtroDestino = null;
        _fechaDesde = null;
        _fechaHasta = null;

        OnPropertyChanged(nameof(Busqueda));
        OnPropertyChanged(nameof(FiltroEstado));
        OnPropertyChanged(nameof(FiltroOrigen));
        OnPropertyChanged(nameof(FiltroDestino));
        OnPropertyChanged(nameof(FechaDesde));
        OnPropertyChanged(nameof(FechaHasta));

        _ = CargarTrasladosAsync();
    }
}
