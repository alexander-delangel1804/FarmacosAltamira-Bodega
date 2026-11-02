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

public partial class DashboardViewModel : ObservableObject
{
    private readonly IInventarioAppService _inventarioService;
    private readonly IAlertaAppService _alertaService;
    private readonly ITrasladoAppService _trasladoService;
    private readonly ISesionAppService _sesionService;

    [ObservableProperty]
    private int _totalProductos;

    [ObservableProperty]
    private int _totalUnidadesBodega;

    [ObservableProperty]
    private int _productosProximosCaducar;

    [ObservableProperty]
    private int _productosCaducados;

    [ObservableProperty]
    private int _productosBajoMinimo;

    [ObservableProperty]
    private int _recepcionesRecientes;

    [ObservableProperty]
    private int _trasladosPendientes;

    [ObservableProperty]
    private int _movimientosRecientes;

    [ObservableProperty]
    private int _alertasPendientes;

    [ObservableProperty]
    private bool _estaCargando;

    [ObservableProperty]
    private string? _mensajeError;

    [ObservableProperty]
    private ObservableCollection<AlertaDto> _alertasRecientes = new();

    public DashboardViewModel(
        IInventarioAppService inventarioService,
        IAlertaAppService alertaService,
        ITrasladoAppService trasladoService,
        ISesionAppService sesionService)
    {
        _inventarioService = inventarioService;
        _alertaService = alertaService;
        _trasladoService = trasladoService;
        _sesionService = sesionService;

        _ = CargarDatosInicialesAsync();
    }

    public async Task CargarDatosInicialesAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            // Cargar existencias para calcular indicadores
            var existencias = await _inventarioService.ConsultarExistenciasAsync(null, _sesionService.IdUbicacion, CancellationToken.None);

            TotalProductos = existencias.Select(e => e.IdProducto).Distinct().Count();
            TotalUnidadesBodega = (int)existencias.Sum(e => e.Cantidad);
            ProductosBajoMinimo = existencias.Count(e => e.Disponible <= 0);

            // Cargar lotes para alertas de caducidad
            var lotes = await _inventarioService.ListarLotesAsync(null, CancellationToken.None);

            ProductosProximosCaducar = lotes.Count(l =>
            {
                var hoy = DateOnly.FromDateTime(DateTime.Now);
                var dias = l.FechaCaducidad.DayNumber - hoy.DayNumber;
                return dias >= 0 && dias <= 30;
            });

            ProductosCaducados = lotes.Count(l =>
            {
                var hoy = DateOnly.FromDateTime(DateTime.Now);
                return l.FechaCaducidad < hoy;
            });

            // Cargar alertas pendientes
            var alertas = await _alertaService.ListarAlertasAsync(_sesionService.IdUbicacion, true, CancellationToken.None);
            AlertasPendientes = alertas.Count;

            AlertasRecientes.Clear();
            foreach (var a in alertas.OrderByDescending(a => a.FechaGeneracion).Take(10))
                AlertasRecientes.Add(a);

            // Cargar traslados pendientes
            var traslados = await _trasladoService.ListarTrasladosAsync("SOLICITADO", _sesionService.IdUbicacion, null, CancellationToken.None);
            TrasladosPendientes = traslados.Count;

            // Movimientos recientes (últimos 30 días)
            var desde = DateTimeOffset.Now.AddDays(-30);
            var movimientos = await _inventarioService.ConsultarMovimientosAsync(null, _sesionService.IdUbicacion, desde, DateTimeOffset.Now, CancellationToken.None);
            MovimientosRecientes = movimientos.Count;

            // Recepciones recientes (últimos 30 días)
            var recepciones = await _inventarioService.ListarRecepcionesAsync(desde, DateTimeOffset.Now, CancellationToken.None);
            RecepcionesRecientes = recepciones.Count;
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar el dashboard: {ex.Message}";
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    private async Task RefrescarAsync()
    {
        await CargarDatosInicialesAsync();
    }
}
