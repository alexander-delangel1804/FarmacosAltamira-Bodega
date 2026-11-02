using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Altamira.Bodega.Presentation.ViewModels;

public partial class UsuariosViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _estaCargando;

    [ObservableProperty]
    private string? _mensajeError;

    public UsuariosViewModel()
    {
        _ = Task.CompletedTask;
    }
}
