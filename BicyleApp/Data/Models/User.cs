using System.ComponentModel.DataAnnotations;
using static BicyleApp.Data.Enums.Enum;

namespace BicyleApp.Data.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(10)]
        public string PasswordHash { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public bool IsAdmin { get; set; } = false;
        public UserRole Role { get; set; } = UserRole.User;


        public ICollection<Ride> Rides { get; set; }
    }
}
