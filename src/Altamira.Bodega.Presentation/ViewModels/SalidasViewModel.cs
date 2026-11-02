using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class SalidasViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _estaCargando;

    [ObservableProperty]
    private string? _mensajeError;

    public SalidasViewModel()
    {
        _ = Task.CompletedTask;
    }
}
