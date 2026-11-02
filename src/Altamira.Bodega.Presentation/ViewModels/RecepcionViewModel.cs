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

public partial class RecepcionViewModel : ObservableObject
{
    private readonly IInventarioAppService _inventarioService;
    private readonly ISesionAppService _sesionService;

    private ObservableCollection<ExistenciaDto> _existencias = new();
    private ObservableCollection<DetalleRecepcionCreateDto> _partidas = new();
    private bool _estaCargando;
    private string? _mensajeError;
    private string _busqueda = string.Empty;

    private int _idUbicacion;
    private string _documentoReferencia = string.Empty;
    private string _observaciones = string.Empty;

    private int _idProductoSeleccionado;
    private string _nombreProductoSeleccionado = string.Empty;
    private string _numeroLoteNuevo = string.Empty;
    private DateTime? _fechaCaducidad = DateTime.Now.AddDays(30);
    private DateTime? _fechaFabricacion;
    private int _cantidad = 1;
    private bool _loteCaducadoPermitido;
    private string _fechaHoraActual;

    public string FechaHoraActual
    {
        get => _fechaHoraActual;
        set => SetProperty(ref _fechaHoraActual, value);
    }

    public ObservableCollection<ExistenciaDto> Existencias => _existencias;
    public ObservableCollection<DetalleRecepcionCreateDto> Partidas => _partidas;

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

    private string _mensajeExito = string.Empty;

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
    public bool HayPartidas => _partidas.Count > 0;
    public bool NoHayPartidas => !HayPartidas;

    public string Busqueda
    {
        get => _busqueda;
        set => SetProperty(ref _busqueda, value);
    }

    public int IdUbicacion
    {
        get => _idUbicacion;
        set => SetProperty(ref _idUbicacion, value);
    }

    public string DocumentoReferencia
    {
        get => _documentoReferencia;
        set => SetProperty(ref _documentoReferencia, value);
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

    public string NumeroLoteNuevo
    {
        get => _numeroLoteNuevo;
        set => SetProperty(ref _numeroLoteNuevo, value);
    }

    public DateTime? FechaCaducidad
    {
        get => _fechaCaducidad;
        set => SetProperty(ref _fechaCaducidad, value);
    }

    public DateTime? FechaFabricacion
    {
        get => _fechaFabricacion;
        set => SetProperty(ref _fechaFabricacion, value);
    }

    public int Cantidad
    {
        get => _cantidad;
        set => SetProperty(ref _cantidad, value);
    }

    public bool LoteCaducadoPermitido
    {
        get => _loteCaducadoPermitido;
        set => SetProperty(ref _loteCaducadoPermitido, value);
    }

    public int TotalPartidas => _partidas.Count;
    public int TotalCantidad => _partidas.Sum(p => p.Cantidad);

    public RecepcionViewModel(IInventarioAppService inventarioService, ISesionAppService sesionService)
    {
        _inventarioService = inventarioService;
        _sesionService = sesionService;
        _idUbicacion = _sesionService.IdUbicacion ?? 0;

        FechaHoraActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        timer.Tick += (s, e) => FechaHoraActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        timer.Start();

        _ = CargarExistenciasAsync();
    }

    private async Task CargarExistenciasAsync()
    {
        EstaCargando = true;
        MensajeError = null;

        try
        {
            var lista = await _inventarioService.ConsultarExistenciasAsync(null, _idUbicacion, CancellationToken.None);
            Existencias.Clear();
            foreach (var e in lista)
                Existencias.Add(e);
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

    [RelayCommand]
    public async Task RefrescarAsync()
    {
        await CargarExistenciasAsync();
    }

    [RelayCommand]
    public void AgregarPartida()
    {
        MensajeError = null;

        if (string.IsNullOrWhiteSpace(_nombreProductoSeleccionado) || _idProductoSeleccionado <= 0)
        {
            MensajeError = "Debe seleccionar un producto válido.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_numeroLoteNuevo))
        {
            MensajeError = "El número de lote es obligatorio.";
            return;
        }

        if (_cantidad <= 0)
        {
            MensajeError = "La cantidad debe ser mayor a cero.";
            return;
        }

        var fechaCaducidad = FechaCaducidad?.Date ?? DateTime.Now.AddDays(30);
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        if (DateOnly.FromDateTime(fechaCaducidad) < hoy && !_loteCaducadoPermitido)
        {
            MensajeError = "No se pueden registrar lotes caducados. Si cuenta con autorización especial, marque 'Permitir lote caducado'.";
            return;
        }

        if (FechaFabricacion.HasValue && FechaFabricacion.Value.Date > DateTime.Now.Date)
        {
            MensajeError = "La fecha de fabricación no puede ser futura.";
            return;
        }

        if (_partidas.Any(p => p.IdProducto == _idProductoSeleccionado && p.NumeroLote == _numeroLoteNuevo))
        {
            MensajeError = "Ya existe una partida con este lote para este producto.";
            return;
        }

        _partidas.Add(new DetalleRecepcionCreateDto(
            _idProductoSeleccionado,
            _numeroLoteNuevo,
            DateOnly.FromDateTime(fechaCaducidad),
            _cantidad));

        _idProductoSeleccionado = 0;
        _nombreProductoSeleccionado = string.Empty;
        _numeroLoteNuevo = string.Empty;
        _fechaCaducidad = DateTime.Now.AddDays(30);
        _fechaFabricacion = null;
        _cantidad = 1;
        _loteCaducadoPermitido = false;

        OnPropertyChanged(nameof(TotalPartidas));
        OnPropertyChanged(nameof(TotalCantidad));
        OnPropertyChanged(nameof(HayPartidas));
        OnPropertyChanged(nameof(NoHayPartidas));
    }

    [RelayCommand]
    public void QuitarPartida(DetalleRecepcionCreateDto partida)
    {
        if (partida != null && _partidas.Contains(partida))
        {
            _partidas.Remove(partida);
            OnPropertyChanged(nameof(TotalPartidas));
            OnPropertyChanged(nameof(TotalCantidad));
            OnPropertyChanged(nameof(HayPartidas));
            OnPropertyChanged(nameof(NoHayPartidas));
        }
    }

    [RelayCommand]
    public async Task RegistrarRecepcionAsync()
    {
        MensajeError = null;
        MensajeExito = string.Empty;

        if (_idUbicacion <= 0)
        {
            MensajeError = "Debe seleccionar una ubicación válida.";
            return;
        }

        if (_partidas.Count == 0)
        {
            MensajeError = "Debe agregar al menos una partida.";
            return;
        }

        var hoy = DateOnly.FromDateTime(DateTime.Now);
        foreach (var p in _partidas)
        {
            if (p.FechaCaducidad < hoy && !_loteCaducadoPermitido)
            {
                MensajeError = $"El lote {p.NumeroLote} está caducado. No se puede registrar sin autorización especial.";
                return;
            }
        }

        try
        {
            var dto = new RecepcionCreateDto(
                _idUbicacion,
                _documentoReferencia ?? string.Empty,
                _observaciones ?? string.Empty,
                _partidas.ToList());

            var resultado = await _inventarioService.RegistrarRecepcionAsync(dto, _sesionService.IdUsuario ?? 0, CancellationToken.None);

            MensajeExito = $"Recepción registrada correctamente. Folio: {resultado.IdRecepcion}";
            MensajeError = null;

            _partidas.Clear();
            _documentoReferencia = string.Empty;
            _observaciones = string.Empty;
            _idProductoSeleccionado = 0;
            _nombreProductoSeleccionado = string.Empty;
            _numeroLoteNuevo = string.Empty;
            _fechaCaducidad = DateTime.Now.AddDays(30);
            _fechaFabricacion = null;
            _cantidad = 1;
            _loteCaducadoPermitido = false;

            OnPropertyChanged(nameof(TotalPartidas));
            OnPropertyChanged(nameof(TotalCantidad));
            OnPropertyChanged(nameof(HayPartidas));
            OnPropertyChanged(nameof(NoHayPartidas));

            await CargarExistenciasAsync();
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al registrar recepción: {ex.Message}";
        }
    }

    [RelayCommand]
    public void LimpiarTodo()
    {
_partidas.Clear();
            _documentoReferencia = string.Empty;
            _observaciones = string.Empty;
            _idProductoSeleccionado = 0;
            _nombreProductoSeleccionado = string.Empty;
            _numeroLoteNuevo = string.Empty;
            _fechaCaducidad = DateTime.Now.AddDays(30);
            _fechaFabricacion = null;
            _cantidad = 1;
            _loteCaducadoPermitido = false;
            MensajeError = null;
            MensajeExito = string.Empty;

            OnPropertyChanged(nameof(TotalPartidas));
            OnPropertyChanged(nameof(TotalCantidad));
            OnPropertyChanged(nameof(HayPartidas));
            OnPropertyChanged(nameof(NoHayPartidas));
        }

        public void SeleccionarProductoDesdeBusqueda(int idProducto, string nombreProducto)
    {
        _idProductoSeleccionado = idProducto;
        _nombreProductoSeleccionado = nombreProducto;
    }
}
