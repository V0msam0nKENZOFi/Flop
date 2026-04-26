using System.Windows;
using AuthApp.Models;
using AuthApp.Services;

namespace AuthApp
{
    public partial class AddEditAppWindow : Window
    {
        private DownloadableApp _editingApp;
        private string _currentUserRole;

        public AddEditAppWindow(DownloadableApp app, string currentUserRole)
        {
            InitializeComponent();
            _currentUserRole = currentUserRole;
            if (app != null)
            {
                _editingApp = app;
                NameBox.Text = app.Name;
                UrlBox.Text = app.DirectUrl;
                IconUrlBox.Text = app.IconPath;
                Title = "Редактировать программу";
            }
            else
            {
                _editingApp = null;
                Title = "Добавить программу";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();
            string url = UrlBox.Text.Trim();
            string iconUrl = IconUrlBox.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url))
            {
                ErrorText.Text = "Заполните название и URL";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (_editingApp == null)
            {
                var newApp = new DownloadableApp { Name = name, DirectUrl = url, IconPath = iconUrl };
                DownloadService.AddApp(newApp, _currentUserRole);
            }
            else
            {
                _editingApp.Name = name;
                _editingApp.DirectUrl = url;
                _editingApp.IconPath = iconUrl;
                DownloadService.UpdateApp(_editingApp, _currentUserRole);
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}