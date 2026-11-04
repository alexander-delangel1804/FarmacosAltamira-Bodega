using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Domain.Exceptions;
using Altamira.Bodega.Domain.Interfaces;
using Altamira.Bodega.Presentation.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthAppService _authService;
    private readonly INavigationService _navigationService;
    private readonly ISesionAppService _sesionService;

    [ObservableProperty]
    private string _nombreUsuario = string.Empty;

    [ObservableProperty]
    private string _contrasena = string.Empty;

    [ObservableProperty]
    private int _idEstacion = 1;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _mensajeExito;

    [ObservableProperty]
    private bool _isLoading;

    public LoginViewModel(IAuthAppService authService, INavigationService navigationService, ISesionAppService sesionService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _sesionService = sesionService;
    }

    [RelayCommand]
    public async Task IniciarSesionAsync()
    {
        ErrorMessage = null;
        MensajeExito = null;
        IsLoading = true;

        try
        {
            if (string.IsNullOrWhiteSpace(NombreUsuario))
            {
                ErrorMessage = "El nombre de usuario es obligatorio.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Contrasena))
            {
                ErrorMessage = "La contraseña es obligatoria.";
                return;
            }

            var request = new Domain.Dtos.LoginRequest(NombreUsuario, Contrasena, IdEstacion, null);
            var response = await _authService.LoginAsync(request, System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName ?? "localhost", CancellationToken.None);

            if (response?.AccessToken != null)
            {
                _sesionService.GuardarSesion(response);

                MensajeExito = $"Bienvenido, {response.NombreCompleto}";

                _navigationService.NavigateTo<DashboardViewModel>();
            }
            else
            {
                ErrorMessage = "Credenciales inválidas o estación no autorizada.";
            }
        }
        catch (SesionExpiradaException ex)
        {
            ErrorMessage = "Credenciales inválidas o estación no autorizada.";
        }
        catch (SinPermisosException ex)
        {
            ErrorMessage = "No tiene permisos para acceder al sistema.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void Limpiar()
    {
        NombreUsuario = string.Empty;
        Contrasena = string.Empty;
        ErrorMessage = null;
        MensajeExito = null;
        IsLoading = false;
    }
}
