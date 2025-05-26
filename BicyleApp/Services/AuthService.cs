using BicyleApp.Data.Models;
using System.Security.Cryptography;
using System.Text;

namespace BicyleApp.Services
{
    public class AuthService
    {
        private readonly UserService userService;

        public AuthService(UserService userService)
        {
            this.userService = userService;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await userService.GetByEmailAsync(email);
            if (user == null) return null;

            var hashed = HashPassword(password);
            return user.PasswordHash == hashed ? user : null;
        }

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
