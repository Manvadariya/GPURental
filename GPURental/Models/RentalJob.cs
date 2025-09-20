using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public enum JobStatus
    {
        Running,
        Completed,
        Failed,
        Cancelled
    }

    public class RentalJob
    {
        [Key]
        public int RentalJobId { get; set; } // Primary Key

        [Required]
        public int ListingId { get; set; } // Foreign Key
        public GpuListing GpuListing { get; set; }

        [Required]
        public int RenterId { get; set; } // Foreign Key
        public User Renter { get; set; }

        public DateTime? ActualStartAt { get; set; }
        public DateTime? ActualEndAt { get; set; }
        public JobStatus Status { get; set; }
        public int? FinalChargeInCents { get; set; }

        // Navigation Properties
        public Review Review { get; set; }
        public Dispute Dispute { get; set; }
        public Invoice Invoice { get; set; }
        public ICollection<WalletLedgerEntry> WalletLedgerEntries { get; set; }
    }
}