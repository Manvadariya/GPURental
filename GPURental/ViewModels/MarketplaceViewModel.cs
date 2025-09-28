using GPURental.Models;
using System.Collections.Generic;

namespace GPURental.ViewModels
{
    public class MarketplaceViewModel
    {
        public IEnumerable<GpuListing> Listings { get; set; }

        public string GpuModelSearch { get; set; }
        public int MinVramSearch { get; set; }
    }
}