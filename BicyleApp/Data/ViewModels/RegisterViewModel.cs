using System.ComponentModel.DataAnnotations;
using static BicyleApp.Data.Enums.Enum;

namespace BicyleApp.Data.ViewModels
{
    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(10)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

    }
}
