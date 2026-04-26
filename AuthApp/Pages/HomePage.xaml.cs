using System.Windows.Controls;
using AuthApp.Models;

namespace AuthApp.Pages
{
    public partial class HomePage : Page
    {
        public HomePage(User currentUser)
        {
            InitializeComponent();
            RoleText.Text = $"Ваша роль: {currentUser.Role}";
        }
    }
}