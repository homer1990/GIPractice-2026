using System.Windows.Controls;

namespace GIPractice.Wpf.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void OnPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm && sender is PasswordBox box)
        {
            vm.Password = box.Password;
        }
    }
}
