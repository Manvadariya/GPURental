using GPURental.Models;
using System.Collections.Generic;

namespace GPURental.ViewModels
{
    public class RenterDashboardViewModel
    {
        // Data for Stat Cards
        public int ActiveJobsCount { get; set; }
        public decimal TotalSpent { get; set; }
        public int JobsCompletedCount { get; set; }
        public int OpenDisputesCount { get; set; }

        // Data for Charts
        public List<ChartDataViewModel> SpendingByGpuData { get; set; }
        public List<ChartDataViewModel> JobCountByGpuData { get; set; }

        // Data for Tables
        public List<RentalJob> ActiveJobs { get; set; }
        public List<RentalJob> JobHistory { get; set; }
        public List<Review> MyReviews { get; set; }
        public List<Dispute> MyDisputes { get; set; }
    }
}