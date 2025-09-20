using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public enum LedgerEntryType
    {
        TopUp,
        Charge,
        Refund
    }

    public enum LedgerEntryStatus
    {
        Completed,
        Pending
    }

    public class WalletLedgerEntry
    {
        [Key]
        public int LedgerId { get; set; } // Primary Key

        [Required]
        public int UserId { get; set; } // Foreign Key
        public User User { get; set; }

        public int? RentalJobId { get; set; } // Optional Foreign Key
        public RentalJob RentalJob { get; set; }

        public LedgerEntryType Type { get; set; }
        public int AmountInCents { get; set; }
        public LedgerEntryStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}