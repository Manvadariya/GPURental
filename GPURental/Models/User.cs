using Microsoft.AspNetCore.Identity; // <-- ADD THIS
using System;
using System.Collections.Generic;

namespace GPURental.Models
{
    // Inherit from IdentityUser
    public class User : IdentityUser
    {
        // IdentityUser provides: Id (string), UserName, Email, PasswordHash, etc.
        // We just need to add our custom properties.
        public string FullName { get; set; }
        public int BalanceInCents { get; set; }
        public string Timezone { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties remain
        public ICollection<GpuListing> GpuListings { get; set; }
        public ICollection<RentalJob> RentalJobs { get; set; }
        public ICollection<WalletLedgerEntry> WalletLedgerEntries { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Dispute> Disputes { get; set; }
    }
}