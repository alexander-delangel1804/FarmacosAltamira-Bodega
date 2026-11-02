using Altamira.Bodega.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _nombreUsuario = string.Empty;

    [ObservableProperty]
    private string _contrasena = string.Empty;

    [ObservableProperty]
    private int _idEstacion = 1;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isLoading;

    public LoginViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task IniciarSesionAsync()
    {
        ErrorMessage = null;
        IsLoading = true;

        try
        {
            var request = new Domain.Dtos.LoginRequest(NombreUsuario, Contrasena, IdEstacion, null);
            var response = await _authService.LoginAsync(request, System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName ?? "localhost", CancellationToken.None);

            // TokenService se inyectaría aquí para guardar la sesión
            _navigationService.NavigateTo<DashboardViewModel>();
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
}
