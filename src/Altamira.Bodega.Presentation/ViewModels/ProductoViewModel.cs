using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class ProductoViewModel : ObservableObject
{
    private readonly IProductoAppService _productoService;
    private readonly ISesionAppService _sesionService;

    [ObservableProperty]
    private ObservableCollection<ProductoDto> _productos = new();

    [ObservableProperty]
    private bool _estaCargando;
    [ObservableProperty]
    private string? _mensajeError;

    [ObservableProperty]
    private string _busqueda = string.Empty;
    [ObservableProperty]
    private bool _soloActivos = true;

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

    [ObservableProperty]
    private ProductoDto? _productoSeleccionado;

    // Formulario
    [ObservableProperty]
    private bool _mostrarFormulario;
    [ObservableProperty]
    private bool _esModoEdicion;
    [ObservableProperty]
    private int _formIdCategoria = 1;
    [ObservableProperty]
    private string _formCodigoBarras = string.Empty;
    [ObservableProperty]
    private string _formNombreComercial = string.Empty;
    [ObservableProperty]
    private string? _formSustanciaActiva;
    [ObservableProperty]
    private string? _formPresentacion;
    [ObservableProperty]
    private decimal _formPrecioVenta;
    [ObservableProperty]
    private decimal _formTasaIva;
    [ObservableProperty]
    private int _formStockMinimo;
    [ObservableProperty]
    private bool _formEsControlado;

    public ProductoViewModel(IProductoAppService productoService, ISesionAppService sesionService)
    {
        _productoService = productoService;
        _sesionService = sesionService;
        _ = CargarProductosAsync();
    }

    private async Task CargarProductosAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            var lista = await _productoService.ListarProductosAsync(_busqueda, _soloActivos, CancellationToken.None);
            Productos.Clear();
            foreach (var p in lista)
                Productos.Add(p);

            TotalRegistros = Productos.Count;
            TotalPaginas = (int)System.Math.Ceiling((double)TotalRegistros / Math.Max(1, _tamanoPagina));
            TieneSiguiente = _paginaActual < TotalPaginas;
            TieneAnterior = _paginaActual > 1;
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar productos: {ex.Message}";
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        _paginaActual = 1;
        await CargarProductosAsync();
    }

    [RelayCommand]
    private async Task RefrescarAsync()
    {
        await CargarProductosAsync();
    }

    [RelayCommand]
    private void LimpiarFiltros()
    {
        Busqueda = string.Empty;
        SoloActivos = true;
        _paginaActual = 1;
        _ = CargarProductosAsync();
    }

    [RelayCommand]
    private void CambiarPagina(int delta)
    {
        var nuevaPagina = _paginaActual + delta;
        if (nuevaPagina < 1 || nuevaPagina > TotalPaginas)
            return;
        _paginaActual = nuevaPagina;
        _ = CargarProductosAsync();
    }

    [RelayCommand]
    private void AbrirCreacion()
    {
        _esModoEdicion = false;
        _formIdCategoria = 1;
        _formCodigoBarras = string.Empty;
        _formNombreComercial = string.Empty;
        _formSustanciaActiva = null;
        _formPresentacion = null;
        _formPrecioVenta = 0;
        _formTasaIva = 0;
        _formStockMinimo = 0;
        _formEsControlado = false;
        _mostrarFormulario = true;
    }

    internal void EditarProducto(ProductoDto producto)
    {
        _esModoEdicion = true;
        _formIdCategoria = producto.IdCategoria;
        _formCodigoBarras = producto.CodigoBarras;
        _formNombreComercial = producto.NombreComercial;
        _formSustanciaActiva = producto.SustanciaActiva;
        _formPresentacion = producto.Presentacion;
        _formPrecioVenta = producto.PrecioVenta;
        _formTasaIva = producto.TasaIva;
        _formStockMinimo = producto.StockMinimo;
        _formEsControlado = producto.EsControlado;
        _productoSeleccionado = producto;
        _mostrarFormulario = true;
    }

    [RelayCommand]
    private async Task GuardarProductoAsync()
    {
        if (string.IsNullOrWhiteSpace(_formCodigoBarras) || string.IsNullOrWhiteSpace(_formNombreComercial))
        {
            MensajeError = "El código de barras y nombre comercial son obligatorios.";
            return;
        }

        try
        {
            if (_esModoEdicion && _productoSeleccionado != null)
            {
                var dto = new ProductoUpdateDto(
                    _formIdCategoria,
                    _formNombreComercial,
                    _formSustanciaActiva,
                    _formPresentacion,
                    _formPrecioVenta,
                    _formTasaIva,
                    _formStockMinimo,
                    _formEsControlado
                );

                var actualizado = await _productoService.ActualizarProductoAsync(_productoSeleccionado.IdProducto, dto, _sesionService.IdUsuario ?? 0, CancellationToken.None);
                var idx = Productos.IndexOf(_productoSeleccionado);
                if (idx >= 0)
                    Productos[idx] = actualizado;

                _productoSeleccionado = null;
            }
            else
            {
                var dto = new ProductoCreateDto(
                    _formIdCategoria,
                    _formCodigoBarras,
                    _formNombreComercial,
                    _formSustanciaActiva,
                    _formPresentacion,
                    _formPrecioVenta,
                    _formTasaIva,
                    _formStockMinimo,
                    _formEsControlado
                );

                var creado = await _productoService.CrearProductoAsync(dto, _sesionService.IdUsuario ?? 0, CancellationToken.None);
                Productos.Insert(0, creado);
                TotalRegistros++;
            }

            _mostrarFormulario = false;
            MensajeError = null;
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al guardar producto: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DesactivarProducto(ProductoDto producto)
    {
        var resultado = MessageBox.Show(
            $"¿Está seguro de desactivar el producto '{producto.NombreComercial}'?",
            "Desactivar producto",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
            return;

        try
        {
            await _productoService.DesactivarProductoAsync(producto.IdProducto, _sesionService.IdUsuario ?? 0, CancellationToken.None);
            Productos.Remove(producto);
            TotalRegistros--;
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al desactivar producto: {ex.Message}";
        }
    }

    [RelayCommand]
    private void CancelarFormulario()
    {
        _mostrarFormulario = false;
        _productoSeleccionado = null;
        MensajeError = null;
    }
}
