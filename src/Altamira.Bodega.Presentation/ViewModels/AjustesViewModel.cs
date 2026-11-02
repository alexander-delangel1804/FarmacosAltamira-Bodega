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

public enum TipoAjuste
{
    Positivo,
    Negativo
}

public partial class AjustesViewModel : ObservableObject
{
    private readonly IInventarioAppService _inventarioService;
    private readonly IProductoAppService _productoService;
    private readonly ISesionAppService _sesionService;

    private ObservableCollection<ProductoDto> _productos = new();
    private ObservableCollection<LoteDto> _lotes = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _busquedaProducto = string.Empty;

    private int _idUbicacion;
    private int _idProductoSeleccionado;
    private string _nombreProductoSeleccionado = string.Empty;
    private int _idLoteSeleccionado;
    private string _numeroLoteSeleccionado = string.Empty;
    private int _existenciaActual;
    private int _cantidadAjuste = 1;
    private TipoAjuste _tipoAjuste = TipoAjuste.Positivo;
    private string _motivo = string.Empty;
    private int _resultadoEsperado;

    private bool _permiteAjustes;

    public ObservableCollection<ProductoDto> Productos => _productos;
    public ObservableCollection<LoteDto> Lotes => _lotes;

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

    public string BusquedaProducto
    {
        get => _busquedaProducto;
        set => SetProperty(ref _busquedaProducto, value);
    }

    public int IdUbicacion
    {
        get => _idUbicacion;
        set => SetProperty(ref _idUbicacion, value);
    }

    public int IdProductoSeleccionado
    {
        get => _idProductoSeleccionado;
        set => SetProperty(ref _idProductoSeleccionado, value);
    }

    public string NombreProductoSeleccionado
    {
        get => _nombreProductoSeleccionado;
        set => SetProperty(ref _nombreProductoSeleccionado, value);
    }

    public int IdLoteSeleccionado
    {
        get => _idLoteSeleccionado;
        set => SetProperty(ref _idLoteSeleccionado, value);
    }

    public string NumeroLoteSeleccionado
    {
        get => _numeroLoteSeleccionado;
        set => SetProperty(ref _numeroLoteSeleccionado, value);
    }

    public int ExistenciaActual
    {
        get => _existenciaActual;
        set => SetProperty(ref _existenciaActual, value);
    }

    public int CantidadAjuste
    {
        get => _cantidadAjuste;
        set
        {
            SetProperty(ref _cantidadAjuste, value);
            CalcularResultado();
        }
    }

    public TipoAjuste TipoAjuste
    {
        get => _tipoAjuste;
        set
        {
            SetProperty(ref _tipoAjuste, value);
            CalcularResultado();
        }
    }

    public string Motivo
    {
        get => _motivo;
        set => SetProperty(ref _motivo, value);
    }

    public int ResultadoEsperado
    {
        get => _resultadoEsperado;
        set => SetProperty(ref _resultadoEsperado, value);
    }

    public bool PermiteAjustes
    {
        get => _permiteAjustes;
        set => SetProperty(ref _permiteAjustes, value);
    }

    public string TipoAjusteTexto => _tipoAjuste == TipoAjuste.Positivo ? "AJUSTE POSITIVO" : "AJUSTE NEGATIVO";
    public string TipoAjusteColor => _tipoAjuste == TipoAjuste.Positivo ? "#2E7D32" : "#C62828";

    public AjustesViewModel(IInventarioAppService inventarioService, IProductoAppService productoService, ISesionAppService sesionService)
    {
        _inventarioService = inventarioService;
        _productoService = productoService;
        _sesionService = sesionService;
        _idUbicacion = _sesionService.IdUbicacion ?? 0;
        PermiteAjustes = _sesionService.PuedeAccederAMinistro("INVENTARIO", "AJUSTES");

        _ = CargarProductosAsync();
    }

    [RelayCommand]
    public async Task CargarProductosAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            var productos = await _productoService.ListarProductosAsync(_busquedaProducto, true, CancellationToken.None);
            _productos.Clear();
            foreach (var p in productos)
                _productos.Add(p);

            OnPropertyChanged(nameof(Productos));
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
    public async Task CargarLotesAsync()
    {
        if (_idProductoSeleccionado <= 0)
        {
            _lotes.Clear();
            OnPropertyChanged(nameof(Lotes));
            ExistenciaActual = 0;
            OnPropertyChanged(nameof(ExistenciaActual));
            return;
        }

        EstaCargando = true;
        MensajeError = null;

        try
        {
            var lotes = await _inventarioService.ListarLotesAsync(_idProductoSeleccionado, CancellationToken.None);
            _lotes.Clear();
            foreach (var l in lotes)
                _lotes.Add(l);

            OnPropertyChanged(nameof(Lotes));

            // Cargar existencia del producto en la ubicación
            await CargarExistenciaAsync();
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar lotes: {ex.Message}";
        }
        finally
        {
            EstaCargando = false;
        }
    }

    private async Task CargarExistenciaAsync()
    {
        if (_idProductoSeleccionado <= 0 || _idUbicacion <= 0)
        {
            ExistenciaActual = 0;
            OnPropertyChanged(nameof(ExistenciaActual));
            return;
        }

        try
        {
            var existencias = await _inventarioService.ConsultarExistenciasAsync(_idProductoSeleccionado, _idUbicacion, CancellationToken.None);
            var existenciaLote = existencias.FirstOrDefault(e => e.IdLote == _idLoteSeleccionado);
            ExistenciaActual = existenciaLote?.Disponible ?? 0;
            OnPropertyChanged(nameof(ExistenciaActual));
            CalcularResultado();
        }
        catch
        {
            ExistenciaActual = 0;
            OnPropertyChanged(nameof(ExistenciaActual));
        }
    }

    [RelayCommand]
    public async Task CargarExistenciaDelLoteAsync()
    {
        await CargarExistenciaAsync();
    }

    private void CalcularResultado()
    {
        if (_tipoAjuste == TipoAjuste.Positivo)
        {
            _resultadoEsperado = _existenciaActual + _cantidadAjuste;
        }
        else
        {
            _resultadoEsperado = _existenciaActual - _cantidadAjuste;
        }
        OnPropertyChanged(nameof(ResultadoEsperado));
        OnPropertyChanged(nameof(TipoAjusteTexto));
        OnPropertyChanged(nameof(TipoAjusteColor));
    }

    [RelayCommand]
    public async Task RegistrarAjusteAsync()
    {
        MensajeError = null;
        MensajeExito = string.Empty;

        if (!_permiteAjustes)
        {
            MensajeError = "No tiene permisos para registrar ajustes de inventario.";
            return;
        }

        if (_idProductoSeleccionado <= 0)
        {
            MensajeError = "Debe seleccionar un producto.";
            return;
        }

        if (_idLoteSeleccionado <= 0)
        {
            MensajeError = "Debe seleccionar un lote.";
            return;
        }

        if (_cantidadAjuste <= 0)
        {
            MensajeError = "La cantidad de ajuste debe ser mayor a cero.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_motivo))
        {
            MensajeError = "El motivo es obligatorio.";
            return;
        }

        // Validar que no genere existencia negativa para ajustes negativos
        if (_tipoAjuste == TipoAjuste.Negativo && _cantidadAjuste > _existenciaActual)
        {
            MensajeError = $"No se puede ajustar a negativo: existencia actual ({_existenciaActual}) es menor que la cantidad solicitada ({_cantidadAjuste}).";
            return;
        }

        // Confirmación implícita: el comando se ejecuta tras validación previa en la vista
        try
        {
            var cantidadAjustada = _tipoAjuste == TipoAjuste.Negativo ? -_cantidadAjuste : _cantidadAjuste;

            var dto = new AjusteCreateDto(
                _idLoteSeleccionado,
                _idUbicacion,
                cantidadAjustada,
                _motivo);

            var resultado = await _inventarioService.RegistrarAjusteAsync(dto, _sesionService.IdUsuario ?? 0, CancellationToken.None);

            MensajeExito = $"Ajuste registrado correctamente. Lote: {resultado.IdLote}, Cantidad ajustada: {Math.Abs(resultado.Cantidad)}.";
            MensajeError = null;

            // Recargar existencia
            await CargarExistenciaAsync();

            // Limpiar formulario
            _cantidadAjuste = 1;
            _motivo = string.Empty;
            _tipoAjuste = TipoAjuste.Positivo;
            _resultadoEsperado = _existenciaActual;

            OnPropertyChanged(nameof(CantidadAjuste));
            OnPropertyChanged(nameof(Motivo));
            OnPropertyChanged(nameof(TipoAjuste));
            OnPropertyChanged(nameof(ResultadoEsperado));
            OnPropertyChanged(nameof(TipoAjusteTexto));
            OnPropertyChanged(nameof(TipoAjusteColor));
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al registrar ajuste: {ex.Message}";
        }
    }

    [RelayCommand]
    public void LimpiarTodo()
    {
        _idProductoSeleccionado = 0;
        _nombreProductoSeleccionado = string.Empty;
        _idLoteSeleccionado = 0;
        _numeroLoteSeleccionado = string.Empty;
        _existenciaActual = 0;
        _cantidadAjuste = 1;
        _tipoAjuste = TipoAjuste.Positivo;
        _motivo = string.Empty;
        _resultadoEsperado = 0;

        OnPropertyChanged(nameof(IdProductoSeleccionado));
        OnPropertyChanged(nameof(NombreProductoSeleccionado));
        OnPropertyChanged(nameof(IdLoteSeleccionado));
        OnPropertyChanged(nameof(NumeroLoteSeleccionado));
        OnPropertyChanged(nameof(ExistenciaActual));
        OnPropertyChanged(nameof(CantidadAjuste));
        OnPropertyChanged(nameof(TipoAjuste));
        OnPropertyChanged(nameof(Motivo));
        OnPropertyChanged(nameof(ResultadoEsperado));
        OnPropertyChanged(nameof(TipoAjusteTexto));
        OnPropertyChanged(nameof(TipoAjusteColor));

        MensajeError = null;
        MensajeExito = string.Empty;
    }

    public void SeleccionarProducto(ProductoDto producto)
    {
        _idProductoSeleccionado = producto.IdProducto;
        _nombreProductoSeleccionado = producto.NombreComercial;
        _idLoteSeleccionado = 0;
        _numeroLoteSeleccionado = string.Empty;
        _existenciaActual = 0;

        OnPropertyChanged(nameof(IdProductoSeleccionado));
        OnPropertyChanged(nameof(NombreProductoSeleccionado));
        OnPropertyChanged(nameof(IdLoteSeleccionado));
        OnPropertyChanged(nameof(NumeroLoteSeleccionado));
        OnPropertyChanged(nameof(ExistenciaActual));

        _ = CargarLotesAsync();
    }

    public void SeleccionarLote(LoteDto lote)
    {
        _idLoteSeleccionado = lote.IdLote;
        _numeroLoteSeleccionado = lote.NumeroLote;

        OnPropertyChanged(nameof(IdLoteSeleccionado));
        OnPropertyChanged(nameof(NumeroLoteSeleccionado));

        _ = CargarExistenciaDelLoteAsync();
    }
}
