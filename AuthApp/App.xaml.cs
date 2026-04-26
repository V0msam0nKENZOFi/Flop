using System.Windows;

namespace AuthApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UserService.EnsureCreatorExists();
        }
    }
}