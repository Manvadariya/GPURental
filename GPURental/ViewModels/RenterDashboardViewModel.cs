using GPURental.Models;
using System.Collections.Generic;

public class RenterDashboardViewModel
{
    public List<RentalJob> ActiveJobs { get; set; }

    // --- ADD THIS PROPERTY ---
    public List<RentalJob> JobHistory { get; set; }
    // -------------------------

    public List<Review> MyReviews { get; set; }
    public List<Dispute> MyDisputes { get; set; }
    public List<WalletLedgerEntry> TransactionHistory { get; set; }
}