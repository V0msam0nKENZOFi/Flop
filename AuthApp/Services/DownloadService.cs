using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using AuthApp.Models;

namespace AuthApp.Services
{
    public static class DownloadService
    {
        private const string AppsFile = "downloadableApps.json";
        private static List<DownloadableApp> _apps = new List<DownloadableApp>();
        private static string _downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        static DownloadService()
        {
            LoadApps();
            LoadSettings();
        }

        private static void LoadApps()
        {
            if (File.Exists(AppsFile))
            {
                var json = File.ReadAllText(AppsFile);
                _apps = JsonSerializer.Deserialize<List<DownloadableApp>>(json) ?? new List<DownloadableApp>();
            }
            else
            {
                _apps.Add(new DownloadableApp { Id = 1, Name = "Пример приложения", DirectUrl = "https://example.com/sample.zip" });
                SaveApps();
            }
        }

        private static void SaveApps()
        {
            var json = JsonSerializer.Serialize(_apps, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(AppsFile, json);
        }

        private static void LoadSettings()
        {
            var settingsFile = "downloadSettings.json";
            if (File.Exists(settingsFile))
            {
                var json = File.ReadAllText(settingsFile);
                var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (settings != null && settings.ContainsKey("DownloadFolder"))
                    _downloadFolder = settings["DownloadFolder"];
            }
        }

        private static void SaveSettings()
        {
            var settings = new Dictionary<string, string> { { "DownloadFolder", _downloadFolder } };
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("downloadSettings.json", json);
        }

        public static List<DownloadableApp> GetApps() => _apps.ToList();

        public static void AddApp(DownloadableApp app, string currentUserRole)
        {
            if (currentUserRole != "Admin" && currentUserRole != "Creator") return;
            app.Id = _apps.Count > 0 ? _apps.Max(a => a.Id) + 1 : 1;
            _apps.Add(app);
            SaveApps();
        }

        public static void UpdateApp(DownloadableApp app, string currentUserRole)
        {
            if (currentUserRole != "Admin" && currentUserRole != "Creator") return;
            var existing = _apps.FirstOrDefault(a => a.Id == app.Id);
            if (existing != null)
            {
                existing.Name = app.Name;
                existing.DirectUrl = app.DirectUrl;
                existing.IconPath = app.IconPath;
                SaveApps();
            }
        }

        public static void DeleteApp(int id, string currentUserRole)
        {
            if (currentUserRole != "Admin" && currentUserRole != "Creator") return;
            var app = _apps.FirstOrDefault(a => a.Id == id);
            if (app != null)
            {
                _apps.Remove(app);
                SaveApps();
            }
        }

        public static async Task<bool> DownloadFileAsync(string url, string fileName, IProgress<int> progress = null)
        {
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                var savePath = Path.Combine(_downloadFolder, fileName);

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                var buffer = new byte[8192];
                long totalRead = 0;
                int bytesRead;
                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    totalRead += bytesRead;
                    if (totalBytes > 0 && progress != null)
                    {
                        var percent = (int)((double)totalRead / totalBytes * 100);
                        progress.Report(percent);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static string GetDownloadFolder() => _downloadFolder;

        public static void SetDownloadFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                _downloadFolder = folder;
                SaveSettings();
            }
        }
    }
}