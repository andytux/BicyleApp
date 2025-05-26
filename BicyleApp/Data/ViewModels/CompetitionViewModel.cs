using System.ComponentModel.DataAnnotations;
using static BicyleApp.Data.Enums.Enum;

namespace BicyleApp.Data.ViewModels
{
    public class CompetitionViewModel
    {
        public int CompetitionId { get; set; } = 0;

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public CompetitionStatus Status { get; set; } = CompetitionStatus.Active;
    }

}
