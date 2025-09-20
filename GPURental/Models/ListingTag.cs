namespace GPURental.Models
{
    public class ListingTag
    {
        // Composite Primary Key, configured in the DbContext
        public int ListingId { get; set; } // Foreign Key
        public GpuListing GpuListing { get; set; }

        public int TagId { get; set; } // Foreign Key
        public Tag Tag { get; set; }
    }
}