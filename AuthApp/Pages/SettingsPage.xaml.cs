using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AuthApp.Models;
using AuthApp.Services;

namespace AuthApp.Pages
{
    public partial class SettingsPage : Page
    {
        private User _currentUser;

        public SettingsPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            DataContext = this;
            UserInfo.Text = $"Логин: {currentUser.Login}\nРоль: {currentUser.Role}";
            DownloadFolderText.Text = DownloadService.GetDownloadFolder();
        }

        public string DownloadFolder => DownloadService.GetDownloadFolder();

        private void SetDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            var resources = App.Current.Resources;
            resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            resources["SurfaceBrush"] = new SolidColorBrush(Color.FromRgb(45, 45, 45));
            resources["PrimaryBrush"] = new SolidColorBrush(Color.FromRgb(121, 0, 181));
            resources["PrimaryDarkBrush"] = new SolidColorBrush(Color.FromRgb(90, 0, 135));
            resources["TextBrush"] = new SolidColorBrush(Colors.White);
            resources["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(170, 170, 170));
            resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(63, 63, 63));
            resources["OverlayBrush"] = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
        }

        private void SetLightTheme_Click(object sender, RoutedEventArgs e)
        {
            var resources = App.Current.Resources;
            resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            resources["SurfaceBrush"] = new SolidColorBrush(Colors.White);
            resources["PrimaryBrush"] = new SolidColorBrush(Color.FromRgb(121, 0, 181));
            resources["PrimaryDarkBrush"] = new SolidColorBrush(Color.FromRgb(90, 0, 135));
            resources["TextBrush"] = new SolidColorBrush(Colors.Black);
            resources["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(80, 80, 80));
            resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            resources["OverlayBrush"] = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
        }

        private void SelectDownloadFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Выберите папку для сохранения загрузок";
            dialog.SelectedPath = DownloadService.GetDownloadFolder();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DownloadService.SetDownloadFolder(dialog.SelectedPath);
                DownloadFolderText.Text = DownloadService.GetDownloadFolder();
            }
        }
    }
}