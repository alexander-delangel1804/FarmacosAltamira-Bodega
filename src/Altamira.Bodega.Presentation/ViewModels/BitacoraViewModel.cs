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

public partial class BitacoraViewModel : ObservableObject
{
    private readonly IBitacoraAppService _bitacoraAppService;
    private readonly ISesionAppService _sesionService;

    private ObservableCollection<BitacoraDto> _bitacora = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _busqueda = string.Empty;
    private string? _filtroModulo;
    private string? _filtroAccion;
    private int? _filtroUsuario;
    private int? _filtroUbicacion;
    private DateTimeOffset? _fechaDesde;
    private DateTimeOffset? _fechaHasta;

    public ObservableCollection<BitacoraDto> Bitacora => _bitacora;

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
    public bool HayBitacora => _bitacora.Count > 0;
    public bool NoHayBitacora => !HayBitacora;

    public string Busqueda
    {
        get => _busqueda;
        set => SetProperty(ref _busqueda, value);
    }

    public string? FiltroModulo
    {
        get => _filtroModulo;
        set => SetProperty(ref _filtroModulo, value);
    }

    public string? FiltroAccion
    {
        get => _filtroAccion;
        set => SetProperty(ref _filtroAccion, value);
    }

    public int? FiltroUsuario
    {
        get => _filtroUsuario;
        set => SetProperty(ref _filtroUsuario, value);
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

    public BitacoraViewModel(IBitacoraAppService bitacoraAppService, ISesionAppService sesionService)
    {
        _bitacoraAppService = bitacoraAppService;
        _sesionService = sesionService;
        _filtroUbicacion = _sesionService.IdUbicacion;

        _ = CargarBitacoraAsync();
    }

    [RelayCommand]
    public async Task CargarBitacoraAsync()
    {
        EstaCargando = true;
        MensajeError = null;
        MensajeExito = string.Empty;

        try
        {
            var bitacoras = await _bitacoraAppService.ConsultarBitacoraAsync(
                _filtroUsuario,
                _filtroUbicacion,
                _filtroModulo,
                _filtroAccion,
                _fechaDesde,
                _fechaHasta,
                CancellationToken.None);

            var filtradas = bitacoras.Where(b =>
                (string.IsNullOrWhiteSpace(_busqueda) ||
                 b.NombreUsuario.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 b.Modulo.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 b.Accion.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 (b.Entidad != null && b.Entidad.Contains(_busqueda, StringComparison.OrdinalIgnoreCase))));

            _bitacora.Clear();
            foreach (var b in filtradas.OrderByDescending(b => b.FechaHora))
                _bitacora.Add(b);

            OnPropertyChanged(nameof(Bitacora));
            OnPropertyChanged(nameof(HayBitacora));
            OnPropertyChanged(nameof(NoHayBitacora));
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar bitácora: {ex.Message}";
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
        _filtroModulo = null;
        _filtroAccion = null;
        _filtroUsuario = null;
        _filtroUbicacion = null;
        _fechaDesde = null;
        _fechaHasta = null;

        OnPropertyChanged(nameof(Busqueda));
        OnPropertyChanged(nameof(FiltroModulo));
        OnPropertyChanged(nameof(FiltroAccion));
        OnPropertyChanged(nameof(FiltroUsuario));
        OnPropertyChanged(nameof(FiltroUbicacion));
        OnPropertyChanged(nameof(FechaDesde));
        OnPropertyChanged(nameof(FechaHasta));

        _ = CargarBitacoraAsync();
    }
}
