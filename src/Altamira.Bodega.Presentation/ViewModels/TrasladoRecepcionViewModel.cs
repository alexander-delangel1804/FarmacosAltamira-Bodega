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

public partial class TrasladoRecepcionViewModel : ObservableObject
{
    private readonly ITrasladoAppService _trasladoAppService;
    private readonly ISesionAppService _sesionService;

    private ObservableCollection<TrasladoDto> _trasladosARecibir = new();
    private TrasladoDto? _trasladoSeleccionado;
    private bool _estaCargando;
    private string? _mensajeError;
    private string _busqueda = string.Empty;

    public ObservableCollection<TrasladoDto> TrasladosARecibir => _trasladosARecibir;

    public TrasladoDto? TrasladoSeleccionado
    {
        get => _trasladoSeleccionado;
        set => SetProperty(ref _trasladoSeleccionado, value);
    }

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

    public int TotalARecibir => _trasladosARecibir.Count;
    public bool HayTraslados => _trasladosARecibir.Count > 0;

    public TrasladoRecepcionViewModel(ITrasladoAppService trasladoAppService, ISesionAppService sesionService)
    {
        _trasladoAppService = trasladoAppService;
        _sesionService = sesionService;
        _ = CargarTrasladosARecibirAsync();
    }

    [RelayCommand]
    public async Task CargarTrasladosARecibirAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            var traslados = await _trasladoAppService.ListarTrasladosAsync("EN_TRANSITO", null, _sesionService.IdUbicacion, CancellationToken.None);

            var filtrados = string.IsNullOrWhiteSpace(_busqueda)
                ? traslados
                : traslados.Where(t =>
                    t.Folio.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                    t.OrigenNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                    t.DestinoNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase));

            _trasladosARecibir.Clear();
            foreach (var t in filtrados.OrderByDescending(t => t.FechaSolicitud))
                _trasladosARecibir.Add(t);
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar traslados para recibir: {ex.Message}";
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    public async Task RecibirTrasladoAsync(int idTraslado)
    {
        MensajeError = null;
        MensajeExito = string.Empty;

        if (idTraslado <= 0)
        {
            MensajeError = "ID de traslado inválido.";
            return;
        }

        // Nota: El backend gestiona las cantidades recibidas por lote internamente.
        // El cliente no inventa un procedimiento de recepción parcial.
        try
        {
            var dto = new TrasladoRecibirDto(new List<DetalleRecibirDto>());

            var resultado = await _trasladoAppService.RecibirTrasladoAsync(idTraslado, dto, _sesionService.IdUsuario ?? 0, CancellationToken.None);

            MensajeExito = $"Traslado {resultado.Folio} recibido correctamente. Estado: {resultado.Estado}";
            MensajeError = null;

            _trasladosARecibir.Remove(_trasladosARecibir.FirstOrDefault(t => t.IdTraslado == idTraslado)!);
            TrasladoSeleccionado = null;
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al recibir traslado: {ex.Message}";
        }
    }

    [RelayCommand]
    public void LimpiarBusqueda()
    {
        _busqueda = string.Empty;
        OnPropertyChanged(nameof(Busqueda));
        _ = CargarTrasladosARecibirAsync();
    }
}
