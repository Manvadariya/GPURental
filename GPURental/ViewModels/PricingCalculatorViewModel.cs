using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GPURental.ViewModels
{
    public class PricingCalculatorViewModel
    {
        // --- Input Properties for the Form ---

        [Display(Name = "Select GPU Type")]
        public string SelectedGpuModel { get; set; }

        [Display(Name = "Rental Duration (in minutes)")]
        [Range(0.1, 44640)] // Allow fractional minutes
        public decimal Minutes { get; set; } = 60; // Default to 60 minutes (1 hour)

        public List<SelectListItem> GpuTypes { get; set; }

        // --- Output Properties for the Result ---

        public decimal EstimatedCost { get; set; }
        public bool CalculationSuccess { get; set; } = false;
    }
}