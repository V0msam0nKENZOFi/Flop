using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuthApp.Models;

namespace AuthApp.Pages
{
    public partial class UserManagementPage : Page
    {
        private User _currentUser;
        private List<User> _users;

        public UserManagementPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadRoles();
            LoadUsers();
        }

        private void LoadRoles()
        {
            RoleCombo.Items.Clear();
            if (_currentUser.Role == "Creator")
            {
                RoleCombo.Items.Add(new ComboBoxItem { Content = "User", Tag = "User" });
                RoleCombo.Items.Add(new ComboBoxItem { Content = "Admin", Tag = "Admin" });
            }
            else if (_currentUser.Role == "Admin")
            {
                RoleCombo.Items.Add(new ComboBoxItem { Content = "User", Tag = "User" });
            }
            RoleCombo.SelectedIndex = 0;
        }

        private void LoadUsers()
        {
            _users = UserService.GetAllUsers(_currentUser.Role);
            UsersListBox.ItemsSource = _users;
        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            string login = NewLoginBox.Text.Trim();
            string password = NewPasswordBox.Password;
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Заполните логин и пароль");
                return;
            }
            string role = ((ComboBoxItem)RoleCombo.SelectedItem).Tag.ToString();
            bool success = UserService.CreateUser(login, password, role, _currentUser.Role);
            if (success)
            {
                NewLoginBox.Text = "";
                NewPasswordBox.Password = "";
                LoadUsers();
                CreateErrorText.Visibility = Visibility.Collapsed;
            }
            else
            {
                ShowError("Не удалось создать пользователя. Логин уже существует или недостаточно прав.");
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string login = button.Tag.ToString();
            var user = _users.FirstOrDefault(u => u.Login == login);
            if (user == null) return;

            var editWindow = new EditUserWindow(user, _currentUser);
            if (editWindow.ShowDialog() == true) LoadUsers();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string login = button.Tag.ToString();
            if (MessageBox.Show($"Удалить пользователя {login}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                bool success = UserService.DeleteUser(login, _currentUser.Role);
                if (success)
                    LoadUsers();
                else
                    MessageBox.Show("Не удалось удалить пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowError(string message)
        {
            CreateErrorText.Text = message;
            CreateErrorText.Visibility = Visibility.Visible;
        }
    }
}