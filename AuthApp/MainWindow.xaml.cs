using System.Windows;
using System.Windows.Input;
using AuthApp.Models;
using AuthApp.Pages;

namespace AuthApp
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            Title = $"Главная - {_currentUser.Login} ({_currentUser.Role})";

            if (_currentUser.Role == "Admin" || _currentUser.Role == "Creator")
                UserManagementItem.Visibility = Visibility.Visible;

            MenuListBox.SelectionChanged += MenuListBox_SelectionChanged;
            LoadPage("Home");

            Overlay.MouseLeftButtonDown += (s, e) => CloseMenu();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (SideMenu.Visibility == Visibility.Visible)
                CloseMenu();
            else
                OpenMenu();
        }

        private void OpenMenu()
        {
            SideMenu.Visibility = Visibility.Visible;
            Overlay.Visibility = Visibility.Visible;
        }

        private void CloseMenu()
        {
            SideMenu.Visibility = Visibility.Collapsed;
            Overlay.Visibility = Visibility.Collapsed;
        }

        private void MenuListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MenuListBox.SelectedItem is System.Windows.Controls.ListBoxItem item && item.Tag is string tag)
            {
                CloseMenu();
                switch (tag)
                {
                    case "Home": LoadPage("Home"); break;
                    case "Settings": LoadPage("Settings"); break;
                    case "UserManagement": LoadPage("UserManagement"); break;
                    case "Downloads": LoadPage("Downloads"); break;
                    case "Logout": Logout(); break;
                }
                MenuListBox.SelectedItem = null;
            }
        }

        private void LoadPage(string pageName)
        {
            switch (pageName)
            {
                case "Home": ContentFrame.Navigate(new HomePage(_currentUser)); break;
                case "Settings": ContentFrame.Navigate(new SettingsPage(_currentUser)); break;
                case "UserManagement": ContentFrame.Navigate(new UserManagementPage(_currentUser)); break;
                case "Downloads": ContentFrame.Navigate(new DownloadsPage(_currentUser)); break;
            }
        }

        private void Logout()
        {
            new LoginWindow().Show();
            Close();
        }
    }
}