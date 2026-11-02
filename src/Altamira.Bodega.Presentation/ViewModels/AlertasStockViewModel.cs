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

public partial class AlertasStockViewModel : ObservableObject
{
    private readonly IAlertaAppService _alertaService;
    private readonly ISesionAppService _sesionService;

    private ObservableCollection<AlertaDto> _alertasStock = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _mensajeExito = string.Empty;
    private DateTimeOffset? _fechaDesde;
    private DateTimeOffset? _fechaHasta;
    private int? _filtroUbicacion;
    private string _busqueda = string.Empty;
    private bool _soloNoAtendidas = true;

    public ObservableCollection<AlertaDto> AlertasStock => _alertasStock;

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

    public int TotalAlertasStock => _alertasStock.Count;
    public bool HayAlertas => TotalAlertasStock > 0;
    public bool NoHayAlertas => !HayAlertas;

    public AlertasStockViewModel(IAlertaAppService alertaService, ISesionAppService sesionService)
    {
        _alertaService = alertaService;
        _sesionService = sesionService;
        _filtroUbicacion = _sesionService.IdUbicacion;

        _ = CargarAlertasAsync();
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
                a.Tipo.Equals("STOCK_MINIMO", StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrWhiteSpace(_busqueda) ||
                 a.ProductoNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 a.UbicacionNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 a.Mensaje.Contains(_busqueda, StringComparison.OrdinalIgnoreCase)) &&
                (_fechaDesde == null || a.FechaGeneracion >= _fechaDesde.Value) &&
                (_fechaHasta == null || a.FechaGeneracion <= _fechaHasta.Value));

            _alertasStock.Clear();
            foreach (var a in filtradas.OrderByDescending(a => a.FechaGeneracion))
                _alertasStock.Add(a);

            OnPropertyChanged(nameof(TotalAlertasStock));
            OnPropertyChanged(nameof(HayAlertas));
            OnPropertyChanged(nameof(NoHayAlertas));
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar alertas de stock: {ex.Message}";
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

            var alerta = _alertasStock.FirstOrDefault(a => a.IdAlerta == idAlerta);
            if (alerta != null)
            {
                _alertasStock.Remove(alerta);
                OnPropertyChanged(nameof(TotalAlertasStock));
                OnPropertyChanged(nameof(HayAlertas));
                OnPropertyChanged(nameof(NoHayAlertas));
            }

            MensajeExito = "Alerta de stock marcada como atendida.";
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al marcar alerta de stock como atendida: {ex.Message}";
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
