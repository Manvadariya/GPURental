using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GPURental.Models
{
    public class ListingImage
    {
        [Key]
        public int ImageId { get; set; } // Primary Key

        [Required]
        public int ListingId { get; set; } // Foreign Key
        public GpuListing GpuListing { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public string AltText { get; set; }
    }
}