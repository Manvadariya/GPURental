using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace GPURental.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; } // Primary Key
        [Required]
        public string Name { get; set; }

        // Navigation Property
        public ICollection<ListingTag> ListingTags { get; set; }
    }
}