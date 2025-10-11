using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GPURental.Models;
using Microsoft.AspNetCore.Http;

namespace GPURental.ViewModels
{
    public class ListingCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "GPU Model")]
        public string GpuModel { get; set; }

        [Display(Name = "VRAM (GB)")]
        [Range(0, int.MaxValue)]
        public int VramInGB { get; set; }

        [Display(Name = "System RAM (GB)")]
        [Range(0, int.MaxValue)]
        public int RamInGB { get; set; }

        [Display(Name = "Disk Space (GB)")]
        [Range(0, int.MaxValue)]
        public int DiskInGB { get; set; }

        [Display(Name = "CPU Model")]
        public string CpuModel { get; set; }

        [Required]
        [Display(Name = "Operating System")]
        public string OperatingSystem { get; set; }

        public string Location { get; set; }

        [Required]
        [Display(Name = "Price Per Hour (in RS.)")]
        [Range(0, double.MaxValue)]
        public decimal PricePerHourInINR { get; set; }

        [Display(Name = "Listing Image")]
        public IFormFile Image { get; set; }
    }
}