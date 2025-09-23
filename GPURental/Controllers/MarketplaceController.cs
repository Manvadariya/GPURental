using GPURental.Data;
using GPURental.Models;
using GPURental.ViewModels; // Make sure this is here
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Make sure this is here
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

        // Replace the old Index action with this one

        public async Task<IActionResult> Index(string gpuModelSearch, int minVramSearch)
        {
            // Start with a base IQueryable. We are NOT including any related data yet.
            var query = _context.GpuListings
                              .Where(l => l.Status == GpuStatus.Published);

            // 1. Conditionally add more filters to the query
            if (!string.IsNullOrEmpty(gpuModelSearch))
            {
                query = query.Where(l => l.GpuModel.Contains(gpuModelSearch));
            }

            if (minVramSearch > 0)
            {
                query = query.Where(l => l.VramInGB >= minVramSearch);
            }

            // 2. NOW, after all filtering is done, shape the final data
            var filteredListings = await query
                .Include(l => l.Provider) // Eager load the provider data
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            // 3. Create and return the ViewModel (this part is the same)
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

            // CORRECTED QUERY: Removed the invalid .Include() and added the one for Provider
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