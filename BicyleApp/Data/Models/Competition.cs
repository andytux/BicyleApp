using System.ComponentModel.DataAnnotations;
using static BicyleApp.Data.Enums.Enum;

namespace BicyleApp.Data.Models
{
    public class Competition
    {
        [Key]
        public int CompetitionId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public CompetitionStatus Status { get; set; }
    }

}
