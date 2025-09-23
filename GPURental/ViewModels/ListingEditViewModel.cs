using System.Collections.Generic;
using GPURental.Models;

namespace GPURental.ViewModels
{
    public class ListingEditViewModel : ListingCreateViewModel
    {
        public string ListingId { get; set; }
        public string ExistingImagePath { get; set; }
    }
}