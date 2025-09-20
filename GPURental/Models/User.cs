using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // <-- ADD THIS

namespace GPURental.Models
{
    public class User
    {
        [Key] // <-- ADD THIS
        public int UserId { get; set; }
        //... rest of the code is the same
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public int BalanceInCents { get; set; }
        public string Timezone { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<GpuListing> GpuListings { get; set; }
        public ICollection<RentalJob> RentalJobs { get; set; }
        public ICollection<WalletLedgerEntry> WalletLedgerEntries { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Dispute> Disputes { get; set; }
    }
}