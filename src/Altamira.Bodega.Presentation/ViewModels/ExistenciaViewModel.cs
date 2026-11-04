using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class ExistenciaViewModel : ObservableObject
{
    private readonly IInventarioAppService _inventarioService;

    [ObservableProperty]
    private ObservableCollection<ExistenciaDto> _existencias = new();

    [ObservableProperty]
    private bool _estaCargando;
    [ObservableProperty]
    private string? _mensajeError;

    [ObservableProperty]
    private string _busqueda = string.Empty;

    [ObservableProperty]
    private int _paginaActual = 1;
    [ObservableProperty]
    private int _tamanoPagina = 50;
    [ObservableProperty]
    private long _totalRegistros;
    [ObservableProperty]
    private int _totalPaginas;
    [ObservableProperty]
    private bool _tieneSiguiente;
    [ObservableProperty]
    private bool _tieneAnterior;

    // Filtros adicionales (filtrado del lado del cliente sobre los datos de la página actual,
    // o recarga del servidor cuando cambian filtros críticos)
    [ObservableProperty]
    private string _filtroProducto = string.Empty;
    [ObservableProperty]
    private string _filtroLote = string.Empty;
    [ObservableProperty]
    private string _filtroEstado = string.Empty; // "VIGENTE", "PROXIMO_A_CADUCAR", "CADUCADO", "AGOTADO", "BAJO_MINIMO"

    public ExistenciaViewModel(IInventarioAppService inventarioService)
    {
        _inventarioService = inventarioService;
        _ = CargarExistenciasAsync();
    }

    private async Task CargarExistenciasAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            // Consulta a la API con paginación
            var lista = await _inventarioService.ConsultarExistenciasAsync(null, null, CancellationToken.None);

            // Aplicar filtros del lado del cliente sobre los datos obtenidos
            var filtrada = AplicarFiltros(lista);
            Existencias.Clear();
            foreach (var e in filtrada)
                Existencias.Add(e);

            TotalRegistros = Existencias.Count;
            TotalPaginas = (int)System.Math.Ceiling((double)TotalRegistros / Math.Max(1, TamanoPagina));
            TieneSiguiente = PaginaActual < TotalPaginas;
            TieneAnterior = PaginaActual > 1;
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar existencias: {ex.Message}";
        }
        finally
        {
            EstaCargando = false;
        }
    }

    private IEnumerable<ExistenciaDto> AplicarFiltros(IEnumerable<ExistenciaDto> lista)
    {
        var resultado = lista.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(Busqueda))
        {
            var b = Busqueda.ToLowerInvariant();
            resultado = resultado.Where(e =>
                e.ProductoNombre.ToLowerInvariant().Contains(b) ||
                e.CodigoBarras.ToLowerInvariant().Contains(b) ||
                e.NumeroLote.ToLowerInvariant().Contains(b) ||
                e.UbicacionNombre.ToLowerInvariant().Contains(b));
        }

        if (!string.IsNullOrWhiteSpace(FiltroProducto))
        {
            var p = FiltroProducto.ToLowerInvariant();
            resultado = resultado.Where(e => e.ProductoNombre.ToLowerInvariant().Contains(p));
        }

        if (!string.IsNullOrWhiteSpace(FiltroLote))
        {
            var l = FiltroLote.ToLowerInvariant();
            resultado = resultado.Where(e => e.NumeroLote.ToLowerInvariant().Contains(l));
        }

        if (!string.IsNullOrWhiteSpace(FiltroEstado))
        {
            var eEstado = FiltroEstado.ToLowerInvariant();
            resultado = resultado.Where(e =>
            {
                // Determinar estado basado en datos disponibles
                var estadoCalculado = CalcularEstado(e);
                return estadoCalculado.Equals(eEstado, StringComparison.OrdinalIgnoreCase);
            });
        }

        return resultado;
    }

private string CalcularEstado(ExistenciaDto e)
    {
        if (e.Cantidad <= 0)
            return "AGOTADO";

        // Verificar caducidad
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var fechaCaducidad = e.FechaCaducidad;
        var dias = (fechaCaducidad.ToDateTime(TimeOnly.MinValue) - hoy.ToDateTime(TimeOnly.MinValue)).Days;

        if (dias < 0)
            return "CADUCADO";
        if (dias <= 30)
            return "PROXIMO_A_CADUCAR";

        // Verificar stock mínimo (usamos cantidad disponible como referencia)
        if (e.Disponible <= 10) // Umbral genérico, en producción vendría de parámetros
            return "BAJO_MINIMO";

        return "VIGENTE";
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        PaginaActual = 1;
        await CargarExistenciasAsync();
    }

    [RelayCommand]
    public async Task RefrescarAsync()
    {
        await CargarExistenciasAsync();
    }

    [RelayCommand]
    private void LimpiarFiltros()
    {
        Busqueda = string.Empty;
        FiltroProducto = string.Empty;
        FiltroLote = string.Empty;
        FiltroEstado = string.Empty;
        PaginaActual = 1;
        _ = CargarExistenciasAsync();
    }

    [RelayCommand]
    private void CambiarPagina(int delta)
    {
        var nuevaPagina = PaginaActual + delta;
        if (nuevaPagina < 1 || nuevaPagina > TotalPaginas)
            return;
        PaginaActual = nuevaPagina;
        _ = CargarExistenciasAsync();
    }
}
