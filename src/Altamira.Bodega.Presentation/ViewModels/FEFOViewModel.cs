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

public enum LoteElegibilidad
{
    Elegible,
    NoElegible,
    Caducado
}

public class LoteFEFOInfo
{
    public int IdLote { get; set; }
    public string NumeroLote { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public DateOnly FechaCaducidad { get; set; }
    public int DiasHastaCaducidad { get; set; }
    public int CantidadDisponible { get; set; }
    public int IdUbicacion { get; set; }
    public string UbicacionNombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public LoteElegibilidad Elegibilidad { get; set; }
    public decimal PrioridadScore { get; set; } // Menor = mayor prioridad (más cercano a caducar)
}

public partial class FEFOViewModel : ObservableObject
{
    private readonly IInventarioAppService _inventarioService;
    private readonly ISesionAppService _sesionService;

    private ObservableCollection<LoteFEFOInfo> _lotesFEFO = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _busqueda = string.Empty;
    private string _filtroEstado = string.Empty;
    private int? _filtroUbicacion;
    private DateOnly? _fechaCaducidadDesde;
    private DateOnly? _fechaCaducidadHasta;
    private bool _soloElegibles = false;

    public ObservableCollection<LoteFEFOInfo> LotesFEFO => _lotesFEFO;

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

    public int? FiltroUbicacion
    {
        get => _filtroUbicacion;
        set => SetProperty(ref _filtroUbicacion, value);
    }

    public DateOnly? FechaCaducidadDesde
    {
        get => _fechaCaducidadDesde;
        set => SetProperty(ref _fechaCaducidadDesde, value);
    }

    public DateOnly? FechaCaducidadHasta
    {
        get => _fechaCaducidadHasta;
        set => SetProperty(ref _fechaCaducidadHasta, value);
    }

    public bool SoloElegibles
    {
        get => _soloElegibles;
        set => SetProperty(ref _soloElegibles, value);
    }

    public int TotalLotes => _lotesFEFO.Count;
    public int LotesElegibles => _lotesFEFO.Count(l => l.Elegibilidad == LoteElegibilidad.Elegible);
    public int LotesCaducados => _lotesFEFO.Count(l => l.Elegibilidad == LoteElegibilidad.Caducado);
    public int LotesNoElegibles => _lotesFEFO.Count(l => l.Elegibilidad == LoteElegibilidad.NoElegible);
    public bool HayLotes => TotalLotes > 0;
    public bool NoHayLotes => !HayLotes;

    public FEFOViewModel(IInventarioAppService inventarioService, ISesionAppService sesionService)
    {
        _inventarioService = inventarioService;
        _sesionService = sesionService;
        _filtroUbicacion = _sesionService.IdUbicacion;

        _ = CargarLotesFEFOAsync();
    }

    private LoteElegibilidad CalcularElegibilidad(ExistenciaDto e)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var dias = e.FechaCaducidad.DayNumber - hoy.DayNumber;

        if (e.FechaCaducidad < hoy)
            return LoteElegibilidad.Caducado;

        if (e.Disponible <= 0)
            return LoteElegibilidad.NoElegible;

        if (dias <= 30)
            return LoteElegibilidad.Elegible;

        return LoteElegibilidad.NoElegible;
    }

    private decimal CalcularPrioridadScore(ExistenciaDto e)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var dias = e.FechaCaducidad.DayNumber - hoy.DayNumber;

        // Menor score = mayor prioridad (más cercano a caducar)
        if (dias < 0) return 1000; // Caducado - máxima prioridad para revisión
        if (dias <= 7) return dias + 1; // Muy cercano
        if (dias <= 30) return dias + 10; // Cercano
        if (dias <= 90) return dias + 100; // Próximo
        return dias + 1000; // Lejano - baja prioridad
    }

    [RelayCommand]
    public async Task CargarLotesFEFOAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            var existencias = await _inventarioService.ConsultarExistenciasAsync(null, _filtroUbicacion, CancellationToken.None);

            var lotesInfo = existencias.Select(e =>
            {
                var elegibilidad = CalcularElegibilidad(e);
                var hoy = DateOnly.FromDateTime(DateTime.Now);
                var dias = e.FechaCaducidad.DayNumber - hoy.DayNumber;

                return new LoteFEFOInfo
                {
                    IdLote = e.IdLote,
                    NumeroLote = e.NumeroLote,
                    ProductoNombre = e.ProductoNombre,
                    CodigoBarras = e.CodigoBarras,
                    FechaCaducidad = e.FechaCaducidad,
                    DiasHastaCaducidad = dias,
                    CantidadDisponible = e.Disponible,
                    IdUbicacion = e.IdUbicacion,
                    UbicacionNombre = e.UbicacionNombre,
                    Estado = e.Disponible <= 0 ? "AGOTADO" : (dias < 0 ? "CADUCADO" : (dias <= 30 ? "PROXIMO_A_CADUCAR" : "VIGENTE")),
                    Elegibilidad = elegibilidad,
                    PrioridadScore = CalcularPrioridadScore(e)
                };
            });

            // Aplicar filtros
            var filtrados = lotesInfo.Where(l =>
                (string.IsNullOrWhiteSpace(_busqueda) ||
                 l.ProductoNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 l.NumeroLote.Contains(_busqueda, StringComparison.OrdinalIgnoreCase) ||
                 l.UbicacionNombre.Contains(_busqueda, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(_filtroEstado) ||
                 l.Estado.Equals(_filtroEstado, StringComparison.OrdinalIgnoreCase)) &&
                (_fechaCaducidadDesde == null || l.FechaCaducidad >= _fechaCaducidadDesde.Value) &&
                (_fechaCaducidadHasta == null || l.FechaCaducidad <= _fechaCaducidadHasta.Value));

            // Filtro solo elegibles
            if (_soloElegibles)
                filtrados = filtrados.Where(l => l.Elegibilidad == LoteElegibilidad.Elegible);

            // Ordenar por prioridad FEFO (menor score = mayor prioridad)
            var ordenados = filtrados.OrderBy(l => l.PrioridadScore).ThenBy(l => l.FechaCaducidad);

            _lotesFEFO.Clear();
            foreach (var l in ordenados)
                _lotesFEFO.Add(l);

            OnPropertyChanged(nameof(TotalLotes));
            OnPropertyChanged(nameof(LotesElegibles));
            OnPropertyChanged(nameof(LotesCaducados));
            OnPropertyChanged(nameof(LotesNoElegibles));
            OnPropertyChanged(nameof(HayLotes));
            OnPropertyChanged(nameof(NoHayLotes));
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar control FEFO: {ex.Message}";
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
        _filtroUbicacion = _sesionService.IdUbicacion;
        _fechaCaducidadDesde = null;
        _fechaCaducidadHasta = null;
        _soloElegibles = false;

        OnPropertyChanged(nameof(Busqueda));
        OnPropertyChanged(nameof(FiltroEstado));
        OnPropertyChanged(nameof(FiltroUbicacion));
        OnPropertyChanged(nameof(FechaCaducidadDesde));
        OnPropertyChanged(nameof(FechaCaducidadHasta));
        OnPropertyChanged(nameof(SoloElegibles));

        _ = CargarLotesFEFOAsync();
    }
}
