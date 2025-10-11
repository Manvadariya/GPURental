using GPURental.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GPURental.ViewModels
{
    public class ProviderDashboardViewModel
    {
        // Data for Tables
        public List<GpuListing> MyListings { get; set; }
        public List<Dispute> DisputesAgainstMe { get; set; }

        // Data for Charts
        public List<ChartDataViewModel> ProfitByListingData { get; set; }
        public List<ChartDataViewModel> UsageByListingData { get; set; }

        // --- ADD THESE PROPERTIES FOR THE STAT CARDS ---
        public int ActiveListingsCount { get; set; }
        public decimal TotalEarnings { get; set; }
        public int RunningJobsCount { get; set; }
        public int PendingDisputesCount { get; set; }
        // ---------------------------------------------
    }
}