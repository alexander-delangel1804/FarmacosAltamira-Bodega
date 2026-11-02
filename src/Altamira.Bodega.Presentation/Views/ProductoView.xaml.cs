using Altamira.Bodega.Domain.Dtos;
using Altamira.Bodega.Presentation.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Altamira.Bodega.Presentation.Views;

public partial class ProductoView : Page
{
    public ProductoView()
    {
        InitializeComponent();
    }

    internal void EditarProducto(ProductoDto producto)
    {
        var vm = DataContext as ProductoViewModel;
        vm?.EditarProducto(producto);
    }

    private void Producto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is ProductoDto producto)
        {
            var vm = DataContext as ProductoViewModel;
            vm?.EditarProducto(producto);
        }
    }
}
