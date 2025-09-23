namespace GPURental.Models
{
    public class ListingTag
    {
        // Composite Primary Key, configured in the DbContext
        public string ListingId { get; set; } // Foreign Key
        public GpuListing GpuListing { get; set; }

        public string TagId { get; set; } // Foreign Key
        public Tag Tag { get; set; }
    }
}