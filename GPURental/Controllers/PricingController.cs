using GPURental.Data;
using GPURental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    public class PricingController : Controller
    {
        private readonly AppDbContext _context;

        public PricingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new PricingCalculatorViewModel
            {
                // Get a distinct list of GPU models from our published listings to populate the dropdown
                GpuTypes = await _context.GpuListings
                    .Where(l => l.Status == Models.GpuStatus.Published)
                    .Select(l => l.GpuModel)
                    .Distinct()
                    .Select(gpu => new SelectListItem { Text = gpu, Value = gpu })
                    .ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice to add this to POST actions
        public async Task<IActionResult> Index(PricingCalculatorViewModel model)
        {
            // Re-populate the GpuTypes list, as it's not sent back with the form post
            model.GpuTypes = await _context.GpuListings
                .Where(l => l.Status == Models.GpuStatus.Published)
                .Select(l => l.GpuModel)
                .Distinct()
                .Select(gpu => new SelectListItem { Text = gpu, Value = gpu })
                .ToListAsync();

            // The rest of the model state is still needed for re-displaying the form on error
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.SelectedGpuModel) && model.Minutes > 0)
            {
                // Find the average price per HOUR for the selected GPU model
                var averagePricePerHourInCents = await _context.GpuListings
                    .Where(l => l.GpuModel == model.SelectedGpuModel && l.Status == Models.GpuStatus.Published)
                    .AverageAsync(l => (double?)l.PricePerHourInCents) ?? 0;

                if (averagePricePerHourInCents > 0)
                {
                    // --- UPDATED CALCULATION LOGIC ---
                    // 1. Calculate the average price per MINUTE
                    decimal averagePricePerMinuteInCents = (decimal)averagePricePerHourInCents / 60.0m;

                    // 2. Calculate the total cost in cents
                    decimal totalCostInCents = averagePricePerMinuteInCents * model.Minutes;

                    // 3. Convert total cost to dollars for display
                    model.EstimatedCost = totalCostInCents / 100.0m;
                    // ------------------------------------

                    model.CalculationSuccess = true;
                }
            }

            return View(model);
        }
    }
}