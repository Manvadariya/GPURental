using System;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public enum LedgerEntryType
    {
        TopUp,
        Charge, 
        Refund,
        Payout
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

        public string? RentalJobId { get; set; }
        public RentalJob RentalJob { get; set; }

        public LedgerEntryType Type { get; set; }
        public decimal AmountInINR { get; set; }
        public LedgerEntryStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}