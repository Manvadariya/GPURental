using GPURental.Models;
using System.Collections.Generic;

namespace GPURental.ViewModels
{
    public class MarketplaceViewModel
    {
        // The results of the search
        public IEnumerable<GpuListing> Listings { get; set; }

        // The search parameters from the form
        public string GpuModelSearch { get; set; }
        public int MinVramSearch { get; set; }
    }
}