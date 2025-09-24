using GPURental.Models;
using System.Collections.Generic;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalListings { get; set; }
    public List<Dispute> PendingDisputes { get; set; }

    public List<RentalJob> ActiveJobs { get; set; }
}