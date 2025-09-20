using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; } // Primary Key

        [Required]
        public int RentalJobId { get; set; } // Foreign Key
        public RentalJob RentalJob { get; set; }

        [Required]
        public int ListingId { get; set; } // Foreign Key
        public GpuListing GpuListing { get; set; }

        [Required]
        public int AuthorId { get; set; } // Foreign Key
        public User Author { get; set; }

        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}