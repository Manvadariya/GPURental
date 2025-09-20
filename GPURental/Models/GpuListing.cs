using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public enum GpuStatus
    {
        Published,
        Paused,
        Draft,
        Disabled
    }

    public class GpuListing
    {
        [Key]
        public int ListingId { get; set; } // Primary Key

        [Required]
        public int ProviderId { get; set; } // Foreign Key
        public User Provider { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string GpuModel { get; set; }

        public int VramInGB { get; set; }
        public int RamInGB { get; set; }
        public int DiskInGB { get; set; }
        public string CpuModel { get; set; }
        public string OperatingSystem { get; set; }
        public string Location { get; set; }
        public int PricePerHourInCents { get; set; }
        public GpuStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public ICollection<ListingImage> ListingImages { get; set; }
        public ICollection<RentalJob> RentalJobs { get; set; }
        public ICollection<ListingTag> ListingTags { get; set; }
    }
}