using System.Collections.Generic;
using GPURental.Models;

namespace GPURental.ViewModels
{
    public class ListingEditViewModel : ListingCreateViewModel
    {
        public int ListingId { get; set; }
        public List<ListingImage> ExistingImages { get; set; }
    }
}