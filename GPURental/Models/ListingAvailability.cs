using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public class ListingAvailability
    {
        [Key]
        public string AvailabilityId { get; set; } // Primary Key

        [Required]
        public string ListingId { get; set; } // Foreign Key
        public GpuListing GpuListing { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}