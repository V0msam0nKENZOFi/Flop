using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuthApp.Models;

namespace AuthApp
{
    public partial class EditUserWindow : Window
    {
        private User _originalUser;
        private User _currentUser;

        public EditUserWindow(User userToEdit, User currentUser)
        {
            InitializeComponent();
            _originalUser = userToEdit;
            _currentUser = currentUser;

            LoginBox.Text = userToEdit.Login;
            LoadRoles();

            if (userToEdit.Login == currentUser.Login)
                LoginBox.IsEnabled = false;
        }

        private void LoadRoles()
        {
            RoleCombo.Items.Clear();
            if (_currentUser.Role == "Creator")
            {
                RoleCombo.Items.Add(new ComboBoxItem { Content = "User", Tag = "User" });
                RoleCombo.Items.Add(new ComboBoxItem { Content = "Admin", Tag = "Admin" });
            }
            else if (_currentUser.Role == "Admin" && _originalUser.Role == "User")
            {
                RoleCombo.Items.Add(new ComboBoxItem { Content = "User", Tag = "User" });
            }

            foreach (ComboBoxItem item in RoleCombo.Items)
            {
                if (item.Tag.ToString() == _originalUser.Role)
                {
                    RoleCombo.SelectedItem = item;
                    break;
                }
            }
            if (RoleCombo.SelectedItem == null && RoleCombo.Items.Count > 0)
                RoleCombo.SelectedIndex = 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string newLogin = LoginBox.Text.Trim();
            string newPassword = PasswordBox.Password;
            string newRole = ((ComboBoxItem)RoleCombo.SelectedItem)?.Tag.ToString() ?? _originalUser.Role;

            if (string.IsNullOrEmpty(newLogin))
            {
                ShowError("Логин не может быть пустым");
                return;
            }

            if (newLogin != _originalUser.Login && UserService.GetAllUsers(_currentUser.Role).Any(u => u.Login == newLogin))
            {
                ShowError("Пользователь с таким логином уже существует");
                return;
            }

            bool success = UserService.UpdateUser(_originalUser.Login, newLogin, newPassword, newRole, _currentUser.Role);
            if (success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                ShowError("Не удалось обновить пользователя. Недостаточно прав или недопустимые изменения.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}