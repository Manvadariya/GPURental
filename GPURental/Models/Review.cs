using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public class Review
    {
        [Key]
        public string ReviewId { get; set; } // Primary Key

        [Required]
        public string RentalJobId { get; set; } // Foreign Key
        public RentalJob RentalJob { get; set; }

        [Required]
        public string ListingId { get; set; } // Foreign Key
        public GpuListing GpuListing { get; set; }

        [Required]
        public string AuthorId { get; set; } // Foreign Key
        public User Author { get; set; }

        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}