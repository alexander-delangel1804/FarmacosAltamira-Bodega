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

public partial class TrasladoDespachoViewModel : ObservableObject
{
    private readonly ITrasladoAppService _trasladoAppService;
    private readonly ISesionAppService _sesionService;

    private ObservableCollection<TrasladoDto> _trasladosPendientes = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _busqueda = string.Empty;

    public ObservableCollection<TrasladoDto> TrasladosPendientes => _trasladosPendientes;

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

    public int TotalPendientes => _trasladosPendientes.Count;

    public TrasladoDespachoViewModel(ITrasladoAppService trasladoAppService, ISesionAppService sesionService)
    {
        _trasladoAppService = trasladoAppService;
        _sesionService = sesionService;
        _ = CargarTrasladosPendientesAsync();
    }

    [RelayCommand]
    public async Task CargarTrasladosPendientesAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            var traslados = await _trasladoAppService.ListarTrasladosAsync("SOLICITADO", _sesionService.IdUbicacion, null, CancellationToken.None);

            var filtrados = string.IsNullOrWhiteSpace(_busqueda)
                ? traslados
                : traslados.Where(t =>
                    t.Folio.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                    t.OrigenNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                    t.DestinoNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                    t.UsuarioSolicitaNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase));

            _trasladosPendientes.Clear();
            foreach (var t in filtrados.OrderByDescending(t => t.FechaSolicitud))
                _trasladosPendientes.Add(t);
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar traslados pendientes: {ex.Message}";
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public async Task DespacharTrasladoAsync(int idTraslado)
    {
        MensajeError = null;
        MensajeExito = string.Empty;

        if (idTraslado <= 0)
        {
            MensajeError = "ID de traslado inválido.";
            return;
        }

        try
        {
            var resultado = await _trasladoAppService.DespacharTrasladoAsync(idTraslado, _sesionService.IdUsuario ?? 0, CancellationToken.None);

            MensajeExito = $"Traslado {resultado.Folio} despachado correctamente. Estado: {resultado.Estado}";
            MensajeError = null;

            _trasladosPendientes.Remove(_trasladosPendientes.FirstOrDefault(t => t.IdTraslado == idTraslado)!);
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al despachar traslado: {ex.Message}";
        }
    }

    [RelayCommand]
    public void LimpiarBusqueda()
    {
        _busqueda = string.Empty;
        OnPropertyChanged(nameof(Busqueda));
        _ = CargarTrasladosPendientesAsync();
    }
}
