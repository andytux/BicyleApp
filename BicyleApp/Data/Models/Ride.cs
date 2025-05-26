using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BicyleApp.Data.Models
{
    public class Ride
    {
        [Key]
        public int RideId { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public int CompetitionId { get; set; }
        [ForeignKey("CompetitionId")]
        public Competition Competition { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public double DistanceKm { get; set; }
    }

}
