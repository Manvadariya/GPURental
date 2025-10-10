using GPURental.Data;
using GPURental.Models;
using GPURental.Services;
using GPURental.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    public class MarketplaceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAiService _aiService;

        public MarketplaceController(AppDbContext context, IAiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        // CORRECTED Index Action - It now reads from TempData
        public async Task<IActionResult> Index(string gpuModelSearch, int minVramSearch)
        {
            var query = _context.GpuListings.Where(l => l.Status == GpuStatus.Published);

            // Check if AI search results exist in TempData
            if (TempData["SuggestedIds"] is string suggestedIdsJson)
            {
                var suggestedIds = JsonSerializer.Deserialize<List<string>>(suggestedIdsJson);
                if (suggestedIds != null && suggestedIds.Any())
                {
                    query = query.Where(l => suggestedIds.Contains(l.ListingId));
                }
            }
            else // Only apply manual filters if AI search was not used
            {
                if (!string.IsNullOrEmpty(gpuModelSearch))
                {
                    query = query.Where(l => l.GpuModel.Contains(gpuModelSearch));
                }
                if (minVramSearch > 0)
                {
                    query = query.Where(l => l.VramInGB >= minVramSearch);
                }
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

        // CORRECTED AiSearch Action - It now uses TempData
        [HttpGet]
        public async Task<IActionResult> AiSearch(string userTask)
        {
            if (string.IsNullOrEmpty(userTask))
            {
                return RedirectToAction("Index");
            }

            var availableListings = await _context.GpuListings
                .Where(l => l.Status == GpuStatus.Published)
                .ToListAsync();

            var suggestedIds = await _aiService.GetGpuSuggestionsAsync(userTask, availableListings);

            // Store the list of IDs in TempData as a JSON string
            TempData["SuggestedIds"] = JsonSerializer.Serialize(suggestedIds);
            // Redirect to the Index action, which will read the TempData
            return RedirectToAction("Index");

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