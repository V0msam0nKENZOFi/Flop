using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using AuthApp.Models;
using AuthApp.Services;

namespace AuthApp.Pages
{
    public partial class DownloadsPage : Page, INotifyPropertyChanged
    {
        private User _currentUser;
        private List<DownloadableApp> _allApps;
        private List<DownloadableApp> _filteredApps;

        public bool IsCreator => _currentUser.Role == "Creator";

        public List<DownloadableApp> FilteredApps
        {
            get => _filteredApps;
            set
            {
                _filteredApps = value;
                OnPropertyChanged(nameof(FilteredApps));
            }
        }

        public DownloadsPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            DataContext = this;
            LoadApps();

            if (IsCreator)
                AdminPanel.Visibility = Visibility.Visible;
        }

        private void LoadApps()
        {
            _allApps = DownloadService.GetApps();
            FilteredApps = _allApps.ToList();
            AppsListBox.ItemsSource = FilteredApps;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text?.Trim().ToLower() ?? "";
            if (string.IsNullOrEmpty(searchText))
                FilteredApps = _allApps.ToList();
            else
                FilteredApps = _allApps.Where(a => a.Name.ToLower().Contains(searchText)).ToList();
            AppsListBox.ItemsSource = FilteredApps;
        }

        private async void DownloadApp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int id = (int)button.Tag;
            var app = _allApps.FirstOrDefault(a => a.Id == id);
            if (app == null) return;

            string fileName = Path.GetFileName(new Uri(app.DirectUrl).LocalPath);
            if (string.IsNullOrEmpty(fileName)) fileName = app.Name + ".file";

            var progressWindow = new Window
            {
                Title = "Загрузка",
                Width = 300,
                Height = 100,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Thickness(10),
                    Children =
                    {
                        new TextBlock { Text = $"Скачивание {app.Name}...", Margin = new Thickness(0,0,0,10) },
                        new ProgressBar { Name = "ProgressBar", Height = 20, Minimum = 0, Maximum = 100 }
                    }
                }
            };
            progressWindow.Owner = Window.GetWindow(this);
            var progressBar = (ProgressBar)((StackPanel)progressWindow.Content).Children[1];
            var progress = new Progress<int>(p => progressBar.Value = p);
            progressWindow.Show();

            bool success = await DownloadService.DownloadFileAsync(app.DirectUrl, fileName, progress);
            progressWindow.Close();

            if (success)
            {
                MessageBox.Show($"Файл {fileName} сохранён в папку:\n{DownloadService.GetDownloadFolder()}", "Загрузка завершена", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddApp_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditAppWindow(null, _currentUser.Role);
            if (dialog.ShowDialog() == true) LoadApps();
        }

        private void EditApp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int id = (int)button.Tag;
            var app = _allApps.FirstOrDefault(a => a.Id == id);
            if (app == null) return;
            var dialog = new AddEditAppWindow(app, _currentUser.Role);
            if (dialog.ShowDialog() == true) LoadApps();
        }

        private void DeleteApp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int id = (int)button.Tag;
            if (MessageBox.Show("Удалить приложение из списка?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DownloadService.DeleteApp(id, _currentUser.Role);
                LoadApps();
            }
        }

        private void RefreshApps_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            LoadApps();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Конвертер для иконок
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string url && !string.IsNullOrEmpty(url))
            {
                try { return new BitmapImage(new Uri(url)); }
                catch { return null; }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Конвертер для проверки, что строка не пуста
    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value as string;
            return !string.IsNullOrEmpty(str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}