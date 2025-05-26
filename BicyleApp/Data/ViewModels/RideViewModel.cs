using System.ComponentModel.DataAnnotations;

namespace BicyleApp.Data.ViewModels
{
    public class RideViewModel
    {
        public int? RideId { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Range(0.1, 1000, ErrorMessage = "Enter a value between 0.1 and 1000 km.")]
        public double DistanceKm { get; set; }

        [Required]
        public int CompetitionId { get; set; }
    }

}
