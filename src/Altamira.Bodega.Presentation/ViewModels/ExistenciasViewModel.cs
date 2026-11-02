using Altamira.Bodega.Application.Interfaces;
using Altamira.Bodega.Domain.Dtos;

namespace Altamira.Bodega.Presentation.ViewModels;

public class ExistenciasViewModel : ExistenciaViewModel
{
    public ExistenciasViewModel(IInventarioAppService inventarioService) : base(inventarioService)
    {
    }
}
