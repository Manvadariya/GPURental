using System.ComponentModel.DataAnnotations;

namespace GPURental.ViewModels
{
    public class SubmitDisputeViewModel
    {
        [Required]
        public int RentalJobId { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }
    }
}