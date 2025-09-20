using System.Collections.Generic;
using GPURental.Models;

namespace GPURental.ViewModels
{
    public class ListingDetailViewModel
    {
        public GpuListing Listing { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public string ProviderName { get; set; }
    }
}