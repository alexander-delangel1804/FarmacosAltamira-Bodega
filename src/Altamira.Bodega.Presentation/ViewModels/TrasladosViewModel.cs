using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class TrasladosViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _estaCargando;

    [ObservableProperty]
    private string? _mensajeError;

    public TrasladosViewModel()
    {
        _ = Task.CompletedTask;
    }
}
