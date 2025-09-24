using GPURental.Models;
using System.Collections.Generic;

namespace GPURental.ViewModels
{
    public class ProviderDashboardViewModel
    {
        public List<GpuListing> MyListings { get; set; }
        public List<Dispute> DisputesAgainstMe { get; set; }
    }
}