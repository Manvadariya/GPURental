using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public class ListingAvailability
    {
        [Key]
        public int AvailabilityId { get; set; } // Primary Key

        [Required]
        public int ListingId { get; set; } // Foreign Key
        public GpuListing GpuListing { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}