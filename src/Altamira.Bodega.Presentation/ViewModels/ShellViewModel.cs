using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Interfaces;
using Altamira.Bodega.Presentation.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly ISesionAppService _sesionService;

    [ObservableProperty]
    private string _tituloPaginaActual = "Dashboard";

    [ObservableProperty]
    private string _usuarioSesion = "Empleado de Bodega";

    [ObservableProperty]
    private string _ubicacionSesion = "Bodega Central";

    [ObservableProperty]
    private string _fechaHoraActual;

    [ObservableProperty]
    private bool _puedeAccederAdministracion;

    public ShellViewModel(INavigationService navigationService, ISesionAppService sesionService)
    {
        _navigationService = navigationService;
        _sesionService = sesionService;

        PuedeAccederAdministracion = _sesionService.PuedeAccederAMinistro("ADMINISTRACION", "USUARIOS");

        FechaHoraActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        timer.Tick += (s, e) => FechaHoraActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        timer.Start();

        if (_sesionService.EstaAutenticado)
        {
            NombreUsuario = _sesionService.NombreUsuario ?? "Usuario";
            UbicacionSesion = _sesionService.UbicacionNombre ?? "Sin ubicación";
        }
    }

    [ObservableProperty]
    private string _nombreUsuario = "Usuario";

    [RelayCommand]
    public void NavigateToDashboard()
    {
        TituloPaginaActual = "Dashboard";
        _navigationService.NavigateTo<DashboardViewModel>();
    }

    [RelayCommand]
    public void NavigateToExistencias()
    {
        TituloPaginaActual = "Existencias por Ubicación";
        _navigationService.NavigateTo<ExistenciasViewModel>();
    }

    [RelayCommand]
    public void NavigateToCatalogo()
    {
        TituloPaginaActual = "Catálogo de Productos";
        _navigationService.NavigateTo<ProductoViewModel>();
    }

    [RelayCommand]
    public void NavigateToLotes()
    {
        TituloPaginaActual = "Lotes y Caducidades";
        _navigationService.NavigateTo<LoteInfoViewModel>();
    }

    [RelayCommand]
    public void NavigateToKardex()
    {
        TituloPaginaActual = "Kárdex y Movimientos";
        _navigationService.NavigateTo<KardexViewModel>();
    }

    [RelayCommand]
    public void NavigateToAjustes()
    {
        TituloPaginaActual = "Ajustes de Inventario";
        _navigationService.NavigateTo<AjustesViewModel>();
    }

    [RelayCommand]
    public void NavigateToRecepcion()
    {
        TituloPaginaActual = "Recepción de Mercancía";
        _navigationService.NavigateTo<RecepcionViewModel>();
    }

    [RelayCommand]
    public void NavigateToRecepcionHistorial()
    {
        TituloPaginaActual = "Historial de Recepciones";
        _navigationService.NavigateTo<RecepcionHistorialViewModel>();
    }

    [RelayCommand]
    public void NavigateToEntradas()
    {
        TituloPaginaActual = "Entradas de Inventario";
        _navigationService.NavigateTo<EntradasViewModel>();
    }

    [RelayCommand]
    public void NavigateToSalidas()
    {
        TituloPaginaActual = "Salidas de Inventario";
        _navigationService.NavigateTo<SalidasViewModel>();
    }

    [RelayCommand]
    public void NavigateToTraslados()
    {
        TituloPaginaActual = "Crear Traslado";
        _navigationService.NavigateTo<TrasladoCrearViewModel>();
    }

    [RelayCommand]
    public void NavigateToTrasladoDespacho()
    {
        TituloPaginaActual = "Despacho de Traslados";
        _navigationService.NavigateTo<TrasladoDespachoViewModel>();
    }

    [RelayCommand]
    public void NavigateToTrasladoRecepcion()
    {
        TituloPaginaActual = "Recepción de Traslados";
        _navigationService.NavigateTo<TrasladoRecepcionViewModel>();
    }

    [RelayCommand]
    public void NavigateToTrasladoHistorial()
    {
        TituloPaginaActual = "Historial de Traslados";
        _navigationService.NavigateTo<TrasladoHistorialViewModel>();
    }

    [RelayCommand]
    public void NavigateToAlertasCaducidad()
    {
        TituloPaginaActual = "Alertas de Caducidad";
        _navigationService.NavigateTo<AlertasCaducidadViewModel>();
    }

    [RelayCommand]
    public void NavigateToAlertasStock()
    {
        TituloPaginaActual = "Alertas de Stock Mínimo";
        _navigationService.NavigateTo<AlertasStockViewModel>();
    }

    [RelayCommand]
    public void NavigateToFEFO()
    {
        TituloPaginaActual = "Control FEFO";
        _navigationService.NavigateTo<FEFOViewModel>();
    }

    [RelayCommand]
    public void NavigateToBitacora()
    {
        TituloPaginaActual = "Bitácora de Operaciones";
        _navigationService.NavigateTo<BitacoraViewModel>();
    }

    [RelayCommand]
    public void NavigateToUsuarios()
    {
        TituloPaginaActual = "Usuarios y Permisos";
        _navigationService.NavigateTo<UsuariosViewModel>();
    }

    [RelayCommand]
    private void CerrarSesion()
    {
        _sesionService.LimpiarSesion();
        var result = MessageBox.Show("Sesión cerrada correctamente.", "Sesión", MessageBoxButton.OK, MessageBoxImage.Information);
if (result == MessageBoxResult.OK)
            {
                System.Windows.Application.Current?.Shutdown();
            }
    }
}
