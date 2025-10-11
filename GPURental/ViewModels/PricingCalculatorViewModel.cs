using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GPURental.ViewModels
{
    public class PricingCalculatorViewModel
    {

        [Display(Name = "Select GPU Type")]
        public string SelectedGpuModel { get; set; }

        [Display(Name = "Rental Duration (in seconds)")]
        public int Seconds { get; set; } = 3600;
        public List<SelectListItem> GpuTypes { get; set; }
        public decimal EstimatedCost { get; set; }
        public bool CalculationSuccess { get; set; } = false;
    }
}