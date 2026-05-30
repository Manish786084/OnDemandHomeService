using System.ComponentModel.DataAnnotations;

namespace OnDemandHomeService.ViewModels
{
    public class ReviewsVM
    {
        public int BookingId { get; set; }

        public int ProviderId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 to 5")]
        public int? Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public string? CustomerName { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
