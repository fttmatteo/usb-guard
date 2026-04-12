using System.Windows;

namespace USBGuard.App.Views;

public partial class PasswordDialog : Window
{
    public string Password { get; private set; } = string.Empty;

    public PasswordDialog(Window owner)
    {
        InitializeComponent();
        Owner = owner;
        PwdBox.Focus();
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        Password = PwdBox.Password;
        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
