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

public enum AlertaPrioridad
{
    Alta,
    Media,
    Baja
}

public partial class AlertasCaducidadViewModel : ObservableObject
{
    private readonly IAlertaAppService _alertaService;
    private readonly ISesionAppService _sesionService;

    private ObservableCollection<AlertaDto> _alertasCaducadas = new();
    private ObservableCollection<AlertaDto> _alertasProximas = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _mensajeExito = string.Empty;
    private DateTimeOffset? _fechaDesde;
    private DateTimeOffset? _fechaHasta;
    private int? _filtroUbicacion;
    private string _busqueda = string.Empty;
    private bool _soloNoAtendidas = true;

    public ObservableCollection<AlertaDto> AlertasCaducadas => _alertasCaducadas;
    public ObservableCollection<AlertaDto> AlertasProximas => _alertasProximas;

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

    public string MensajeExito
    {
        get => _mensajeExito;
        set
        {
            SetProperty(ref _mensajeExito, value);
            OnPropertyChanged(nameof(HayMensajeExito));
        }
    }

    public bool HayMensajeError => !string.IsNullOrEmpty(_mensajeError);
    public bool HayMensajeExito => !string.IsNullOrEmpty(MensajeExito);

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

    public int? FiltroUbicacion
    {
        get => _filtroUbicacion;
        set => SetProperty(ref _filtroUbicacion, value);
    }

    public string Busqueda
    {
        get => _busqueda;
        set => SetProperty(ref _busqueda, value);
    }

    public bool SoloNoAtendidas
    {
        get => _soloNoAtendidas;
        set => SetProperty(ref _soloNoAtendidas, value);
    }

    public int TotalCaducadas => _alertasCaducadas.Count;
    public int TotalProximas => _alertasProximas.Count;
    public int TotalAlertas => TotalCaducadas + TotalProximas;
    public int PrioridadAltaCount => _alertasCaducadas.Count(a => ObtenerPrioridad(a) == AlertaPrioridad.Alta);
    public int PrioridadMediaCount => _alertasProximas.Count(a => ObtenerPrioridad(a) == AlertaPrioridad.Media);
    public int PrioridadBajaCount => _alertasProximas.Count(a => ObtenerPrioridad(a) == AlertaPrioridad.Baja);

    public bool HayAlertas => TotalAlertas > 0;
    public bool NoHayAlertas => !HayAlertas;

    public AlertasCaducidadViewModel(IAlertaAppService alertaService, ISesionAppService sesionService)
    {
        _alertaService = alertaService;
        _sesionService = sesionService;
        _filtroUbicacion = _sesionService.IdUbicacion;

        _ = CargarAlertasAsync();
    }

    private AlertaPrioridad ObtenerPrioridad(AlertaDto alerta)
    {
        if (alerta.Tipo.Equals("CADUCIDAD", StringComparison.OrdinalIgnoreCase))
            return AlertaPrioridad.Alta;
        if (alerta.Tipo.Equals("PROXIMA_CADUCIDAD", StringComparison.OrdinalIgnoreCase))
            return AlertaPrioridad.Media;
        return AlertaPrioridad.Baja;
    }

    private string ObtenerTextoPrioridad(AlertaDto alerta)
    {
        return ObtenerPrioridad(alerta) switch
        {
            AlertaPrioridad.Alta => "ALTA",
            AlertaPrioridad.Media => "MEDIA",
            _ => "BAJA"
        };
    }

    [RelayCommand]
    public async Task CargarAlertasAsync()
    {
        EstaCargando = true;
        MensajeError = null;
        MensajeExito = string.Empty;

        try
        {
            var alertas = await _alertaService.ListarAlertasAsync(_filtroUbicacion, _soloNoAtendidas, CancellationToken.None);

            var filtradas = alertas.Where(a =>
                (string.IsNullOrWhiteSpace(_busqueda) ||
                 a.ProductoNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 a.NumeroLote.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 a.UbicacionNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 a.Tipo.Contains(_busqueda, StringComparison.OrdinalIgnoreCase)) &&
                (_fechaDesde == null || a.FechaGeneracion >= _fechaDesde.Value) &&
                (_fechaHasta == null || a.FechaGeneracion <= _fechaHasta.Value));

            _alertasCaducadas.Clear();
            _alertasProximas.Clear();

            foreach (var a in filtradas.OrderByDescending(a => a.FechaGeneracion))
            {
                if (a.Tipo.Equals("CADUCIDAD", StringComparison.OrdinalIgnoreCase))
                    _alertasCaducadas.Add(a);
                else if (a.Tipo.Equals("PROXIMA_CADUCIDAD", StringComparison.OrdinalIgnoreCase))
                    _alertasProximas.Add(a);
            }

            OnPropertyChanged(nameof(TotalCaducadas));
            OnPropertyChanged(nameof(TotalProximas));
            OnPropertyChanged(nameof(TotalAlertas));
            OnPropertyChanged(nameof(PrioridadAltaCount));
            OnPropertyChanged(nameof(PrioridadMediaCount));
            OnPropertyChanged(nameof(PrioridadBajaCount));
            OnPropertyChanged(nameof(HayAlertas));
            OnPropertyChanged(nameof(NoHayAlertas));
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar alertas de caducidad: {ex.Message}";
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public async Task MarcarAtendidaAsync(int idAlerta)
    {
        MensajeError = null;
        MensajeExito = string.Empty;

        try
        {
            await _alertaService.MarcarAlertaAtendidaAsync(idAlerta, _sesionService.IdUsuario ?? 0, CancellationToken.None);

            var alerta = _alertasCaducadas.FirstOrDefault(a => a.IdAlerta == idAlerta)
                      ?? _alertasProximas.FirstOrDefault(a => a.IdAlerta == idAlerta);

            if (alerta != null)
            {
                if (_alertasCaducadas.Contains(alerta))
                    _alertasCaducadas.Remove(alerta);
                else if (_alertasProximas.Contains(alerta))
                    _alertasProximas.Remove(alerta);

                OnPropertyChanged(nameof(TotalCaducadas));
                OnPropertyChanged(nameof(TotalProximas));
                OnPropertyChanged(nameof(TotalAlertas));
                OnPropertyChanged(nameof(PrioridadAltaCount));
                OnPropertyChanged(nameof(PrioridadMediaCount));
                OnPropertyChanged(nameof(PrioridadBajaCount));
                OnPropertyChanged(nameof(HayAlertas));
                OnPropertyChanged(nameof(NoHayAlertas));
            }

            MensajeExito = "Alerta marcada como atendida.";
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al marcar alerta como atendida: {ex.Message}";
        }
    }

    [RelayCommand]
    public void LimpiarFiltros()
    {
        _busqueda = string.Empty;
        _fechaDesde = null;
        _fechaHasta = null;
        _filtroUbicacion = _sesionService.IdUbicacion;
        _soloNoAtendidas = true;

        OnPropertyChanged(nameof(Busqueda));
        OnPropertyChanged(nameof(FechaDesde));
        OnPropertyChanged(nameof(FechaHasta));
        OnPropertyChanged(nameof(FiltroUbicacion));
        OnPropertyChanged(nameof(SoloNoAtendidas));

        _ = CargarAlertasAsync();
    }
}
