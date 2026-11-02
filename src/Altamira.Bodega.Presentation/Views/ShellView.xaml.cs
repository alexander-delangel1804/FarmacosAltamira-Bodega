using Altamira.Bodega.Domain.Interfaces;
using Altamira.Bodega.Presentation.Services;
using Altamira.Bodega.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Altamira.Bodega.Presentation.Views;

public partial class ShellView : Window
{
    private readonly INavigationService _navigationService;
    private readonly ShellViewModel _viewModel;

    public ShellView(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _navigationService = serviceProvider.GetRequiredService<INavigationService>();
        _viewModel = serviceProvider.GetRequiredService<ShellViewModel>();

        _navigationService.ViewChanged += OnViewChanged;

        DataContext = _viewModel;

        _navigationService.NavigateTo<DashboardViewModel>();
    }

    private void OnViewChanged(object view)
    {
        ContentArea.Content = view as FrameworkElement;
    }
}
