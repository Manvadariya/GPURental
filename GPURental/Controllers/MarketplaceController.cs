using GPURental.Data;
using GPURental.Models;
using GPURental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    public class MarketplaceController : Controller
    {
        private readonly AppDbContext _context;

        public MarketplaceController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string gpuModelSearch, int minVramSearch)
        {
            var query = _context.GpuListings
                              .Where(l => l.Status == GpuStatus.Published);

            if (!string.IsNullOrEmpty(gpuModelSearch))
            {
                query = query.Where(l => l.GpuModel.Contains(gpuModelSearch));
            }

            if (minVramSearch > 0)
            {
                query = query.Where(l => l.VramInGB >= minVramSearch);
            }

            var filteredListings = await query
                .Include(l => l.Provider)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            var viewModel = new MarketplaceViewModel
            {
                Listings = filteredListings,
                GpuModelSearch = gpuModelSearch,
                MinVramSearch = minVramSearch
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var listing = await _context.GpuListings
                .Include(l => l.Provider)
                .FirstOrDefaultAsync(l => l.ListingId == id && l.Status == GpuStatus.Published);

            if (listing == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Where(r => r.ListingId == id)
                .Include(r => r.Author)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var viewModel = new ListingDetailViewModel
            {
                Listing = listing,
                Reviews = reviews,
                ProviderName = listing.Provider.FullName
            };

            return View(viewModel);
        }
    }
}