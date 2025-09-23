using System.ComponentModel.DataAnnotations;

namespace GPURental.ViewModels
{
    public class SubmitReviewViewModel
    {
        [Required]
        public string RentalJobId { get; set; } // <-- int to string

        [Required]
        public string ListingId { get; set; } // <-- int to string

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }
    }
}