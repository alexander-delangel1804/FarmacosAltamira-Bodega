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

public partial class TrasladoCrearViewModel : ObservableObject
{
    private readonly IInventarioAppService _inventarioService;
    private readonly ITrasladoAppService _trasladoService;
    private readonly ISesionAppService _sesionService;

    private ObservableCollection<TrasladoDto> _traslados = new();
    private ObservableCollection<DetalleTrasladoCreateDto> _detalles = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _busquedaProducto = string.Empty;

    private int _idOrigen;
    private int _idDestino;
    private string _observaciones = string.Empty;
    private int _idProductoSeleccionado;
    private string _nombreProductoSeleccionado = string.Empty;
    private int _idLoteSeleccionado;
    private string _numeroLoteSeleccionado = string.Empty;
    private int _existenciaDisponible;
    private int _cantidadEnviar = 1;

    public ObservableCollection<TrasladoDto> Traslados => _traslados;
    public ObservableCollection<DetalleTrasladoCreateDto> Detalles => _detalles;

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

    public int IdOrigen
    {
        get => _idOrigen;
        set => SetProperty(ref _idOrigen, value);
    }

    public int IdDestino
    {
        get => _idDestino;
        set => SetProperty(ref _idDestino, value);
    }

    public string Observaciones
    {
        get => _observaciones;
        set => SetProperty(ref _observaciones, value);
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

    public int ExistenciaDisponible
    {
        get => _existenciaDisponible;
        set => SetProperty(ref _existenciaDisponible, value);
    }

    public int CantidadEnviar
    {
        get => _cantidadEnviar;
        set
        {
            SetProperty(ref _cantidadEnviar, value);
            OnPropertyChanged(nameof(PuedeAgregarDetalle));
        }
    }

    public int TotalDetalles => _detalles.Count;
    public int TotalCantidad => _detalles.Sum(d => d.CantidadEnviada);
    public bool PuedeAgregarDetalle => _cantidadEnviar > 0 && _idProductoSeleccionado > 0 && _idLoteSeleccionado > 0;

    public bool HayDetalles => _detalles.Count > 0;

    public TrasladoCrearViewModel(IInventarioAppService inventarioService, ITrasladoAppService trasladoService, ISesionAppService sesionService)
    {
        _inventarioService = inventarioService;
        _trasladoService = trasladoService;
        _sesionService = sesionService;
        _idOrigen = _sesionService.IdUbicacion ?? 0;

        _ = CargarProductosAsync();
    }

    [RelayCommand]
    public async Task CargarProductosAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            // Usamos la API de productos para obtener el catálogo
            // Nota: IProductoAppService no está inyectado, usamos la API de inventario para obtener lotes por producto
            // Esta es una simplificación; en producción se inyectaría IProductoAppService
            await Task.CompletedTask;
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
        if (_idProductoSeleccionado <= 0 || _idOrigen <= 0)
        {
            _detalles.Clear();
            ExistenciaDisponible = 0;
            OnPropertyChanged(nameof(ExistenciaDisponible));
            return;
        }

        EstaCargando = true;
        MensajeError = null;

        try
        {
            var lotes = await _inventarioService.ListarLotesAsync(_idProductoSeleccionado, CancellationToken.None);

            // Filtrar lotes que tengan existencia en la ubicación de origen
            var lotesConExistencia = lotes.Where(l => !string.IsNullOrEmpty(l.NumeroLote)).ToList();

            // Cargar existencia disponible del producto en la ubicación origen
            var existencias = await _inventarioService.ConsultarExistenciasAsync(_idProductoSeleccionado, _idOrigen, CancellationToken.None);
            var existenciaLote = existencias.FirstOrDefault(e => e.IdLote == _idLoteSeleccionado);
            ExistenciaDisponible = existenciaLote?.Disponible ?? 0;
            OnPropertyChanged(nameof(ExistenciaDisponible));
            OnPropertyChanged(nameof(PuedeAgregarDetalle));
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

    [RelayCommand]
    public async Task CargarExistenciaDelLoteAsync()
    {
        if (_idProductoSeleccionado <= 0 || _idLoteSeleccionado <= 0 || _idOrigen <= 0)
        {
            ExistenciaDisponible = 0;
            OnPropertyChanged(nameof(ExistenciaDisponible));
            OnPropertyChanged(nameof(PuedeAgregarDetalle));
            return;
        }

        try
        {
            var existencias = await _inventarioService.ConsultarExistenciasAsync(_idProductoSeleccionado, _idOrigen, CancellationToken.None);
            var existenciaLote = existencias.FirstOrDefault(e => e.IdLote == _idLoteSeleccionado);
            ExistenciaDisponible = existenciaLote?.Disponible ?? 0;
            OnPropertyChanged(nameof(ExistenciaDisponible));
            OnPropertyChanged(nameof(PuedeAgregarDetalle));
        }
        catch
        {
            ExistenciaDisponible = 0;
            OnPropertyChanged(nameof(ExistenciaDisponible));
        }
    }

    [RelayCommand]
    public void AgregarDetalle()
    {
        MensajeError = null;

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

        if (_cantidadEnviar <= 0)
        {
            MensajeError = "La cantidad debe ser mayor a cero.";
            return;
        }

        if (_cantidadEnviar > ExistenciaDisponible)
        {
            MensajeError = $"La cantidad ({_cantidadEnviar}) excede la existencia disponible ({ExistenciaDisponible}).";
            return;
        }

        if (_detalles.Any(d => d.IdLote == _idLoteSeleccionado))
        {
            MensajeError = "Ya existe un detalle para este lote.";
            return;
        }

        _detalles.Add(new DetalleTrasladoCreateDto(_idLoteSeleccionado, _cantidadEnviar));

        _idProductoSeleccionado = 0;
        _nombreProductoSeleccionado = string.Empty;
        _idLoteSeleccionado = 0;
        _numeroLoteSeleccionado = string.Empty;
        _existenciaDisponible = 0;
        _cantidadEnviar = 1;

        OnPropertyChanged(nameof(IdProductoSeleccionado));
        OnPropertyChanged(nameof(NombreProductoSeleccionado));
        OnPropertyChanged(nameof(IdLoteSeleccionado));
        OnPropertyChanged(nameof(NumeroLoteSeleccionado));
        OnPropertyChanged(nameof(ExistenciaDisponible));
        OnPropertyChanged(nameof(CantidadEnviar));
        OnPropertyChanged(nameof(TotalDetalles));
        OnPropertyChanged(nameof(TotalCantidad));
        OnPropertyChanged(nameof(HayDetalles));
        OnPropertyChanged(nameof(PuedeAgregarDetalle));
    }

    [RelayCommand]
    public void QuitarDetalle(DetalleTrasladoCreateDto detalle)
    {
        if (detalle != null && _detalles.Contains(detalle))
        {
            _detalles.Remove(detalle);
            OnPropertyChanged(nameof(TotalDetalles));
            OnPropertyChanged(nameof(TotalCantidad));
            OnPropertyChanged(nameof(HayDetalles));
        }
    }

    [RelayCommand]
    public async Task SolicitarTrasladoAsync()
    {
        MensajeError = null;
        MensajeExito = string.Empty;

        if (_idDestino <= 0)
        {
            MensajeError = "Debe seleccionar una ubicación de destino.";
            return;
        }

        if (_idDestino == _idOrigen)
        {
            MensajeError = "El origen y destino no pueden ser la misma ubicación.";
            return;
        }

        if (_detalles.Count == 0)
        {
            MensajeError = "Debe agregar al menos un detalle al traslado.";
            return;
        }

        try
        {
            var dto = new TrasladoCreateDto(
                _idOrigen,
                _idDestino,
                _observaciones ?? string.Empty,
                _detalles.ToList());

            var resultado = await _trasladoService.SolicitarTrasladoAsync(dto, _sesionService.IdUsuario ?? 0, CancellationToken.None);

            MensajeExito = $"Traslado solicitado correctamente. Folio: {resultado.Folio}";
            MensajeError = null;

            _detalles.Clear();
            _idDestino = 0;
            _observaciones = string.Empty;
            _idProductoSeleccionado = 0;
            _nombreProductoSeleccionado = string.Empty;
            _idLoteSeleccionado = 0;
            _numeroLoteSeleccionado = string.Empty;
            _existenciaDisponible = 0;
            _cantidadEnviar = 1;

            OnPropertyChanged(nameof(IdDestino));
            OnPropertyChanged(nameof(Observaciones));
            OnPropertyChanged(nameof(IdProductoSeleccionado));
            OnPropertyChanged(nameof(NombreProductoSeleccionado));
            OnPropertyChanged(nameof(IdLoteSeleccionado));
            OnPropertyChanged(nameof(NumeroLoteSeleccionado));
            OnPropertyChanged(nameof(ExistenciaDisponible));
            OnPropertyChanged(nameof(CantidadEnviar));
            OnPropertyChanged(nameof(TotalDetalles));
            OnPropertyChanged(nameof(TotalCantidad));
            OnPropertyChanged(nameof(HayDetalles));
            OnPropertyChanged(nameof(PuedeAgregarDetalle));
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al solicitar traslado: {ex.Message}";
        }
    }

    [RelayCommand]
    public void LimpiarTodo()
    {
        _detalles.Clear();
        _idDestino = 0;
        _observaciones = string.Empty;
        _idProductoSeleccionado = 0;
        _nombreProductoSeleccionado = string.Empty;
        _idLoteSeleccionado = 0;
        _numeroLoteSeleccionado = string.Empty;
        _existenciaDisponible = 0;
        _cantidadEnviar = 1;
        MensajeError = null;
        MensajeExito = string.Empty;

        OnPropertyChanged(nameof(IdDestino));
        OnPropertyChanged(nameof(Observaciones));
        OnPropertyChanged(nameof(IdProductoSeleccionado));
        OnPropertyChanged(nameof(NombreProductoSeleccionado));
        OnPropertyChanged(nameof(IdLoteSeleccionado));
        OnPropertyChanged(nameof(NumeroLoteSeleccionado));
        OnPropertyChanged(nameof(ExistenciaDisponible));
        OnPropertyChanged(nameof(CantidadEnviar));
        OnPropertyChanged(nameof(TotalDetalles));
        OnPropertyChanged(nameof(TotalCantidad));
        OnPropertyChanged(nameof(HayDetalles));
        OnPropertyChanged(nameof(PuedeAgregarDetalle));
    }

    public void SeleccionarProducto(int idProducto, string nombreProducto)
    {
        _idProductoSeleccionado = idProducto;
        _nombreProductoSeleccionado = nombreProducto;
        _idLoteSeleccionado = 0;
        _numeroLoteSeleccionado = string.Empty;
        _existenciaDisponible = 0;

        OnPropertyChanged(nameof(IdProductoSeleccionado));
        OnPropertyChanged(nameof(NombreProductoSeleccionado));
        OnPropertyChanged(nameof(IdLoteSeleccionado));
        OnPropertyChanged(nameof(NumeroLoteSeleccionado));
        OnPropertyChanged(nameof(ExistenciaDisponible));
        OnPropertyChanged(nameof(PuedeAgregarDetalle));
    }

    public void SeleccionarLote(int idLote, string numeroLote)
    {
        _idLoteSeleccionado = idLote;
        _numeroLoteSeleccionado = numeroLote;

        OnPropertyChanged(nameof(IdLoteSeleccionado));
        OnPropertyChanged(nameof(NumeroLoteSeleccionado));

        _ = CargarExistenciaDelLoteAsync();
    }
}
