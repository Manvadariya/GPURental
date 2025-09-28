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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PricingCalculatorViewModel model)
        {
            model.GpuTypes = await _context.GpuListings
                .Where(l => l.Status == Models.GpuStatus.Published)
                .Select(l => l.GpuModel)
                .Distinct()
                .Select(gpu => new SelectListItem { Text = gpu, Value = gpu })
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.SelectedGpuModel) && model.Minutes > 0)
            {
                var averagePricePerHourInCents = await _context.GpuListings
                    .Where(l => l.GpuModel == model.SelectedGpuModel && l.Status == Models.GpuStatus.Published)
                    .AverageAsync(l => (double?)l.PricePerHourInCents) ?? 0;

                if (averagePricePerHourInCents > 0)
                {
                    decimal averagePricePerMinuteInCents = (decimal)averagePricePerHourInCents / 60.0m;

                    decimal totalCostInCents = averagePricePerMinuteInCents * model.Minutes;

                    model.EstimatedCost = totalCostInCents / 100.0m;

                    model.CalculationSuccess = true;
                }
            }

            return View(model);
        }
    }
}