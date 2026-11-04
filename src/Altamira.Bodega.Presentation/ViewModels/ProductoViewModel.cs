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
            var lista = await _productoService.ListarProductosAsync(Busqueda, SoloActivos, CancellationToken.None);
            Productos.Clear();
            foreach (var p in lista)
                Productos.Add(p);

            TotalRegistros = Productos.Count;
            TotalPaginas = (int)System.Math.Ceiling((double)TotalRegistros / Math.Max(1, TamanoPagina));
            TieneSiguiente = PaginaActual < TotalPaginas;
            TieneAnterior = PaginaActual > 1;
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
        PaginaActual = 1;
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
        PaginaActual = 1;
        _ = CargarProductosAsync();
    }

    [RelayCommand]
    private void CambiarPagina(int delta)
    {
        var nuevaPagina = PaginaActual + delta;
        if (nuevaPagina < 1 || nuevaPagina > TotalPaginas)
            return;
        PaginaActual = nuevaPagina;
        _ = CargarProductosAsync();
    }

    [RelayCommand]
    private void AbrirCreacion()
    {
        EsModoEdicion = false;
        FormIdCategoria = 1;
        FormCodigoBarras = string.Empty;
        FormNombreComercial = string.Empty;
        FormSustanciaActiva = null;
        FormPresentacion = null;
        FormPrecioVenta = 0;
        FormTasaIva = 0;
        FormStockMinimo = 0;
        FormEsControlado = false;
        MostrarFormulario = true;
    }

    internal void EditarProducto(ProductoDto producto)
    {
        EsModoEdicion = true;
        FormIdCategoria = producto.IdCategoria;
        FormCodigoBarras = producto.CodigoBarras;
        FormNombreComercial = producto.NombreComercial;
        FormSustanciaActiva = producto.SustanciaActiva;
        FormPresentacion = producto.Presentacion;
        FormPrecioVenta = producto.PrecioVenta;
        FormTasaIva = producto.TasaIva;
        FormStockMinimo = producto.StockMinimo;
        FormEsControlado = producto.EsControlado;
        ProductoSeleccionado = producto;
        MostrarFormulario = true;
    }

    [RelayCommand]
    private async Task GuardarProductoAsync()
    {
        if (string.IsNullOrWhiteSpace(FormCodigoBarras) || string.IsNullOrWhiteSpace(FormNombreComercial))
        {
            MensajeError = "El código de barras y nombre comercial son obligatorios.";
            return;
        }

        try
        {
            if (EsModoEdicion && ProductoSeleccionado != null)
            {
                var dto = new ProductoUpdateDto(
                    FormIdCategoria,
                    FormNombreComercial,
                    FormSustanciaActiva,
                    FormPresentacion,
                    FormPrecioVenta,
                    FormTasaIva,
                    FormStockMinimo,
                    FormEsControlado
                );

                var actualizado = await _productoService.ActualizarProductoAsync(ProductoSeleccionado.IdProducto, dto, _sesionService.IdUsuario ?? 0, CancellationToken.None);
                var idx = Productos.IndexOf(ProductoSeleccionado);
                if (idx >= 0)
                    Productos[idx] = actualizado;

                ProductoSeleccionado = null;
            }
            else
            {
                var dto = new ProductoCreateDto(
                    FormIdCategoria,
                    FormCodigoBarras,
                    FormNombreComercial,
                    FormSustanciaActiva,
                    FormPresentacion,
                    FormPrecioVenta,
                    FormTasaIva,
                    FormStockMinimo,
                    FormEsControlado
                );

                var creado = await _productoService.CrearProductoAsync(dto, _sesionService.IdUsuario ?? 0, CancellationToken.None);
                Productos.Insert(0, creado);
                TotalRegistros++;
            }

            MostrarFormulario = false;
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
        MostrarFormulario = false;
        ProductoSeleccionado = null;
        MensajeError = null;
    }
}
