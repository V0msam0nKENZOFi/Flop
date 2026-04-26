using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthApp.Models;

namespace AuthApp
{
    public static class UserService
    {
        private const string UsersFile = "users.json";
        private static List<User> _users = new List<User>();

        static UserService()
        {
            LoadUsers();
        }

        private static void LoadUsers()
        {
            if (File.Exists(UsersFile))
            {
                var json = File.ReadAllText(UsersFile);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
        }

        private static void SaveUsers()
        {
            var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(UsersFile, json);
        }

        public static void EnsureCreatorExists()
        {
            if (!_users.Any(u => u.Role == "Creator"))
            {
                _users.Add(new User
                {
                    Login = "creator",
                    PasswordHash = HashPassword("creator123"),
                    Role = "Creator"
                });
                SaveUsers();
            }
        }

        public static User Authenticate(string login, string password)
        {
            var hash = HashPassword(password);
            return _users.FirstOrDefault(u => u.Login == login && u.PasswordHash == hash);
        }

        public static List<User> GetAllUsers(string currentUserRole)
        {
            if (currentUserRole == "Creator")
                return _users.ToList();
            if (currentUserRole == "Admin")
                return _users.Where(u => u.Role == "User").ToList();
            return new List<User>();
        }

        public static bool CreateUser(string login, string password, string role, string currentUserRole)
        {
            if (currentUserRole != "Admin" && currentUserRole != "Creator")
                return false;
            if (_users.Any(u => u.Login == login))
                return false;
            if (currentUserRole == "Admin" && role != "User")
                return false;
            if (currentUserRole == "Creator" && role == "Creator")
                return false;

            _users.Add(new User
            {
                Login = login,
                PasswordHash = HashPassword(password),
                Role = role
            });
            SaveUsers();
            return true;
        }

        public static bool UpdateUser(string oldLogin, string newLogin, string newPassword, string newRole, string currentUserRole)
        {
            var user = _users.FirstOrDefault(u => u.Login == oldLogin);
            if (user == null) return false;
            if (user.Role == "Creator" && currentUserRole != "Creator") return false;
            if (currentUserRole == "Admin" && user.Role != "User") return false;
            if (currentUserRole == "Admin" && newRole != "User") return false;

            if (oldLogin != newLogin && _users.Any(u => u.Login == newLogin))
                return false;

            user.Login = newLogin;
            if (!string.IsNullOrEmpty(newPassword))
                user.PasswordHash = HashPassword(newPassword);
            if (currentUserRole == "Creator")
                user.Role = newRole;
            else if (currentUserRole == "Admin" && user.Role == "User")
                user.Role = newRole;
            SaveUsers();
            return true;
        }

        public static bool DeleteUser(string login, string currentUserRole)
        {
            var user = _users.FirstOrDefault(u => u.Login == login);
            if (user == null) return false;
            if (user.Role == "Creator") return false;
            if (currentUserRole == "Creator" || (currentUserRole == "Admin" && user.Role == "User"))
            {
                _users.Remove(user);
                SaveUsers();
                return true;
            }
            return false;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}