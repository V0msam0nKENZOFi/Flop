using System.Windows;
using AuthApp.Models;

namespace AuthApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password;

            var user = UserService.Authenticate(login, password);
            if (user != null)
            {
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                Close();
            }
            else
            {
                ErrorText.Text = "Неверный логин или пароль";
                ErrorText.Visibility = Visibility.Visible;
            }
        }
    }
}