using System.ComponentModel.DataAnnotations;

namespace GPURental.ViewModels
{
    public class SubmitReviewViewModel
    {
        [Required]
        public int RentalJobId { get; set; }

        [Required]
        public int ListingId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }
    }
}