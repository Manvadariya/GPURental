using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public enum LedgerEntryType
    {
        TopUp,  // When a user adds their own money
        Charge, // When a renter is charged for a job
        Refund, // When an admin issues a refund
        Payout  // When a provider is paid for a completed job
    }

    public enum LedgerEntryStatus
    {
        Completed,
        Pending
    }

    public class WalletLedgerEntry
    {
        [Key]
        public string LedgerId { get; set; } // Primary Key

        [Required]
        public string UserId { get; set; } // Foreign Key
        public User User { get; set; }

        public string? RentalJobId { get; set; } // Optional Foreign Key
        public RentalJob RentalJob { get; set; }

        public LedgerEntryType Type { get; set; }
        public int AmountInCents { get; set; }
        public LedgerEntryStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}