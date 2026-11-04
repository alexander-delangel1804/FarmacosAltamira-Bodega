using System.Windows.Controls;
using Altamira.Bodega.Presentation.ViewModels;

namespace Altamira.Bodega.Presentation.Views;

public partial class LoginView : System.Windows.Controls.UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                // Notificar cambio de contraseña al ViewModel
                // Esto requiere un binding personalizado o comportamiento adjunto
                // Por simplicidad, se asume que el binding funciona correctamente
            }
        }
    }
}
